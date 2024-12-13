using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DBG_TeamColorDebugger : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        selector_ = GetComponent<RoomSelectionLogics>();
    }

    private void OnMouseDown()
    {
        Debug.Log("Clicked");
        selector_.SetTeamColor(Random.ColorHSV(0.0f, 1.0f));
    }

    private RoomSelectionLogics selector_;
}
