using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDialogNode", menuName = "Narrative/Dialog Node")]
public class DialogNodeSO : ScriptableObject
{
    [TextArea] public string text;
    public List<Choice> choices;
    public DiceCheck diceCheck; // Optional dice roll check
}

[System.Serializable]
public class Choice
{
    public string choiceText;
    public DialogNodeSO nextNode;
}

[System.Serializable]
public class DiceCheck
{
    public bool requiresRoll;
    public string abilityScore;
    public DialogNodeSO successNode;
    public DialogNodeSO failureNode;
}
