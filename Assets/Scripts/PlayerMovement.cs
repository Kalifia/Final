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

        moveDirection.y = gravity;


        controller.Move(moveDirection * moveSpeed * Time.deltaTime);
        anim.SetTrigger("walkTrigger");
        float movementSpeed = moveSpeed; //RUNNING //обычная скорость движения
        if (isRunning) //RUNNING //если включен бег
        { //RUNNING
            movementSpeed = runSpeed;
            anim.SetTrigger("runTrigger"); //RUNNING //скорость бега
        } //RUNNING
        controller.Move(moveDirection * movementSpeed * Time.deltaTime);
        //RUNNING
    }

    void Rotate()
    {
        float mouseHorizontal = Input.GetAxis("Mouse X");

        //float cameraRotation = mainCamera.transform.rotation.eulerAngles.y;
        print("Camera rotation: " + mainCamera.transform.rotation);
        transform.Rotate(Vector3.up, mouseHorizontal * rotationSpeed * Time.deltaTime);
        //transform.rotation = Quaternion.Euler(new Vector3(0, cameraRotation, 0));
    }

    void CheckRunning()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            float timeSinceLastButton = Time.time - lastTimeButton; //время, которое прошло от последнего нажатия кнопки бега
            if (timeSinceLastButton > runInputDelay)
            {
                //интервал нажатий превышает заданный - запомнить последнее время
                lastTimeButton = Time.time;
            }
            else
            {
                //нажатие произошло в допустимом интервале - включить бег
                lastTimeButton = Time.time;
                isRunning = true; //включить бег
            }
        }

        if (Input.GetKeyUp(KeyCode.W))
        {
            isRunning = false; //если кнопка была отжата - выключить бег
        }
    }
}
