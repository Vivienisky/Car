using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ChunkManager : MonoBehaviour
{
    public Transform CameraTransform;
    public List<GameObject> Chunks = new List<GameObject>();

    public int InitialBlockCount = 8;
    public float BlockLength = 10;

    public float StartMoveSpeed = 10;
    public float MaxSpeed = 30f;
    public float SpeedIncreasePerSecond = 0.4f;

    public float recycleDistanceBehindCamera = 15;

    private List<Transform> _activeChunks = new List<Transform>();
    private float _currentSpeed = 0;
    private int _lastChunkIndex = -1;

    private void Awake()
    {
        SpawnInitialChunks();
    }

    private void Update()
    {
        RecalculateSpeed();
        MoveBlocks(_currentSpeed);
        RecycleBlockPassedCamera();
    }

    private void RecalculateSpeed()
    {
        if (_currentSpeed < StartMoveSpeed)
        {
            _currentSpeed += StartMoveSpeed * Time.deltaTime;
            if (_currentSpeed > StartMoveSpeed)
            {
                _currentSpeed = StartMoveSpeed;
            }
        }
        else
        {
            if (_currentSpeed != MaxSpeed)
            {
                _currentSpeed += SpeedIncreasePerSecond * Time.deltaTime;
            }
        }

        if (_currentSpeed > MaxSpeed)
        {
            _currentSpeed = MaxSpeed;
        }
    }

    private void SpawnInitialChunks()
    {
        float nextSpawnPositionZ = CameraTransform.position.z;
        for (int i = 0; i < InitialBlockCount; i++)
        {
            Transform spawnedChunk = InstantiateChunk(nextSpawnPositionZ);
            _activeChunks.Add(spawnedChunk);
            nextSpawnPositionZ += BlockLength;
        }
    }

    private void MoveBlocks(float moveSpeed)
    {
        float moveDistance = moveSpeed * Time.deltaTime;
        Vector3 moveOffset = new Vector3(0, 0, -moveDistance);
        foreach (var activeChunk in _activeChunks)
        {
            activeChunk.transform.position += moveOffset;
        }
    }

    private void RecycleBlockPassedCamera()
    {
        float recycleThreshold = CameraTransform.position.z - recycleDistanceBehindCamera;
        while (_activeChunks.Count > 0)
        {
            Transform oldestBlock = _activeChunks[0];
            if (oldestBlock.position.z >= recycleThreshold)
            {
                return;
            }

            Transform recycleChunk = _activeChunks[0];
            _activeChunks.Remove(recycleChunk);
            
            float frontBlockZPosition = _activeChunks.Count == 0 ? recycleChunk.position.z : GetFrontPositionZ();
            float nextBlockZPosition = frontBlockZPosition + BlockLength;
            
            Destroy(oldestBlock.gameObject);
            
            Transform newBlock = InstantiateChunk(nextBlockZPosition);
            _activeChunks.Add(newBlock);
        }
    }

    private float GetFrontPositionZ()
    {
        float returnValue = float.MinValue;
        foreach (var activeChunk in _activeChunks)
        {
            if (activeChunk.position.z > returnValue)
            {
                returnValue = activeChunk.position.z;
            }
        }
        return returnValue;
    }

    private Transform InstantiateChunk(float zPosition)
    {
        int randomIndex = GetRandomUniqueIndex();
        _lastChunkIndex = randomIndex;

        Vector3 spawnPosition = new Vector3(transform.position.x, transform.position.y, zPosition);
        GameObject newChunk = Instantiate(Chunks[randomIndex]);
        newChunk.transform.position = spawnPosition;
        newChunk.transform.SetParent(transform);
        return newChunk.transform;
    }

    private int GetRandomUniqueIndex()
    {
        if (Chunks.Count <= 1) return 0;

        int randomIndex = _lastChunkIndex;
        while (randomIndex == _lastChunkIndex)
        {
            randomIndex = Random.Range(0, Chunks.Count);
        }
        return randomIndex;
    }
}