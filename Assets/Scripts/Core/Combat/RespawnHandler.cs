using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class RespawnHandler : NetworkBehaviour
{
    [SerializeField] private TankPlayer playerPrefab;
    [SerializeField] private float keptCoinPercentage;

    public override void OnNetworkSpawn()
    {
        if(!IsServer) return;

        TankPlayer[] players = FindObjectsByType<TankPlayer>(FindObjectsSortMode.None);

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
        int keptCoinds = (int)(player.CoinWallet.TotalCoins.Value * (keptCoinPercentage / 100));

        Destroy(player.gameObject);

        // 삭제 후 다음 프레임에서 스폰..
        StartCoroutine(RespawnPlayer(player.OwnerClientId, keptCoinds));
    }

    IEnumerator RespawnPlayer(ulong ownerClientId, int keptCoins)
    {
        yield return null;

        TankPlayer playerInstance = Instantiate(playerPrefab, SpawnPoint.GetRandomSpawnPos(), Quaternion.identity);

        // 새로 생성된 오브젝트의 소유권을 기존 클라이언트로 다시 설정..
        playerInstance.NetworkObject.SpawnAsPlayerObject(ownerClientId);

        playerInstance.CoinWallet.TotalCoins.Value += keptCoins;
    }

}

