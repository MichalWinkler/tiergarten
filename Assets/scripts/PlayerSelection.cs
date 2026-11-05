
using UnityEngine;

public class PlayerSelection : MonoBehaviour
{
    [Header("References")]
    public Camera mainCamera;

    [Header("Raycast Layers")]
    public LayerMask playerLayer;
    public LayerMask groundLayer;

    private PlayerMovement selectedPlayer;

    void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;
    }

    void Update()
    {
        if (!Input.GetMouseButtonDown(0))
            return;

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Najpierw sprawdzamy, czy klikniêto postaæ
        if (Physics.Raycast(ray, out hit, 1000f, playerLayer))
        {
            PlayerMovement player =
                hit.collider.GetComponentInParent<PlayerMovement>();

            if (player != null)
            {
                selectedPlayer = player;

                Debug.Log("Selected player: " + player.name);
                return;
            }
        }

        // Jeœli nie klikniêto postaci, sprawdzamy pod³ogê
        if (selectedPlayer != null)
        {
            if (Physics.Raycast(ray, out hit, 1000f, groundLayer))
            {
                selectedPlayer.MoveTo(hit.point);

                Debug.Log("New destination: " + hit.point);
            }
        }
    }
}
