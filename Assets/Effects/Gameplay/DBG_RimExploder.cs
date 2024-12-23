using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DBG_RimExploder : MonoBehaviour
{
    void OnMouseDown()
    {
        GetComponentInChildren<RimExplosion>().PlayAnimation(true);
    }
}
