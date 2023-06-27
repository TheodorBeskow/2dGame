using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;

public class Bullet : NetworkBehaviour
{

    [SerializeField] private float speed = 50;
    [SerializeField] private float damage = 5;
    [SerializeField] private float spread;
    // dead from (to make a kill feed)
    public Rigidbody2D rb;
    void Start(){
        transform.Rotate(0f, 0f, Random.Range(-spread, spread));
        rb.velocity = transform.right * speed;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        Health player_h = collision.GetComponent<Health>();
        if(player_h != null) {
            // Debug.Log(collision);
            bool isdead = player_h.takeDamage(damage);
            if(!IsOwner) return;
            if(player_h.dead.Value) return;
        }
        destroyBulletServerRpc();

    }

    [ServerRpc]
    private void destroyBulletServerRpc() {
        Destroy(gameObject);
    }
}
