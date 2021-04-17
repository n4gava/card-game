using UnityEngine;

/// <summary>
/// This script should be attached to the card game object to display card`s rotation correctly.
/// </summary>

[ExecuteInEditMode]
public class BetterCardRotation : MonoBehaviour {

    public RectTransform CardFront;
    public RectTransform CardBack;

    void Update()
    {
        if (Camera.main == null)
            return;

        var showFront = Vector3.Dot(transform.forward, Camera.main.transform.forward) > 0;
        CardFront.gameObject.SetActive(showFront);
        CardBack.gameObject.SetActive(!showFront);
    }
}
