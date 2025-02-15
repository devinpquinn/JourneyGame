using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class DialogManager : MonoBehaviour
{
    public TextMeshProUGUI dialogText;
    public Transform choicesContainer;
    public Button choiceButtonPrefab;
    
    private DialogNodeSO currentNode;
    private ScenarioSO currentScenario;

    public void StartScenario(ScenarioSO scenario)
    {
        currentScenario = scenario;
        currentNode = scenario.nodes[0]; // Start from the first node
        DisplayNode();
    }

    void DisplayNode()
    {
        dialogText.text = currentNode.text;

        // Clear previous choices
        foreach (Transform child in choicesContainer) Destroy(child.gameObject);

        if (currentNode.diceCheck.requiresRoll)
        {
            CreateChoiceButton("Roll the Dice", () => ResolveDiceCheck());
        }
        else if (currentNode.choices.Count > 0)
        {
            foreach (var choice in currentNode.choices)
            {
                CreateChoiceButton(choice.choiceText, () => ChooseOption(choice.nextNode));
            }
        }
        else
        {
            // Default "Continue" button
            int index = currentScenario.nodes.IndexOf(currentNode);
            if (index + 1 < currentScenario.nodes.Count)
            {
                CreateChoiceButton("Continue", () => ChooseOption(currentScenario.nodes[index + 1]));
            }
        }
    }

    void ChooseOption(DialogNodeSO nextNode)
    {
        currentNode = nextNode;
        DisplayNode();
    }

    void ResolveDiceCheck()
    {
        int roll = Roll3d6();
        int playerStat = GetPlayerStat(currentNode.diceCheck.abilityScore);
        bool success = roll < playerStat; // Success if roll is under ability score

        currentNode = success ? currentNode.diceCheck.successNode : currentNode.diceCheck.failureNode;
        DisplayNode();
    }

    int Roll3d6()
    {
        return Random.Range(1, 7) + Random.Range(1, 7) + Random.Range(1, 7);
    }

    int GetPlayerStat(string ability)
    {
        // Placeholder: Fetch actual ability score from the player's stats
        return 10; // Default ability score
    }

    void CreateChoiceButton(string text, System.Action onClick)
    {
        Button btn = Instantiate(choiceButtonPrefab, choicesContainer);
        TextMeshProUGUI btnText = btn.GetComponentInChildren<TextMeshProUGUI>();
        if (btnText != null)
        {
            btnText.text = text;
        }
        btn.onClick.AddListener(() => onClick());
    }
}
