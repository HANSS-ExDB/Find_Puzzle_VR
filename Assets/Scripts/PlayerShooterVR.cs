using UnityEngine;
using UnityEngine.XR;

public class PlayerShooterVR : MonoBehaviour
{
    [Header("Bullet Settings")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 20f;

    [Header("Fire Rate")]
    [Tooltip("�ʴ� �߻� Ƚ��")]
    public float fireRate = 5f;
    private float nextFireTime = 0f;

    [Header("Input")]
    public XRNode inputSource = XRNode.RightHand;

    void Update()
    {
        // ��ٿ� üũ
        if (Time.time < nextFireTime)
            return;

        // Ʈ���� �Է�
        InputDevice device = InputDevices.GetDeviceAtXRNode(inputSource);
        if (device.TryGetFeatureValue(CommonUsages.triggerButton, out bool isPressed) && isPressed)
        {
            FireBullet();
            nextFireTime = Time.time + 1f / fireRate;
        }
    }

    void FireBullet()
    {
        if (bulletPrefab == null || firePoint == null)
            return;

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
            rb.linearVelocity = firePoint.forward * bulletSpeed;
    }
}
