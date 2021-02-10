using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class ProjectMovement : MonoBehaviour
{
    /*   public float turnSpeed = 20f;

       Animator m_Animator;
       Rigidbody m_Rigidbody;
       AudioSource m_AudioSource;
       Vector3 m_Movement;
       Quaternion m_Rotation = Quaternion.identity;

       void Start ()
       {
           m_Animator = GetComponent<Animator> ();
           m_Rigidbody = GetComponent<Rigidbody> ();
           m_AudioSource = GetComponent<AudioSource> ();
       }

       void FixedUpdate ()
       {
           float horizontal = Input.GetAxis ("Horizontal");
           float vertical = Input.GetAxis ("Vertical");

           m_Movement.Set(horizontal, 0f, vertical);
           m_Movement.Normalize ();

           bool hasHorizontalInput = !Mathf.Approximately (horizontal, 0f);
           bool hasVerticalInput = !Mathf.Approximately (vertical, 0f);
           bool isWalking = hasHorizontalInput || hasVerticalInput;
           m_Animator.SetBool ("IsWalking", isWalking);

           if (isWalking)
           {
               if (!m_AudioSource.isPlaying)
               {
                   m_AudioSource.Play();
               }
           }
           else
           {
               m_AudioSource.Stop ();
           }

           Vector3 desiredForward = Vector3.RotateTowards (transform.forward, m_Movement, turnSpeed * Time.deltaTime, 0f);
           m_Rotation = Quaternion.LookRotation (desiredForward);
       }

       void OnAnimatorMove ()
       {
           m_Rigidbody.MovePosition (m_Rigidbody.position + m_Movement * m_Animator.deltaPosition.magnitude);
           m_Rigidbody.MoveRotation (m_Rotation);
       }
       */

    public float walkingSpeed = 7.5f;
    public float runningSpeed = 11.5f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;
    public Camera playerCamera;
    public float lookSpeed = 2.0f;
    public float lookXLimit = 45.0f;
    public int floorStatus = 0;
    public int enemyStatus = 0;
    //AudioSource FootstepAudioSource;  
    [SerializeField] AudioSource woodStep;
    [SerializeField] AudioSource tileStep;
    [SerializeField] AudioSource carpetStep;

    [SerializeField] AudioMixerSnapshot Safe;
    [SerializeField] AudioMixerSnapshot Danger;

    CharacterController characterController;
    Vector3 moveDirection = Vector3.zero;
    float rotationX = 0;

    [HideInInspector]
    public bool canMove = true;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        gameObject.tag = "Player";
        // Lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        //FootstepAudioSource = GetComponent<AudioSource> ();
    }


    void OnTriggerStay(Collider other)
    {
        if (other.tag == "Tiles")
        {
           // Debug.Log("Tiles");
            floorStatus = 1;
        }

        if (other.tag == "Carpet")
        {
           // Debug.Log("Carpet");
            floorStatus = 2;
        }

        if (other.tag == "Enemy")
        {
           // Debug.Log("Danger");
           enemyStatus = 1;
        }

        else
        {
           // Debug.Log("Safe");
        }
        
    }

    void OnTriggerExit(Collider other)
    {
           // Debug.Log("Wood");
            floorStatus = 0;
            enemyStatus = 0;
    }

    void Update()
    {
        // We are grounded, so recalculate move direction based on axes
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);
        // Press Left Shift to run
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float curSpeedX = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Horizontal") : 0;
        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        if (Input.GetButton("Jump") && canMove && characterController.isGrounded)
        {
            moveDirection.y = jumpSpeed;
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }
/*
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.Up) || Input.GetKey(KeyCode.Down) || Input.GetKey(KeyCode.Left) || Input.GetKey(KeyCode.Right)  )
        {
          //  Debug.Log("W");
            if(!FootstepAudioSource.isPlaying)
                FootstepAudioSource.Play();
        }

        else
        {
            FootstepAudioSource.Stop();
        }
*/

        if ( curSpeedX !=0 || curSpeedY != 0 )
        {
          //  Debug.Log("W");
            if(!woodStep.isPlaying && floorStatus == 0 )
            {
                woodStep.Play();
                tileStep.Stop();
                carpetStep.Stop();
               
            }
            if(!tileStep.isPlaying && floorStatus == 1 )
            {
                tileStep.Play();
                woodStep.Stop();
                carpetStep.Stop();               
            }
            if(!carpetStep.isPlaying && floorStatus == 2 )
            {
                carpetStep.Play();
                woodStep.Stop();
                tileStep.Stop();
            }
        }

        else
        {                  
                woodStep.Stop();
                tileStep.Stop();
                carpetStep.Stop();
        }


        // Apply gravity. Gravity is multiplied by deltaTime twice (once here, and once below
        // when the moveDirection is multiplied by deltaTime). This is because gravity should be applied
        // as an acceleration (ms^-2)
        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        // Move the controller
        characterController.Move(moveDirection * Time.deltaTime);

        // Player and Camera rotation
        if (canMove)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }

       // Debug.Log(enemyStatus);

       if (enemyStatus==0)
       {
            Safe.TransitionTo (0.1f);
        }

       if (enemyStatus==1)
       {
            Danger.TransitionTo (0.1f);
       }

    }
}