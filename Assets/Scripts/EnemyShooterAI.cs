using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EnemyChaseWithLimit : MonoBehaviour
{
    public Transform target;            // ���� ��� (�÷��̾�)
    public float moveSpeed = 3f;        // �̵� �ӵ�
    public float detectionRange = 15f;  // ���� ���� ����
    public float stopDistance = 2f;     // �ʹ� ��������� ���ߴ� �Ÿ�
    public float rotationSpeed = 5f;    // ȸ�� �ӵ�

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    void FixedUpdate()
    {
        if (target == null) return;

        float distance = Vector3.Distance(transform.position, target.position);

        if (distance > detectionRange) return; // �ʹ� �ָ� ���� X
        if (distance <= stopDistance) return;  // �ʹ� ������ ����

        // ���� ���
        Vector3 direction = (target.position - transform.position).normalized;
        direction.y = 0f;

        // ȸ��
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);

        // �̵� (�浹 ������)
        Vector3 nextPos = rb.position + direction * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(nextPos);
    }
}
