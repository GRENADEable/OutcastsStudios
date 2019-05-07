﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class PlayerControls : MonoBehaviour
{
    #region  Player Variables
    [Header("Player Movement Variables")]
    public float walkingSpeed;
    public float runningSpeed;
    public float crouchWalkSpeed;
    public float crouchRunSpeed;
    public float maxClampValue;
    [Range(0f, 10.0f)]
    public float rotationSpeed;
    [Range(0f, 5f)]
    public float pushRotationSpeed;
    public float crouchColShrinkValue; //Initial Value is 0.5f
    public float crouchColCenterValue; //Initial Value is 2

    [Header("Player Jump Variables")]
    public float jumpPower;
    private LightMechanic lightMechanic;

    [Header("Player Gravity Variables")]
    public float defaultGravity;
    public float gravityAfterRopeBreak;

    [Header("Rope Variables")]
    public float climbSpeed;
    public float sprintClimbSpeed;
    #endregion

    #region Events
    public delegate void SendEvents();
    public static event SendEvents onChangeLevelToHallway;
    public static event SendEvents onChangeLevelText;
    public static event SendEvents onPlayHallwayOST;
    public static event SendEvents onRopeBreakMessage;
    public static event SendEvents onKeyMove;
    #endregion

    #region Player Movement
    [Header("Player Movement Variables")]
    public bool isPushingOrPulling;
    public bool isPickingObject;
    public float pushPower;
    public bool onRope;
    public bool isCrouching;
    #endregion

    #region Trigger Colliders
    private Collider interactCol;
    #endregion

    private float gravity;
    private Vector3 moveDirection = Vector3.zero;
    private CharacterController charController;
    private PlayerInteraction plyInteract;
    private Animator courageAnim;

    #region Cheats
    [Header("Cheats Section :3")]
    [SerializeField]
    private float flashSpeed;
    [SerializeField]
    private float defaultRunningSpeed;
    [SerializeField]
    private float superJump;
    [SerializeField]
    private float defaultJump;
    #endregion

    private float playerHeight;
    private float moveHorizontal;
    private float moveVertical;
    private float playerCenter;
    [SerializeField]
    private float movementClamp;

    void OnEnable()
    {
        charController = GetComponent<CharacterController>();
        lightMechanic = GetComponent<LightMechanic>();
        gravity = defaultGravity;
        courageAnim = GetComponent<Animator>();
        playerHeight = charController.height;
        playerCenter = charController.center.y;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1) && plyInteract != null && !isCrouching)
            plyInteract.StartInteraction();
        else if (Input.GetMouseButton(1) && plyInteract != null && !isCrouching)
            plyInteract.UpdateInteraction();
        if (Input.GetMouseButtonUp(1) && plyInteract != null && !isCrouching)
            plyInteract.EndInteraction();

        if (!onRope)
        {
            //Storing Player's Character Controllers Height and Center
            float localHeight = playerHeight;
            float localCenter = playerCenter;

            //Gets Player Inputs
            moveVertical = Input.GetAxis("Vertical");
            moveHorizontal = Input.GetAxis("Horizontal");

            //Checks if the player is on the Ground
            if (charController.isGrounded)
            {
                //Applies Movement
                moveDirection = new Vector3(-moveVertical, 0.0f, moveHorizontal).normalized;

                //Add Input Floats to Blend Tee "speed" Parameters
                movementClamp = Mathf.Clamp(Mathf.Abs(moveVertical) + Mathf.Abs(moveHorizontal), 0f, maxClampValue);
                courageAnim.SetFloat("speed", movementClamp);

                //Applies Roatation relative to What Key is Pressed
                if (moveDirection != Vector3.zero && !isPushingOrPulling)
                    transform.rotation = Quaternion.Lerp(this.transform.rotation, Quaternion.LookRotation(moveDirection), rotationSpeed * Time.deltaTime);

                if (isPushingOrPulling && moveDirection != Vector3.zero)
                    transform.rotation = Quaternion.Lerp(this.transform.rotation, Quaternion.LookRotation(moveDirection), pushRotationSpeed * Time.deltaTime);

                if (Input.GetKey(KeyCode.LeftShift) && !isCrouching && !isPickingObject && !isPushingOrPulling)
                {
                    moveDirection *= runningSpeed;
                    // Debug.Log("Running");
                }
                else if (Input.GetKey(KeyCode.C) && !isPushingOrPulling && !isPickingObject)
                {
                    //Made Character Controller Collider Shrink Variables Dynamic
                    //Player Crouching
                    localHeight = playerHeight * crouchColShrinkValue;
                    localCenter = playerCenter / crouchColCenterValue;
                    isCrouching = true;
                    moveDirection *= crouchWalkSpeed;
                    courageAnim.SetBool("isCrouching", true);
                    // Debug.Log("Crouch Walk");
                }
                else
                {
                    moveDirection *= walkingSpeed;
                    isCrouching = false;
                    courageAnim.SetBool("isCrouching", false);
                    // Debug.Log("Walking");
                }

                if (Input.GetButtonDown("Jump") && !isPushingOrPulling && !isCrouching && !isPickingObject)
                {
                    Jump();
                }

                charController.height = Mathf.Lerp(charController.height, localHeight, 5 * Time.deltaTime);
                charController.center = new Vector3(0, Mathf.Lerp(charController.center.y, localCenter, 5 * Time.deltaTime), 0);
                Mathf.Clamp(charController.center.y, 0.05f, 0.1f);
            }
            else
                moveDirection.y -= gravity * Time.deltaTime;
        }
        else
        {
            moveDirection = new Vector3(0.0f, Input.GetAxis("Vertical") * climbSpeed, 0.0f);
            // Debug.Log("Climbing");
        }

        if (interactCol != null && Input.GetKey(KeyCode.E))
        {
            onRope = true;
            courageAnim.SetBool("isOnRope", true);
        }
        else
        {
            onRope = false;
            courageAnim.SetBool("isOnRope", false);
        }

        charController.Move(moveDirection * Time.deltaTime);
    }

    #region Cheats :P
    public void SuperJumpToggle(bool isSuperJump)
    {
        if (isSuperJump)
        {
            jumpPower = superJump;
        }
        else if (!isSuperJump)
        {
            jumpPower = defaultJump;
        }
    }

    public void FlashSpeedToggle(bool isFlash)
    {
        if (isFlash)
        {
            runningSpeed = flashSpeed;
        }
        else if (!isFlash)
        {
            runningSpeed = defaultRunningSpeed;
        }
    }
    #endregion

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "PickUp" && plyInteract == null)
        {
            plyInteract = GetComponent<ObjectPickup>();
            plyInteract.interactCol = other;

            if (Input.GetMouseButton(1))
                plyInteract.StartInteraction();

            Debug.Log("Pickup Reference Added");
        }

        if (other.tag == "PushAndPull" && plyInteract == null)
        {
            plyInteract = GetComponent<ObjectPushAndPull>();
            plyInteract.interactCol = other;

            if (Input.GetMouseButton(1))
                plyInteract.StartInteraction();

            Debug.Log("Push and Pull Reference Added");
        }

        if (other.tag == "Rope")
        {
            interactCol = other;
        }

        if (other.gameObject.tag == "Matchstick")
        {
            interactCol = other;
        }

        if (other.tag == "RopeBreak")
        {
            if (onRopeBreakMessage != null)
                onRopeBreakMessage();

            gravity = gravityAfterRopeBreak;
            // Debug.Log("Rope Broken");
        }

        if (other.gameObject.tag == "End")
        {
            if (onChangeLevelToHallway != null && onChangeLevelText != null && onPlayHallwayOST != null)
            {
                onChangeLevelToHallway();
                onChangeLevelText();
                onPlayHallwayOST();
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (plyInteract != null)
        {
            if (other.tag == "PickUp" && plyInteract.interactCol == other)
                ResetInteraction();

            if (other.tag == "PushAndPull" && plyInteract.interactCol == other)
            {
                // plyInteract.EndInteraction();
                ResetInteraction();
            }

            Debug.Log("Interaction Script Reference Removed");
        }

        if (other.tag == "Rope")
            interactCol = null;

        if (other.gameObject.tag == "Matchstick")
            interactCol = null;
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.collider.tag == "HallwayEndDoor" && Input.GetKey(KeyCode.E))
        {
            Rigidbody body = hit.collider.attachedRigidbody;

            if (body == null || body.isKinematic)
                return;

            if (hit.moveDirection.y < -0.3f)
                return;

            Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);
            body.velocity = pushDir * pushPower;
            // Debug.Log("Pushing Door");
        }
    }

    void Jump()
    {
        float jumpForce = Mathf.Sqrt(jumpPower * Mathf.Abs(defaultGravity) * 2);
        moveDirection.y = jumpForce;

        if (!lightMechanic.lightOn)
            courageAnim.SetTrigger("isJumping"); // Courage Jump Animation

        else
            courageAnim.SetTrigger("isJumpingWithTorch"); // Courage Jump Animation with Light
    }

    public void ResetInteraction()
    {
        plyInteract.interactCol = null;
        plyInteract = null;
    }
}