using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class FlashBang : NetworkBehaviour
{
    [SerializeField] Image spriteRend;
    [ClientRpc]
    public void effectClientRpc(float time) {
        if(IsOwner) StartCoroutine(effect(time));
    }
    private IEnumerator effect(float time) {
        Color t_color = spriteRend.color;
        float t_prog = 0;
        while(t_prog < 1) {
            t_color.a = Mathf.Lerp(t_color.a, 1f, Time.deltaTime * 30);
            spriteRend.color = t_color;
            t_prog += Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(time);
        t_prog = 0;
        while(t_prog < 4) {
            t_color.a = Mathf.Lerp(t_color.a, 0f, Time.deltaTime * 0.8f);
            spriteRend.color = t_color;
            t_prog += Time.deltaTime;
            yield return null;
        }
        t_color.a = 0;
        spriteRend.color = t_color;
    }
}
