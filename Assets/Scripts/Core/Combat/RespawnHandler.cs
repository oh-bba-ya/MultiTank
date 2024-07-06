using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class RespawnHandler : NetworkBehaviour
{
    [SerializeField] private NetworkObject playerPrefab;

    public override void OnNetworkSpawn()
    {
        if(!IsServer) return;

        TankPlayer[] players = FindObjectsOfType<TankPlayer>();

        foreach(TankPlayer player in players)
        {
            HandlePlayerSpawned(player);
        }

        TankPlayer.OnPlayerSpawned += HandlePlayerSpawned;
        TankPlayer.OnPlayerDespawned += HandlePlayerDespawned;

    }


    public override void OnNetworkDespawn()
    {
        if (!IsServer) return;
        TankPlayer.OnPlayerSpawned -= HandlePlayerSpawned;
        TankPlayer.OnPlayerDespawned -= HandlePlayerDespawned;
    }

    private void HandlePlayerSpawned(TankPlayer player)
    {
        player.Health.OnDie += (health) => HandelPlayerDie(player);
    }

    private void HandlePlayerDespawned(TankPlayer player)
    {
        player.Health.OnDie -= (health) => HandelPlayerDie(player);
    }

    private void HandelPlayerDie(TankPlayer player)
    {
        Destroy(player.gameObject);

        // ���� �� ���� �����ӿ��� ����..
        StartCoroutine(RespawnPlayer(player.OwnerClientId));
    }

    IEnumerator RespawnPlayer(ulong ownerClientId)
    {
        yield return null;

        NetworkObject playerInstance = Instantiate(playerPrefab, SpawnPoint.GetRandomSpawnPos(), Quaternion.identity);

        // ���� ������ ������Ʈ�� �������� ���� Ŭ���̾�Ʈ�� �ٽ� ����..
        playerInstance.SpawnAsPlayerObject(ownerClientId);
    }

}

