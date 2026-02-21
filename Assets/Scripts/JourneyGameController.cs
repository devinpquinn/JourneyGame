using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JourneyGameController : MonoBehaviour
{
    private enum GameState
    {
        EventNode,
        RoundEnd,
        InterRound
    }

    private enum RoundEndType
    {
        None,
        Win,
        HealthDepleted,
        MoraleDepleted
    }

    [Header("Region")]
    [SerializeField] private string regionName = "Starter Region";
    [SerializeField] private int progressToClear = 20;

    [Header("Event Panel")]
    [SerializeField] private TMP_Text eventTitleText;
    [SerializeField] private TMP_Text eventBodyText;
    [SerializeField] private Button okayButton;

    [Header("Hero Panel - Attributes")]
    [SerializeField] private TMP_Text strengthText;
    [SerializeField] private TMP_Text courageText;
    [SerializeField] private TMP_Text wisdomText;
    [SerializeField] private TMP_Text graceText;
    [SerializeField] private TMP_Text luckText;

    [Header("Hero Panel - Vitals")]
    [SerializeField] private TMP_Text healthLabelText;
    [SerializeField] private Image healthFillImage;
    [SerializeField] private TMP_Text moraleLabelText;
    [SerializeField] private Image moraleFillImage;

    [Header("Region Panel")]
    [SerializeField] private TMP_Text regionLabelText;
    [SerializeField] private TMP_Text progressLabelText;
    [SerializeField] private Image progressFillImage;

    private readonly List<RegionEventData> allRegionEvents = new List<RegionEventData>();
    private readonly List<RegionEventData> eventDeck = new List<RegionEventData>();
    private readonly HashSet<string> seenNonRepeatableEventIds = new HashSet<string>();

    private RegionEventData currentEvent;
    private EventNodeData currentNode;

    private bool prependTestResult;
    private bool lastTestPassed;
    private HeroAttribute lastTestAttribute = HeroAttribute.None;

    private bool depletedFromCurrentNode;
    private RoundEndType pendingRoundEndType = RoundEndType.None;

    private GameState gameState;
    private int progress;
    private int roundsCompleted;

    private void Awake()
    {
        Hero.InitializeDefaults();

        if (okayButton != null)
        {
            okayButton.onClick.RemoveListener(OnOkayPressed);
            okayButton.onClick.AddListener(OnOkayPressed);
        }
    }

    private void Start()
    {
        LoadRegionEvents();
        StartNewRound(showInterRoundMessage: false);
    }

    private void LoadRegionEvents()
    {
        allRegionEvents.Clear();
        string path = "Events/Regions/" + regionName;
        RegionEventData[] loaded = Resources.LoadAll<RegionEventData>(path);

        for (int index = 0; index < loaded.Length; index++)
        {
            if (loaded[index] != null)
            {
                allRegionEvents.Add(loaded[index]);
            }
        }

        RebuildDeck();
    }

    private void RebuildDeck()
    {
        eventDeck.Clear();

        for (int index = 0; index < allRegionEvents.Count; index++)
        {
            RegionEventData entry = allRegionEvents[index];
            if (entry == null)
            {
                continue;
            }

            if (!entry.Repeatable && seenNonRepeatableEventIds.Contains(entry.name))
            {
                continue;
            }

            eventDeck.Add(entry);
        }

        Shuffle(eventDeck);
    }

    private void OnOkayPressed()
    {
        switch (gameState)
        {
            case GameState.EventNode:
                AdvanceFromEventNode();
                break;
            case GameState.RoundEnd:
                HandleRoundEndAcknowledge();
                break;
            case GameState.InterRound:
                BeginNextEvent();
                break;
        }
    }

    private void AdvanceFromEventNode()
    {
        if (depletedFromCurrentNode)
        {
            ShowRoundEnd(Hero.CurrentHealth <= 0 ? RoundEndType.HealthDepleted : RoundEndType.MoraleDepleted);
            return;
        }

        if (pendingRoundEndType == RoundEndType.Win)
        {
            ShowRoundEnd(RoundEndType.Win);
            return;
        }

        if (currentNode == null)
        {
            BeginNextEvent();
            return;
        }

        if (currentNode.HasTest)
        {
            bool passed = RollTest(currentNode.TestAttribute);
            string branchNodeId = passed ? currentNode.SuccessNodeId : currentNode.FailureNodeId;

            prependTestResult = true;
            lastTestPassed = passed;
            lastTestAttribute = currentNode.TestAttribute;

            if (string.IsNullOrWhiteSpace(branchNodeId))
            {
                CompleteCurrentEvent();
                return;
            }

            EventNodeData nextNode = currentEvent.GetNode(branchNodeId);
            if (nextNode == null)
            {
                CompleteCurrentEvent();
                return;
            }

            EnterNode(nextNode);
            return;
        }

        if (!string.IsNullOrWhiteSpace(currentNode.NextNodeId))
        {
            EventNodeData nextNode = currentEvent.GetNode(currentNode.NextNodeId);
            if (nextNode != null)
            {
                EnterNode(nextNode);
                return;
            }
        }

        CompleteCurrentEvent();
    }

    private void CompleteCurrentEvent()
    {
        AddProgress(1);
        if (pendingRoundEndType == RoundEndType.Win)
        {
            ShowRoundEnd(RoundEndType.Win);
            return;
        }

        BeginNextEvent();
    }

    private void BeginNextEvent()
    {
        gameState = GameState.EventNode;
        prependTestResult = false;
        depletedFromCurrentNode = false;
        pendingRoundEndType = RoundEndType.None;

        if (eventDeck.Count == 0)
        {
            RebuildDeck();
        }

        if (eventDeck.Count == 0)
        {
            eventTitleText.text = regionName;
            eventBodyText.text = "No events were found for this region.";
            return;
        }

        int lastIndex = eventDeck.Count - 1;
        currentEvent = eventDeck[lastIndex];
        eventDeck.RemoveAt(lastIndex);

        if (!currentEvent.Repeatable)
        {
            seenNonRepeatableEventIds.Add(currentEvent.name);
        }

        currentNode = currentEvent.GetStartNode();
        if (currentNode == null)
        {
            CompleteCurrentEvent();
            return;
        }

        EnterNode(currentNode);
    }

    private void EnterNode(EventNodeData node)
    {
        currentNode = node;
        depletedFromCurrentNode = false;
        pendingRoundEndType = RoundEndType.None;

        List<string> lines = new List<string>();

        if (prependTestResult)
        {
            string passText = lastTestPassed ? "passed" : "failed";
            lines.Add("You " + passText + " a Test of " + HeroNames.Attribute(lastTestAttribute) + "!");
            lines.Add(string.Empty);
            prependTestResult = false;
            lastTestAttribute = HeroAttribute.None;
        }

        if (!string.IsNullOrWhiteSpace(node.BodyText))
        {
            lines.Add(node.BodyText.Trim());
        }

        List<string> effectLines = ApplyNodeEffects(node);
        if (effectLines.Count > 0)
        {
            lines.Add(string.Empty);
            for (int index = 0; index < effectLines.Count; index++)
            {
                lines.Add(effectLines[index]);
            }
        }

        if (node.HasTest)
        {
            lines.Add(string.Empty);
            lines.Add("You face a Test of " + HeroNames.Attribute(node.TestAttribute) + "...");
        }

        eventTitleText.text = currentEvent != null ? currentEvent.Title : regionName;
        eventBodyText.text = string.Join("\n", lines);
        RefreshAllUi();
    }

    private List<string> ApplyNodeEffects(EventNodeData node)
    {
        List<string> effectLines = new List<string>();
        IReadOnlyList<EventEffect> effects = node.Effects;

        for (int index = 0; index < effects.Count; index++)
        {
            EventEffect effect = effects[index];
            int actualDelta = ApplyEffect(effect.Target, effect.Amount);
            if (actualDelta == 0)
            {
                continue;
            }

            effectLines.Add(FormatEffectLine(actualDelta, HeroNames.EffectTarget(effect.Target)));
        }

        if (Hero.CurrentHealth <= 0)
        {
            depletedFromCurrentNode = true;
        }
        else if (Hero.CurrentMorale <= 0)
        {
            depletedFromCurrentNode = true;
        }

        return effectLines;
    }

    private int ApplyEffect(HeroEffectTarget target, int amount)
    {
        switch (target)
        {
            case HeroEffectTarget.Strength:
                return Hero.AddAttribute(HeroAttribute.Strength, amount);
            case HeroEffectTarget.Courage:
                return Hero.AddAttribute(HeroAttribute.Courage, amount);
            case HeroEffectTarget.Wisdom:
                return Hero.AddAttribute(HeroAttribute.Wisdom, amount);
            case HeroEffectTarget.Grace:
                return Hero.AddAttribute(HeroAttribute.Grace, amount);
            case HeroEffectTarget.Luck:
                return Hero.AddAttribute(HeroAttribute.Luck, amount);
            case HeroEffectTarget.Health:
                return Hero.AddHealth(amount);
            case HeroEffectTarget.Morale:
                return Hero.AddMorale(amount);
            case HeroEffectTarget.Progress:
                return AddProgress(amount);
            default:
                return 0;
        }
    }

    private int AddProgress(int amount)
    {
        int before = progress;
        progress = Mathf.Max(0, progress + amount);

        if (progress >= progressToClear)
        {
            pendingRoundEndType = RoundEndType.Win;
        }

        return progress - before;
    }

    private bool RollTest(HeroAttribute attribute)
    {
        int chance = Hero.GetAttributeValue(attribute);
        int roll = Random.Range(1, 101);
        return roll <= chance;
    }

    private void ShowRoundEnd(RoundEndType roundEndType)
    {
        gameState = GameState.RoundEnd;
        eventTitleText.text = "Round End";

        switch (roundEndType)
        {
            case RoundEndType.HealthDepleted:
                eventBodyText.text = "Health Depleted. Your round is over.";
                break;
            case RoundEndType.MoraleDepleted:
                eventBodyText.text = "Morale Depleted. Your round is over.";
                break;
            case RoundEndType.Win:
                eventBodyText.text = "Congratulations, you have cleared " + regionName + "!";
                break;
            default:
                eventBodyText.text = "Your round is over.";
                break;
        }

        RefreshAllUi();
    }

    private void HandleRoundEndAcknowledge()
    {
        roundsCompleted++;
        progress = 0;
        Hero.ResetRoundVitals();

        RebuildDeck();

        gameState = GameState.InterRound;
        eventTitleText.text = "Camp";

        string restMessage = "You rest up for your next attempt.";
        string bonusLine;

        if (roundsCompleted % 4 == 0)
        {
            bool boostHealth = Random.value < 0.5f;
            if (boostHealth)
            {
                Hero.AddMaxHealth(1);
                bonusLine = "+1 Max Health";
            }
            else
            {
                Hero.AddMaxMorale(1);
                bonusLine = "+1 Max Morale";
            }
        }
        else
        {
            HeroAttribute attribute = GetRandomAttribute();
            int actualDelta = Hero.AddAttribute(attribute, 5);
            bonusLine = FormatEffectLine(actualDelta, HeroNames.Attribute(attribute));
        }

        eventBodyText.text = restMessage + "\n\n" + bonusLine;
        RefreshAllUi();
    }

    private HeroAttribute GetRandomAttribute()
    {
        int index = Random.Range(0, 5);
        switch (index)
        {
            case 0:
                return HeroAttribute.Strength;
            case 1:
                return HeroAttribute.Courage;
            case 2:
                return HeroAttribute.Wisdom;
            case 3:
                return HeroAttribute.Grace;
            default:
                return HeroAttribute.Luck;
        }
    }

    private void StartNewRound(bool showInterRoundMessage)
    {
        progress = 0;
        Hero.ResetRoundVitals();

        if (showInterRoundMessage)
        {
            gameState = GameState.InterRound;
            eventTitleText.text = "Camp";
            eventBodyText.text = "You rest up for your next attempt.";
            RefreshAllUi();
            return;
        }

        BeginNextEvent();
    }

    private void RefreshAllUi()
    {
        if (strengthText != null)
        {
            strengthText.text = Hero.Strength + "%";
        }

        if (courageText != null)
        {
            courageText.text = Hero.Courage + "%";
        }

        if (wisdomText != null)
        {
            wisdomText.text = Hero.Wisdom + "%";
        }

        if (graceText != null)
        {
            graceText.text = Hero.Grace + "%";
        }

        if (luckText != null)
        {
            luckText.text = Hero.Luck + "%";
        }

        if (healthLabelText != null)
        {
            healthLabelText.text = Hero.CurrentHealth + "/" + Hero.MaxHealth;
        }

        if (moraleLabelText != null)
        {
            moraleLabelText.text = Hero.CurrentMorale + "/" + Hero.MaxMorale;
        }

        if (healthFillImage != null)
        {
            healthFillImage.fillAmount = Hero.MaxHealth > 0 ? (float)Hero.CurrentHealth / Hero.MaxHealth : 0f;
        }

        if (moraleFillImage != null)
        {
            moraleFillImage.fillAmount = Hero.MaxMorale > 0 ? (float)Hero.CurrentMorale / Hero.MaxMorale : 0f;
        }

        if (regionLabelText != null)
        {
            regionLabelText.text = regionName;
        }

        if (progressLabelText != null)
        {
            progressLabelText.text = progress + "/" + progressToClear;
        }

        if (progressFillImage != null)
        {
            progressFillImage.fillAmount = progressToClear > 0 ? Mathf.Clamp01((float)progress / progressToClear) : 0f;
        }
    }

    private void Shuffle<T>(List<T> list)
    {
        for (int index = list.Count - 1; index > 0; index--)
        {
            int swapIndex = Random.Range(0, index + 1);
            T temp = list[index];
            list[index] = list[swapIndex];
            list[swapIndex] = temp;
        }
    }

    private static string FormatEffectLine(int amount, string label)
    {
        if (amount == 0)
        {
            return "+0 " + label;
        }

        string sign = amount > 0 ? "+" : "-";
        return sign + Mathf.Abs(amount) + " " + label;
    }
}