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
    /// 클라이언트가 연결이 끊기면 MainMenu 씬으로 이동
    /// </summary>
    /// <param name="clientId"></param>
    private void OnClientDisconnect(ulong clientId)
    {
        // host의 경우 clientId = 0으로 된다.  , 다른 클라이언트 일 경우
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

    // 클라이언트 연결 종료시 콜백에 추가했던 함수들을 삭제하는것
    public void Dispose()
    {
        if(_networkManager != null)
        {
            _networkManager.OnClientDisconnectCallback -= OnClientDisconnect;
        }
    }

}
