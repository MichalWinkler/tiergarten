
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 4f;
    public float rotationSpeed = 10f;
    public float stoppingDistance = 1.5f;

    [Header("Gravity")]
    public float gravity = -20f;
    public float groundCheckDistance = 0.2f;

    private CharacterController controller;
    private Vector3 targetPosition;
    private Vector3 verticalVelocity;

    private bool hasTarget;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        // Grawitacja
        if (controller.isGrounded && verticalVelocity.y < 0)
        {
            verticalVelocity.y = -2f;
        }

        verticalVelocity.y += gravity * Time.deltaTime;

        Vector3 movement = Vector3.zero;

        // Ruch do celu
        if (hasTarget)
        {
            Vector3 direction = targetPosition - transform.position;
            direction.y = 0f;

            float distance = direction.magnitude;

            if (distance <= stoppingDistance)
            {
                hasTarget = false;
            }
            else
            {
                direction.Normalize();

                Quaternion targetRotation =
                    Quaternion.LookRotation(direction);

                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    targetRotation,
                    rotationSpeed * Time.deltaTime
                );

                movement = direction * moveSpeed;
            }
        }

        // Ruch poziomy i pionowy
        movement.y = verticalVelocity.y;

        controller.Move(movement * Time.deltaTime);
    }

    public void MoveTo(Vector3 destination)
    {
        targetPosition = destination;
        hasTarget = true;
    }

    public void StopMovement()
    {
        hasTarget = false;
    }
}
