using UnityEngine;

public static class TargetingOptionsExtensions
{
    public static bool IsTargetValid(this TargetingOptions targetOptions, Player playerOwner, GameObject targetObject)
    {
        if (targetObject == null)
            return false;

        var targetIsEnemy = targetObject.tag.IsEnemy(playerOwner.gameObject);

        switch (targetOptions)
        {
            case TargetingOptions.AllCharacters:
                return true;
            case TargetingOptions.AllCreatures:
                return targetObject.tag.IsCreature();
            case TargetingOptions.EnemyCharacters:
                return targetIsEnemy && (targetObject.tag.IsCreature() || targetObject.tag.IsPlayer());
            case TargetingOptions.EnemyCreatures:
                return targetIsEnemy && targetObject.tag.IsCreature();
            case TargetingOptions.YourCharacters:
                return !targetIsEnemy && (targetObject.tag.IsCreature() || targetObject.tag.IsPlayer());
            case TargetingOptions.YourCreatures:
                return !targetIsEnemy && targetObject.tag.IsCreature();
            default:
                return false;
        }
    }
}
