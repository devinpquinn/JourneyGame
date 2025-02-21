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

	public void StartScenario(ScenarioSO scenario)
	{
		ClearDialogLog();

		if (scenario.nodes.Count > 0) // Ensure the scenario has nodes
		{
			AdvanceToNode(scenario.nodes[0]); // Start from the first node
		}
		else
		{
			Debug.LogWarning("Scenario has no dialog nodes!");
		}
	}

	public void AdvanceToNode(DialogNodeSO node)
	{
		currentNode = node;

		if (currentNode == null)
		{
			scenarioManager.SelectNextScenario(); // Move to the next scenario when no more nodes exist
			return;
		}

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
			AdvanceToNode(choice.nextNode);
		}
	}

	private void PerformDiceCheck(Choice choice)
	{
		int roll = Random.Range(1, 21); // Roll 1d20
		int abilityScore = playerCharacter.GetVirtueScore(choice.abilityToCheck); // Get the relevant virtue

		bool success = roll < abilityScore || roll == 20; // Roll under virtue = success, 20 always succeeds, 1 always fails
		bool autoFail = roll == 1; // Roll of 1 always fails

		DialogNodeSO resultNode = (success && !autoFail) ? choice.nextNode : choice.nextNodeOnFailure;

		string resultText = success ? "<color=green>PASSED</color>" : "<color=red>FAILED</color>";
		AppendDialogText($"{resultText} - rolled a {roll} against your {abilityScore} ({choice.abilityToCheck}).");

		AdvanceToNode(resultNode);
	}

	public void ClearDialogLog()
	{
		foreach (Transform child in dialogLogContainer)
		{
			Destroy(child.gameObject);
		}
	}
}
