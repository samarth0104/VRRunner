using System.Collections.Generic;
using UnityEngine;

public class TrackManager : MonoBehaviour
{
    [SerializeField]
    private GameObject trackPrefab; // Prefab for the track segments
    [SerializeField]
    private int initialTrackCount = 5; // Number of track segments to spawn initially
    [SerializeField]
    private Transform playerTransform; // Reference to the player transform
    [SerializeField]
    private float trackLength = 10.72f; // Length of each track segment
    [SerializeField]
    private GameObject[] barricadePrefabs; // Array of potential barricade prefabs
    [SerializeField]
    private GameObject coinPrefab; // Prefab for the coins
    [SerializeField]
    private int maxBarricadesPerTrack = 2; // Maximum number of barricades per track segment
    [SerializeField]
    private int coinsPerLane = 1; // Number of coins per lane
    [SerializeField]
    private float laneWidth = 4f; // Width of each lane
    [SerializeField]
    private float forwardSpeed = 5f; // Forward speed of the player
    [SerializeField]
    private float sideSpeed = 10f; // Side speed of the player
    [SerializeField]
    private float noBarricadeTime = 0f; // Time for which no barricades should spawn

    private Queue<GameObject> trackQueue;
    private float spawnZ;
    private Vector3 initialSpawnPoint = new Vector3(-3.86f, -0.05f, 3); // Starting point for track spawning
    private float elapsedTime = 0f;

    private void Start()
    {
        trackQueue = new Queue<GameObject>();
        spawnZ = initialSpawnPoint.z;

        // Spawn initial track segments
        for (int i = 0; i < initialTrackCount; i++)
        {
            SpawnTrack();
        }
    }

    private void Update()
    {
        elapsedTime += Time.deltaTime;

        // Check if the player has passed the threshold to recycle the oldest track segment
        if (playerTransform.position.z - trackLength > trackQueue.Peek().transform.position.z)
        {
            // Recycle the oldest track segment
            RecycleTrack();
        }
    }

    private void SpawnTrack()
    {
        GameObject track = Instantiate(trackPrefab, new Vector3(initialSpawnPoint.x, initialSpawnPoint.y, spawnZ), Quaternion.identity);
        if (elapsedTime > noBarricadeTime)
        {
            HashSet<float> barricadeLanes = SpawnBarricades(track.transform);
            SpawnCoins(track.transform, barricadeLanes);
        }
        trackQueue.Enqueue(track);
        spawnZ += trackLength;
    }

    private void RecycleTrack()
    {
        GameObject oldTrack = trackQueue.Dequeue();
        oldTrack.transform.position = new Vector3(initialSpawnPoint.x, initialSpawnPoint.y, spawnZ);
        if (elapsedTime > noBarricadeTime)
        {
            HashSet<float> barricadeLanes = SpawnBarricades(oldTrack.transform);
            SpawnCoins(oldTrack.transform, barricadeLanes);
        }
        trackQueue.Enqueue(oldTrack);
        spawnZ += trackLength;
    }

    private HashSet<float> SpawnBarricades(Transform trackTransform)
    {
        // Clear existing barricades and coins
        foreach (Transform child in trackTransform)
        {
            if (child.CompareTag("Barricade") || child.CompareTag("Coin"))
            {
                Destroy(child.gameObject);
            }
        }

        // Calculate the interval distance based on the player's speed and reaction time
        float reactionTime = 1f; // Example reaction time
        float switchLaneTime = laneWidth / sideSpeed;
        float intervalDistance = forwardSpeed * (reactionTime + switchLaneTime);

        // Keep track of lanes with barricades
        HashSet<float> barricadeLanes = new HashSet<float>();

        // Ensure the barricades are spaced at least intervalDistance apart
        float spawnZPosition = 0;
        while (spawnZPosition < trackLength)
        {
            // Define lane positions to ensure they are within track boundaries
            float[] lanePositions = new float[] { 0, laneWidth, laneWidth + 4 };

            // Spawn new barricades
            for (int i = 0; i < maxBarricadesPerTrack; i++)
            {
                GameObject barricade = Instantiate(barricadePrefabs[Random.Range(0, barricadePrefabs.Length)], trackTransform);
                float lanePosition = lanePositions[Random.Range(0, lanePositions.Length)]; // Randomly place barricade in one of the lanes
                barricade.transform.localPosition = new Vector3(lanePosition, 0, spawnZPosition);
                barricade.tag = "Barricade"; // Tag the barricade for easy identification and cleanup

                // Mark lane as having a barricade
                barricadeLanes.Add(lanePosition);
            }

            spawnZPosition += intervalDistance;
        }

        return barricadeLanes;
    }

    private void SpawnCoins(Transform trackTransform, HashSet<float> barricadeLanes)
    {
        // Define lane positions to ensure they are within track boundaries
        float[] lanePositions = new float[] { 0, laneWidth, laneWidth + 4 };

        // Spawn coins only in lanes without barricades
        foreach (float lanePosition in lanePositions)
        {
            if (!barricadeLanes.Contains(lanePosition))
            {
                for (int i = 0; i < coinsPerLane; i++)
                {
                    GameObject coin = Instantiate(coinPrefab, trackTransform);
                    coin.transform.localPosition = new Vector3(lanePosition, 0.63f, Random.Range(0, trackLength));
                    coin.tag = "Coin"; // Tag the coin for easy identification and cleanup
                }

                // Adjust coin positions to be evenly spaced
                for (int i = 0; i < coinsPerLane; i++)
                {
                    GameObject coin = Instantiate(coinPrefab, trackTransform);
                    coin.transform.localPosition = new Vector3(lanePosition, 0.63f, i * 1.5f);
                    coin.tag = "Coin"; // Tag the coin for easy identification and cleanup
                }
            }
        }
    }
}
