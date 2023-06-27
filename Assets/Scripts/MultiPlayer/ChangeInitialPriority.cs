using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ChangeInitialPriority : NetworkBehaviour{
    [SerializeField] private CinemachineVirtualCamera vcam;

    private void Start() {
        if(!IsOwner) return;
        vcam.Priority = 11;
    }
}
