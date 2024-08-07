using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : NetworkBehaviour
{
    private InputActions inputActions;

    private Vector2 moveInput;
    private float speed = 20;

    private void Awake()
    {
        inputActions = new InputActions();
    }

    private void OnEnable()
    {
        inputActions.Enable();
        inputActions.Player.Move.performed += OnMove;
        inputActions.Player.Move.canceled += OnStopMove;
        inputActions.Player.Shoot.performed += OnShoot;
    }

    private void OnDisable()
    {
        inputActions.Player.Move.performed -= OnMove;
        inputActions.Player.Move.canceled -= OnStopMove;
        inputActions.Player.Shoot.performed -= OnShoot;
        inputActions.Disable();
    }

    private void OnMove(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<Vector2>();
    }

    private void OnStopMove(InputAction.CallbackContext ctx)
    {
        moveInput = Vector2.zero;
    }

    private void OnShoot(InputAction.CallbackContext ctx)
    {
        Debug.Log("Pew");
    }
    
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += (Vector3)moveInput * speed * Time.deltaTime;
    }
}
