using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using System.Linq;

public class TableVisual : MonoBehaviour 
{
    public static bool CursorOverSomeTable =>  GameObject.FindObjectsOfType<TableVisual>().Any(table => table.CursorOverThisTable);

    public AreaPosition owner;
    public float tableWidth;
    public float maxCreaturesGap = 2f;

    private List<GameObject> CreaturesOnTable = new List<GameObject>();
    private bool cursorOverThisTable = false;

    private BoxCollider boxCollider;


    public bool CursorOverThisTable => cursorOverThisTable;


    void Awake()
    {
        boxCollider = GetComponent<BoxCollider>();
        CalculateCardPositions();
    }

    void Update()
    {
        var hits = Physics.RaycastAll(Camera.main.ScreenPointToRay(Input.mousePosition), 30f);

        bool passedThroughTableCollider = false;
        foreach (RaycastHit h in hits)
        {
            if (h.collider == boxCollider)
                passedThroughTableCollider = true;
        }
        cursorOverThisTable = passedThroughTableCollider;
    }
   
    private void CalculateCardPositions()
    {
        var creaturesOnTable = CreaturesOnTable.Count();
        if (creaturesOnTable == 0)
            return;

        var creaturesGap = tableWidth / (creaturesOnTable + 1);
        creaturesGap = creaturesGap > maxCreaturesGap ? maxCreaturesGap : creaturesGap;

        var leftPadding = ((creaturesOnTable - 1) / 2f) * creaturesGap * -1;
        var i = 0;
        foreach (var creature in CreaturesOnTable)
        {
            creature.transform.localPosition = new Vector3(leftPadding, 0, 0);

            WhereIsTheCardOrCreature whereIsCreature = creature.GetComponent<WhereIsTheCardOrCreature>();
            whereIsCreature.Slot = i;
            whereIsCreature.VisualState = owner == AreaPosition.Low ? VisualStates.LowTable : VisualStates.TopTable;

            creature.transform.rotation = Quaternion.identity;
            leftPadding += creaturesGap;
            i++;
        }
    }
    // method to create a new creature and add it to the table
    public void AddCreatureAtIndex(CardAsset ca, int UniqueID ,int index)
    {
        GameObject creature = GameObject.Instantiate(GlobalSettings.Instance.CreaturePrefab, this.transform.position, Quaternion.identity) as GameObject;

        OneCreatureManager manager = creature.GetComponent<OneCreatureManager>();
        manager.cardAsset = ca;
        manager.LoadFromCardAsset();

        foreach (Transform t in creature.GetComponentsInChildren<Transform>())
            t.tag = owner.ToString()+"Creature";
        
        creature.transform.SetParent(this.transform);

        CreaturesOnTable.Insert(index, creature);

        IDHolder id = creature.AddComponent<IDHolder>();
        id.UniqueID = UniqueID;

        CalculateCardPositions();

        // end command execution
        Command.CommandExecutionComplete();
    }


    // returns an index for a new creature based on mousePosition
    // included for placing a new creature to any positon on the table
    public int TablePosForNewCreature(float MouseX)
    {
        return 0;
        /*
        // if there are no creatures or if we are pointing to the right of all creatures with a mouse.
        // right - because the table slots are flipped and 0 is on the right side.
        if (CreaturesOnTable.Count == 0 || MouseX > slots.Children[0].transform.position.x)
            return 0;
        else if (MouseX < slots.Children[CreaturesOnTable.Count - 1].transform.position.x) // cursor on the left relative to all creatures on the table
            return CreaturesOnTable.Count;
        for (int i = 0; i < CreaturesOnTable.Count; i++)
        {
            if (MouseX < slots.Children[i].transform.position.x && MouseX > slots.Children[i + 1].transform.position.x)
                return i + 1;
        }
        Debug.Log("Suspicious behavior. Reached end of TablePosForNewCreature method. Returning 0");
        return 0;
        */
    }

    // Destroy a creature
    public void RemoveCreatureWithID(int IDToRemove)
    {
        GameObject creatureToRemove = IDHolder.GetGameObjectWithID(IDToRemove);
        CreaturesOnTable.Remove(creatureToRemove);
        Destroy(creatureToRemove);
        CalculateCardPositions();
        Command.CommandExecutionComplete();
    }
}
