
using UnityEngine;
using System.Collections;

public class FoodSpawner : MonoBehaviour
{
    [Header("Food")]
    public GameObject[] foodPrefabs;

    [Header("Spawn Settings")]
    public float spawnInterval = 1.5f;

    [Header("Random Position X")]
    public float minX = -5f;
    public float maxX = 5f;

    [Header("Random Position Y")]
    public float minY = 0f;
    public float maxY = 0f;

    [Header("Random Position Z")]
    public float minZ = 0f;
    public float maxZ = 0f;

    private Coroutine spawnCoroutine;

    void OnEnable()
    {
        spawnCoroutine = StartCoroutine(SpawnFood());
    }

    void OnDisable()
    {
        if (spawnCoroutine != null)
            StopCoroutine(spawnCoroutine);
    }

    IEnumerator SpawnFood()
    {
        while (true)
        {
            if (foodPrefabs.Length > 0)
            {
                int randomIndex = Random.Range(
                    0,
                    foodPrefabs.Length
                );

                // Losowanie pozycji wzglêdem FoodSpawner
                Vector3 spawnPosition = transform.position + new Vector3(
                    Random.Range(minX, maxX),
                    Random.Range(minY, maxY),
                    Random.Range(minZ, maxZ)
                );

                Instantiate(
                    foodPrefabs[randomIndex],
                    spawnPosition,
                    Quaternion.identity
                );
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }
}
