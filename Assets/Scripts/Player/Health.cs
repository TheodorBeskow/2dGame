using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class Health : NetworkBehaviour {
    public float hp = 100;
    public NetworkVariable<bool> dead = new NetworkVariable<bool>(true, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    [SerializeField] private float deadTime;
    public Animator animator;
    [SerializeField] private Ability playerAbility;
    private Transform spawnPoints;
    public List<Tuple<float, float>> timeRec = new();
    public List<Tuple<float, float>> t_timeRec = new();

    void Start() {
        spawnPoints = GameObject.Find("SpawnPoints").transform;
    }

    public bool takeDamage(float damage) {
        if(!IsOwner) return false;
        ShootServerRpc();
        hp -= damage;
        if(hp <= 0f && !dead.Value) {
            animator.SetBool("Dead", true);
            dead.Value = true;
            gameObject.layer = 11;
            StartCoroutine(deadDelay());
        }
        return hp > 0f;
    }
    private IEnumerator deadDelay() {
        yield return new WaitForSeconds(deadTime);
        dead.Value = false;
        animator.SetBool("Dead", false);
        hp = 100;
        List<Transform> spawnPointsList = new();
        foreach(Transform child in spawnPoints) {
            spawnPointsList.Add(child);
        }
        Transform pos = spawnPointsList[UnityEngine.Random.Range(0, spawnPointsList.Count)];
        transform.position = new Vector2(pos.position.x, pos.position.y);
        gameObject.layer = 10;
    }

    public void updateHP(float t_time, float dtime) {
        t_timeRec.Clear();
        t_timeRec.Add(Tuple.Create(dtime, hp));
        for(int timestamp = 0; timestamp < timeRec.Count; timestamp++) {
            if(dtime >= t_time) break;
            t_timeRec.Add(timeRec[timestamp]);
            dtime += timeRec[timestamp].Item1;
        }
        timeRec.Clear();
        timeRec = t_timeRec.ToList();
    }
    public void setHP() {
        hp = timeRec[timeRec.Count - 1].Item2;
    }

    [ServerRpc]
    private void ShootServerRpc() {
        playerAbility.showSpriteClientRpc();
    }
}
