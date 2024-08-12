using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Bullet : NetworkBehaviour
{
    [SerializeField] private float speed = 20f;
    [SerializeField] private float lifetime = 5f;
    private ulong playerID;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            Destroy(gameObject, lifetime); 
        }
    }

    public void Initialize(Vector2 direction, ulong playerID)
    {
        if (rb != null)
        {
            rb.velocity = direction * speed;
        }

        this.playerID = playerID;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (IsServer)
        {
            if (collision.GetComponent<Player>())
            {
                NetworkManagerUI networkManagerUI = FindObjectOfType<NetworkManagerUI>();
                
                networkManagerUI.SetPlayerUIServerRpc(playerID, false, 1);
                
                Destroy(collision.gameObject);
                
            }
            
            Destroy(gameObject);
        }
    }
}
