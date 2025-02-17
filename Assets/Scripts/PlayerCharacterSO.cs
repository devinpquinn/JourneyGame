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

    public int Health = 3;
    public int Morale = 3;

    public void ModifyHealth(int amount)
    {
        Health = Mathf.Max(0, Health + amount); // Prevent negative health
    }

    public void ModifyMorale(int amount)
    {
        Morale = Mathf.Max(0, Morale + amount); // Prevent negative morale
    }

    public int GetVirtueScore(string virtue)
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
}
