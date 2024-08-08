using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManagerUI : MonoBehaviour
{
    [SerializeField] private Button hostButton;
    [SerializeField] private Button joinButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Canvas menuCanvas;
    
    private NetworkManager networkManager;

    private void Awake()
    {
        networkManager = FindObjectOfType<NetworkManager>();

        if (networkManager == null)
            return;
        
        hostButton.onClick.AddListener(() =>
        {
            networkManager.StartHost();
        });
        
        joinButton.onClick.AddListener(() =>
        {
            networkManager.StartClient();
        });
        
        quitButton.onClick.AddListener(() =>
        {
            Application.Quit();
        });
        
        networkManager.OnClientConnectedCallback += OnClientConnect;
        networkManager.OnClientDisconnectCallback += OnClientDisconnect;
    }

    private void OnDestroy()
    {
        networkManager.OnClientConnectedCallback -= OnClientConnect;
        networkManager.OnClientDisconnectCallback -= OnClientDisconnect;
    }

    private void OnClientConnect(ulong clientId)
    {
        if (networkManager.LocalClientId == clientId && menuCanvas != null)
        {
            menuCanvas.gameObject.SetActive(false);
        }
    }

    private void OnClientDisconnect(ulong clientId)
    {
        Debug.Log("Disconnect");
    }
    
}
