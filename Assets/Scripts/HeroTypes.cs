public enum HeroAttribute
{
    None = 0,
    Strength = 1,
    Courage = 2,
    Wisdom = 3,
    Grace = 4,
    Luck = 5
}

public enum HeroEffectTarget
{
    Strength = 0,
    Courage = 1,
    Wisdom = 2,
    Grace = 3,
    Luck = 4,
    Health = 5,
    Morale = 6,
    Progress = 7
}

public static class HeroNames
{
    public static string Attribute(HeroAttribute attribute)
    {
        switch (attribute)
        {
            case HeroAttribute.Strength:
                return "Strength";
            case HeroAttribute.Courage:
                return "Courage";
            case HeroAttribute.Wisdom:
                return "Wisdom";
            case HeroAttribute.Grace:
                return "Grace";
            case HeroAttribute.Luck:
                return "Luck";
            default:
                return "Unknown";
        }
    }

    public static string EffectTarget(HeroEffectTarget target)
    {
        switch (target)
        {
            case HeroEffectTarget.Strength:
                return "Strength";
            case HeroEffectTarget.Courage:
                return "Courage";
            case HeroEffectTarget.Wisdom:
                return "Wisdom";
            case HeroEffectTarget.Grace:
                return "Grace";
            case HeroEffectTarget.Luck:
                return "Luck";
            case HeroEffectTarget.Health:
                return "Health";
            case HeroEffectTarget.Morale:
                return "Morale";
            case HeroEffectTarget.Progress:
                return "Progress";
            default:
                return "Unknown";
        }
    }
}