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
    /// 동적 생성시 Start()보다 더 일찍 호출된다.
    /// </summary>
    public override void OnNetworkSpawn()
    {
        if(!IsOwner) {  return; }

        _inputReader.MoveEvent += HandleMove;
    }


    /// <summary>
    /// Destroy()보다 더 빨리 호출된다.
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
    /// 물리 기능을 사용할때는 FixedUpdate를 사용한다.
    /// Update는 프레임 속도에 영향을 받기 때문에 불규칙하게 호출되어서 물리 검사가 제대로 되지 않을 수 가 있다.
    /// 또한, 과도한 Update 호출을 막기 위해 FixedUpdate를 사용한다.
    /// </summary>
    private void FixedUpdate()
    {
        if (!IsOwner) { return; }

        _rb.velocity = (Vector2)_bodyTransform.up * _previousMovementInput.y * _movementSpeed;  // Fixedupdate는 고정된 주기로 실행되기 때문에 Time.DeletaTime 곱할 필요가 없다.
    }

    void HandleMove(Vector2 movementInput)
    {
        _previousMovementInput = movementInput;
    }
}
