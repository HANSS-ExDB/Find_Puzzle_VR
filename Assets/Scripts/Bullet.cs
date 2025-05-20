using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20f;         // �Ѿ��� �̵� �ӵ�
    public float lifetime = 5f;       // �Ѿ��� ����ִ� �ð� (��)

    void Start()
    {
        // ���� �ð��� ������ �Ѿ� ������Ʈ�� �ڵ����� ����
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        // ���� ��ǥ���� Z��(�� ����)���� �̵� (�ӵ� * �ð�)
        // transform.Translate�� local ���� �̵�
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        // �浹�� ������Ʈ�� �±װ� "Player", "Obstacle", �Ǵ� "Enermy"�� ���
        // �Ѿ��� ���� (��� ����)
        if (other.CompareTag("Player") || other.CompareTag("Obstacle") || other.CompareTag("Enermy"))
        {
            Destroy(gameObject);
        }
    }
}
