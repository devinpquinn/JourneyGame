using System.Collections.Generic;
using UnityEngine;

public class ScenarioManager : MonoBehaviour
{
	private List<ScenarioSO> availableScenarios = new List<ScenarioSO>();

	public DialogManager dialogManager; // Assign via Inspector
	public string scenariosResourceFolder = "Scenarios"; // Folder under Resources

	private void Start()
	{
		LoadScenariosFromResources();
		dialogManager.playerCharacter.ResetStats();
		SelectNextScenario();
	}

	// Loads all ScenarioSO assets from the specified Resources folder
	private void LoadScenariosFromResources()
	{
		ScenarioSO[] loadedScenarios = Resources.LoadAll<ScenarioSO>(scenariosResourceFolder);
		availableScenarios = new List<ScenarioSO>(loadedScenarios);
		if (availableScenarios.Count == 0)
		{
			Debug.LogWarning("No scenarios found in Resources/" + scenariosResourceFolder);
		}
	}

	// Resets the pool by re-loading all scenarios
	public void ResetScenarioPool()
	{
		LoadScenariosFromResources();
	}

	// Selects and starts a random scenario from the available pool, removing it afterward
	public void SelectNextScenario()
	{
		if (availableScenarios.Count == 0)
		{
			ResetScenarioPool(); // Optionally, you can stop the game or do something else here
		}

		if (availableScenarios.Count > 0)
		{
			int index = Random.Range(0, availableScenarios.Count);
			ScenarioSO selectedScenario = availableScenarios[index];
			availableScenarios.RemoveAt(index);

			dialogManager.ClearDialogLog(); // Clear the dialog log before starting a new scenario
			dialogManager.StartScenario(selectedScenario);
		}
		else
		{
			Debug.LogWarning("No available scenarios to select!");
		}
	}
}
