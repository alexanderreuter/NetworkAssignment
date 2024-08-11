using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class NetworkManagerUI : MonoBehaviour
{
    private NetworkManager networkManager;
    private InputActions inputActions; 
    private bool isInMainMenu;
    private bool isInPauseMenu;
    
    [SerializeField] private Button hostButton;
    [SerializeField] private Button joinButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Canvas menuCanvas;

    [SerializeField] private Button resumeButton;
    [SerializeField] private Button disconnectButton;
    [SerializeField] private Canvas pauseCanvas;
    
    private void Awake()
    {
        networkManager = FindObjectOfType<NetworkManager>();

        if (networkManager == null || menuCanvas == null || pauseCanvas == null)
            return;
        
        inputActions = new InputActions();
        inputActions.Enable();
        
        isInMainMenu = true;
        menuCanvas.gameObject.SetActive(true);
        pauseCanvas.gameObject.SetActive(false);
        
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
        
        resumeButton.onClick.AddListener(() =>
        {
            isInPauseMenu = false;
            pauseCanvas.gameObject.SetActive(false);
        });
        
        disconnectButton.onClick.AddListener(() =>
        {
            // works for both server and client =)
            networkManager.Shutdown();
        });

        inputActions.Player.Menu.performed += TogglePauseMenu;
        networkManager.OnClientConnectedCallback += OnClientConnect;
        networkManager.OnClientDisconnectCallback += OnClientDisconnect;
    }

    private void OnDestroy()
    {
        inputActions.Player.Menu.performed -= TogglePauseMenu;
        networkManager.OnClientConnectedCallback -= OnClientConnect;
        networkManager.OnClientDisconnectCallback -= OnClientDisconnect;
    }

    private void OnClientConnect(ulong clientId)
    {
        if (networkManager.LocalClientId == clientId)
        {
            menuCanvas.gameObject.SetActive(false);
            isInMainMenu = false;
        }
    }

    private void OnClientDisconnect(ulong clientId)
    {
        menuCanvas.gameObject.SetActive(true);
        isInMainMenu = true;
        pauseCanvas.gameObject.SetActive(false);
        isInPauseMenu = false;
    }

    private void TogglePauseMenu(InputAction.CallbackContext ctx)
    {
        if (!isInMainMenu)
        {
            if (!isInPauseMenu)
            {
                pauseCanvas.gameObject.SetActive(true);
                isInPauseMenu = true;
            }
            else if (isInPauseMenu)
            {
                pauseCanvas.gameObject.SetActive(false);
                isInPauseMenu = false;
            }
        }
    }
}
