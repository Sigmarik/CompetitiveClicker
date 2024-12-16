using Unity.VisualScripting;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    void Start()
    {
        RecalculateOrigin();

        deltaPosition_ = transform.position - anchorPoint_;
        zoomRatio_.y = deltaPosition_.y;
        zoomRatio_.x = Vector3.ProjectOnPlane(deltaPosition_, Vector3.up).magnitude;
        delayedAnchor_ = anchorPoint_;
        delayedDelta_ = deltaPosition_;
    }

    void Update()
    {
        transform.rotation = Quaternion.LookRotation(-delayedDelta_, Vector3.up);
        anchorPoint_ += GetMovementInput() * Time.deltaTime * movementSpeed * Mathf.Exp(zoom_);
        anchorPoint_.x = Mathf.Clamp(anchorPoint_.x, -areaBounds, areaBounds);
        anchorPoint_.z = Mathf.Clamp(anchorPoint_.z, -areaBounds, areaBounds);
        ApplyZoomInput();
        ApplyRotationalInput();
        delayedAnchor_ = Vector3.Lerp(anchorPoint_, delayedAnchor_, 0.95f);
        delayedDelta_ = Vector3.Lerp(deltaPosition_, delayedDelta_, 0.95f);
        transform.position = delayedAnchor_ + delayedDelta_;
    }

    Vector3 GetMovementInput()
    {
        float forwardInput = 0.0f;
        float strafeInput = 0.0f;
        if (Input.GetKey(KeyCode.W))
        {
            forwardInput += 1.0f;
        }
        if (Input.GetKey(KeyCode.S))
        {
            forwardInput -= 1.0f;
        }

        if (Input.GetKey(KeyCode.D))
        {
            strafeInput += 1.0f;
        }
        if (Input.GetKey(KeyCode.A))
        {
            strafeInput -= 1.0f;
        }

        Vector3 forward = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
        Vector3 right = Vector3.ProjectOnPlane(transform.right, Vector3.up).normalized;

        return forward * forwardInput + right * strafeInput;
    }

    void ApplyRotationalInput()
    {
        float rotInput = 0.0f;
        if (Input.GetKey(KeyCode.Q))
        {
            rotInput += 1.0f;
        }
        if (Input.GetKey(KeyCode.E))
        {
            rotInput -= 1.0f;
        }

        Quaternion rotation = Quaternion.Euler(0, rotInput * rotationSpeed * Time.deltaTime, 0);
        deltaPosition_ = rotation * deltaPosition_;
    }

    void ApplyZoomInput()
    {
        float input = -Input.GetAxis("Mouse ScrollWheel");
        zoom_ += input;

        zoom_ = Mathf.Clamp(zoom_, minZoom, maxZoom);

        float newX = Mathf.Exp(zoom_) * zoomRatio_.x;
        float newY = newX * newX / zoomRatio_.x / zoomRatio_.x * zoomRatio_.y;

        Vector3 forward = Vector3.ProjectOnPlane(-deltaPosition_, Vector3.up).normalized;

        deltaPosition_ = -forward * newX + Vector3.up * newY;
    }

    private void OnDrawGizmos()
    {
        float boxHeight = 5.0f;
        Gizmos.color = new Color(0.5f, 0.5f, 0.5f);
        Gizmos.DrawWireCube(Vector3.up * boxHeight / 2.0f, new Vector3(areaBounds * 2.0f, boxHeight, areaBounds * 2.0f));

        RecalculateOrigin();

        Gizmos.DrawWireSphere(anchorPoint_, 0.5f);
    }

    private void RecalculateOrigin()
    {
        anchorPoint_.y = viewLevel;
        anchorPoint_.x = transform.position.x;
        anchorPoint_.z = transform.position.z;

        Vector3 flattened = Vector3.ProjectOnPlane(transform.forward, Vector3.up);

        if (transform.forward.y >= 0)
        {
            return;
        }

        float fraction = (transform.position.y - viewLevel) / transform.forward.y;
        anchorPoint_ += -flattened * fraction;

        deltaPosition_ = transform.position - anchorPoint_;
    }

    public float areaBounds = 10.0f;
    public float movementSpeed = 30.0f;
    public float rotationSpeed = 180.0f;
    public float viewLevel = 1.0f;
    public float minZoom = -1.0f;
    public float maxZoom = 0.3f;

    private Vector3 anchorPoint_;
    private Vector3 delayedAnchor_;
    private Flow slopeMultiplier_;
    private Vector3 deltaPosition_;
    private Vector3 delayedDelta_;

    private Vector2 zoomRatio_;
    private float zoom_ = 0.0f;
}
