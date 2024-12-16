using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RimExplosion : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (!effectCopy_)
        {
            enabled = false;
            selfCopy_ = Instantiate(gameObject, null);
            selfCopy_.transform.position = transform.position;
            selfCopy_.transform.localScale = transform.lossyScale;
            selfCopy_.transform.rotation = transform.rotation;
            selfCopy_.SetActive(false);
            selfCopy_.GetComponent<RimExplosion>().effectCopy_ = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        float time = Time.time - deathTime_;
        float stage = time / animDuration_;

        if (stage > 1.0f)
        {
            Destroy(gameObject);
            return;
        }

        // stage *= stage;

        if (collapseInwards)
        {
            stage *= -1.0f;
        }

        float sizeHor = (1.0f + stage) * Mathf.Exp(stage);
        float sizeVert = (1.0f - stage) / Mathf.Exp(stage);

        transform.localScale = Vector3.Scale(new Vector3(sizeHor, sizeHor, sizeVert), ogScale_);
    }

    private void OnDestroy()
    {
        if (!effectCopy_)
        {
            if (selfCopy_ == null || selfCopy_.IsDestroyed())
            {
                return;
            }
            selfCopy_.SetActive(true);
            selfCopy_.transform.position = transform.position;
            selfCopy_.GetComponent<RimExplosion>().Explode();
        }

    }

    public void OnKilled()
    {
        Destroy(gameObject);
    }

    void Explode()
    {
        enabled = true;
        deathTime_ = Time.time;
        ogScale_ = transform.localScale;
    }

    private bool effectCopy_ = false;

    [HideInInspector]
    public bool collapseInwards = false;
    private float deathTime_ = 0.0f;
    private float animDuration_ = 0.1f;
    private GameObject selfCopy_;
    private Vector3 ogScale_;
}
