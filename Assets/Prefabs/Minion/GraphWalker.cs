using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Splines;

public class GraphWalker : MonoBehaviour
{
    void Start()
    {
        //enabled = false;
    }

    void Update()
    {
        if (hopInfo.stage == HopInfo.HopStage.OnTheWay && inTransition_)
        {
            inTransition_ = false;
            onIntDeparture?.Invoke(currentNode);
        }

        hopInfo.Update(GetNetSynchronizedTime());

        hopInfo.GetTransform(out Vector3 position, out Vector3 tangent);

        transform.position = position;
        transform.forward = tangent;

        if (hopInfo.stage == HopInfo.HopStage.Completed)
        {
            onIntArrival?.Invoke(currentNode);

            UpdateNextHop();

            if (IsClient())
            {
                //* Play "confused" animation or something while
                //* the client is waiting for a package from the server.
            }
        }
    }

    private void UpdateNextHop()
    {
        inTransition_ = true;

        if (!IsServer()) return;

        if (target_ == currentNode)
        {
            enabled = false;
            hopInfo.Reset();
            onArrival?.Invoke(target_);
            return;
        }

        GraphNavigator.RouteEntry entry = currentNode.GetComponent<GraphNavigator>().NextHopTo(target_);
        hopInfo.ScrapFrom(entry);

        currentNode = entry.target;

        // Set departure/arrival times
        hopInfo.departureTime = GetNetSynchronizedTime();
        float hopDuration = hopInfo.spline.GetLength() / speed;
        hopInfo.arrivalTime = hopInfo.departureTime + hopDuration;

        // TODO: NET: Synchronize hop data with clients
    }

    private bool IsServer()
    {
        // TODO: NET: Either implement, or remove.

        return true;
    }
    private bool IsClient()
    {
        // TODO: NET: Either implement, or remove.

        return false;
    }

    // Return a globally synchronized-ish time
    private float GetNetSynchronizedTime()
    {
        // TODO: NET: Implement

        return Time.time;
    }

    public void Bind(GameObject node)
    {
        if (currentNode != null)
        {
            Debug.LogWarning("A `Bind` call on a graph walker which was already bound.");
        }

        currentNode = node;
        hopInfo.Reset();
        target_ = null;
    }

    public void BindAndSend(GameObject start, GameObject finish)
    {
        Bind(start);
        GoTo(finish);
    }

    public void GoTo(GameObject target)
    {
        target_ = target;

        onDeparture?.Invoke(target);

        UpdateNextHop();

        enabled = true;
    }

    [System.Serializable]
    public struct HopInfo
    {
        public enum HopStage
        {
            Completed,
            OnTheWay,
        }

        public GameObject splineObject;
        public Spline spline;  // The spline we are currently following
        public float arrivalTime;  // Local arrival time
        public float departureTime;  // Local departure time
        public float progress;
        public bool splineInverted;
        public HopStage stage { get; private set; }

        public void Reset()
        {
            splineObject = null;
            spline = null;
            arrivalTime = 0.0f;
            departureTime = 0.0f;
            progress = 0.0f;
            splineInverted = false;
        }

        public void GetTransform(out Vector3 position, out Vector3 tangent)
        {
            spline.GetPoint(splineInverted ? 1.0f - progress : progress,
                out position, out tangent);

            position = splineObject.transform.TransformPoint(position);
            tangent = splineObject.transform.TransformDirection(tangent);
            if (splineInverted)
            {
                tangent = -tangent;
            }
        }

        public void ScrapFrom(GraphNavigator.RouteEntry entry)
        {
            splineObject = entry.path;
            spline = entry.path.GetComponent<SplineContainer>()[0];
            progress = 0.0f;
            splineInverted = entry.inverted;

            // Caller-filled
            arrivalTime = 0.0f;
            departureTime = 0.0f;

            stage = HopStage.OnTheWay;
        }

        public void Update(float time)
        {
            progress = (time - departureTime) / arrivalTime;

            if (progress > 1.0f)
            {
                stage = HopStage.Completed;
            }
            else
            {
                stage = HopStage.OnTheWay;
            }

            progress = Mathf.Clamp01(progress);
        }
    }

    // Global routing target
    private GameObject target_;

    // The node to which the minion "belongs to" (the node where the current hop leads)
    public GameObject currentNode;
    public HopInfo hopInfo;
    private bool inTransition_ = true;

    public delegate void OnArrival(GameObject target);
    public delegate void OnDeparture(GameObject target);
    public delegate void OnIntArrival(GameObject target);
    public delegate void OnIntDeparture(GameObject target);

    // Called when the object arrives at its final destination
    public OnArrival onArrival;
    // Called when the object departs to its final destination
    public OnDeparture onDeparture;
    // Called every time the object arrives into a node
    public OnIntArrival onIntArrival;
    // Called every time the object departs from a node
    public OnIntDeparture onIntDeparture;

    public float speed = 4.0f;  // meters per second
}
