using UnityEngine;
using Photon.Pun;

public class PlayerController : MonoBehaviourPun
{
    public float speed = 5f;

    void Start()
    {
        if (photonView.IsMine)
        {
            transform.position = new Vector2(Random.Range(1, 5), Random.Range(1, 5));
        }
    }

    void Update()
    {
        if (!photonView.IsMine) return;

        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        Vector2 movement = new Vector2(moveX, moveY).normalized * speed * Time.deltaTime;
        transform.Translate(movement);
    }
}