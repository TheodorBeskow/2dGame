using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class Ability : NetworkBehaviour {
    public float progress = 70;
    [SerializeField] private float PassiveProgress;
    [SerializeField] private float ActiveProgress;
    public string playerType;

    [SerializeField] private SpriteRenderer spriteRend; 
    [SerializeField] private float fadeTime; 
    [SerializeField] private float invisalpha; 

    [SerializeField] private float reverseTime;
    [SerializeField] public Health playerHealth;
    [SerializeField] public Movement playerMovement;
    private float virtualDtime = 0f;

    [SerializeField] private int amountDecoys;
    [SerializeField] private float spawnWaitTime;
    [SerializeField] private GameObject decoyPrefab;

    public Animator animator;
    [SerializeField] private KeyBindData keyBind;
    [SerializeField] private GameObject missilePrefab;
    [SerializeField] private Transform missilePoint;

    [SerializeField] private LayerMask obstacle;
    [SerializeField] private float flash_time;

    [SerializeField] private RuntimeAnimatorController BlackAnimation;
    [SerializeField] private RuntimeAnimatorController RedAnimation;
    [SerializeField] private RuntimeAnimatorController YellowAnimation;
    [SerializeField] private RuntimeAnimatorController BlueAnimation;
    [SerializeField] private RuntimeAnimatorController GreenAnimation;

    void Start() {
        if(!IsOwner) return;
        progress = 70f;  
    }
    void Update() {
        if(!IsOwner) return;

        if(playerType == "Decoy") return;
        if(playerType == "Blue") updateReverse();
        if(progress >= 100f && Input.GetKeyDown(KeyCode.E) && !playerHealth.dead.Value) doAbility(); // och att man inte är död !!!!
        progress = Mathf.Min(progress + Time.deltaTime*PassiveProgress, 100);

        if(playerType != "Decoy") UIAnimatorServerRpc(playerType);
    }

    [ServerRpc]
    private void UIAnimatorServerRpc(string anim) {
        UIAnimatorClientRpc(anim);
    }

    [ClientRpc]
    private void UIAnimatorClientRpc(string anim) {
        if(anim == "Black") animator.runtimeAnimatorController = BlackAnimation;
        else if(anim == "Red") animator.runtimeAnimatorController = RedAnimation;
        else if(anim == "Green") animator.runtimeAnimatorController = GreenAnimation;
        else if(anim == "Yellow") animator.runtimeAnimatorController = YellowAnimation;
        else if(anim == "Blue") animator.runtimeAnimatorController = BlueAnimation;
    }

    void doAbility() {
            
        if(playerType == "Red" && !playerMovement.IsGrounded()) return;
        AbilityServerRpc(playerType);

        progress = 0;
    }
    [ServerRpc]
    private void AbilityServerRpc(string mode) {
        if(mode == "Yellow") YellowAbility();
        else if(mode == "Green") StartCoroutine(GreenAbility());
        else AbilityClientRpc(mode);
    }
    [ClientRpc]
    private void AbilityClientRpc(string mode) {
        if(mode == "Black") StartCoroutine(BlackAbility());
        else if(mode == "Blue") {
            playerHealth.setHP();
            playerMovement.setMove();
        }
        else if(mode == "Red") StartCoroutine(RedAbility());
    }
    private IEnumerator BlackAbility() {
        if(!IsOwner) invisalpha = 0f;
        Color t_color = spriteRend.color;
        float t_prog = 0;
        while(t_prog < 0.5f) {
            float t_time = Time.deltaTime;
            t_color.a = Mathf.Lerp(t_color.a, invisalpha, t_time*10);
            spriteRend.color = t_color;
            t_prog += t_time;
            yield return null;
        }
    }
    private IEnumerator GreenAbility() {
        for(int i = 0; i < amountDecoys; i++) {
            GameObject spawnedObjectTransform = Instantiate(decoyPrefab, transform.position, transform.rotation);
            spawnedObjectTransform.GetComponent<NetworkObject>().Spawn(true);
            yield return new WaitForSeconds(spawnWaitTime);
        }
    }
    private IEnumerator RedAbility() {
        int remember = keyBind.inType;
        keyBind.inType = 3;
        animator.SetBool("Sitting", true);
        yield return new WaitForSeconds(0.40f);
        animator.SetBool("Sitting", false);
        if(!playerHealth.dead.Value && IsServer) {
            GameObject spawnedObjectTransform = Instantiate(missilePrefab, missilePoint.position, missilePoint.rotation);
            spawnedObjectTransform.GetComponent<NetworkObject>().Spawn(true);
        }
        yield return new WaitForSeconds(0.60f);
        keyBind.inType = remember;
        yield return null;
    }
    private void YellowAbility() {
        List<Transform> playerList = new();
        GameObject[] playersObj = GameObject.FindGameObjectsWithTag("Character");
        foreach(GameObject player in playersObj) {
            if(player.transform.GetChild(0).gameObject.layer == 12) continue;
            if(player.transform.GetChild(0).transform == transform) continue;
            playerList.Add(player.transform.GetChild(0).transform);
        }
        for(int i = 0; i < playerList.Count; i++) {
            Vector2 dir = (playerList[i].position - transform.position).normalized;
            Vector3 vec = transform.position - playerList[i].position;

            if(Physics2D.Raycast(transform.position, dir, vec.magnitude, obstacle)) continue;

            FlashBang fb = playerList[i].GetComponent<FlashBang>();
            fb.effectClientRpc(flash_time);
        }
    }

    [ClientRpc]
    public void showSpriteClientRpc() {
        Color t_color = spriteRend.color;
        t_color.a = 1f;
        spriteRend.color = t_color;
    }
    public void updateReverse() {
        if(!IsOwner) return;
        if(playerHealth.dead.Value) progress = 70f;
        virtualDtime += Time.deltaTime;
        if(virtualDtime < 0.05f) return;
        playerHealth.updateHP(reverseTime, virtualDtime);
        playerMovement.updateTrans(reverseTime, virtualDtime);
        playerMovement.updateRb(reverseTime, virtualDtime);
        virtualDtime = 0f;
    }

}
