using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkClient : IDisposable
{
    private NetworkManager _networkManager;

    private const string MenuSceneName = "Menu";

    public NetworkClient(NetworkManager networkManager)
    {
        this._networkManager = networkManager;

        _networkManager.OnClientDisconnectCallback += OnClientDisconnect;

    }

    /// <summary>
    /// Ŭ���̾�Ʈ�� ������ ����� MainMenu ������ �̵�
    /// </summary>
    /// <param name="clientId"></param>
    private void OnClientDisconnect(ulong clientId)
    {
        // host�� ��� clientId = 0���� �ȴ�.  , �ٸ� Ŭ���̾�Ʈ �� ���
        if(clientId != 0 &&clientId != _networkManager.LocalClientId) {
            return;
        }

        Disconect();

    }

    public void Disconect()
    {
        if (SceneManager.GetActiveScene().name != MenuSceneName)
        {
            SceneManager.LoadScene(MenuSceneName);
        }

        if (_networkManager.IsConnectedClient)
        {
            _networkManager.Shutdown();
        }
    }

    // Ŭ���̾�Ʈ ���� ����� �ݹ鿡 �߰��ߴ� �Լ����� �����ϴ°�
    public void Dispose()
    {
        if(_networkManager != null)
        {
            _networkManager.OnClientDisconnectCallback -= OnClientDisconnect;
        }
    }

}
