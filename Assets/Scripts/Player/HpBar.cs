using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class HpBar : NetworkBehaviour {
    [SerializeField] private Health health;
    [SerializeField] private RectTransform HPTrans;
    private float posStart;
    private float scaleStart;

    private void Start() {

        posStart = HPTrans.anchoredPosition.x;
        scaleStart = HPTrans.localScale.x;
    }
    void Update(){
        if(!IsOwner) {
            HPTrans.sizeDelta =  new Vector2(55 * (0 / 100f), 1);
            HPTrans.anchoredPosition = new Vector2(posStart - (55 * ((100-0) / 100f)) * scaleStart/2, HPTrans.anchoredPosition.y);
            return;
        }
        HPTrans.sizeDelta =  new Vector2(55 * (health.hp / 100f), 1);
        HPTrans.anchoredPosition = new Vector2(posStart - (55 * ((100-health.hp) / 100f)) * scaleStart/2, HPTrans.anchoredPosition.y);
    }
}
