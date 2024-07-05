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
    private Allocation _allocation;  // 릴레이 할당 정보를 저장하는 변수
    private string _joinCode;  // 다른 플레이어가 참여할 수 있는 조인 코드를 저장하는 변수

    private const int MaxConnections = 20;   // 릴레이 서버 최대 연결개수 20이 최대 무료이다..
    private const string GameSceneName = "Game";


    // 호스트를 시작하는 비동기 메서드
    public async Task StartHostAsync()
    {
        try
        {
            // 릴레이를 얻어 할당
            _allocation = await Relay.Instance.CreateAllocationAsync(MaxConnections);
        }
        catch(Exception e)
        {
            Debug.Log(e);
            return;
        }

        try
        {
            // 릴레이 할당을 바탕으로 조인 코드 생성
            _joinCode = await Relay.Instance.GetJoinCodeAsync(_allocation.AllocationId);
            Debug.Log(_joinCode);
        }
        catch (Exception e)
        {
            Debug.Log(e);
            return;
        }

        // 네트워크 전송 설정을 가져옴
        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();

        // 릴레이 서버 데이터를 생성 , dtls로 보안 추가.
        RelayServerData relayServerData = new RelayServerData(_allocation, "dtls");

        // 네트워크 전송에 릴레이 서버 데이터 설정
        transport.SetRelayServerData(relayServerData);


        // 호스트 시작
        NetworkManager.Singleton.StartHost();

        // 게임 씬을 로드
        NetworkManager.Singleton.SceneManager.LoadScene(GameSceneName, LoadSceneMode.Single);

    }
}
