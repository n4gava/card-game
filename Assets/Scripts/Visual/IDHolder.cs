using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class IDHolder : MonoBehaviour {

    private static List<IDHolder> allIDHolders = new List<IDHolder>();

    public static GameObject GetGameObjectWithID(int id)
    {
        var idHolder = allIDHolders.FirstOrDefault(i => i.UniqueID == id);
        return idHolder == null ? null : idHolder.gameObject;
    }

    public int UniqueID;

    void Awake()
    {
        allIDHolders.Add(this);   
    }

    public static void ClearIDHoldersList()
    {
        allIDHolders.Clear();
    }
}
