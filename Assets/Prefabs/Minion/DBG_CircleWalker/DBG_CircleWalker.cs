using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DBG_CircleWalker : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        center_ = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = center_ +
            Vector3.forward * Mathf.Sin(Time.time) * radius +
            Vector3.right * Mathf.Cos(Time.time) * radius;
        transform.rotation = Quaternion.LookRotation(
            -Vector3.right * Mathf.Sin(Time.time) * radius +
            Vector3.forward * Mathf.Cos(Time.time) * radius, Vector3.up);
    }

    private Vector3 center_;
    public float radius = 3.0f;
}
