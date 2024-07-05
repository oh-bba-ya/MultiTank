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
        // 유니티 서비스 연결
        await UnityServices.InitializeAsync();

        // 인증 시도..
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


        // 릴레이 서버 데이터를 생성
        RelayServerData relayServerData = new RelayServerData(_allocation, "dtls");

        // 네트워크 전송에 릴레이 서버 데이터 설정
        transport.SetRelayServerData(relayServerData);


        // 호스트 시작
        NetworkManager.Singleton.StartClient();

    }
}
