using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DBG_RoomSwitchDebugger : MonoBehaviour
{
    void Start()
    {
        roomChanger_ = GetComponent<ScoreAnimator>();
    }

    private void OnMouseDown()
    {
        Score score = new Score();
        Team[] teams = Score.overallTeams;
        score.team = teams[Random.Range(0, 4)];
        score.score = (uint)Random.Range(0, 20);

        Debug.Log("Set team to " + score.team + ", score to " + score.score + ", and level to " + score.Level());

        roomChanger_.UpdateScore(score);
    }

    private ScoreAnimator roomChanger_;
}
