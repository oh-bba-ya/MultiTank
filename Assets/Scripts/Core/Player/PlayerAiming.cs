using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerAiming : NetworkBehaviour
{
    [SerializeField] private Transform turretTransform;
    [SerializeField] private InputReader inputReader;


    /// <summary>
    /// ������Ʈ�� ��ġ�� ������ ���Ŀ� ���� ������ ������ �� �ֵ���.. lateUpdate ���
    /// LateUpdate()�� Update���Ŀ� ȣ��ȴ�.
    /// </summary>
    private void LateUpdate()
    {
        if(!IsOwner) { return; }

        Vector2 aimScreenPosition = inputReader.AimPosition;
        Vector2 aimWorldPosition = Camera.main.ScreenToWorldPoint(aimScreenPosition);  /// ��ũ�� ��ǥ -> ���� ��ǥ���


        // ���콺 ����
        turretTransform.up = new Vector2(
            aimWorldPosition.x - turretTransform.position.x,
            aimWorldPosition.y - turretTransform.position.y
            );
    }
}
