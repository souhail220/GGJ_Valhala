using System;
using System.Collections;
using UnityEngine;

public class BossWalking : MonoBehaviour
{
    [Header("Movement")]
    public Transform[] patrolPoints;
    public Transform player;

    public float walkSpeed = 2f;
    public float runSpeed = 6f;
    public float detectionRange = 8f;
    public float stopDistance = 1.5f;

    private int currentPoint = 0;
    private Animator animator;
    [HideInInspector] public bool canMove = false;

    private enum BossState { Idle, Walking, Running, Fighting }
    private BossState currentState = BossState.Idle;

    void Start()
    {
        animator = GetComponent<Animator>();
        StartCoroutine(StartIntro());
    }

    IEnumerator StartIntro()
    {
        animator.Play("sittingIdle_com");
        yield return new WaitForSeconds(2f);

        animator.Play("standing_com");
        yield return new WaitForSeconds(2f);

        animator.Play("jumping");
        yield return new WaitForSeconds(2f);

        animator.Play("standing_com 0");
        yield return new WaitForSeconds(2.5f);

        canMove = true;
        SetState(BossState.Walking);
    }

    void Update()
    {
        if (!canMove) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance < detectionRange)
        {
            if (distance > stopDistance)
            {
                SetState(BossState.Running);
                Move(player.position, runSpeed);
            }
            else
            {
                SetState(BossState.Idle);
            }
        }
        else
        {
            SetState(BossState.Walking);
            Patrol();
        }
    }

    public void StopBoss()
    {
        canMove = false;

        SetState(BossState.Idle);

        // Optional: play idle animation
        animator.Play("standing_com"); // or "Idle" state in your Animator
    }


    void SetState(BossState newState)
    {
        if (currentState == newState) return;
        currentState = newState;

        switch (currentState)
        {
            case BossState.Idle:
                animator.SetBool("IsWalking", false);
                animator.SetBool("IsRunning", false);
                break;

            case BossState.Walking:
                animator.SetBool("IsWalking", true);
                animator.SetBool("IsRunning", false);
                break;

            case BossState.Running:
                animator.SetBool("IsWalking", true); // keep true for blend
                animator.SetBool("IsRunning", true);
                break;
        }
    }

    void Patrol()
    {
        Transform target = patrolPoints[currentPoint];
        Move(target.position, walkSpeed);

        if (Vector3.Distance(transform.position, target.position) < 0.5f)
        {
            currentPoint = (currentPoint + 1) % patrolPoints.Length;
        }
    }

    void Move(Vector3 target, float speed)
    {
        Vector3 dir = (target - transform.position).normalized;
        //dir.y = 2f;
        //dir.Normalize();

        transform.position += dir * speed * Time.deltaTime;

        if (dir.sqrMagnitude > 0.01f)
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(dir),
                10f * Time.deltaTime
            );
        }
    }

    public void ResumeBoss()
    {
        throw new NotImplementedException();
    }
}
