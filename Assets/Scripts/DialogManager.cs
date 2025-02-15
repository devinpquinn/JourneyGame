using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class DialogManager : MonoBehaviour
{
    public Text dialogText;
    public Button continueButton;
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

        // If there are choices, show them; otherwise, show continue button
        if (currentNode.choices.Count > 0)
        {
            continueButton.gameObject.SetActive(false);
            foreach (var choice in currentNode.choices)
            {
                Button btn = Instantiate(choiceButtonPrefab, choicesContainer);
                btn.GetComponentInChildren<Text>().text = choice.choiceText;
                btn.onClick.AddListener(() => ChooseOption(choice.nextNode));
            }
        }
        else if (currentNode.diceCheck.requiresRoll)
        {
            continueButton.gameObject.SetActive(true);
            continueButton.onClick.RemoveAllListeners();
            continueButton.onClick.AddListener(() => ResolveDiceCheck());
        }
        else
        {
            continueButton.gameObject.SetActive(true);
            continueButton.onClick.RemoveAllListeners();
            continueButton.onClick.AddListener(() => Continue());
        }
    }

    void Continue()
    {
        // Move to the next node, or end scenario if none
        int index = currentScenario.nodes.IndexOf(currentNode);
        if (index + 1 < currentScenario.nodes.Count)
        {
            currentNode = currentScenario.nodes[index + 1];
            DisplayNode();
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
}
