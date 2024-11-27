using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Score visualizer
// Something like room reassembly animation should be implemented here.

public class ScoreAnimator : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        bool running = TickAnimation();
        if (running)
        {
            DisableUpdates();
        }
    }

    public void UpdateScore(Score newScore)
    {
        PresetAnimation(newScore);
        EnableUpdates();
    }

    protected virtual void PresetAnimation(Score newScore)
    {
        // Do nothing...
    }

    protected virtual bool TickAnimation()
    {
        // Immediately finish the "animation"

        return false;
    }

    private void DisableUpdates()
    {
        enabled = false;
    }

    private void EnableUpdates()
    {
        enabled = true;
    }
}
