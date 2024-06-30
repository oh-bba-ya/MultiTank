using System.Collections;
using System.Collections.Generic;
using Unity.Netcode.Components;
using UnityEngine;

public class ClientNetworkTransform : NetworkTransform
{
    // 서버 권한 설정 재정의
    protected override bool OnIsServerAuthoritative()
    {
        return false; // 클라이언트 권한 사용  , true 일때 서버가 권한을 갖고 있음
    }


    // 매 프레임마다 소유권, 위치 동기화
    protected override void Update()
    {
        CanCommitToTransform = IsOwner; // 소유자가 변환을 커밋할 수 있도록 설정
        base.Update();

        if(NetworkManager != null)
        {
            if(NetworkManager.IsConnectedClient || NetworkManager.IsListening)
            {
                if (CanCommitToTransform)
                {
                    TryCommitTransformToServer(transform, NetworkManager.LocalTime.Time);
                }
            }
        }
    }


    // 네트워크 오브젝트가 생성될 때 호출
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        CanCommitToTransform = IsOwner;  // 소유자가 변환을 커밋할 수 있도록 설정
    }
}
