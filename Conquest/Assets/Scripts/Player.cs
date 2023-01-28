using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class Player : MonoBehaviour
{
    [SerializeField] float runSpeed = 10f;
    [SerializeField] float jumpSpeed = 15f;
    [SerializeField] float climingSpeed = 8f;
    [SerializeField] float attackRadius = 3f;
    [SerializeField] Vector2 hitKick = new Vector2(50f, 50f);
    [SerializeField] Transform hurtBox;


    Rigidbody2D myRigidBody2d;
    Animator myAnimator;
    BoxCollider2D myBoxCollider2D;
    PolygonCollider2D myPlayersFeet;

    float startingGravityScale;
    bool isHurting = false;

    // Start is called before the first frame update
    void Start()
    {
        myRigidBody2d = GetComponent < Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        myBoxCollider2D = GetComponent<BoxCollider2D>();
        myPlayersFeet = GetComponent<PolygonCollider2D>();

        startingGravityScale = myRigidBody2d.gravityScale;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isHurting)
        {
            Run();
            Jump();
            Climb();
            Attack();

            if (myBoxCollider2D.IsTouchingLayers(LayerMask.GetMask("Enemy")))
            {
                PlayerHit();
            }

            ExitLevel();

        }



    }

    private void ExitLevel()
    {
        if (!myBoxCollider2D.IsTouchingLayers(LayerMask.GetMask("Interactable"))) { return; }
        {
            if (CrossPlatformInputManager.GetButtonDown("Vertical"))
            {
                FindObjectOfType<ExitDoor>().StartLoadingNextLevel();
            }
        }
    }

    private void Attack()
    {
        if (CrossPlatformInputManager.GetButtonDown("Fire1"))
        {
            myAnimator.SetTrigger("Attacking");

            Collider2D[] enemiesToHit = Physics2D.OverlapCircleAll(hurtBox.position, attackRadius, LayerMask.GetMask("Enemy"));
        
            foreach(Collider2D enemy in enemiesToHit)
            {
                enemy.GetComponent<Enemy>().Dying();
            }
        
        }
    }

    public void PlayerHit()
    {
        myRigidBody2d.velocity = hitKick * new Vector2(-transform.localScale.x, 1f);

        myAnimator.SetTrigger("Hitting");
        isHurting = true;

        FindObjectOfType<GameSession>().ProcessPlayerDeath();

        StartCoroutine(StopHurting());

    }

    IEnumerator StopHurting()
    {
        yield return new WaitForSeconds(2f);

        isHurting = false;

    }


    private void Climb()
    {
        if (myBoxCollider2D.IsTouchingLayers(LayerMask.GetMask("Climbing")))
        {
            float controlthrow = CrossPlatformInputManager.GetAxis("Vertical");
            Vector2 climbingVelocity = new Vector2(myRigidBody2d.velocity.x, controlthrow * climingSpeed);

            myRigidBody2d.velocity = climbingVelocity;

            myAnimator.SetBool("Climbing", true);

            myRigidBody2d.gravityScale = 0f;
        }
        else
        {
            myAnimator.SetBool("Climbing", false);
            myRigidBody2d.gravityScale = startingGravityScale;
        }


    }

    private void Jump()
    {
        if (!myPlayersFeet.IsTouchingLayers(LayerMask.GetMask("Ground")))
        {
            return;
        }

        bool isJumping = CrossPlatformInputManager.GetButtonDown("Jump");

        if (isJumping)
        {
            Vector2 jumpVelocity = new Vector2(myRigidBody2d.velocity.x, jumpSpeed);
            myRigidBody2d.velocity = jumpVelocity;
        }
    }

    private void Run()
    {
        float controlThrow = CrossPlatformInputManager.GetAxis("Horizontal");


        Vector2 playerVelocity = new Vector2(controlThrow * runSpeed, myRigidBody2d.velocity.y);
        myRigidBody2d.velocity = playerVelocity;


        FlipSprite();
        ChangingToRunningState();

    }

    private void ChangingToRunningState()
    {
        bool runningHorizontaly = Mathf.Abs(myRigidBody2d.velocity.x) > Mathf.Epsilon;
        myAnimator.SetBool("Running", runningHorizontaly);
    }

    private void FlipSprite()
    {
        bool runningHorizontaly = Mathf.Abs(myRigidBody2d.velocity.x) > Mathf.Epsilon;

        if (runningHorizontaly)
        {
            transform.localScale = new Vector2(Mathf.Sign(myRigidBody2d.velocity.x), 1f);
        }
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(hurtBox.position, attackRadius);
    }
}
