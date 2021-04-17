using UnityEngine;

public static class IDHolderExtensions
{
    public static IDHolder GetObjectIDHolder(this GameObject gameObject)
    {
        var idHolder = gameObject.GetComponent<IDHolder>();
        if (idHolder == null)
            idHolder = gameObject.GetComponentInParent<IDHolder>();

        return idHolder;
    }

    public static IDHolder GetObjectIDHolder(this MonoBehaviour monoBehaviour)
    {
        return monoBehaviour.gameObject.GetObjectIDHolder();
    }

    public static int GetObjectUniqueID(this GameObject gameObject)
    {
        var idHolder = gameObject.GetObjectIDHolder();
        return idHolder == null ? -1 : idHolder.UniqueID;
    }

    public static int GetObjectUniqueID(this MonoBehaviour monoBehaviour)
    {
        return monoBehaviour.gameObject.GetObjectUniqueID();
    }
}
