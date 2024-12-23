using UnityEngine;

public class AnimationController : MonoBehaviour
{
    void Start()
    {
        animator_ = GetComponent<Animator>();
        position_ = transform.position;
    }

    void Update()
    {
        Vector3 newPos = transform.position;
        Vector3 velocity = (newPos - position_) / Time.deltaTime;
        bool isMoving = velocity.magnitude > speedThreshold;
        UpdateAnimationState(isMoving);
        animator_.Update(Time.deltaTime);
        position_ = newPos;
    }

    private void UpdateAnimationState(bool isMoving)
    {
        animator_.SetBool("isMoving", isMoving);
    }

    // Changes the death animation from outwards rim to inwards beam.
    public void PlayAnimation(bool successful = true)
    {
        RimExplosion explosion = GetComponentInChildren<RimExplosion>();
        if (explosion != null)
        {
            explosion.PlayAnimation(successful);
        }
    }

    private Animator animator_;
    private Vector3 position_;
    public float speedThreshold = 0.1f;
}