using Mirror;
using UnityEngine;
using UnityEngine.Splines;

public class GraphWalker : NetworkBehaviour
{
    void Update()
    {
        if (!initialized_) {
            return;
        }

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

            if (isClient) {
                //* Play "confused" animation or something while
                //* the client is waiting for a package from the server.
            }
        }
    }

    private float GetNetSynchronizedTime()
    {
        return (float) NetworkTime.time;
    }

    private void UpdateNextHop()
    {
        inTransition_ = true;

        if (!isServer) return;

        if (target_ == currentNode)
        {
            enabled = false;
            hopInfo.Reset();
            onArrival?.Invoke(target_);
            return;
        }

        GraphNavigator.RouteEntry entry = currentNode.GetComponent<GraphNavigator>().NextHopTo(target_);
        hopInfo.ScrapFrom(entry);

        var current_prev = currentNode;
        currentNode = entry.target;

        // Set departure/arrival times
        hopInfo.departureTime = GetNetSynchronizedTime();
        float hopDuration = hopInfo.spline.GetLength() / speed;
        hopInfo.arrivalTime = hopInfo.departureTime + hopDuration;

        // Send nexthop info
        RpcSetNextHop(current_prev, entry.target, hopInfo.arrivalTime);
    }

    [ClientRpc]
    private void RpcSetNextHop(GameObject current_, GameObject nexthop, float arrivalTime)
    {
        // Because we don't want run this code in host mode
        if (isServer) {
            return;
        }

        initialized_ = true;

        currentNode = current_;
        GraphNavigator.RouteEntry entry = currentNode.GetComponent<GraphNavigator>().NextHopTo(nexthop);
        hopInfo.ScrapFrom(entry);

        currentNode = entry.target;
        hopInfo.departureTime = GetNetSynchronizedTime();
        hopInfo.arrivalTime = arrivalTime;
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

    [Server]
    public void GoTo(GameObject target)
    {
        target_ = target;

        onDeparture?.Invoke(target);

        UpdateNextHop();

        initialized_ = true;
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
            progress = (time - departureTime) / (arrivalTime - departureTime);

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

    private bool initialized_ = false;

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
