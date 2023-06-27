using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Weapon : NetworkBehaviour {

    [SerializeField] private Transform FirePoint;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private MuzzleFlashScript mf;
    [SerializeField] private Ability playerAbility;
    [SerializeField] private Health playerHealth;
    [SerializeField] private KeyBindData keyBind;

    void Update() {
        if(!IsOwner) return;

        if(keyBind.inType != 3)if((((Input.GetButtonDown("Fire1")||Input.GetKeyDown(KeyCode.V)) && keyBind.inType != 2)||(Input.GetButtonDown("Fire2") || Input.GetKeyDown(KeyCode.M)) && keyBind.inType != 1) && !playerHealth.dead.Value) {
            ShootServerRpc(FirePoint.position, FirePoint.rotation);
        }
    }

    [ServerRpc]
    private void ShootServerRpc(Vector3 clientFirePointpos, Quaternion clientFirePointrot) {
        GameObject spawnedObjectTransform = Instantiate(bulletPrefab, clientFirePointpos, clientFirePointrot);
        spawnedObjectTransform.GetComponent<NetworkObject>().Spawn(true);
        playerAbility.showSpriteClientRpc();
        ShootClientRpc();
        // Debug.Log(OwnerClientId);
    }

    [ClientRpc]
    private void ShootClientRpc() {
        StartCoroutine(mf.showFlash());
    }
}
