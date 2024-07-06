using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Cinemachine;

public class TankPlayer : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private CinemachineVirtualCamera _virtualCamera;

    [Header("Settings")]
    [SerializeField] private int ownerPriority = 15;
    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            _virtualCamera.Priority = ownerPriority;
        }
    }
}

