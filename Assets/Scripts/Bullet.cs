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
        // 충돌 대상이 Player 또는 Obstacle인 경우
        if (other.CompareTag("Player") || other.CompareTag("Obstacle"))
        {
            Destroy(gameObject);
            Debug.Log("Hit : "+ other.tag);
        }
        // 적(Enemy)인 경우
        else if (other.CompareTag("Enemy"))
        {
            // EnemyChaseWithLimit 스크립트 가져와서 Stun 호출
            EnemyChaseWithLimit enemy = other.GetComponent<EnemyChaseWithLimit>();
            if (enemy != null)
            {
                enemy.Stun();
            }

            Destroy(gameObject);
            Debug.Log("Hit : " + other.tag + "is Stunned");
        }
    }
}
