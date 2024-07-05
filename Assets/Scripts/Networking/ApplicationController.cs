using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ApplicationController : MonoBehaviour
{
    [SerializeField] private ClientSingleton _clientPrefab;

    [SerializeField] private HostSingleton _hostPrefab;

    // 유니티의 start() 또한 비동기 처리가 가능하다.
    async void Start()
    {
        DontDestroyOnLoad(gameObject);

        // 현재 접속한 컴퓨터가 서버인지 체크
        // 서버 컴퓨터는 사람이 플레이하는게 아니기 때문에 그래픽 렌더링이 필요없다.
        await LaunchInMode(SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null);
    }



    private async Task LaunchInMode(bool isDedicatedServer)
    {

        // 전용 서버일경우..
        if(isDedicatedServer)
        {

        }
        else
        {

            HostSingleton hostsingleton = Instantiate(_hostPrefab);

            // 플레이어 인증될 때까지 기다림
            await hostsingleton.CreateHost();

            /* -------------------------------------- */

            ClientSingleton clientsingleton = Instantiate(_clientPrefab);

            // 플레이어 인증될 때까지 기다림
            bool authenticated = await clientsingleton.CreateClient();


            // 메인 메뉴로 이동
            if(authenticated)
            {
                clientsingleton.GameManager.GoToMenu();
            }
        }
    }

}
