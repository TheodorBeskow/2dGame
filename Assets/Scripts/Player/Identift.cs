using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Identift : NetworkBehaviour
{
    public bool isTheOwner() {
        return IsOwner;
    }
}
