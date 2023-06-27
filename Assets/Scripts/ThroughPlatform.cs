using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ThroughPlatform : NetworkBehaviour {
    private TilemapCollider2D PlatformCollider;
    [SerializeField] private CapsuleCollider2D playerCollider;
    [SerializeField] private Health playerHealth;
    [SerializeField] private KeyBindData keyBind;

    private void Start() {
        if(!IsOwner) return;
        PlatformCollider = GameObject.Find("Tilemap_Through").GetComponent<TilemapCollider2D>();
    }

    void Update()
    {
        if(!IsOwner) return;
        if(((Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.S)&& keyBind.inType != 2) ||
            (Input.GetKeyDown(KeyCode.DownArrow)|| Input.GetKeyDown(KeyCode.RightShift) && keyBind.inType!=1)) && !playerHealth.dead.Value) {
            StartCoroutine(DisableCollision());       
        }
    }
    private IEnumerator DisableCollision() {
        Physics2D.IgnoreCollision(playerCollider, PlatformCollider);
        yield return new WaitForSeconds(0.25f);
        Physics2D.IgnoreCollision(playerCollider, PlatformCollider, false);
    }
}
