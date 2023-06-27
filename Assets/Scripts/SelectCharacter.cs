using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectCharacter : NetworkBehaviour{


    [SerializeField] private Transform playerTransform;
    [SerializeField] private GameObject pausMenuObject;
    [SerializeField] private Health playerHealth;
    [SerializeField] private Ability playerAbility;
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private RuntimeAnimatorController BlackAnimation;
    [SerializeField] private RuntimeAnimatorController RedAnimation;
    [SerializeField] private RuntimeAnimatorController YellowAnimation;
    [SerializeField] private RuntimeAnimatorController BlueAnimation;
    [SerializeField] private RuntimeAnimatorController GreenAnimation;
    [SerializeField] private Identift iden;

    private void Start() {
        if(!iden.isTheOwner()) Destroy(gameObject);
    }

    public void redPlayerBtn() {
        finalActions(RedAnimation, "Red");
    }
    public void blackPlayerBtn() {
        finalActions(BlackAnimation, "Black");
    }
    public void bluePlayerBtn() {
        finalActions(BlueAnimation, "Blue");
    }
    public void yellowPlayerBtn() {
        finalActions(YellowAnimation, "Yellow");
    }
    public void greenPlayerBtn() {
        finalActions(GreenAnimation, "Green");
    }

    void finalActions(RuntimeAnimatorController colorAnimation, string playerType) {

        playerAnimator.runtimeAnimatorController = colorAnimation;
        playerAbility.playerType = playerType;
        gameObject.SetActive(false);
        playerHealth.dead.Value = false;

        Transform spawnPoints = GameObject.Find("SpawnPoints").transform;
        List<Transform> spawnPointsList = new();
        foreach(Transform child in spawnPoints) {
            spawnPointsList.Add(child);
        }
        Transform pos = spawnPointsList[UnityEngine.Random.Range(0, spawnPointsList.Count)];
        playerTransform.position = new Vector2(pos.position.x, pos.position.y);
    }


    [ServerRpc]
    private void locatePlayersServerRpc() {
        GameObject[] playersObj = GameObject.FindGameObjectsWithTag("Character");
        foreach(GameObject player in playersObj) {
            if(player.transform.GetChild(0).gameObject.layer == 12) continue;
            Transform t_transform = player.transform.GetChild(0).transform;
            t_transform.position = new Vector3(t_transform.position.x + 0.002f, t_transform.position.y + 0.002f, t_transform.position.z);
        }
    }
}
