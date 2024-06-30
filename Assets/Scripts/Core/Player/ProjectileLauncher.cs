using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ProjectileLauncher : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private InputReader _inputReader;
    [SerializeField] private Transform _projectileSpawnPoint;
    [SerializeField] private GameObject _serverProjectilePrefab;
    [SerializeField] private GameObject _clientProjectilePrefab;
    [SerializeField] private GameObject _muzzleFlash;
    [SerializeField] private Collider2D _playerCollider;

    [Header("Settings")]
    [SerializeField] private float _projectileSpeed;
    [SerializeField] private float _fireRate;
    [SerializeField] private float _muzzleFlashDuration;

    private bool _shouldFire;
    private float _previousFireTime;
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

        if(!_shouldFire) { return; }

        if(Time.time < ( 1 / _fireRate) + _previousFireTime)
        {
            return;
        }

        PrimaryFireServerRpc(_projectileSpawnPoint.position, _projectileSpawnPoint.up);

        SpawnDummyProjectile(_projectileSpawnPoint.position, _projectileSpawnPoint.up);


        // �߻� �ð� ����... (�ð��� �������� �ε��Ҽ����� �׿� ������ �����Ѵ�.)
        _previousFireTime = Time.time;
    }

    void HandlePrimaryFire(bool shouldFire)
    {
        this._shouldFire = shouldFire;
    }


    [ServerRpc]
    void PrimaryFireServerRpc(Vector3 spawnPos, Vector3 direction)
    {
        // ������ƿ ����..
        GameObject projectileInstance = Instantiate(_serverProjectilePrefab, spawnPos, Quaternion.identity);

        // ���� ����
        projectileInstance.transform.up = direction;


        Physics2D.IgnoreCollision(_playerCollider, projectileInstance.GetComponent<Collider2D>());

        // Rigidbody�� �����Ѵٸ�
        if (projectileInstance.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
        {
            // Projectile �߻�
            rb.velocity = rb.transform.up * _projectileSpeed;
        }

        SpawnDummyProjectileClientRpc(spawnPos,direction);
    }

    [ClientRpc]
    private void SpawnDummyProjectileClientRpc(Vector3 spawnPos, Vector3 direction)
    {

        // �����ڰ� �ƴ� ����� ȣ�Ⱑ��..
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
