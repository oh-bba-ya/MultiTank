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
    /// �÷��̾ ������ �����ϸ� ���ο� ��ġ�� ����
    /// </summary>
    /// <param name="coin">�÷��̾ ������ ����</param>
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

            // OverlapCircleNonAlloc : ����Ƽ Physics2D Ŭ������ ���� �ִ� �Լ�
            // ������ �� �������� �ִ� ��� �ݶ��̴��� �Ҵ� ���� ����
            // Ư�� ���� �浹 �˻縦 �ؾ� �ϴ� ���ӿ� �����ϴ�. ���ο� �޸𸮸� �Ҵ����� �ʱ� ������
            // ������ �÷��� ������带 ���� �� �ִ�.
            int numColliders = Physics2D.OverlapCircleNonAlloc(spawnPoint, _coinRadius, _coinBuffer, _layerMask);


            // ���� ��ġ, ���� �������� �������� �ٸ� ������ �������� �ʴ´ٸ�..
            if (numColliders == 0)
            {
                return spawnPoint;
            }
        }

    }
}
