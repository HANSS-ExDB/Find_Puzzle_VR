using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
public class CameraController : MonoBehaviour
{
    public float moveSpeed = 5f;           // 이동 속도
    public float lookSpeed = 2f;           // 마우스 회전 속도
    public float verticalLookLimit = 80f;  // 상하 회전 제한 각도

    private float rotationX = 0f;
    private Rigidbody rb;

    void Start()
    {
        if (SceneManager.GetActiveScene().name == "BasicScene")
        {
            rb = GetComponent<Rigidbody>();
            rb.freezeRotation = true;  // 물리 회전 방지

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            Debug.Log("Mouse Locked");
        }
    }

    void Update()
    {
        // 마우스 입력 처리
        float mouseX = Input.GetAxis("Mouse X") * lookSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * lookSpeed;

        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -verticalLookLimit, verticalLookLimit);

        transform.localRotation = Quaternion.Euler(rotationX, transform.localEulerAngles.y + mouseX, 0f);
    }

    void FixedUpdate()
    {
        // 이동 처리 (FixedUpdate에서)
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        move.y = 0f; // 지면 고정

        Vector3 newPosition = rb.position + move * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(newPosition);
    }
}
