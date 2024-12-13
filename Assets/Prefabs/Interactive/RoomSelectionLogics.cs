using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomSelectionLogics : MonoBehaviour
{
    private void Start()
    {
        objectRenderer_ = GetComponent<Renderer>();
        objectRenderer_.material.SetFloat("_Top", 0.0f);
        currentLevel_ = 0.0f;
    }

    public void SetTeamColor(Color color)
    {
        teamColor = color;
        objectRenderer_.material.SetColor("_Color", teamColor);
    }

    public Color GetColor()
    {
        return teamColor;
    }

    void Update()
    {
        float ogLevel = currentLevel_;
        currentLevel_ += Mathf.Sign(targetLevel_ - currentLevel_) *
            Time.deltaTime / animDuration * accelerator_;
        if ((currentLevel_ - targetLevel_) * (ogLevel - currentLevel_) < 0.0f)
        {
            currentLevel_ = targetLevel_;
        }

        objectRenderer_.material.SetFloat("_Top", currentLevel_);

        if (currentLevel_ == targetLevel_)
        {
            enabled = false;
        }
    }

    private void PlayAnimation(float target, float accelerator = 1.0f)
    {
        targetLevel_ = target;
        accelerator_ = accelerator;
        enabled = true;
    }

    private void OnMouseEnter()
    {
        PlayAnimation(1.0f);
    }

    private void OnMouseExit()
    {
        PlayAnimation(0.0f);
    }

    private void OnMouseDown()
    {
        currentLevel_ = 0.8f;
        PlayAnimation(1.0f, 0.1f);
    }

    private float accelerator_ = 1.0f;

    private float currentLevel_ = 0.0f;
    private float targetLevel_ = 0.0f;
    private Renderer objectRenderer_;

    public float animDuration = 0.1f;

    public Color teamColor = Color.white;
}
