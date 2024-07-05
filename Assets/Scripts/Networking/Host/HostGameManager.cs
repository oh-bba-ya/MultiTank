using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HostGameManager
{
    private Allocation _allocation;  // ������ �Ҵ� ������ �����ϴ� ����
    private string _joinCode;  // �ٸ� �÷��̾ ������ �� �ִ� ���� �ڵ带 �����ϴ� ����

    private const int MaxConnections = 20;   // ������ ���� �ִ� ���ᰳ�� 20�� �ִ� �����̴�..
    private const string GameSceneName = "Game";


    // ȣ��Ʈ�� �����ϴ� �񵿱� �޼���
    public async Task StartHostAsync()
    {
        try
        {
            // �����̸� ��� �Ҵ�
            _allocation = await Relay.Instance.CreateAllocationAsync(MaxConnections);
        }
        catch(Exception e)
        {
            Debug.Log(e);
            return;
        }

        try
        {
            // ������ �Ҵ��� �������� ���� �ڵ� ����
            _joinCode = await Relay.Instance.GetJoinCodeAsync(_allocation.AllocationId);
            Debug.Log(_joinCode);
        }
        catch (Exception e)
        {
            Debug.Log(e);
            return;
        }

        // ��Ʈ��ũ ���� ������ ������
        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();

        // ������ ���� �����͸� ���� , dtls�� ���� �߰�.
        RelayServerData relayServerData = new RelayServerData(_allocation, "dtls");

        // ��Ʈ��ũ ���ۿ� ������ ���� ������ ����
        transport.SetRelayServerData(relayServerData);


        // ȣ��Ʈ ����
        NetworkManager.Singleton.StartHost();

        // ���� ���� �ε�
        NetworkManager.Singleton.SceneManager.LoadScene(GameSceneName, LoadSceneMode.Single);

    }
}
