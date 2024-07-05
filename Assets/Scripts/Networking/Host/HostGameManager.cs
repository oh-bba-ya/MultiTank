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
    private Allocation _allocation;  // ������ �Ҵ� ������ �����ϴ� ����
    private string _joinCode;  // �ٸ� �÷��̾ ������ �� �ִ� ���� �ڵ带 �����ϴ� ����
    private string _lobbyId;

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

        // Host �� �����ϰԵǸ� �κ�(��)�� �����غ���.
        try
        {
            CreateLobbyOptions lobbyOptions = new CreateLobbyOptions();  // �κ� �ɼ�
            lobbyOptions.IsPrivate = false;    // ������
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

        


        // ȣ��Ʈ ����
        NetworkManager.Singleton.StartHost();

        // ���� ���� �ε�
        NetworkManager.Singleton.SceneManager.LoadScene(GameSceneName, LoadSceneMode.Single);

    }

    /// <summary>
    /// ����Ƽ �κ�ý����� ��� ���� �ð����� �κ� ����ִٴ°��� �˷�����Ѵ�.
    /// ������ heartbeat�� ���� �����ð����� ������ �κ�(��)(��)�� �����Ѵٴ� ���� �˷��ִ°��̴�.
    /// </summary>
    /// <param name="waitTimeSecdons"></param>
    /// <returns></returns>
    private IEnumerator HeartbeatLobby(float waitTimeSecdons)
    {
        // return new �� �ϰ� �Ǹ� 15�ʸ��� waitforseondsRealtime �ν��Ͻ��� �����ϹǷ� Caching�� �ؼ� ��뵵�� ����.
        WaitForSecondsRealtime delay = new WaitForSecondsRealtime(waitTimeSecdons);
        while (true)
        {
            Lobbies.Instance.SendHeartbeatPingAsync(_lobbyId);
            yield return delay;
        }
    }
}
