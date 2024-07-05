using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ApplicationController : MonoBehaviour
{
    [SerializeField] private ClientSingleton _clientPrefab;

    [SerializeField] private HostSingleton _hostPrefab;

    // ����Ƽ�� start() ���� �񵿱� ó���� �����ϴ�.
    async void Start()
    {
        DontDestroyOnLoad(gameObject);

        // ���� ������ ��ǻ�Ͱ� �������� üũ
        // ���� ��ǻ�ʹ� ����� �÷����ϴ°� �ƴϱ� ������ �׷��� �������� �ʿ����.
        await LaunchInMode(SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null);
    }



    private async Task LaunchInMode(bool isDedicatedServer)
    {

        // ���� �����ϰ��..
        if(isDedicatedServer)
        {

        }
        else
        {

            HostSingleton hostsingleton = Instantiate(_hostPrefab);

            // �÷��̾� ������ ������ ��ٸ�
            await hostsingleton.CreateHost();

            /* -------------------------------------- */

            ClientSingleton clientsingleton = Instantiate(_clientPrefab);

            // �÷��̾� ������ ������ ��ٸ�
            bool authenticated = await clientsingleton.CreateClient();


            // ���� �޴��� �̵�
            if(authenticated)
            {
                clientsingleton.GameManager.GoToMenu();
            }
        }
    }

}
