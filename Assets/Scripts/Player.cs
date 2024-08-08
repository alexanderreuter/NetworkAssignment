using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : NetworkBehaviour
{
    private InputActions inputActions;
    [SerializeField] private float speed = 10f;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform shootPoint;

    private Vector2 localMoveInput;
    private NetworkVariable<Vector2> serverMoveInput;
    private NetworkVariable<Quaternion> serverRotation;
    
    private void Awake()
    {
        inputActions = new InputActions();
        serverMoveInput = new NetworkVariable<Vector2>();
        serverRotation = new NetworkVariable<Quaternion>();
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
            localMoveInput = ctx.ReadValue<Vector2>();
            MoveServerRpc(localMoveInput);
        }
    }

    private void OnStopMove(InputAction.CallbackContext ctx)
    {
        if (IsLocalPlayer)
        {
            localMoveInput = Vector2.zero;
            MoveServerRpc(localMoveInput);
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
        if (IsLocalPlayer) // Update local stuff
        {
            transform.position += (Vector3)localMoveInput * speed * Time.deltaTime;
            UpdateRotation();
        }
        else // Update from server as client
        {
            transform.position += (Vector3)serverMoveInput.Value * speed * Time.deltaTime;
            transform.rotation = serverRotation.Value;
        }
        
    }

    private void UpdateRotation()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = transform.position.z;

        Vector3 direction = (mousePosition - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        
        Quaternion localRotation = Quaternion.Euler(new Vector3(0, 0, angle - 90f));
        transform.rotation = localRotation;
        RotationServerRpc(localRotation);
    }

    [ServerRpc]
    private void MoveServerRpc(Vector2 input)
    {
        serverMoveInput.Value = input;
    }

    [ServerRpc]
    private void RotationServerRpc(Quaternion rotation)
    {
        serverRotation.Value = rotation;
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
                bulletNetworkObject.Spawn(true); 
                Bullet bulletScript = bullet.GetComponent<Bullet>();
                bulletScript.Initialize(shootPoint.up);
            }
        }
    }
}
