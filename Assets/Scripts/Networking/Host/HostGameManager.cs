using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Lobbies;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Services.Lobbies.Models;
using UnityEngine.UIElements;

public class HostGameManager
{
    private Allocation _allocation;  // 릴레이 할당 정보를 저장하는 변수
    private string _joinCode;  // 다른 플레이어가 참여할 수 있는 조인 코드를 저장하는 변수
    private string _lobbyId;

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

        // Host 로 시작하게되면 로비(방)을 생성해보자.
        try
        {
            CreateLobbyOptions lobbyOptions = new CreateLobbyOptions();  // 로비 옵션
            lobbyOptions.IsPrivate = false;    // 공개방
            lobbyOptions.Data = new Dictionary<string, Unity.Services.Lobbies.Models.DataObject>()
            {
                {
                    "JoinCode", new DataObject
                    (
                        visibility: DataObject.VisibilityOptions.Member,
                        value: _joinCode
                    )
                }
            };
            Lobby lobby = await Lobbies.Instance.CreateLobbyAsync("My Lobby",MaxConnections, lobbyOptions);

            _lobbyId = lobby.Id;

            HostSingleton.Instance.StartCoroutine(HeartbeatLobby(15));
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
            return;
        }

        


        // 호스트 시작
        NetworkManager.Singleton.StartHost();

        // 게임 씬을 로드
        NetworkManager.Singleton.SceneManager.LoadScene(GameSceneName, LoadSceneMode.Single);

    }

    /// <summary>
    /// 유니티 로비시스템의 경우 일정 시간마다 로비가 살아있다는것을 알려줘야한다.
    /// 때문에 heartbeat를 통해 일정시간마다 생성된 로비(방)(이)가 존재한다는 것을 알려주는것이다.
    /// </summary>
    /// <param name="waitTimeSecdons"></param>
    /// <returns></returns>
    private IEnumerator HeartbeatLobby(float waitTimeSecdons)
    {
        // return new 를 하게 되면 15초마다 waitforseondsRealtime 인스턴스를 생성하므로 Caching을 해서 사용도록 하자.
        WaitForSecondsRealtime delay = new WaitForSecondsRealtime(waitTimeSecdons);
        while (true)
        {
            Lobbies.Instance.SendHeartbeatPingAsync(_lobbyId);
            yield return delay;
        }
    }
}
