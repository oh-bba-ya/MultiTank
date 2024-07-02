using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ProjectileLauncher : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private InputReader _inputReader;
    [SerializeField] private CoinWallet _coinWallet;
    [SerializeField] private Transform _projectileSpawnPoint;
    [SerializeField] private GameObject _serverProjectilePrefab;
    [SerializeField] private GameObject _clientProjectilePrefab;
    [SerializeField] private GameObject _muzzleFlash;
    [SerializeField] private Collider2D _playerCollider;

    [Header("Settings")]
    [SerializeField] private float _projectileSpeed;
    [SerializeField] private float _fireRate;
    [SerializeField] private float _muzzleFlashDuration;
    [SerializeField] private int _costToFire;    // 발사에 필요한 코인 점수

    private bool _shouldFire;
    private float _timer;
    private float _muzzleFlashTimer;


    public override void OnNetworkSpawn()
    {
        if(!IsOwner) { return; }

        _inputReader.PrimaryFireEvenet += HandlePrimaryFire;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner) { return; }

        _inputReader.PrimaryFireEvenet -= HandlePrimaryFire;
    }

    // Update is called once per frame
    void Update()
    {

        if(_muzzleFlashTimer > 0f)
        {
            _muzzleFlashTimer -= Time.deltaTime;
            if(_muzzleFlashTimer <= 0f)
            {
                _muzzleFlash.SetActive(false);
            }
        }

        if (!IsOwner) { return ; }

        if (_timer > 0)
        {
            _timer -= Time.deltaTime;
        }

        if(!_shouldFire) { return; }

        if(_timer > 0)
        {
            return;
        }

        // 플레이어가 소지한 코인 점수가 발사에 필요한 코인 점수 미만이라면..
        if (_coinWallet.TotalCoins.Value < _costToFire) { return; }


        PrimaryFireServerRpc(_projectileSpawnPoint.position, _projectileSpawnPoint.up);

        SpawnDummyProjectile(_projectileSpawnPoint.position, _projectileSpawnPoint.up);


        // 발사 시간 저장... (시간이 지날수록 부동소수점이 쌓여 오차가 존재한다.)
        _timer = 1 / _fireRate;
    }

    void HandlePrimaryFire(bool shouldFire)
    {
        this._shouldFire = shouldFire;
    }


    [ServerRpc]
    void PrimaryFireServerRpc(Vector3 spawnPos, Vector3 direction)
    {
        // 플레이어가 소지한 코인 점수가 발사에 필요한 코인 점수 미만이라면..
        if (_coinWallet.TotalCoins.Value < _costToFire) {  return ; }

        _coinWallet.SpendCoins(_costToFire);

        // 프로젝틸 생성..
        GameObject projectileInstance = Instantiate(_serverProjectilePrefab, spawnPos, Quaternion.identity);

        // 방향 설정
        projectileInstance.transform.up = direction;


        Physics2D.IgnoreCollision(_playerCollider, projectileInstance.GetComponent<Collider2D>());

        if(projectileInstance.TryGetComponent<DealDamageOnContact>(out DealDamageOnContact dealDamage))
        {
            dealDamage.SetOwner(OwnerClientId);
        }

        // Rigidbody가 존재한다면
        if (projectileInstance.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
        {
            // Projectile 발사
            rb.velocity = rb.transform.up * _projectileSpeed;
        }

        SpawnDummyProjectileClientRpc(spawnPos,direction);
    }

    [ClientRpc]
    private void SpawnDummyProjectileClientRpc(Vector3 spawnPos, Vector3 direction)
    {

        // 소유자가 아닌 사람만 호출가능..
        if(IsOwner) { return; }

        
        SpawnDummyProjectile(spawnPos, direction);

    }


    private void SpawnDummyProjectile(Vector3 spawnPos, Vector3 direction)
    {
        _muzzleFlash.SetActive(true);
        _muzzleFlashTimer = _muzzleFlashDuration;


       GameObject projectileInstance =  Instantiate(_clientProjectilePrefab, spawnPos, Quaternion.identity);


        projectileInstance.transform.up = direction;

        Physics2D.IgnoreCollision(_playerCollider, projectileInstance.GetComponent<Collider2D>());

        if(projectileInstance.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
        {
            rb.velocity = rb.transform.up * _projectileSpeed;
        }
    }

}
