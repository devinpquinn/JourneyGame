using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tester : MonoBehaviour
{
	public DialogManager dialogManager;
	public ScenarioSO scenario;
	
	void Start()
	{
		dialogManager.StartScenario(scenario);
	}
}
