using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
public class CameraController : MonoBehaviour
{
    public float moveSpeed = 5f;           // �̵� �ӵ�
    public float lookSpeed = 2f;           // ���콺 ȸ�� �ӵ�
    public float verticalLookLimit = 80f;  // ���� ȸ�� ���� ����

    private float rotationX = 0f;
    private Rigidbody rb;

    void Start()
    {
        if (SceneManager.GetActiveScene().name == "BasicScene")
        {
            rb = GetComponent<Rigidbody>();
            rb.freezeRotation = true;  // ���� ȸ�� ����

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            Debug.Log("Mouse Locked");
        }
    }

    void Update()
    {
        // ���콺 �Է� ó��
        float mouseX = Input.GetAxis("Mouse X") * lookSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * lookSpeed;

        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -verticalLookLimit, verticalLookLimit);

        transform.localRotation = Quaternion.Euler(rotationX, transform.localEulerAngles.y + mouseX, 0f);
    }

    void FixedUpdate()
    {
        // �̵� ó�� (FixedUpdate����)
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        move.y = 0f; // ���� ����

        Vector3 newPosition = rb.position + move * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(newPosition);
    }
}
