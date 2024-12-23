using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DBG_CharacterPresentation : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        center_ = transform.position;

        if (!cloned_)
        {
            secondInstance = Instantiate(gameObject, null);
            DBG_CharacterPresentation presentation = secondInstance.GetComponent<DBG_CharacterPresentation>();
            presentation.cloned_ = true;
            enabled = false;
        }
    }

    void Update()
    {
        if (!cloned_)
        {
            return;
        }

        float portion = Mathf.Sin(Time.time / 5.0f) * 5.0f;

        transform.position = center_ +
            Vector3.forward * Mathf.Sin(portion) * radius +
            Vector3.right * Mathf.Cos(portion) * radius;
        transform.rotation = Quaternion.LookRotation(
            -Vector3.right * Mathf.Sin(portion) * radius +
            Vector3.forward * Mathf.Cos(portion) * radius, Vector3.up);
    }

    private GameObject secondInstance;

    private Vector3 center_;
    public float radius = 5.0f;

    private bool cloned_ = false;
}
