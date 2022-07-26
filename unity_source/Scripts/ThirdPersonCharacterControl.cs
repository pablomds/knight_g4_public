using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCharacterControl : MonoBehaviour
{
    public float WalkSpeed;
    public float RunSpeed;

    private Animator animator;
    private Collider swordCollider;

    private void Start()
    {
        animator = GetComponent<Animator>();
        swordCollider = GameObject.FindGameObjectWithTag("Sword").GetComponent<Collider>();
        swordCollider.enabled = false;
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            Debug.Break();
        }
        PlayerMovement();
    }

    void PlayerMovement()
    {
        var isRunning = Input.GetKey(KeyCode.LeftShift);
        var speed = isRunning ? RunSpeed : WalkSpeed;
        //float hor = Input.GetAxis("Horizontal");
        float ver = Mathf.Clamp(Input.GetAxis("Vertical"), 0, isRunning ? 1f : 0.5f);

        //animator.SetFloat("Side", hor/2f);
        animator.SetFloat("Forward", ver, 0.1f, Time.deltaTime);

        Vector3 playerMovement = new Vector3(0f, 0f, ver) * speed * Time.deltaTime;
        transform.Translate(playerMovement, Space.Self);

        
        if (swordCollider.enabled && !animator.GetCurrentAnimatorStateInfo(1).IsTag("Attack"))
        {
            //Debug.Break();
            swordCollider.enabled = false;
        }

        if (Input.GetMouseButtonDown(0))
        {
            animator.SetTrigger("Attack");
            swordCollider.enabled = true;
        }
    }
}