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
            secondInstance = Instantiate(gameObject, transform);
            DBG_CharacterPresentation presentation = secondInstance.GetComponent<DBG_CharacterPresentation>();
            presentation.cloned_ = true;
            secondInstance.transform.localPosition = Vector3.zero;
            secondInstance.transform.localScale = Vector3.one;
            enabled = false;
        }
    }

    void Update()
    {
        if (!cloned_)
        {
            return;
        }

        transform.position = center_ +
            Vector3.forward * Mathf.Sin(Time.time) * radius +
            Vector3.right * Mathf.Cos(Time.time) * radius;
        transform.rotation = Quaternion.LookRotation(
            -Vector3.right * Mathf.Sin(Time.time) * radius +
            Vector3.forward * Mathf.Cos(Time.time) * radius, Vector3.up);
    }

    private GameObject secondInstance;

    private Vector3 center_;
    public float radius = 5.0f;

    private bool cloned_ = false;
}
