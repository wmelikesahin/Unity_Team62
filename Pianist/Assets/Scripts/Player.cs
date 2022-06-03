using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] GameObject rayObject;
    [SerializeField] public Transform refForGameover;

    
    public float moveSpeed = 10f;
    public Vector2 direction;
    private bool facingRight = true;


   
    public float jumpSpeed = 15f;
    public float jumpDelay = 0.25f;
    private float jumpTimer;


    public Rigidbody rb;
    public Animator animator;
    public LayerMask groundLayer;
    public GameObject characterHolder;


    public float maxSpeed = 7f;
    public float linearDrag = 4f;
    public float gravity = 1f;
    public float fallMultiplier = 5f;


    public bool onGround = false;
    public float groundLength = 0.6f;
    public Vector3 colliderOffset;


    //Wall movement
    public bool walled, wallMove;
    bool[] wallHits = new bool[4];

    private void Start()
    {
       
    }

    
    void Update()
    {
          bool wasOnGround = onGround;
            onGround = Physics.Raycast(transform.position + colliderOffset, Vector2.down, groundLength, groundLayer) ||
                Physics.Raycast(transform.position - colliderOffset, Vector2.down, groundLength, groundLayer) ||
                Physics.Raycast(transform.position, Vector2.down, groundLength, groundLayer);

           

            if (Input.GetButtonDown("Jump"))
            {
                jumpTimer = Time.time + jumpDelay;
                isJumping = true;
            }

            walled = Physics.Raycast(transform.position, transform.forward, out hit, 4f, LayerMask.GetMask("ladder"));


            direction = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            isMoving = (direction.x != 0);


            castRays();

            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                if (onGround)
                    rb.velocity = new Vector3(0, 0, 0);
            }

            if (Input.GetKey(KeyCode.UpArrow) && walled)
            {
                wallMove = true;
                animator.SetBool("isClimb", true);

                rb.isKinematic = true;
            }
            if (Input.GetKeyDown(KeyCode.DownArrow) && wallMove)
            {
                rb.isKinematic = false;
                wallMove = false;
                animator.SetBool("isClimb", false);
            }


           
        
    }
    void FixedUpdate()
    {
      
        
            if (!wallMove)
                moveCharacter(direction.x);


            if (wallMove)
            {
                if (!walled) { wallMove = false; animator.SetBool("isClimb", false); rb.isKinematic = false; };


     

                if (RotRef.side % 2 == 0)
                {
                    if (RotRef.side == 0) dir = 1;
                    else dir = -1;

                    transform.Translate(direction.x * dir * 0.05f, direction.y * 0.05f, 0);
                }
                else
                {
                    if (RotRef.side == 1) dir = 1;
                    else dir = -1;

                    transform.Translate(0, direction.y * 0.05f, direction.x * dir * 0.05f);
                }


            }


            if (jumpTimer > Time.time && (onGround || walled))
            {
                walled = false; wallMove = false;
                rb.isKinematic = false;
                Jump();
            }

            modifyPhysics();
        
    
    }

    int dir = 1;
    void moveCharacter(float horizontal)
    {


        if (RotRef.side % 2 == 0)
        {
            if (RotRef.side == 0) dir = 1;
            else dir = -1;
      
            rb.AddForce(new Vector3(horizontal * dir * moveSpeed, 0, 0));
        }
        else
        {
            if (RotRef.side == 1) dir = 1;
            else dir = -1;
           
            rb.AddForce(new Vector3(0, 0, horizontal * dir * moveSpeed));
        }

        if ((horizontal > 0 && !facingRight) || (horizontal < 0 && facingRight))
        {
            Flip();
        }
       


     
        if (onGround)
           rb.velocity = new Vector3(0, 0, 0);
        
    }
    void Jump()
    {

        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.AddForce(Vector2.up * jumpSpeed, ForceMode.Impulse);
        jumpTimer = 0;
     
      
    }
    void modifyPhysics()
    {
        bool changingDirections = (direction.x > 0 && rb.velocity.x < 0) || (direction.x < 0 && rb.velocity.x > 0);

        if (onGround)
        {
           
            rb.useGravity = false;
        }
        else
        {
            rb.useGravity = true;
          
        }
    }
    void Flip()
    {
        facingRight = !facingRight;
        characterHolder.gameObject.GetComponent<SpriteRenderer>().flipX = !facingRight;
       
    }
    IEnumerator JumpSqueeze(float xSqueeze, float ySqueeze, float seconds)
    {
        Vector3 originalSize = Vector3.one;
        Vector3 newSize = new Vector3(xSqueeze, ySqueeze, originalSize.z);
        float t = 0f;
        while (t <= 1.0)
        {
            t += Time.deltaTime / seconds;
            characterHolder.transform.localScale = Vector3.Lerp(originalSize, newSize, t);
            yield return null;
        }
        t = 0f;
        while (t <= 1.0)
        {
            t += Time.deltaTime / seconds;
            characterHolder.transform.localScale = Vector3.Lerp(newSize, originalSize, t);
            yield return null;
        }

    }

   
   
    RaycastHit hit, hit1;
    RaycastHit[] hits;
 

    public static bool isMoving, isJumping;
    Vector3 targetp;
    [HideInInspector]
    public bool grounded;
    public void castRays()
    {

        if (isMoving || isJumping)
        {

            grounded = Physics.Raycast(transform.position, Vector3.down, out hit, 0.6f, LayerMask.GetMask("ground")) ? true : false;

           
            if (RotRef.side % 2 == 0)
            {
                targetp = -rayObject.transform.forward;
                targetp.z *= 1000f;
                targetp.y = rayObject.transform.position.y;
                targetp.x = rayObject.transform.position.x;
            }
            else
            {
                if (RotRef.side == 1)
                    targetp = rayObject.transform.position + Vector3.right * 1000;
                else
                    targetp = rayObject.transform.position + Vector3.left * 1000;
               
            }
            hits = Physics.RaycastAll(rayObject.transform.position, targetp, 1000f, LayerMask.GetMask("ground"));

            Debug.DrawLine(rayObject.transform.position, targetp, Color.red);
            if (hits.Length > 0)
            {

               
                if (!Physics.Raycast(rayObject.transform.position, transform.forward, out hit, 20f, LayerMask.GetMask("Default")))
                {
                  
                    Vector3 pos = transform.position;

                    if (RotRef.side % 2 == 0)
                        pos.z = hits[0].collider.transform.position.z;
                    else
                    {
                        pos.x = hits[0].collider.transform.position.x;

                    }
                  
                    transform.position = pos;
                    
                    hits = null;
                }
               
            }
            else
            {
                if (RotRef.side % 2 == 0)
                {
                    targetp = rayObject.transform.forward;
                    targetp.z *= 1000f;
                    targetp.y = rayObject.transform.position.y;
                    targetp.x = rayObject.transform.position.x;
                }
                else
                {
                    if (RotRef.side == 1)
                        targetp = rayObject.transform.position + Vector3.left * 1000;
                    else
                        targetp = rayObject.transform.position + Vector3.right * 1000;
                  
                }
                Debug.DrawLine(rayObject.transform.position, targetp, Color.blue);

                if (Physics.Raycast(rayObject.transform.position, targetp, out hit1, 1000f, LayerMask.GetMask("ground")) && !grounded)
                {
                    Vector3 pos = transform.position;

                    if (!Physics.Raycast(rayObject.transform.position, transform.forward, out hit, 20f, LayerMask.GetMask("Default")))
                    {
                      
                        if (RotRef.side % 2 == 0)
                            pos.z = hit1.collider.transform.position.z;
                        else
                        {
                            pos.x = hit1.collider.transform.position.x;

                        }
                        transform.position = pos;

                    }

                  
                }
              

            }
        }

    }



}