using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDialogNode", menuName = "Narrative/Dialog Node")]
public class DialogNodeSO : ScriptableObject
{
	[TextArea] public string text;
	public List<Choice> choices;
	
	public List<DialogEvent> events; // List of events to trigger when this node is reached
}

[System.Serializable]
public class Choice
{
	public string choiceText;
	public DialogNodeSO nextNode; // The node this choice leads to
	public bool isDiceCheck;
	public string abilityToCheck;
	public DialogNodeSO nextNodeOnFailure; // The node to go to if the roll fails
}

[System.Serializable]
public class DialogEvent
{
	public string eventName;
	public int intValue;
	public string stringValue;
}
