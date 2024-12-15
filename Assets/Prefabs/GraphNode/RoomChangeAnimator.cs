using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomChangeAnimator : ScoreAnimator
{
    // Start is called before the first frame update
    void Start()
    {
        foreach (ScoreMapEntry entry in rooms)
        {
            RoomInfo info = new RoomInfo();
            info.root = entry.roomRoot;
            info.ogPosition = info.root.transform.position;
            info.hiddenPosition = info.ogPosition + Vector3.down * 10.0f;
            RegisterInfo(info, entry.team, (int)entry.requiredLevel);
            info.root.transform.position = info.hiddenPosition;
            info.root.SetActive(false);
        }
    }

    void RegisterInfo(RoomInfo info, Team team, int level)
    {
        if (!bakedRooms_.ContainsKey(team))
        {
            bakedRooms_[team] = new List<RoomInfo>();
        }

        while (bakedRooms_[team].Count <= level)
        {
            int count = bakedRooms_[team].Count;
            if (count == 0)
            {
                bakedRooms_[team].Add(new RoomInfo());
            }
            else
            {
                bakedRooms_[team].Add(bakedRooms_[team][count - 1]);
            }
        }

        for (int id = level + 1; id < bakedRooms_[team].Count; ++id)
        {
            if (bakedRooms_[team][id].root == bakedRooms_[team][level].root)
            {
                bakedRooms_[team][id] = info;
            }
            else
            {
                break;
            }
        }

        bakedRooms_[team][level] = info;
    }

    protected override void PresetAnimation(Score newScore)
    {
        RoomInfo newInfo;

        if (!bakedRooms_.ContainsKey(newScore.team))
        {
            bakedRooms_[newScore.team] = new List<RoomInfo>();
        }

        if (!newScore.Teamed())
        {
            newInfo = new RoomInfo();
        }
        else
        {
            int count = bakedRooms_[newScore.team].Count;

            if (newScore.Level() >= count)
            {
                if (count == 0)
                {
                    newInfo = new RoomInfo();
                }
                else
                {
                    newInfo = bakedRooms_[newScore.team][count - 1];
                }
            }
            else
            {
                newInfo = bakedRooms_[newScore.team][(int)newScore.Level()];
            }
        }

        if (currentRoom_.root == newInfo.root)
        {
            return;
        }


        FinalizeAnimation();

        lastRoom_ = currentRoom_;
        currentRoom_ = newInfo;

        if (currentRoom_.root != null)
        {
            currentRoom_.root.SetActive(true);
        }

        animTimeStart_ = Time.time;
    }

    protected override bool TickAnimation()
    {
        float time = Time.time - animTimeStart_;
        float relTime = time / animDuration;

        if (relTime > 1.0f)
        {
            FinalizeAnimation();
            return false;
        }

        float coefficient = Mathf.Sqrt(relTime);

        Vector3 currentPos = Vector3.Lerp(currentRoom_.hiddenPosition, currentRoom_.ogPosition, coefficient);
        Vector3 lastPos = Vector3.Lerp(lastRoom_.ogPosition, lastRoom_.hiddenPosition, coefficient);

        if (currentRoom_.root != null)
        {
            currentRoom_.root.transform.position = currentPos;
        }
        if (lastRoom_.root != null)
        {
            lastRoom_.root.transform.position = lastPos;
        }

        return true;
    }

    void FinalizeAnimation()
    {
        if (currentRoom_.root != null)
        {
            currentRoom_.root.transform.position = currentRoom_.ogPosition;
            currentRoom_.root.SetActive(true);
        }
        if (lastRoom_.root != null)
        {
            lastRoom_.root.transform.position = currentRoom_.hiddenPosition;
            lastRoom_.root.SetActive(false);
        }
    }

    void DrawGizmos()
    {
        Gizmos.color = Color.cyan;
        foreach (ScoreMapEntry entry in rooms)
        {
            if (entry.roomRoot != null)
            {
                Vector3 start = entry.roomRoot.transform.position;
                Gizmos.DrawLine(start, start + submersionVector);
            }
        }
    }

    [System.Serializable]
    public class ScoreMapEntry
    {
        public Team team;
        public uint requiredLevel;
        public GameObject roomRoot;
    }

    public List<ScoreMapEntry> rooms;

    [System.Serializable]
    private struct RoomInfo
    {
        public GameObject root;
        public Vector3 ogPosition;
        public Vector3 hiddenPosition;
    }

    public Vector3 submersionVector = new Vector3(0.0f, -10.0f, 0.0f);
    public float animDuration = 0.2f;

    private Dictionary<Team, List<RoomInfo>> bakedRooms_ = new Dictionary<Team, List<RoomInfo>>();

    private RoomInfo currentRoom_ = new RoomInfo();
    private RoomInfo lastRoom_ = new RoomInfo();
    private float animTimeStart_ = -9999.0f;
}
