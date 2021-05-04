using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CharacterController controller;
    [SerializeField] private Animator anim;

    [Header("Config")]
    [SerializeField] float rotationSpeed = 100f;
    [SerializeField] float moveSpeed = 100f;
    [SerializeField] private float runSpeed = 200f; //RUNNING
    [SerializeField] private float runInputDelay = 0.2f; //RUNNING //допустимое время между двойным нажатием для старта бега


    [Header("Gravity")]
    [SerializeField] private float jumpHeight = 10f;
    [SerializeField] private float gravityScale = 2;

    private float gravity;
    private float runRate = 0.1f;
    private Camera mainCamera;
    private bool isRunning; //RUNNING
    private float lastTimeButton;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        Move();
        Fight();
        CheckRunning();

    }

    private void LateUpdate()
    {
        Rotate();
    }

    private void Move()
    {
        float inputH = Input.GetAxis("Horizontal");
        float inputV = Input.GetAxis("Vertical");

        Vector3 forward = mainCamera.transform.forward;
        forward.y = 0;
        forward.Normalize();

        Vector3 right = mainCamera.transform.right;
        right.y = 0;
        right.Normalize();

        Vector3 moveDirection = (inputV * forward + inputH * right);

        if (moveDirection.magnitude > 1)
        {
            moveDirection.Normalize();
        }

        if (controller.isGrounded) //прыгает
        {
            gravity = -0.1f;

            if (Input.GetButton("Jump"))
            {
                gravity = jumpHeight;
            }
        }
        else
        {
            gravity += gravityScale * Physics.gravity.y * Time.deltaTime;
        }

        //anim for jump

        if (gravity > 0)
        {
            anim.SetInteger("Gravity", 1);
        }
        else if (gravity < -0.3f)
        {
            anim.SetInteger("Gravity", -1);
        }
        else
        {
            anim.SetInteger("Gravity", 0);
        }

        moveDirection.y = gravity;
        controller.Move(moveDirection * moveSpeed * Time.deltaTime);

        if (Mathf.Abs(inputH) > 0 || Mathf.Abs(inputV) > 0)
        {
            anim.SetBool("Walk", true);
            //transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(moveDirection), Time.deltaTime * rotationSpeed);
        }
        else
        {
            anim.SetBool("Walk", false);
        }

        float movementSpeed = moveSpeed;
        if (isRunning)
        {
            movementSpeed = runSpeed;
            anim.SetTrigger("Running");
        }
        controller.Move(moveDirection * movementSpeed * Time.deltaTime);

    }

    void Rotate()
    {
        float mouseHorizontal = Input.GetAxis("Mouse X");
        print("Camera rotation: " + mainCamera.transform.rotation);
        transform.Rotate(Vector3.up, mouseHorizontal * rotationSpeed * Time.deltaTime);
    }

    void CheckRunning()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            float timeSinceLastButton = Time.time - lastTimeButton;
            if (timeSinceLastButton > runInputDelay)
            {
                lastTimeButton = Time.time;
            }
            else
            {

                lastTimeButton = Time.time;
                isRunning = true;
            }
        }

        if (Input.GetKeyUp(KeyCode.W))
        {
            isRunning = false;
        }
    }

    private void Fight()
    {
        if (Input.GetMouseButtonDown(0))
        {
            anim.SetTrigger("Attack");
        }
    }
}
