using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst.CompilerServices;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Animations;

public class Missile : NetworkBehaviour
{

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private LayerMask obstacle;
    [SerializeField] private float speed;
    [SerializeField] private float slerpSpeed;
    [SerializeField] private float damage;
    [SerializeField] private float maxAngle;
    List<Transform> playerList = new();
    List<Transform> t_playerList = new();
    private GameObject[] playersObj;
    void Start(){
        rb.velocity = speed*transform.right;
        
        if(transform.rotation.y < -0.1) {
            transform.Rotate(0, 180, 180);
        }
    }

    void Update() {
        if(!IsHost) return;
        t_playerList.Clear();
        playerList.Clear();
        playersObj = GameObject.FindGameObjectsWithTag("Character");
        foreach(GameObject player in playersObj) {
            if(player.transform.GetChild(0).gameObject.layer == 11) continue;
            playerList.Add(player.transform.GetChild(0).transform);
        }
        while(playerList.Count != 0) {
            int pos = 0;
            double shortest = 1e6f;
            for(int j = 0; j < playerList.Count; j++) {
                Vector3 vec = transform.position - playerList[j].position;
                float dist = Mathf.Sqrt(Mathf.Pow(vec.x, 2) + Mathf.Pow(vec.y, 2));
                if(dist < shortest) {
                    shortest = dist;
                    pos = j;
                }
            }
            t_playerList.Add(playerList[pos]);
            playerList.RemoveAt(pos);
        }
        playerList = t_playerList.ToList();
        for(int i = 0; i < playerList.Count; i++) {
            Vector3 look = playerList[i].position - transform.position;
            float angle = Mathf.Atan2(look.y, look.x) * Mathf.Rad2Deg;
            float anglediff = Mathf.Min(Mathf.Abs(angle - transform.rotation.eulerAngles.z), 360f - Mathf.Abs(angle - transform.rotation.eulerAngles.z));
            // Debug.Log(anglediff);
            //Debug.Log(Physics.Linecast(transform.position, playerList[i].transform.position));

            Vector2 dir = (playerList[i].position - transform.position).normalized;
            Vector3 vec = transform.position - playerList[i].position;

    
            if(anglediff < maxAngle) {
                //Debug.Log(!(!Physics2D.Raycast(transform.position, dir, vec.magnitude, obstacle)));
                //if(Physics2D.Raycast(transform.position, dir, vec.magnitude, obstacle)) continue;
                Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
                transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime * slerpSpeed);
                break;
            }
        }
        rb.velocity = new Vector2(Mathf.Cos(transform.rotation.eulerAngles.z / 180f * Mathf.PI), Mathf.Sin(transform.rotation.eulerAngles.z / 180f * Mathf.PI)) * speed;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        Health player_h = collision.GetComponent<Health>();
        if(player_h != null) {
            player_h.takeDamage(damage);
            if(!IsOwner) return;
            if(player_h.dead.Value) return;
        }
        destroyMissileServerRpc();

    }

    [ServerRpc]
    private void destroyMissileServerRpc() {
        Destroy(gameObject);
    }
}
