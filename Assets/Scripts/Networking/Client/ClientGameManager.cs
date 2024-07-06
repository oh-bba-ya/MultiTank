using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;


public class ClientGameManager : IDisposable
{
    private JoinAllocation _allocation;

    private NetworkClient _networkClient;


    private const string MenuSceneName = "Menu";

    public async Task<bool> InitAsync()
    {
        // ����Ƽ ���� ����
        await UnityServices.InitializeAsync();

        // ��Ʈ��ũ Ŭ���̾�Ʈ ���� Ŭ���� ����
        _networkClient = new NetworkClient(NetworkManager.Singleton);

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


        UserData userData = new UserData()
        {
            userName = PlayerPrefs.GetString(NameSelector.PlayerNameKey, "Missing Name"),
            userAuthId = AuthenticationService.Instance.PlayerId
        };

        string payload = JsonUtility.ToJson(userData);
        byte[] payloadBytes = Encoding.UTF8.GetBytes(payload);

        NetworkManager.Singleton.NetworkConfig.ConnectionData = payloadBytes;


        // ȣ��Ʈ ����
        NetworkManager.Singleton.StartClient();

    }

    public void Dispose()
    {
        // IDispose
        // .Net �����ӿ�ũ���� ���ҽ��� ��������� �����ϱ� ���� ���Ǵ� �������̽�
        // ������ �÷��Ͱ� �������� �ʴ� ���ҽ�(���� �ڵ�, ������ ���̽� ����, ��Ʈ��ũ ����)�� ��������� �����ϱ� ����
        // ��Ŀ���� ����
        // Dispose : ���ҽ��� �����ϴ� �۾��� ����
        _networkClient?.Dispose();
    }
}
