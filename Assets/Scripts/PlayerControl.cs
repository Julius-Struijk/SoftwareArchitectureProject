using UnityEngine;
using UnityEngine.InputSystem;
using System;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerControl : MonoBehaviour
{
    public static Action onInteract;

    private Rigidbody2D rigidBody2D;
    [SerializeField] int speedMultiplier = 2;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rigidBody2D = GetComponent<Rigidbody2D>();
    }

    public void Move(InputAction.CallbackContext context)
    {
        Vector2 moveVector = context.ReadValue<Vector2>();
        rigidBody2D.linearVelocity = moveVector * speedMultiplier;
    }

    public void Interact(InputAction.CallbackContext context)
    {
        onInteract?.Invoke();
    }
}
