using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class DialogManager : MonoBehaviour
{
	public Transform dialogLogContainer; // Container for scrollable text
	public ScrollRect scrollRect;
	public TextMeshProUGUI dialogTextPrefab; // Prefab for text blocks
	public Transform choicesContainer;
	public Button choiceButtonPrefab;

	private DialogNodeSO currentNode;
	private ScenarioSO currentScenario;
	
	public PlayerCharacterSO playerCharacter;

	public void StartScenario(ScenarioSO scenario)
	{
		currentScenario = scenario;
		currentNode = scenario.nodes[0]; // Start from the first node
		DisplayNode();
	}

	void DisplayNode()
	{
		AppendDialogText(currentNode.text); // Append new dialog text

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

		ScrollToBottom(); // Auto-scroll to the latest dialog entry
	}

	void ChooseOption(DialogNodeSO nextNode)
	{
		currentNode = nextNode;
		DisplayNode();
	}

	void ResolveDiceCheck()
	{
		int roll = Random.Range(1, 21); // Roll 1d20
		int playerStat = GetPlayerStat(currentNode.diceCheck.abilityScore);

		bool success;
		if (roll == 20) // Natural 20 always succeeds
		{
			success = true;
		}
		else if (roll == 1) // Natural 1 always fails
		{
			success = false;
		}
		else
		{
			success = roll < playerStat; // Normal success check
		}

		string rollResult = $"(Rolled {roll}, Need Under {playerStat})";
		AppendDialogText(rollResult);

		currentNode = success ? currentNode.diceCheck.successNode : currentNode.diceCheck.failureNode;
		DisplayNode();
	}

	int GetPlayerStat(string ability)
	{
		return playerCharacter.GetVirtueScore(ability);
	}

	void AppendDialogText(string text)
	{
		// Reduce opacity of existing dialog entries
		foreach (Transform child in dialogLogContainer)
		{
			TextMeshProUGUI previousText = child.GetComponent<TextMeshProUGUI>();
			if (previousText != null)
			{
				Color fadedColor = previousText.color;
				fadedColor.a = 0.25f; // Set to half opacity
				previousText.color = fadedColor;
			}
		}

		// Add new dialog entry with full opacity
		TextMeshProUGUI newDialogText = Instantiate(dialogTextPrefab, dialogLogContainer);
		newDialogText.text = text;
		newDialogText.color = new Color(1, 1, 1, 1); // Full opacity for the latest text
	}

	void ScrollToBottom()
	{
		Canvas.ForceUpdateCanvases();
		scrollRect.verticalNormalizedPosition = 0f;
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
