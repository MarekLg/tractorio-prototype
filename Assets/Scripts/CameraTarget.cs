using System;
using Unity.Netcode;
using UnityEngine;

public sealed class CameraTarget : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        
        if (IsLocalPlayer)
        {
            FindAnyObjectByType<CameraController>().SetTarget(transform);
        }
    }
}
