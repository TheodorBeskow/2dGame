using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PwBar : NetworkBehaviour {
    [SerializeField] private Ability power;
    [SerializeField] private RectTransform PWTrans;
    private float posStart;
    private float scaleStart;

    private void Start() {
        if(!IsOwner) return;

        posStart = PWTrans.anchoredPosition.x;
        scaleStart = PWTrans.localScale.x;
    }
    void Update() {
        if(!IsOwner) {
            PWTrans.sizeDelta = new Vector2(55 * (0 / 100f), 1);
            PWTrans.anchoredPosition = new Vector2(posStart - (55 * ((100 - 0) / 100f)) * scaleStart / 2, PWTrans.anchoredPosition.y);
        }
        else {
            PWTrans.sizeDelta = new Vector2(55 * (power.progress / 100f), 1);
            PWTrans.anchoredPosition = new Vector2(posStart - (55 * ((100 - power.progress) / 100f)) * scaleStart / 2, PWTrans.anchoredPosition.y);
            // Debug.Log(power.progress);
        }

    }
}
