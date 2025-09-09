using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;

public class RoadGenerator : MonoBehaviour
{
    public Transform player;
    public int initialRoadPieces = 11;

    public int maxRoads = 12;
    public float zSpawn = 0;
    public float roadLength = 35;
    public float roadWidth = 26;

    public int lanesCount = 3;

    public float sidewalkWidth = 5;

    private List<GameObject> activeRoads = new List<GameObject>();

    public float[] lanePositions = { 0f };

    [SerializeField] private string folder = "Assets/Models/Road/Prefabs";
    [SerializeField] private string nameStartsWith = "Road";
    public List<GameObject> roadPrefabs;

    void Awake()
    {
         FindPrefabs();
    }

    void Start()
    {
        player = GameManager.Instance.player.transform;

        float lanesWidth = roadWidth - 2 * sidewalkWidth;

        lanePositions = new float[lanesCount];
        
        float laneWidth = lanesWidth / lanesCount;

        for (int i = 0; i < lanesCount; i++)
        {
            int laneOffset = i - (lanesCount / 2);
            float lanePosition = laneWidth * laneOffset;

            float offset = lanePosition == 0 ? 1f : 2f;

            if (lanePosition < 0)
            {
                offset *= -1;
            }

            lanePositions[i] = lanePosition + offset;
        }

        // Initialize with initial road pieces
        for (int i = 0; i < initialRoadPieces; i++)
        {
            SpawnRoad();
        }
    }

    void Update()
    {
        if (!player)
        {
            return;
        }

        if (player.position.z - roadLength > zSpawn - (initialRoadPieces * roadLength))
        {
            SpawnRoad();
            DeleteRoad();
        }
    }

    void SpawnRoad()
    {
        if (roadPrefabs == null || roadPrefabs.Count == 0)
        {
            return;
        }

        GameObject road = Instantiate(roadPrefabs[Random.Range(0, roadPrefabs.Count)]);

        // if (road.GetComponent<MeshCollider>() == null){ 
        //     road.AddComponent<BoxCollider>();
        // }

        road.transform.position = Vector3.forward * zSpawn;
        zSpawn += roadLength;
        activeRoads.Add(road);
    }

    void DeleteRoad()
    {
        if (activeRoads.Count > maxRoads)
        {
            Destroy(activeRoads[0]);
            activeRoads.RemoveAt(0);
        }
    }

    public void ResetRoad()
    {
        // Delete all active roads
        foreach (GameObject road in activeRoads)
        {
            Destroy(road);
        }
        activeRoads.Clear();

        // Reset spawn position
        zSpawn = 0;

        // Spawn initial roads again
        for (int i = 0; i < initialRoadPieces; i++)
        {
            SpawnRoad();
        }
    }

    // [MenuItem("Tools/Find Road Prefabs")]
    public void FindPrefabs()
    {
       roadPrefabs = PrefabSearch.FindPrefabsInFolder(
            folder, 
            nameStartsWith
        );
    }
}
