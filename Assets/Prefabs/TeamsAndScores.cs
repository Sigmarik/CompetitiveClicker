using System.Collections.Generic;
using Mirror;
using UnityEngine;

// TODO: Rename teams
public enum Team
{
    Default,
    Skeletons,
    Wizards,
    Bandits,
    Knights
}


public class Score
{
    public static readonly Team[] overallTeams = {Team.Skeletons, Team.Wizards, Team.Bandits, Team.Knights}; 

    public Team team = Team.Default;
    public uint score = 0;

    public delegate void changingAction ();
    public changingAction onOwnershipChange;

    public bool Teamed()
    {
        if (score > 0 && team == Team.Default) {

            Debug.Log("Warning! Default command shouldn't got score!");
        }

        if (score == 0 && team != Team.Default) {

            Debug.Log("Warning! Team got zero score!");
        }

        return score > 0 && team != Team.Default;
    }

    private static readonly Dictionary<Team, string> TEAM_NAMES =
    new Dictionary<Team, string>{
        {Team.Default,   "Default"},
        {Team.Skeletons, "Skeletons"},
        {Team.Wizards,   "Wizards"},
        {Team.Bandits,   "Bandits"},
        {Team.Knights,   "Knights"}
    };

    public static readonly Dictionary<Team, Color> TEAM_COLORS =
    new Dictionary<Team, Color>{
        {Team.Default,   new Color(0.75f, 0.75f, 0.75f)},
        {Team.Skeletons, new Color(0.9396226f, 0.8564242f, 0.5513634f)},
        {Team.Wizards,   new Color(1.0f, 0.0f, 0.9592928f)},
        {Team.Bandits,   new Color(0.03493261f, 1.0f, 0.0f)},
        {Team.Knights,   new Color(0.3215685f, 0.9019516f, 1.0f)}
    };

    public Color GetColor()
    {
        return TEAM_COLORS[team];
    }

    public string HolderName()
    {
        return TEAM_NAMES[team];
    }

    public bool Change(Team effector, int change)
    {
        Team oldTeam = team;
        uint oldLevel = Level();

        if (Teamed() && effector == team)
        {
            if (change < 0 && -change >= score)
            {
                score = 0;
            }
            else
            {
                score = (uint)(score + change);
            }
        }
        else
        {
            if (change > 0 && change > score)
            {
                team = effector;
                score = (uint)(change - score);

                onOwnershipChange?.Invoke();
            }
            else if (change == score)
            {
                team = Team.Default;
                score = 0;

                onOwnershipChange?.Invoke();
            }
            else
            {
                score = (uint)(score - change);
            }
        }

        // Return weather the level or the holding team has changed.
        return (Teamed() && team != oldTeam) || Level() != oldLevel;
    }

    public void Set(Team newTeam, uint newScore)
    {
        team = newTeam;
        score = newScore;
    }

    public uint Level()
    {
        // Maximum possible level
        const uint MAX_LEVEL = 4;
        // The bigger this variable is, the slower it is to gain levels
        const uint SCORE_DIVISOR = 10;

        if (!Teamed())
        {
            return 0;
        }

        return 1 + MAX_LEVEL - MAX_LEVEL / (1 + 2 * score / SCORE_DIVISOR);
    }
}
