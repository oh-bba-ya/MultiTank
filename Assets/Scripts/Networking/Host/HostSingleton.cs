using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class HostSingleton : MonoBehaviour
{
    private static HostSingleton instance;

    public HostGameManager GameManager { get; private set; }

    public static HostSingleton Instance
    {
        get
        {
            if (instance != null) { return instance; }

            instance = FindObjectOfType<HostSingleton>();

            if (instance == null)
            {
                Debug.LogError("No HostSingleton in the scene!!!");
                return null;
            }

            return instance;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }


    public async Task CreateHost()
    {
        GameManager = new HostGameManager();

        await GameManager.StartHostAsync();
    }
}