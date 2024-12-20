using System.Collections.Generic;
using UnityEngine;

// TODO: Rename teams
public enum Team
{
    Spiders,
    Skeletons,
    Wizards,
    Goblins
}

public class Score
{
    public Team team = Team.Spiders;
    public uint score = 0;

    public bool Teamed()
    {
        return score > 0;
    }

    private static readonly Dictionary<Team, string> TEAM_NAMES =
    new Dictionary<Team, string>{
        {Team.Skeletons, "Skeletons"},
        {Team.Spiders, "Spiders"},
        {Team.Wizards, "Wizards"},
        {Team.Goblins, "Goblins"}
    };

    public static readonly Dictionary<Team, Color> TEAM_COLORS =
    new Dictionary<Team, Color>{
        {Team.Skeletons, new Color(0.9396226f, 0.8564242f, 0.5513634f)},
        {Team.Spiders, new Color(0.03493261f, 1.0f, 0.0f)},
        {Team.Wizards, new Color(1.0f, 0.0f, 0.9592928f)},
        {Team.Goblins, new Color(0.3215685f, 0.9019516f, 1.0f)}
    };
    public static readonly Color NEUTRAL_COLOR = new Color(0.75f, 0.75f, 0.75f);

    public Color GetColor()
    {
        if (!Teamed())
        {
            return NEUTRAL_COLOR;
        }

        return TEAM_COLORS[team];
    }

    public string HolderName()
    {
        if (!Teamed())
        {
            return "None";
        }

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
