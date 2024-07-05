using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class ClientGameManager
{
    private JoinAllocation _allocation;


    private const string MenuSceneName = "Menu";

    public async Task<bool> InitAsync()
    {
        // ����Ƽ ���� ����
        await UnityServices.InitializeAsync();

        // ���� �õ�..
        AuthState authState = await AuthenticationWrapper.DoAuth();

        if(authState == AuthState.Authenticated)
        {
            return true;
        }

        return false;
    }

    public void GoToMenu()
    {
        SceneManager.LoadScene(MenuSceneName);
    }

    public async Task StartClientAsync(string joinCode)
    {
        try
        {
            _allocation = await Relay.Instance.JoinAllocationAsync(joinCode);
        }
        catch (Exception ex)
        {
            Debug.Log(ex);
            return;
        }

        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();


        // ������ ���� �����͸� ����
        RelayServerData relayServerData = new RelayServerData(_allocation, "dtls");

        // ��Ʈ��ũ ���ۿ� ������ ���� ������ ����
        transport.SetRelayServerData(relayServerData);


        // ȣ��Ʈ ����
        NetworkManager.Singleton.StartClient();

    }
}
