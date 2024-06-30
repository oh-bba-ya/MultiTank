using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private InputReader _inputReader;
    [SerializeField] private Transform _bodyTransform;
    [SerializeField] private Rigidbody2D _rb;

    [Header("Settings")]
    [SerializeField] private float _movementSpeed = 4f;
    [SerializeField] private float turningRate = 270f;


    private Vector2 _previousMovementInput;

    /// <summary>
    /// ���� ������ Start()���� �� ���� ȣ��ȴ�.
    /// </summary>
    public override void OnNetworkSpawn()
    {
        if(!IsOwner) {  return; }

        _inputReader.MoveEvent += HandleMove;
    }


    /// <summary>
    /// Destroy()���� �� ���� ȣ��ȴ�.
    /// </summary>
    public override void OnNetworkDespawn()
    {
        if (!IsOwner) { return; }

        _inputReader.MoveEvent -= HandleMove;
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) { return; }

        float zRotation = _previousMovementInput.x * -turningRate * Time.deltaTime;
        _bodyTransform.Rotate(0f, 0f, zRotation);


    }

    /// <summary>
    /// ���� ����� ����Ҷ��� FixedUpdate�� ����Ѵ�.
    /// Update�� ������ �ӵ��� ������ �ޱ� ������ �ұ�Ģ�ϰ� ȣ��Ǿ ���� �˻簡 ����� ���� ���� �� �� �ִ�.
    /// ����, ������ Update ȣ���� ���� ���� FixedUpdate�� ����Ѵ�.
    /// </summary>
    private void FixedUpdate()
    {
        if (!IsOwner) { return; }

        _rb.velocity = (Vector2)_bodyTransform.up * _previousMovementInput.y * _movementSpeed;  // Fixedupdate�� ������ �ֱ�� ����Ǳ� ������ Time.DeletaTime ���� �ʿ䰡 ����.
    }

    void HandleMove(Vector2 movementInput)
    {
        _previousMovementInput = movementInput;
    }
}
