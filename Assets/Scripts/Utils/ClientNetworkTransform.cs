using System.Collections;
using System.Collections.Generic;
using Unity.Netcode.Components;
using UnityEngine;

public class ClientNetworkTransform : NetworkTransform
{
    // ���� ���� ���� ������
    protected override bool OnIsServerAuthoritative()
    {
        return false; // Ŭ���̾�Ʈ ���� ���  , true �϶� ������ ������ ���� ����
    }


    // �� �����Ӹ��� ������, ��ġ ����ȭ
    protected override void Update()
    {
        CanCommitToTransform = IsOwner; // �����ڰ� ��ȯ�� Ŀ���� �� �ֵ��� ����
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


    // ��Ʈ��ũ ������Ʈ�� ������ �� ȣ��
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        CanCommitToTransform = IsOwner;  // �����ڰ� ��ȯ�� Ŀ���� �� �ֵ��� ����
    }
}
