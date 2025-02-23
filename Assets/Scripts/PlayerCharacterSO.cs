using UnityEngine;

[CreateAssetMenu(fileName = "NewPlayerCharacter", menuName = "Game/Player Character")]
public class PlayerCharacterSO : ScriptableObject
{
	public int Might = 10;
	public int Mettle = 10;
	public int Courage = 10;
	public int Cunning = 10;
	public int Goodness = 10;
	public int Grace = 10;

	public int MaxHealth = 3;
	public int MaxMorale = 3;
	public int Luck = 0;
	
	public int CurrentHealth;
	public int CurrentMorale;
	
	public void ResetStats()
	{
		CurrentHealth = MaxHealth;
		CurrentMorale = MaxMorale;
	}

	public int GetVirtue(string virtue)
	{
		switch (virtue)
		{
			case "Might": return Might;
			case "Mettle": return Mettle;
			case "Courage": return Courage;
			case "Cunning": return Cunning;
			case "Goodness": return Goodness;
			case "Grace": return Grace;
			default: return 10; // Default fallback
		}
	}
	
	public void ModifyVirtue(string virtue, int value)
	{
		switch (virtue)
		{
			case "Might": Might += value; break;
			case "Mettle": Mettle += value; break;
			case "Courage": Courage += value; break;
			case "Cunning": Cunning += value; break;
			case "Goodness": Goodness += value; break;
			case "Grace": Grace += value; break;
		}
	}
	
	public void ModifyLuck(int value)
	{
		Luck += value;
	}
	
	public void ModifyHealth(int value)
	{
		CurrentHealth += value;
		CurrentHealth = Mathf.Clamp(CurrentHealth, 0, MaxHealth);
	}
	
	public void ModifyMaxHealth(int value)
	{
		MaxHealth += value;
		MaxHealth = Mathf.Max(MaxHealth, 1);
		CurrentHealth = Mathf.Clamp(CurrentHealth, 0, MaxHealth);
	}
	
	public void KillHealth()
	{
		CurrentHealth = 0;
	}
	
	public void ModifyMorale(int value)
	{
		CurrentMorale += value;
		CurrentMorale = Mathf.Clamp(CurrentMorale, 0, MaxMorale);
	}
	
	public void ModifyMaxMorale(int value)
	{
		MaxMorale += value;
		MaxMorale = Mathf.Max(MaxMorale, 1);
		CurrentMorale = Mathf.Clamp(CurrentMorale, 0, MaxMorale);
	}
	
	public void KillMorale()
	{
		CurrentMorale = 0;
	}
}
