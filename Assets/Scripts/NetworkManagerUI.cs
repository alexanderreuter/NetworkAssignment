using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class NetworkManagerUI : NetworkBehaviour
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

    [SerializeField] private TextMeshProUGUI player1Text;
    [SerializeField] private TextMeshProUGUI player2Text;
    [SerializeField] private TextMeshProUGUI player3Text;
    [SerializeField] private TextMeshProUGUI player4Text;

    private Dictionary<ulong, int> playerScoreDictionary = new Dictionary<ulong, int>();
    
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
            SetPlayerUIServerRpc(clientId, false, 0);
        }
    }

    private void OnClientDisconnect(ulong clientId)
    {
        menuCanvas.gameObject.SetActive(true);
        isInMainMenu = true;
        pauseCanvas.gameObject.SetActive(false);
        isInPauseMenu = false;

        SetPlayerUIServerRpc(clientId, true, 0);
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

    [ServerRpc(RequireOwnership = false)] //ändra?
    public void SetPlayerUIServerRpc(ulong clientId, bool removeUI, int playerScore)
    {
        if (playerScore == 0)
            RecieveSetPlayerUIClientRpc(clientId, removeUI, playerScore);
        else
        {
            if (playerScoreDictionary.TryGetValue(clientId, out int value))
            {
                playerScoreDictionary[clientId] = value++;
                RecieveSetPlayerUIClientRpc(clientId, removeUI, value++);
            }
        }
        UpdateScoreClientRpc(player1Text.text, player2Text.text, player3Text.text, player4Text.text);
    }

    [ClientRpc]
    private void RecieveSetPlayerUIClientRpc(ulong clientId, bool removeUI, int playerScore)
    {
        string formatedScore = "";

        if (!removeUI)
            formatedScore = $"Player {clientId + 1}: {playerScore}";
        
        switch (clientId)
        {
            case 0:
                player1Text.text = formatedScore;

                try
                {
                    playerScoreDictionary.Add(clientId, playerScore);
                }
                catch (ArgumentException)
                {
                    Debug.Log("Key already exists");
                }
                
                break;
            case 1:
                player2Text.text = formatedScore;
                
                try
                {
                    playerScoreDictionary.Add(clientId, playerScore);
                }
                catch (ArgumentException)
                {
                    Debug.Log("Key already exists");
                }
                
                break;
            case 2:
                player3Text.text = formatedScore;
                
                try
                {
                    playerScoreDictionary.Add(clientId, playerScore);
                }
                catch (ArgumentException)
                {
                    Debug.Log("Key already exists");
                }
                
                break;
            case 3:
                player4Text.text = formatedScore;
                
                try
                {
                    playerScoreDictionary.Add(clientId, playerScore);
                }
                catch (ArgumentException)
                {
                    Debug.Log("Key already exists");
                }
                
                break;
        }
    }

    [ClientRpc]
    private void UpdateScoreClientRpc(string player1Score, string player2Score, string player3Score, string player4Score)
    {
        player1Text.text = player1Score;
        player2Text.text = player2Score;
        player3Text.text = player3Score;
        player4Text.text = player4Score;
    }
}
