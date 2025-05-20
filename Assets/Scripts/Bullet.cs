using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20f;         // 총알의 이동 속도
    public float lifetime = 5f;       // 총알이 살아있는 시간 (초)

    void Start()
    {
        // 일정 시간이 지나면 총알 오브젝트를 자동으로 제거
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        // 로컬 좌표계의 Z축(앞 방향)으로 이동 (속도 * 시간)
        // transform.Translate는 local 기준 이동
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        // 충돌한 오브젝트의 태그가 "Player", "Obstacle", 또는 "Enermy"일 경우
        // 총알을 제거 (즉시 삭제)
        if (other.CompareTag("Player") || other.CompareTag("Obstacle") || other.CompareTag("Enermy"))
        {
            Destroy(gameObject);
        }
    }
}
