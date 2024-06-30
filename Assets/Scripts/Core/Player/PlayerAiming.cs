using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerAiming : NetworkBehaviour
{
    [SerializeField] private Transform turretTransform;
    [SerializeField] private InputReader inputReader;


    /// <summary>
    /// 오브젝트의 위치가 설정된 이후에 포신 방향이 설정될 수 있도록.. lateUpdate 사용
    /// LateUpdate()가 Update이후에 호출된다.
    /// </summary>
    private void LateUpdate()
    {
        if(!IsOwner) { return; }

        Vector2 aimScreenPosition = inputReader.AimPosition;
        Vector2 aimWorldPosition = Camera.main.ScreenToWorldPoint(aimScreenPosition);  /// 스크린 좌표 -> 월드 좌표계로


        // 마우스 방향
        turretTransform.up = new Vector2(
            aimWorldPosition.x - turretTransform.position.x,
            aimWorldPosition.y - turretTransform.position.y
            );
    }
}
