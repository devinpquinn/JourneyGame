using UnityEngine;

public static class Hero
{
    public static int CurrentHealth = 3;
    public static int MaxHealth = 3;
    public static int CurrentMorale = 3;
    public static int MaxMorale = 3;

    public static int Strength = 35;
    public static int Courage = 35;
    public static int Wisdom = 35;
    public static int Grace = 35;
    public static int Luck = 35;

    public static void InitializeDefaults()
    {
        MaxHealth = 3;
        CurrentHealth = 3;
        MaxMorale = 3;
        CurrentMorale = 3;

        Strength = 35;
        Courage = 35;
        Wisdom = 35;
        Grace = 35;
        Luck = 35;
    }

    public static void ResetRoundVitals()
    {
        CurrentHealth = MaxHealth;
        CurrentMorale = MaxMorale;
    }

    public static int GetAttributeValue(HeroAttribute attribute)
    {
        switch (attribute)
        {
            case HeroAttribute.Strength:
                return Strength;
            case HeroAttribute.Courage:
                return Courage;
            case HeroAttribute.Wisdom:
                return Wisdom;
            case HeroAttribute.Grace:
                return Grace;
            case HeroAttribute.Luck:
                return Luck;
            default:
                return 0;
        }
    }

    public static int AddAttribute(HeroAttribute attribute, int amount)
    {
        int before = GetAttributeValue(attribute);
        int after = Mathf.Clamp(before + amount, 0, 100);

        switch (attribute)
        {
            case HeroAttribute.Strength:
                Strength = after;
                break;
            case HeroAttribute.Courage:
                Courage = after;
                break;
            case HeroAttribute.Wisdom:
                Wisdom = after;
                break;
            case HeroAttribute.Grace:
                Grace = after;
                break;
            case HeroAttribute.Luck:
                Luck = after;
                break;
        }

        return after - before;
    }

    public static int AddHealth(int amount)
    {
        int before = CurrentHealth;
        CurrentHealth = Mathf.Clamp(CurrentHealth + amount, 0, MaxHealth);
        return CurrentHealth - before;
    }

    public static int AddMorale(int amount)
    {
        int before = CurrentMorale;
        CurrentMorale = Mathf.Clamp(CurrentMorale + amount, 0, MaxMorale);
        return CurrentMorale - before;
    }

    public static int AddMaxHealth(int amount)
    {
        int before = MaxHealth;
        MaxHealth = Mathf.Max(1, MaxHealth + amount);
        CurrentHealth = Mathf.Min(CurrentHealth + amount, MaxHealth);
        return MaxHealth - before;
    }

    public static int AddMaxMorale(int amount)
    {
        int before = MaxMorale;
        MaxMorale = Mathf.Max(1, MaxMorale + amount);
        CurrentMorale = Mathf.Min(CurrentMorale + amount, MaxMorale);
        return MaxMorale - before;
    }
}
