using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement Instance { get; private set; }

    [Header("References")]
    [SerializeField] private CharacterController controller;
    [SerializeField] private Animator anim;

    [Header("Config")]
    [SerializeField] float rotationSpeed = 100f;
    [SerializeField] float moveSpeed = 100f;

    [Header("Gravity")]
    [SerializeField] private float jumpHeight = 10f;
    [SerializeField] private float gravityScale = 2;

    private float gravity;
    private Camera mainCamera;



    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        Move();
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

        Vector3 moveDirection = (inputH * forward + inputV * right);

        if (moveDirection.magnitude > 1)
        {
            moveDirection.Normalize();
        }

        if (controller.isGrounded)
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
    }

    void Rotate()
    {
        float mouseHorizontal = Input.GetAxis("Mouse X");

        transform.Rotate(Vector3.up, mouseHorizontal * rotationSpeed * Time.deltaTime);
    }
}
