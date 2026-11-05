
using UnityEngine;

public class MinigameAnimalController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 8f;

    [Header("Movement Boundaries")]
    public float leftLimit = -5f;
    public float rightLimit = 5f;

    void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");

        Vector3 movement = Vector3.right * horizontal;

        transform.position -= movement * moveSpeed * Time.deltaTime;

        // Ograniczenie ruchu
        Vector3 position = transform.position;

        position.x = Mathf.Clamp(
            position.x,
            leftLimit,
            rightLimit
        );

        transform.position = position;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Food"))
        {
            MinigameManager manager =
                FindFirstObjectByType<MinigameManager>();

            if (manager != null)
                manager.AddScore(1);

            Destroy(other.gameObject);
        }
    }
}
