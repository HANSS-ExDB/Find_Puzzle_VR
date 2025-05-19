using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EnemyChaseWithLimit : MonoBehaviour
{
    public Transform target;            // 따라갈 대상 (플레이어)
    public float moveSpeed = 3f;        // 이동 속도
    public float detectionRange = 15f;  // 추적 시작 범위
    public float stopDistance = 2f;     // 너무 가까워지면 멈추는 거리
    public float rotationSpeed = 5f;    // 회전 속도

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

        if (distance > detectionRange) return; // 너무 멀면 추적 X
        if (distance <= stopDistance) return;  // 너무 가까우면 멈춤

        // 방향 계산
        Vector3 direction = (target.position - transform.position).normalized;
        direction.y = 0f;

        // 회전
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);

        // 이동 (충돌 감지됨)
        Vector3 nextPos = rb.position + direction * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(nextPos);
    }
}
