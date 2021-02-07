using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CommandHandler : MonoBehaviour
{
    //manually placed in objects
    public Transform playerTransform;
    public Transform attackPoint;
    public Transform movePlatformTransform;
    public GameObject playerObj;
    public Animator animator;
    public DangerZone dZone;
    public AudioManager a_audio;
    public AudioSource audioSrc;
    public AudioSource Rewind;
    public PlayerHealth playerHealth;
    public LadderZone ladderObj;
    

    public Rigidbody2D rb;
    public Rigidbody2D erb;

    // Insert the layers here.
    public LayerMask groundLayer;
    public LayerMask OneWayLayer;
    public LayerMask ladder;
    public LayerMask enemy;

    public bool isMoving=false;
    public bool isRewinding = false;
    float distToPlayer;
    public bool isGrounded = false;
    public bool isOneWay = false;

    //timer for rewind ability
    public float downTime, upTime, pressTime = 0;
    public float countDown = 1.0f;
    public bool ready = false;




    //Knockback
    public float knockback = 0;
    public float knockTime = 0;
    public float knockbackCount = 0;
    public bool knockFromRight;

    //ladder
    public bool onLadder = false;
    public float climbSpeed;
    public float climbVelocity;
    public float gravityStore;
    public bool coolDownTimer = false;

    //attacking
    public float attackRange = 0.5f;

    //Command type has the button variables
    Commands buttonW, buttonA, buttonD, buttonU, buttonS, spaceBar, 
        LeftClick, passiveRecord;

    // creat list of Command type
    //Vector2 so it can store player position
    internal List<Vector2> commandList;

    

    void Start()
    {
        Debug.Log("Game Start");
        //buttons for player input
        buttonW = new Jump();
        buttonA = new MoveLeft();
        buttonD = new MoveRight();
        buttonU = new Undue();
        LeftClick = new PlayerAttack();


        dZone = FindObjectOfType<DangerZone>();
        a_audio = FindObjectOfType<AudioManager>();

        //a_audio.Play("Theme");

        //recording positions
        passiveRecord = new GetPositions();
        rb = playerTransform.GetComponent<Rigidbody2D>();


        //ladder
        gravityStore = rb.gravityScale;

        //commands list
        //list stores all vector2 positions of player as they input controls
        commandList = new List<Vector2>();


        playerHealth = FindObjectOfType<PlayerHealth>();
        ladderObj = FindObjectOfType<LadderZone>();

        


    }

    private void Update()
    {
        //jumping
        Vector2 position = playerTransform.position;
        Vector2 direction = Vector2.down;
        float distance = 1f;
        isGroundedCheck(position, direction, distance);

        if (!onLadder)
            animator.SetBool("OnLadder", false);

        //knockback register
        if (!playerHealth.PlayerDead && Time.timeScale != 0)
            HandlInput();
     

        //countdown starts when U is held down
        if (Input.GetKeyDown(KeyCode.E) && !ready)
        {
            downTime = Time.time;
            pressTime = downTime + countDown;
            ready = true;
        }

        //is U is released, timer is terminated
        if (Input.GetKeyUp(KeyCode.E))
        {
            ready = false;
        }


        //triggers if cooldown exceedes given time
        if (Time.time >= pressTime && ready)
        {
            ready = false;
            Debug.Log("Stop rewinding!");
        }

        if (playerHealth.PlayerDead)
            audioSrc.Stop();

        if (Time.timeScale != 0)
            Cursor.visible = false;

    }


    //all the controls for player
    void HandlInput()
    {
        //Debug.Log("CAlled");
        //ladder check
        if (onLadder)
        {
            
            rb.gravityScale = 0f;

            climbVelocity = climbSpeed * Input.GetAxisRaw("Vertical");

            rb.velocity = new Vector2(rb.velocity.x, climbVelocity);

            animator.SetBool("IsJumping", false);
        }
        if (!onLadder)
        {
            animator.SetBool("OnLadderIdle", false);
            rb.gravityScale = 2f;
            if (Input.GetKey(KeyCode.W) && !onLadder && !animator.GetBool("IsJumping"))
                animator.SetBool("IsJumping", true);
        }

        

        //Player Key Inputs
        if (Input.GetKeyDown(KeyCode.W) && !isRewinding)
        {
            buttonW.Execute(playerTransform, this, buttonW, rb, groundLayer, OneWayLayer, ladder, animator, a_audio);
        }
        else if (Input.GetKey(KeyCode.A) && !isRewinding)
        {
            
            
            buttonA.Execute(playerTransform, this, buttonW, rb, groundLayer, OneWayLayer, ladder, animator, a_audio);
        }
        else if (Input.GetKey(KeyCode.D) && !isRewinding)
        {
            
            buttonD.Execute(playerTransform, this, buttonW, rb, groundLayer, OneWayLayer, ladder, animator, a_audio);
        }
        else if (Input.GetKey(KeyCode.E) && ready)
        {
            Rewind.enabled = true;
            Rewind.loop = true;
            StartRewind();
            buttonU.Execute(playerTransform, this, buttonW, rb, groundLayer, OneWayLayer, ladder, animator, a_audio);
        }
        else if (Input.GetMouseButtonDown(0) && !isRewinding)
        {
            a_audio.PlayRandom();
            LeftClick.Execute(attackPoint, this, buttonW, rb, enemy, OneWayLayer, ladder, animator, a_audio);
        }
        else if (Input.GetKey(KeyCode.Space) && !isRewinding)
        {

            //spaceBar.Execute(playerTransform, this, buttonW, rb, groundLayer, OneWayLayer, ladder, animator, audio);
        }
        else
        {
           
            StopRewind();
            //stops player movement. Set's velocity to 0
            rb.velocity = new Vector2(0, rb.velocity.y);
            passiveRecord.Execute(playerTransform, this, buttonW, rb, groundLayer, OneWayLayer, ladder, animator, a_audio);
            animator.SetFloat("Speed", 0);

            Rewind.enabled = false;
            Rewind.loop = false;





        }

    }


    //functions that switch on/off rewind
    public void StartRewind()
    {
        
        isRewinding = true;
        rb.isKinematic = true;
        animator.SetBool("IsRewinding", true);
        playerObj.GetComponent<CapsuleCollider2D>().enabled = false;

    }

    public void StopRewind()
    {
        
        isRewinding = false;
        rb.isKinematic = false;
        animator.SetBool("IsRewinding", false);
        playerObj.GetComponent<CapsuleCollider2D>().enabled = true;
    }

    //attack range displayed
    void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }


    //For animation switch
    public void isGroundedCheck(Vector2 position, Vector2 direction, float distance)
    {
        //OneWay Layer Raycast
        Debug.DrawRay(position, direction, Color.green);
        RaycastHit2D hit2 = Physics2D.Raycast(position, direction, distance, OneWayLayer);

        //Code for raycast for ground layer
        //debul shows raycast in editor mode
        Debug.DrawRay(position, direction, Color.green);
        RaycastHit2D hit = Physics2D.Raycast(position, direction, distance, groundLayer);

        if (Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0 && onLadder)
        {
            animator.SetBool("OnLadderIdle", false);
            animator.SetBool("OnLadder", true);
        }
        else
        {
            animator.SetBool("OnLadderIdle", true);
        }

            //resets amount of jumps if the player touches the ground
            if (hit.collider != null || hit2.collider != null)
        {
            //Debug.Log("you are grounded");
            isGrounded = true;

            animator.SetBool("IsJumping", false);

            //footstep sounds
            if (Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0)
            {
                
                if (!audioSrc.isPlaying)
                    audioSrc.Play();
            }
            else
            {
                audioSrc.Stop();
                
               
            }

            if (Input.GetKey(KeyCode.S)) 
                { audioSrc.Stop(); }   
        }
        else
            audioSrc.Stop();

       // Debug.Log(hit.collider);
        if (hit && hit.collider.tag == "DangerZone")
        {
            dZone.ready = true;
        }
        else
            dZone.ready = false;

        
    }


}
