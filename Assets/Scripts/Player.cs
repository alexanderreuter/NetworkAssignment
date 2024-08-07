using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : NetworkBehaviour
{
    private InputActions inputActions;
    private NetworkVariable<Vector2> moveInput;
    [SerializeField] private float speed = 20f;

    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private float bulletSpeed = 1f;
    
    private void Awake()
    {
        inputActions = new InputActions();
        moveInput = new NetworkVariable<Vector2>();
    }

    public override void OnNetworkSpawn()
    {
        if (IsLocalPlayer)
        {
            inputActions.Enable();
            inputActions.Player.Move.performed += OnMove;
            inputActions.Player.Move.canceled += OnStopMove;
            inputActions.Player.Shoot.performed += OnShoot;
        }
    }

    private void OnDisable()
    {
        if (IsLocalPlayer)
        {
        inputActions.Player.Move.performed -= OnMove;
        inputActions.Player.Move.canceled -= OnStopMove;
        inputActions.Player.Shoot.performed -= OnShoot;
        inputActions.Disable();
        }
    }

    private void OnMove(InputAction.CallbackContext ctx)
    {
        if (IsLocalPlayer)
        {
            Vector2 input = ctx.ReadValue<Vector2>();
            MoveServerRpc(input);
            moveInput.Value = input;
        }
    }

    private void OnStopMove(InputAction.CallbackContext ctx)
    {
        if (IsLocalPlayer)
        {
            Vector2 input = Vector2.zero;
            MoveServerRpc(input);
            moveInput.Value = input;
        }
    }

    private void OnShoot(InputAction.CallbackContext ctx)
    {
        if (IsLocalPlayer)
        {
            ShootServerRpc();
        }
    }
    
    void Update()
    {
        if (IsLocalPlayer || IsServer)
        {
            transform.position += (Vector3)moveInput.Value * speed * Time.deltaTime;
        }
    }

    [ServerRpc]
    private void MoveServerRpc(Vector2 input)
    {
        moveInput.Value = input;
    }

    [ServerRpc]
    private void ShootServerRpc()
    {
        if (bulletPrefab && shootPoint)
        {
            GameObject bullet = Instantiate(bulletPrefab, shootPoint.position, shootPoint.rotation);
            
            NetworkObject bulletNetworkObject = bullet.GetComponent<NetworkObject>();
            if (bulletNetworkObject != null)
            {
                bulletNetworkObject.Spawn(); 
            }
            
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = shootPoint.up * bulletSpeed;
            }
        }
    }
}
