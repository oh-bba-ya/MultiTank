using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbiesList : MonoBehaviour
{
    [SerializeField] private Transform lobbyItemParent;
    [SerializeField] private LobbyItem lobbyItemPrefab;

    private bool isJoining;
    private bool isRefreshing;


    private void OnEnable()
    {
        RefreshList();
    }

    public async void RefreshList()
    {
        if (isRefreshing) return;

        isRefreshing = true;

        try
        {

            QueryLobbiesOptions options = new QueryLobbiesOptions();
            options.Count = 25;   // 로비를 나타날 개수

            options.Filters = new List<QueryFilter>() {
                new QueryFilter(
                    field: QueryFilter.FieldOptions.AvailableSlots,   // 자리가 있는 로비(방)
                    op: QueryFilter.OpOptions.GT,
                    value:"0"),   // 로비 표시 안함
                new QueryFilter(
                    field: QueryFilter.FieldOptions.IsLocked,   // 잠겨있는지
                    op: QueryFilter.OpOptions.EQ,
                    value:"0") // 로비 표시 안함
            };
            QueryResponse lobbies = await Lobbies.Instance.QueryLobbiesAsync(options);

            foreach(Transform child in lobbyItemParent)
            {
                Destroy(child.gameObject);
            }

            foreach(Lobby lobby in lobbies.Results)
            {
                LobbyItem lobbyItem = Instantiate(lobbyItemPrefab, lobbyItemParent);
                lobbyItem.Initialize(this, lobby);
            }
        }
        catch(LobbyServiceException e)
        {
            Debug.Log(e);
        }


        isRefreshing = false;
    }

    public async void JoinAsync(Lobby lobby)
    {
        if(isJoining) return;

        isJoining = true;

        try
        {
            Lobby joiningLobby = await Lobbies.Instance.JoinLobbyByIdAsync(lobby.Id);
            string joinCode = joiningLobby.Data["JoinCode"].Value;    // Lobby.Data 는 HostGmaeManager.cs의 StartHostAsync()에서 설정했다.

            await ClientSingleton.Instance.GameManager.StartClientAsync(joinCode);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);

        }

        isJoining = false;
    }
}
