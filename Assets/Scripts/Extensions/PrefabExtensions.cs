using UnityEngine;

public static class PrefabExtensions
{
    public static GameObject GetRootPrefab(this MonoBehaviour monoBehavior)
    {
        var idHolder = monoBehavior.GetObjectIDHolder();
        return idHolder == null ? monoBehavior.gameObject : idHolder.gameObject;
    }
}
