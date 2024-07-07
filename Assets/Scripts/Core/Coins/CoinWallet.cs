using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CoinWallet : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private Health health;
    [SerializeField] private BountyCoin coinPrefab;


    [Header("Settings")]
    [SerializeField] private float coinSpread = 3f;
    [SerializeField] private float bountyPercentage = 50f;
    [SerializeField] private int bountyCoinCount = 10;
    [SerializeField] private int minBountyCoinValue = 5;
    [SerializeField] private LayerMask layerMask;

    private Collider2D[] coinBuffer = new Collider2D[1];
    private float coinRadius;


    public NetworkVariable<int> TotalCoins = new NetworkVariable<int>();


    public override void OnNetworkSpawn()
    {
        if (!IsServer) { return; }

        coinRadius = coinPrefab.GetComponent<CircleCollider2D>().radius;

        health.OnDie += HandleDie;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsServer) { return; }

        health.OnDie -= HandleDie;
    }


    public void SpendCoins(int costToFire)
    {
        TotalCoins.Value -= costToFire;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!collision.TryGetComponent<Coin>(out Coin coin)) {  return; }

        int coinValue = coin.Collect();

        if (!IsServer)
        {
            return;
        }

        TotalCoins.Value += coinValue;
    }

    private void HandleDie(Health health)
    {
        int bountyValue = (int)(TotalCoins.Value * (bountyPercentage / 100));
        int bountyCoinValue = bountyValue / bountyCoinCount;

        if(bountyCoinValue < minBountyCoinValue) {
            Debug.Log("코인 안떨구기");
            return; }

        for(int i=0;i<bountyCoinCount; i++)
        {
            Debug.Log("코인 떨구기");
            BountyCoin coinInstance = Instantiate(coinPrefab, GetSpawnPoint(), Quaternion.identity);
            coinInstance.SetValue(bountyCoinValue);
            coinInstance.NetworkObject.Spawn();
        }
    }


    private Vector2 GetSpawnPoint()
    {

        while (true)
        {
            Vector2 spawnPoint = (Vector2)transform.position + UnityEngine.Random.insideUnitCircle * coinSpread;


            // OverlapCircleNonAlloc : 유니티 Physics2D 클래스에 속해 있는 함수
            // 지정된 원 영역내에 있는 모든 콜라이더를 할당 없이 검출
            // 특히 자주 충돌 검사를 해야 하는 게임에 유용하다. 새로운 메모리를 할당하지 않기 때문에
            // 가비지 컬레션 오버헤드를 피할 수 있다.
            int numColliders = Physics2D.OverlapCircleNonAlloc(spawnPoint, coinRadius, coinBuffer, layerMask);


            // 현재 위치, 코인 반지름의 범위내에 다른 코인이 존재하지 않는다면..
            if (numColliders == 0)
            {
                return spawnPoint;
            }
        }

    }

}
