using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum VisualStates
{
    Transition,
    LowHand, 
    TopHand,
    LowTable,
    TopTable,
    Dragging
}

public class WhereIsTheCardOrCreature : MonoBehaviour {

    public HoverPreview hoverPreview;
    public Canvas cardCanvas;

    public int Slot { get; set; }

    private VisualStates state;
    public VisualStates VisualState
    {
        get{ return state; }  

        set
        {
            state = value;
            switch (state)
            {
                case VisualStates.LowHand:
                    hoverPreview.ThisPreviewEnabled = true;
                    break;
                case VisualStates.LowTable:
                case VisualStates.TopTable:
                    hoverPreview.ThisPreviewEnabled = true; 
                    break;
                case VisualStates.Transition:
                    hoverPreview.ThisPreviewEnabled = false;
                    break;
                case VisualStates.Dragging:
                    hoverPreview.ThisPreviewEnabled = false;
                    break;
                case VisualStates.TopHand:
                    hoverPreview.ThisPreviewEnabled = false;
                    break;
            }
        }
    }


    public void BringToFront()
    {
        cardCanvas.sortingLayerName = "AboveEverything";
    }

    public void BringToHandCards()
    {
        cardCanvas.sortingLayerName = "Cards";
    }

    public void BringToCreaturesTable()
    {
        cardCanvas.sortingLayerName = "Creatures";
    }
}
