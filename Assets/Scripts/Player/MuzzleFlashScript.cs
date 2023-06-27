using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MuzzleFlashScript : NetworkBehaviour {
    [SerializeField] private float flashTime;

    private void Start() {
        transform.localPosition = new Vector3(0f, -200f, 0f);
    }

    public IEnumerator showFlash() {
        transform.Rotate(180f, 0f, 0f);
        transform.localPosition = new Vector3(0.4131f, 0.0616f, 0f);
        yield return new WaitForSeconds(flashTime);
        transform.localPosition = new Vector3(0f, -200f, 0f);
    }
}
