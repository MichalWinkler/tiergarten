
using UnityEngine;
using TMPro;

public class MinigameManager : MonoBehaviour
{
    [Header("Cameras")]
    public Camera mainCamera;
    public Camera minigameCamera;

    [Header("Minigame Objects")]
    public GameObject minigameAnimal;
    public GameObject foodSpawner;

    [Header("UI")]
    public TMP_Text scoreText;
    public TMP_Text timerText;
    public GameObject minigameUI;

    [Header("Game Settings")]
    public float gameDuration = 30f;

    private int score = 0;
    private float timeLeft;
    private bool isPlaying = false;

    [Header("Scripts to disable during minigame")]
    public MonoBehaviour[] scriptsToDisable;

    void Start()
    {
        mainCamera.enabled = true;
        minigameCamera.enabled = false;

        if (minigameAnimal != null)
            minigameAnimal.SetActive(false);

        if (foodSpawner != null)
            foodSpawner.SetActive(false);

        if (minigameUI != null)
            minigameUI.SetActive(false);
    }

    void Update()
    {
        if (!isPlaying)
            return;

        timeLeft -= Time.deltaTime;

        if (timerText != null)
            timerText.text = "Czas: " + Mathf.CeilToInt(timeLeft);

        if (timeLeft <= 0)
            EndMinigame();
    }

    public void StartMinigame()
    {
        Debug.Log("testtsststst");
        if (isPlaying)
            return;

        isPlaying = true;
        score = 0;
        timeLeft = gameDuration;

        mainCamera.enabled = false;
        minigameCamera.enabled = true;

        if (minigameAnimal != null)
            minigameAnimal.SetActive(true);

        if (foodSpawner != null)
            foodSpawner.SetActive(true);

        if (minigameUI != null)
            minigameUI.SetActive(true);

        foreach (MonoBehaviour script in scriptsToDisable)
        {
            if (script != null)
                script.enabled = false;
        }

        UpdateScore();

        Debug.Log("Minigra rozpoczêta!");
    }

    public void AddScore(int amount)
    {
        if (!isPlaying)
            return;

        score += amount;
        UpdateScore();
    }

    void UpdateScore()
    {
        if (scoreText != null)
            scoreText.text = "Punkty: " + score;
    }

    public void EndMinigame()
    {
        if (!isPlaying)
            return;

        isPlaying = false;

        mainCamera.enabled = true;
        minigameCamera.enabled = false;

        if (minigameAnimal != null)
            minigameAnimal.SetActive(false);

        if (foodSpawner != null)
            foodSpawner.SetActive(false);

        if (minigameUI != null)
            minigameUI.SetActive(false);

        foreach (MonoBehaviour script in scriptsToDisable)
        {
            if (script != null)
                script.enabled = true;
        }

        Debug.Log("Koniec minigry! Wynik: " + score);
    }
}
