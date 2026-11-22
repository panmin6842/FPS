using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float sprintSpeed = 8f;
    [SerializeField] private float jumpHeight = 1.5f;
    [SerializeField] private float gravity = -19.62f; //중력2배

    [Header("Look")]
    [SerializeField] private Transform cameraRoot;
    [SerializeField] private float mouseSensitivity = 0.15f;
    [SerializeField] private float maxLookAngle = 85f;

    private CharacterController controller;
    private Vector2 moveInput;
    private Vector2 lookInput;
    private Vector3 velocity;
    private float xRotation = 0f;

    private bool isSprinting;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }
    void Start()
    {
        //게임 시작 시 마우스 커서 화면 중앙에 고정
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        HandleMove();
        HandleLook();
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    public void OnLook(InputValue value)
    {
        lookInput = value.Get<Vector2>();
    }

    public void OnJump(InputValue value)
    {
        if(value.isPressed && controller.isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }
    public void OnSprint(InputValue value)
    {
        isSprinting = value.isPressed;
    }

    private void HandleLook()
    {
        float mouseX = lookInput.x * mouseSensitivity;
        float mouseY = lookInput.y * mouseSensitivity;

        //상하 회전
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -maxLookAngle, maxLookAngle);
        cameraRoot.localRotation = Quaternion.Euler(xRotation, 0, 0);

        //좌우 회전
        transform.Rotate(Vector3.up * mouseX);
    }

    private void HandleMove()
    {
        //바닥에 닿아 있을 때 중력이 누적되는 것을 방지
        if (controller.isGrounded && velocity.y < 0f)
        {
            velocity.y = -2f;
        }

        //캐릭터가 바라보는 방향 기준으로 이동 벡터 계산
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;

        float speed = isSprinting ? sprintSpeed : moveSpeed;

        controller.Move(move * speed * Time.deltaTime);

        //중력 적용
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
