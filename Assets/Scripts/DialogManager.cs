using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
	public Transform dialogLogContainer; // UI container for dialog log entries
	public GameObject dialogTextPrefab; // Prefab for displaying dialog text
	public GameObject choiceButtonPrefab; // Prefab for displaying choices
	public Transform choiceContainer; // UI container for choices

	public ScenarioManager scenarioManager; // Reference to the scenario manager
	public PlayerCharacterSO playerCharacter; // Reference to the player character

	private DialogNodeSO currentNode;
	
	public DialogNodeSO deathHealthNode;
	public DialogNodeSO deathMoraleNode;

	public void StartScenario(ScenarioSO scenario)
	{
		ClearDialogLog();

		if (scenario.nodes.Count > 0) // Ensure the scenario has nodes
		{
			GoToNode(scenario.nodes[0]); // Start from the first node
		}
		else
		{
			Debug.LogWarning("Scenario has no dialog nodes!");
		}
	}

	public void GoToNode(DialogNodeSO node)
	{
		//check if health or morale is 0
		if (playerCharacter.CurrentHealth <= 0)
		{
			node = deathHealthNode;
		}
		if (playerCharacter.CurrentMorale <= 0)
		{
			node = deathMoraleNode;
		}
		
		currentNode = node;

		if (currentNode == null)
		{
			scenarioManager.SelectNextScenario(); // Move to the next scenario when no more nodes exist
			return;
		}

		ProcessNodeEvents(currentNode);
		AppendDialogText(currentNode.text); // Ensure this matches your actual property name
		CreateChoiceButtons();
	}

	private void AppendDialogText(string text)
	{
		// Instantiate new dialog entry in the log
		GameObject newDialogText = Instantiate(dialogTextPrefab, dialogLogContainer);
		TMP_Text tmpText = newDialogText.GetComponent<TMP_Text>();
		tmpText.text = text;

		// Fade out previous dialog entries
		foreach (Transform child in dialogLogContainer)
		{
			if (child != newDialogText.transform)
			{
				TMP_Text childText = child.GetComponent<TMP_Text>();
				if (childText != null)
				{
					childText.color = new Color(childText.color.r, childText.color.g, childText.color.b, 0.5f);
				}
			}
		}
	}

	private void CreateChoiceButtons()
	{
	    // Clear existing choices
	    foreach (Transform child in choiceContainer)
	    {
	        Destroy(child.gameObject);
	    }

	    foreach (Choice choice in currentNode.choices)
	    {
	        GameObject choiceButtonObj = Instantiate(choiceButtonPrefab, choiceContainer);
	        TMP_Text buttonText = choiceButtonObj.GetComponentInChildren<TMP_Text>();
	        
	        // Set button text to "Continue" if choice text is empty
	        if (string.IsNullOrEmpty(choice.choiceText))
	        {
	            buttonText.text = "Continue";
	        }
	        else
	        {
	            buttonText.text = choice.choiceText;
	            if (choice.isDiceCheck)
	            {
	                buttonText.text += $" ({choice.abilityToCheck})";
	            }
	            if (choice.isLuckCheck)
	            {
	                buttonText.text += " (Luck Check)";
	            }
	        }

	        Button button = choiceButtonObj.GetComponent<Button>();
	        button.onClick.AddListener(() => OnChoiceSelected(choice));
	    }
	    if (currentNode.choices.Count == 0)
	    {
	        //if there are no choices, generate a button that starts the next scenario
	        GameObject choiceButtonObj = Instantiate(choiceButtonPrefab, choiceContainer);
	        TMP_Text buttonText = choiceButtonObj.GetComponentInChildren<TMP_Text>();
	        buttonText.text = "End Scenario";
	        
	        Button button = choiceButtonObj.GetComponent<Button>();
	        button.onClick.AddListener(() => scenarioManager.SelectNextScenario());
	    }
	}

	public void OnChoiceSelected(Choice choice)
	{
		if (choice.isDiceCheck)
		{
			PerformDiceCheck(choice);
		}
		else
		{
			GoToNode(choice.nextNode);
		}
	}

	private void PerformDiceCheck(Choice choice)
	{
	    if (choice.isLuckCheck)
	    {
	        PerformLuckCheck(choice);
	        return;
	    }

	    int roll = Random.Range(1, 21); // Roll 1d20
	    int abilityScore = playerCharacter.GetVirtue(choice.abilityToCheck); // Get the relevant virtue

	    bool success = roll < abilityScore || roll == 20; // Roll under virtue = success, 20 always succeeds, 1 always fails
	    bool autoFail = roll == 1; // Roll of 1 always fails

	    DialogNodeSO resultNode = (success && !autoFail) ? choice.nextNode : choice.nextNodeOnFailure;

	    string resultText = success ? "<color=green>PASSED</color>" : "<color=red>FAILED</color>";
	    AppendDialogText($"{resultText} - rolled a {roll} against your {abilityScore} ({choice.abilityToCheck}).");

	    GoToNode(resultNode);
	}

	private void PerformLuckCheck(Choice choice)
	{
	    int roll = Random.Range(1, 21); // Roll 1d20
	    int luckBonus = playerCharacter.Luck; // Get the player's luck bonus
	    int total = roll + luckBonus;

	    DialogNodeSO resultNode = null;
	    foreach (var threshold in choice.luckThresholds)
	    {
	        if (total <= threshold.threshold)
	        {
	            resultNode = threshold.node;
	            break;
	        }
	    }

	    // If no threshold was met, use the last node in the list
	    if (resultNode == null && choice.luckThresholds.Count > 0)
	    {
	        resultNode = choice.luckThresholds[choice.luckThresholds.Count - 1].node;
	    }

	    string resultText = $"<color=blue>Luck Check</color> - rolled a {roll} + {luckBonus} = {total}";
	    AppendDialogText(resultText);

	    GoToNode(resultNode);
	}

	public void ClearDialogLog()
	{
		foreach (Transform child in dialogLogContainer)
		{
			Destroy(child.gameObject);
		}
	}
	
	private void ProcessNodeEvents(DialogNodeSO node)
	{
		foreach (DialogEvent dialogEvent in node.events)
		{
			switch (dialogEvent.eventName)
			{
				case "ModifyHealth":
					playerCharacter.ModifyHealth(dialogEvent.intValue);
					break;
				case "ModifyMorale":
					playerCharacter.ModifyMorale(dialogEvent.intValue);
					break;
				case "ModifyVirtue":
					playerCharacter.ModifyVirtue(dialogEvent.stringValue, dialogEvent.intValue);
					break;
				case "ModifyProgress":
					scenarioManager.ModifyProgress(dialogEvent.intValue);
					break;
			}
		}
	}
}
