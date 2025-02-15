using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewScenario", menuName = "Narrative/Scenario")]
public class ScenarioSO : ScriptableObject
{
    public string scenarioTitle;
    public List<DialogNodeSO> nodes;
}
