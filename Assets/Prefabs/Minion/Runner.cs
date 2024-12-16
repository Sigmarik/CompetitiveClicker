using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Runner : NetworkBehaviour
{
    [Server]
    public void Init(GameObject start, Team team)
    {
        team_ = team;

        var walker = gameObject.GetComponent<GraphWalker>();
        if (walker == null)

        {
            Debug.LogError("Missing `GraphWalker` component.");
            return;
        }

        walker.onArrival = OnArrival;
        walker.onDeparture = OnDeparture;
        walker.onIntArrival = OnIntArrival;
        walker.onIntDeparture = OnIntDeparture;

        walker.Bind(start);
    }

    [Server]
    public void SendTo(GameObject finish)
    {
        TryGetComponent<GraphWalker>(out GraphWalker walker);

        if (walker == null)
        {
            Debug.LogError("Missing `GraphWalker` component.");
            return;
        }

        walker.GoTo(finish);
    }

    public void OnArrival(GameObject target)
    {
        if (TryGetComponent<AnimationController>(out AnimationController animator))
        {
            animator.SetSuccessful(true);
        }

        if (target == null)
        {
            Debug.LogWarning("Minion target is null");
            Destroy(gameObject);
            return;
        }
        target.TryGetComponent<ScoreHolder>(out ScoreHolder scoreHolder);
        if (scoreHolder == null)
        {
            Debug.LogWarning("Can't change score: No ScoreHolder!");
            Destroy(gameObject);
            return;
        }

        scoreHolder.logic.scoreData.Change(team_, 1);
        Destroy(gameObject);
    }

    public void OnDeparture(GameObject target)
    {}

    public void OnIntArrival(GameObject target)
    {
        /* Feature: kill if enemy's cell + ANIMATION OF FAIL */
    }

    public void OnIntDeparture(GameObject target)
    {}

    private Team team_;
}
