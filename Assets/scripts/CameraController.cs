
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float edgeSize = 20f;

    void Update()
    {
        Vector3 movement = Vector3.zero;

        // Pozycja kursora
        Vector3 mousePosition = Input.mousePosition;

        // Ruch kamery przy krawêdziach ekranu
        if (mousePosition.x >= Screen.width - edgeSize)
            movement += transform.right;

        if (mousePosition.x <= edgeSize)
            movement -= transform.right;

        if (mousePosition.y >= Screen.height - edgeSize)
            movement += transform.up;

        if (mousePosition.y <= edgeSize)
            movement -= transform.up;

        // Nie przesuwamy kamery w pionie œwiata
        movement.y = 0;

        // Normalizacja ruchu
        movement.Normalize();

        transform.position += movement * moveSpeed * Time.deltaTime;
    }
}
