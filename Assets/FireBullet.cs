using UnityEngine;

public class FireBullet : MonoBehaviour
{
    private Transform muzzle;
    [SerializeField] private GameObject bulletPrefab;

    private GameObject projectileHolder;

    private void Start()
    {
        muzzle = transform.Find("Muzzle").transform;
        projectileHolder = GameObject.Find("Projectiles");
    }

    public void Shoot(float damage, float spread, int fireAmount)
    {
        for (int i = 0; i < fireAmount; i++)
        {
            GameObject bullet = Instantiate(bulletPrefab, muzzle.position, Quaternion.Euler(0, 0, muzzle.transform.eulerAngles.z + Random.Range(-spread, spread)), projectileHolder.transform);
            Projectile bullet_Projectile = bullet.GetComponent<Projectile>();
            bullet_Projectile.damage = damage;
        }
    }
}
