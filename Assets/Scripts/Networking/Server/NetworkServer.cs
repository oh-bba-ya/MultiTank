using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkServer : IDisposable
{
    private NetworkManager _networkManager;

    private Dictionary<ulong, string> _clientIdToAuth = new Dictionary<ulong, string>();
    private Dictionary<string,UserData> authIdToUserData = new Dictionary<string,UserData>();

    public NetworkServer(NetworkManager networkManager)
    {
        this._networkManager = networkManager;

        _networkManager.ConnectionApprovalCallback += ApprovalCheck;
        _networkManager.OnServerStarted += OnNetworkReady;
    }

    private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        string payload = System.Text.Encoding.UTF8.GetString(request.Payload);
        UserData userData =  JsonUtility.FromJson<UserData>(payload);

        _clientIdToAuth[request.ClientNetworkId] = userData.userAuthId;
        authIdToUserData[userData.userAuthId] = userData;

        response.Approved = true;
        response.CreatePlayerObject = true;   // �÷��̾� ��ü ����, false�� �Ǿ� ������ �÷��̾� Prefab�� �������� �ʴ´�.
    }

    private void OnNetworkReady()
    {
        _networkManager.OnClientDisconnectCallback += OnClientDisconnect;
    }

    /// <summary>
    /// ������ ����� ����..
    /// </summary>
    /// <param name="clientId"></param>
    private void OnClientDisconnect(ulong clientId)
    {
        if(_clientIdToAuth.TryGetValue(clientId, out string authId))
        {
            _clientIdToAuth.Remove(clientId);

            authIdToUserData.Remove(authId);
        }
    }

    public void Dispose()
    {
        if (_networkManager == null) { return; }

        _networkManager.ConnectionApprovalCallback -= ApprovalCheck;
        _networkManager.OnClientDisconnectCallback -= OnClientDisconnect;
        _networkManager.OnServerStarted -= OnNetworkReady;

        if(_networkManager.IsListening)
        {
            _networkManager.Shutdown();
        }
    }
}
