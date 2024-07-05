using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NameSelector : MonoBehaviour
{
    [SerializeField] private TMP_InputField nameField;
    [SerializeField] private Button connectionButton;
    [SerializeField] private int minNameLength = 1;
    [SerializeField] private int maxNameLength = 12;

    private const string PlayerNameKey = "PlayerName";

    private void Start()
    {
        // ���� ����̽����� üũ..
        if(SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);   // �ݵ�� 2��° ������ �̵�
            return;
        }

        nameField.text = PlayerPrefs.GetString(PlayerNameKey,string.Empty);
        HandleNameChanged();
    }


    public void HandleNameChanged()
    {
        connectionButton.interactable = nameField.text.Length >= minNameLength && nameField.text.Length <= maxNameLength;
    }

    public void Connect()
    {
        PlayerPrefs.SetString(PlayerNameKey,nameField.text);



        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);   // �ݵ�� 2��° ������ �̵�
    }
}
