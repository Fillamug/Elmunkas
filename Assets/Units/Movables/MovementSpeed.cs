using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementSpeed : MonoBehaviour
{
    [SerializeField] private float movementSpeed;

    private Vector3 velocity;

    public void SetVelocity(Vector3 velocity)
    {
        this.velocity = velocity;
    }

    private void FixedUpdate()
    {
        transform.position += velocity * movementSpeed * Time.deltaTime;
    }
}
