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
        // 유니티 서비스 연결
        await UnityServices.InitializeAsync();

        // 네트워크 클라이언트 전용 클래스 연결
        _networkClient = new NetworkClient(NetworkManager.Singleton);

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


        UserData userData = new UserData()
        {
            userName = PlayerPrefs.GetString(NameSelector.PlayerNameKey, "Missing Name"),
            userAuthId = AuthenticationService.Instance.PlayerId
        };

        string payload = JsonUtility.ToJson(userData);
        byte[] payloadBytes = Encoding.UTF8.GetBytes(payload);

        NetworkManager.Singleton.NetworkConfig.ConnectionData = payloadBytes;


        // 호스트 시작
        NetworkManager.Singleton.StartClient();

    }

    public void Dispose()
    {
        // IDispose
        // .Net 프레임워크에서 리소스를 명시적으로 해제하기 위해 사용되는 인터페이스
        // 가비지 컬렉터가 관리하지 않는 리소스(파일 핸들, 데이터 베이스 연결, 네트워크 소켓)를 명시적으로 해제하기 위한
        // 메커니즘 제공
        // Dispose : 리소스를 정리하는 작업을 수행
        _networkClient?.Dispose();
    }
}
