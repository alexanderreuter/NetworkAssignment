using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkManagerUI : MonoBehaviour
{
    private NetworkManager networkManager;

    private void Awake()
    {
        networkManager = FindObjectOfType<NetworkManager>();
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Host"))
        {
            networkManager.StartHost();
        }

        if (GUILayout.Button("Join"))
        {
            networkManager.StartClient();
        }
    }
}
