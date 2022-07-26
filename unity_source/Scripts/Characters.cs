// Patrol.cs
using UnityEngine;
using UnityEngine.AI;
using System.Collections;


public class Characters : MonoBehaviour
{
    public int PointDeVie = 3;

    public Transform[] points;
    private int destPoint = 0;
    private NavMeshAgent agent;
    private Animator anim;
    Vector2 smoothDeltaPosition = Vector2.zero;
    Vector2 velocity = Vector2.zero;

    bool wasHit = false;
    bool hasStartedHitAnim = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        // Disabling auto-braking allows for continuous movement
        // between points (ie, the agent doesn't slow down as it
        // approaches a destination point).
        //agent.autoBraking = false;
        // Don’t update position automatically
        agent.updatePosition = false;
    }

    private void FixedUpdate()
    {
        if (wasHit && anim.GetCurrentAnimatorStateInfo(0).IsName("Hit"))
            hasStartedHitAnim = true;
        if (wasHit && hasStartedHitAnim && !anim.GetCurrentAnimatorStateInfo(0).IsName("Hit"))
        {
            hasStartedHitAnim = false;
            wasHit = false;
            agent.enabled = true;
        }
    }

    void Update()
    {
        if (!wasHit && points.Length != 0)
        {
            Patrol();
        }
    }

    void Patrol()
    {
        // Choose the next destination point when the agent gets
        // close to the current one.
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
            GotoNextPoint();

        Vector3 worldDeltaPosition = agent.nextPosition - transform.position;

        // Map 'worldDeltaPosition' to local space
        float dx = Vector3.Dot(transform.right, worldDeltaPosition);
        float dy = Vector3.Dot(transform.forward, worldDeltaPosition);
        Vector2 deltaPosition = new Vector2(dx, dy);

        // Low-pass filter the deltaMove
        float smooth = Mathf.Min(1.0f, Time.deltaTime / 0.15f);
        smoothDeltaPosition = Vector2.Lerp(smoothDeltaPosition, deltaPosition, smooth);

        // Update velocity if time advances
        if (Time.deltaTime > 1e-5f)
            velocity = smoothDeltaPosition / Time.deltaTime;

        bool shouldMove = velocity.magnitude > 0.5f && agent.remainingDistance > agent.radius;

        // Update animation parameters
        anim.SetBool("move", shouldMove);
        anim.SetFloat("velx", velocity.x);
        anim.SetFloat("vely", velocity.y);
    }

    void GotoNextPoint()
    {
        // Returns if no points have been set up
        if (points.Length == 0)
            return;

        // Set the agent to go to the currently selected destination.
        agent.destination = points[destPoint].position;

        // Choose the next point in the array as the destination,
        // cycling to the start if necessary.
        destPoint = (destPoint + 1) % points.Length;
    }

    void OnAnimatorMove()
    {
        // Update position to agent position
        if (!wasHit)
            transform.position = agent.nextPosition;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Sword" && !wasHit)
        {
            if (PointDeVie > 1)
            {
                wasHit = true;
                anim.SetTrigger("Hit");
                agent.enabled = false;
                PointDeVie--;
            }
            else
            {
                wasHit = true;
                anim.SetTrigger("Die");
                agent.enabled = false;
            }

        }
    }
}
