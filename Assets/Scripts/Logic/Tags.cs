using UnityEngine;

public static class Tags
{
    public static string Low = "Low";
    public static string Top = "Top";
    public static string TopCard = "TopCard";
    public static string LowCard = "LowCard";
    public static string LowCreature = "LowCreature";
    public static string TopCreature = "TopCreature";
    public static string LowPlayer = "LowPlayer";
    public static string TopPlayer = "TopPlayer";

    public static bool IsCreature(this string tag)
    {
        return tag == LowCreature || tag == TopCreature;
    }

    public static bool IsPlayer(this string tag)
    {
        return tag == LowPlayer || tag == TopPlayer;
    }

    public static bool IsFromLow(this string tag)
    {
        return tag.Contains(Low);
    }

    public static bool IsFromTop(this string tag)
    {
        return tag.Contains(Top);
    }

    public static bool IsEnemy(this string tag, GameObject gameobject)
    {
        return (gameobject.tag.IsFromLow() && tag.IsFromTop() ||
                gameobject.tag.IsFromTop() && tag.IsFromLow());
    }

    public static Player GetOwnerFromTag(this string tag)
    {
        return tag.IsFromLow() ? GlobalSettings.Instance.LowPlayer : GlobalSettings.Instance.TopPlayer;
    }
}