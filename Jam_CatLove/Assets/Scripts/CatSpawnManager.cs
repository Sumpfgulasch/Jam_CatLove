using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class CatSpawnManager : MonoBehaviour
{
    [Header("Grid Settings")] [SerializeField]
    private int gridWidth = 4;

    [SerializeField] private int gridHeight = 4;
    [SerializeField] private float cellSize = 2f;
    [SerializeField] private Vector3 gridOrigin = Vector3.zero;

    [Header("Cat Settings")] [SerializeField]
    private GameObject catPrefab;
    [SerializeField] private CatInteractionDetector catInteractionDetector;
    [SerializeField] private float spawnInterval = 3f;

    
    private HashSet<int> blockedIndices = new HashSet<int>();
    private Dictionary<Cat, int> catToIndexMap = new Dictionary<Cat, int>();

    private void Start()
    {
        StartCoroutine(SpawnCatsRoutine());
    }
    
    private IEnumerator SpawnCatsRoutine()
    {
        while (true)
        {
            int freeIndex = FindFreeGridPosition();

            if (freeIndex != -1)
            {
                SpawnCatAtIndex(freeIndex);
            }
            
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private int FindFreeGridPosition()
    {
        int totalCells = gridWidth * gridHeight;

        // If all positions are blocked, return -1
        if (blockedIndices.Count >= totalCells)
        {
            return -1;
        }

        // Find a random free position
        List<int> freeIndices = new List<int>();
        for (int i = 0; i < totalCells; i++)
        {
            if (!blockedIndices.Contains(i))
            {
                freeIndices.Add(i);
            }
        }

        return freeIndices[Random.Range(0, freeIndices.Count)];
    }

    private void SpawnCatAtIndex(int index)
    {
        // Convert index to grid position
        int x = index % gridWidth;
        int y = index / gridWidth;

        Vector3 worldPosition = gridOrigin + new Vector3(x * cellSize, 0, y * cellSize);

        // Instantiate cat
        GameObject catObj = Instantiate(catPrefab, worldPosition, Quaternion.identity);
        catObj.transform.eulerAngles = new Vector3(0f, 180f, 0f);
        Cat cat = catObj.GetComponent<Cat>();

        // Mark position as blocked
        blockedIndices.Add(index);
        catToIndexMap[cat] = index;
    }

    private void HandleCatFinished(Cat cat)
    {
        // Call dissolve on the cat
        cat.Vanish();

        // Unblock the grid position
        if (catToIndexMap.ContainsKey(cat))
        {
            int index = catToIndexMap[cat];
            blockedIndices.Remove(index);
            catToIndexMap.Remove(cat);
        }
    }

    // Optional: Visualize the grid in the editor
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                Vector3 pos = gridOrigin + new Vector3(x * cellSize, 0, y * cellSize);
                Gizmos.DrawWireCube(pos, new Vector3(cellSize * 1f, 0.001f, cellSize * 1f));
            }
        }
    }
}