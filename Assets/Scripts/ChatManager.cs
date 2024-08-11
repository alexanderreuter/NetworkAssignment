using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class ChatManager : NetworkBehaviour
{
    [SerializeField] public InputField inputField;
    [SerializeField] private TextMeshProUGUI displayText;
    [SerializeField] private float messageLifeTime = 10f;

    private void Awake()
    {
        inputField.onEndEdit.AddListener(OnSendMessage);
    }

    private void OnSendMessage(string message)
    {
        if (!string.IsNullOrEmpty(message))
        {
            SendMessageServerRpc(NetworkManager.Singleton.LocalClientId, message);
            inputField.text = "";
            inputField.gameObject.SetActive(false);
        }
    }
    
    [ServerRpc(RequireOwnership = false)] 
    private void SendMessageServerRpc(ulong senderClientId, string message)
    {
        string formatedMessage = $"Player {senderClientId + 1}: {message}";

        ReceiveMessageClientRpc(formatedMessage);
    }

    [ClientRpc]
    private void ReceiveMessageClientRpc(string message)
    {
        displayText.text += message + "\n";

        StartCoroutine(RemoveMessageAfterTime(message, messageLifeTime));
    }

    private IEnumerator RemoveMessageAfterTime(string message, float time)
    {
        yield return new WaitForSeconds(time);
        
        string messageToRemove = message + "\n";
        if (displayText.text.Contains(messageToRemove))
        {
            displayText.text = displayText.text.Replace(messageToRemove, "");
        }
    }
}
