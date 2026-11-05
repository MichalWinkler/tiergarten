
using UnityEngine;
using TMPro;

public class MinigameInteraction : MonoBehaviour
{
    public MinigameManager minigameManager;
    public TMP_Text interactionText;

    private bool playerNearby = true;

    void Start()
    {
        if (interactionText != null)
            interactionText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (playerNearby && Input.GetKeyDown(KeyCode.E))
        {
            minigameManager.StartMinigame();

            if (interactionText != null)
                interactionText.gameObject.SetActive(false);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //playerNearby = true;

            if (interactionText != null)
            {
                interactionText.gameObject.SetActive(true);
                interactionText.text = "Naciœnij E, aby rozpocz¹æ minigrê";
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //playerNearby = false;

            if (interactionText != null)
                interactionText.gameObject.SetActive(false);
        }
    }
}
