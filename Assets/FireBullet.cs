using UnityEngine;

public class FireBullet : MonoBehaviour
{
    private Transform muzzle;
    [SerializeField] private GameObject bulletPrefab;

    private void Start()
    {
        muzzle = transform.Find("Muzzle").transform;
    }

    public void Shoot()
    {
        Instantiate(bulletPrefab, muzzle.position, muzzle.rotation);
    }
}
