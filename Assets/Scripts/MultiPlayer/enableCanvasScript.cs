using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class enableCanvasScript : NetworkBehaviour
{
    private void Start() {
        if(!IsOwner) gameObject.SetActive(false);
    }
}
