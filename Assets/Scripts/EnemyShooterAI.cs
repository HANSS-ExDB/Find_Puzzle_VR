using UnityEngine;

// Rigidbody 컴포넌트를 필수로 요구함 (없으면 자동으로 추가됨)
[RequireComponent(typeof(Rigidbody))]
public class EnemyChaseWithLimit : MonoBehaviour
{
    public Transform target;              // 추적할 대상 (플레이어 등)
    public float moveSpeed = 3f;          // 이동 속도
    public float detectionRange = 15f;    // 탐지 거리 (이 거리 이내에 있어야 추적 시작)
    public float stopDistance = 2f;       // 이 거리 이하로 가까워지면 이동 멈춤
    public float rotationSpeed = 5f;      // 적이 타깃을 향해 회전하는 속도

    public GameObject bulletPrefab;       // 발사할 총알 프리팹
    public Transform firePoint;           // 총알이 발사되는 위치
    public float fireInterval = 2f;       // 총알 발사 간격 (초)

    private Rigidbody rb;                 // 적의 Rigidbody 컴포넌트
    private float fireTimer = 0f;         // 총알 발사 간격을 계산할 타이머

    void Start()
    {
        rb = GetComponent<Rigidbody>();   // Rigidbody 컴포넌트 가져오기
        rb.freezeRotation = true;         // 물리 엔진에 의해 회전하지 않도록 고정
    }

    void FixedUpdate()
    {
        if (target == null) return;       // 타깃이 없으면 아무 동작도 하지 않음

        float distance = Vector3.Distance(transform.position, target.position);

        if (distance > detectionRange) return;  // 탐지 거리보다 멀면 아무 동작도 하지 않음

        // 타깃을 향한 방향 벡터 계산
        Vector3 direction = (target.position - transform.position).normalized;
        direction.y = 0f; // 수직 방향 무시 (Y축 회전 안 하도록)

        // 타깃 방향으로 부드럽게 회전
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);

        // 일정 거리 이상이면 타깃을 향해 이동
        if (distance > stopDistance)
        {
            Vector3 nextPos = rb.position + direction * moveSpeed * Time.fixedDeltaTime;
            rb.MovePosition(nextPos); // Rigidbody를 사용한 이동 (충돌 고려)
        }

        // 총알 발사 타이머 작동
        fireTimer += Time.fixedDeltaTime;
        if (fireTimer >= fireInterval)
        {
            fireTimer = 0f;
            FireBullet(); // 총알 발사 함수 호출
        }
    }

    // 총알을 생성하고 발사하는 함수
    void FireBullet()
    {
        if (bulletPrefab != null && firePoint != null)
        {
            // 총알 인스턴스 생성
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

            // 총알에 Rigidbody가 있다면 물리적으로 발사
            Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
            if (bulletRb != null)
            {
                Vector3 direction = firePoint.forward; // 발사 방향 (Z축 기준)
                direction.y = 0; // 수직 방향 제거
                bulletRb.linearVelocity = direction.normalized * 10f; // 일정 속도로 발사
            }
        }
    }
}
