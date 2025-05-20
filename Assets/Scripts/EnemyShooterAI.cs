using UnityEngine;

// Rigidbody ������Ʈ�� �ʼ��� �䱸�� (������ �ڵ����� �߰���)
[RequireComponent(typeof(Rigidbody))]
public class EnemyChaseWithLimit : MonoBehaviour
{
    public Transform target;              // ������ ��� (�÷��̾� ��)
    public float moveSpeed = 3f;          // �̵� �ӵ�
    public float detectionRange = 15f;    // Ž�� �Ÿ� (�� �Ÿ� �̳��� �־�� ���� ����)
    public float stopDistance = 2f;       // �� �Ÿ� ���Ϸ� ��������� �̵� ����
    public float rotationSpeed = 5f;      // ���� Ÿ���� ���� ȸ���ϴ� �ӵ�

    public GameObject bulletPrefab;       // �߻��� �Ѿ� ������
    public Transform firePoint;           // �Ѿ��� �߻�Ǵ� ��ġ
    public float fireInterval = 2f;       // �Ѿ� �߻� ���� (��)

    private Rigidbody rb;                 // ���� Rigidbody ������Ʈ
    private float fireTimer = 0f;         // �Ѿ� �߻� ������ ����� Ÿ�̸�

    void Start()
    {
        rb = GetComponent<Rigidbody>();   // Rigidbody ������Ʈ ��������
        rb.freezeRotation = true;         // ���� ������ ���� ȸ������ �ʵ��� ����
    }

    void FixedUpdate()
    {
        if (target == null) return;       // Ÿ���� ������ �ƹ� ���۵� ���� ����

        float distance = Vector3.Distance(transform.position, target.position);

        if (distance > detectionRange) return;  // Ž�� �Ÿ����� �ָ� �ƹ� ���۵� ���� ����

        // Ÿ���� ���� ���� ���� ���
        Vector3 direction = (target.position - transform.position).normalized;
        direction.y = 0f; // ���� ���� ���� (Y�� ȸ�� �� �ϵ���)

        // Ÿ�� �������� �ε巴�� ȸ��
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);

        // ���� �Ÿ� �̻��̸� Ÿ���� ���� �̵�
        if (distance > stopDistance)
        {
            Vector3 nextPos = rb.position + direction * moveSpeed * Time.fixedDeltaTime;
            rb.MovePosition(nextPos); // Rigidbody�� ����� �̵� (�浹 ���)
        }

        // �Ѿ� �߻� Ÿ�̸� �۵�
        fireTimer += Time.fixedDeltaTime;
        if (fireTimer >= fireInterval)
        {
            fireTimer = 0f;
            FireBullet(); // �Ѿ� �߻� �Լ� ȣ��
        }
    }

    // �Ѿ��� �����ϰ� �߻��ϴ� �Լ�
    void FireBullet()
    {
        if (bulletPrefab != null && firePoint != null)
        {
            // �Ѿ� �ν��Ͻ� ����
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

            // �Ѿ˿� Rigidbody�� �ִٸ� ���������� �߻�
            Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
            if (bulletRb != null)
            {
                Vector3 direction = firePoint.forward; // �߻� ���� (Z�� ����)
                direction.y = 0; // ���� ���� ����
                bulletRb.linearVelocity = direction.normalized * 10f; // ���� �ӵ��� �߻�
            }
        }
    }
}
