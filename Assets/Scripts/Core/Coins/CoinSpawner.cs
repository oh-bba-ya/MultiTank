using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CoinSpawner : NetworkBehaviour
{
    [SerializeField] private RespawningCoin _coinPrefab;

    [SerializeField] private int _maxCoins = 50;
    [SerializeField] private int _coinValue = 10;
    [SerializeField] private Vector2 _xSpawnRange;
    [SerializeField] private Vector2 _ySpawnRange;
    [SerializeField] private LayerMask _layerMask;

    private float _coinRadius;


    private Collider2D[] _coinBuffer = new Collider2D[1];

    public override void OnNetworkSpawn()
    {
        if(!IsServer) return;

        _coinRadius = _coinPrefab.GetComponent<CircleCollider2D>().radius;

        for(int i=0;i< _maxCoins; i++)
        {
            SpawnCoin();
        }
    }

    private void SpawnCoin()
    {
        RespawningCoin coinInstance = Instantiate(_coinPrefab, GetSpawnPoint(), Quaternion.identity);

        coinInstance.SetValue(_coinValue);
        coinInstance.GetComponent<NetworkObject>().Spawn();

        coinInstance.OnCollected += HandleCoinCollected;
    }


    /// <summary>
    /// 플레이어가 코인을 수집하면 새로운 위치로 변경
    /// </summary>
    /// <param name="coin">플레이어가 수집한 코인</param>
    private void HandleCoinCollected(RespawningCoin coin)
    {
        coin.transform.position = GetSpawnPoint();
        coin.Reset();
    }

    private Vector2 GetSpawnPoint()
    {
        float x = 0;
        float y = 0;

        while(true)
        {
            x = Random.Range(_xSpawnRange.x, _xSpawnRange.y);
            y = Random.Range(_ySpawnRange.x, _ySpawnRange.y);
            Vector2 spawnPoint = new Vector2(x, y);

            // OverlapCircleNonAlloc : 유니티 Physics2D 클래스에 속해 있는 함수
            // 지정된 원 영역내에 있는 모든 콜라이더를 할당 없이 검출
            // 특히 자주 충돌 검사를 해야 하는 게임에 유용하다. 새로운 메모리를 할당하지 않기 때문에
            // 가비지 컬레션 오버헤드를 피할 수 있다.
            int numColliders = Physics2D.OverlapCircleNonAlloc(spawnPoint, _coinRadius, _coinBuffer, _layerMask);


            // 현재 위치, 코인 반지름의 범위내에 다른 코인이 존재하지 않는다면..
            if (numColliders == 0)
            {
                return spawnPoint;
            }
        }

    }
}
