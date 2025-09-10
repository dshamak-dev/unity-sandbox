using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

[System.Serializable]
public class SpawnPoint
{
    public Vector3 position;
    public Quaternion rotation;

    public int[] gridPos;

    public SpawnPoint(int[] gridPosInput, Vector3 posInput, Quaternion rotInput)
    {
        gridPos = gridPosInput;
        position = posInput;
        rotation = rotInput;
    }
}

public class Road : MonoBehaviour
{
    [Header("Settings")]
    public int lanes = 3;
    public int laneSlots = 3;

    public List<GameObject> spawnPrefabs;

    public Transform shape;

    [Header("Debug")]
    [SerializeField]
    public List<SpawnPoint> spawnPoints;
    public Transform[] spawnObjects;
    public Renderer shapeRenderer;
    public Vector2 roadSize;
    public Vector3 roadCenterOffset;

    void Start()
    {
        if (shape != null)
        {
            shapeRenderer = shape.GetComponent<Renderer>();
        }

        if (shapeRenderer != null)
        {
            Bounds bounds = shapeRenderer.bounds;
            float width = bounds.size.x;
            float height = bounds.size.z;

            roadSize = new Vector2(width, height);

            roadCenterOffset = new Vector3(roadSize.x / 2, 0, roadSize.y / 2);

            int col = GetOneOf(lanes);
            int row = GetOneOf(laneSlots);
            CreateSpawnPoint(col, row);

            // for (int col=0; col < lanes; col++)
            // {
            //     for (int row=0; row < laneSlots; row++)
            //     {
            //         CreateSpawnPoint(col, row);
            //     }
            // }
        }
    }

    void Update()
    {

    }

    public void CreateSpawnPointDebug(Vector3 positionInput)
    {
        int prefabIndex = GetOneOf(spawnPrefabs != null ? spawnPrefabs.Count : 0);        

        GameObject prefab = spawnPrefabs != null ? spawnPrefabs[prefabIndex] : null;

        if (prefab == null) {
            return;
        }

        GameObject spawnObject = Instantiate(prefab);
        spawnObject.transform.position = positionInput - roadCenterOffset + transform.position;

        spawnObject.transform.parent = transform;

        if (spawnObjects == null)
        {
            spawnObjects = new Transform[1];
            spawnObjects[0] = spawnObject.transform;
        }
        else
        {
            int length = spawnObjects.Length;
            Transform[] newArray = new Transform[length + 1];
            for (int i = 0; i < length; i++)
            {
                newArray[i] = spawnObjects[i];
            }
            newArray[length] = spawnObject.transform;
            spawnObjects = newArray;
        }
    }

    public void CreateSpawnPoint(int colInput, int rowInput)
    {
        int col = colInput + 1;
        int row = rowInput + 1;

        float colWidth = roadSize.x / lanes;
        float rowWidth = roadSize.y / laneSlots;

        float colPos = colWidth * col - colWidth / 2;
        float rowPos = rowWidth * row;

        Vector3 position = new Vector3(colPos, 0, rowPos);

        SpawnPoint spawnPoint = new SpawnPoint(new int[] { colInput, rowInput }, position, Quaternion.identity);

        if (spawnPoints == null)
        {
            spawnPoints = new List<SpawnPoint>();
        }

        spawnPoints.Add(spawnPoint);

        CreateSpawnPointDebug(position);
    }

    public bool GetRandomState(float factorInput)
    {
        float factor = Random.value;

        return factor >= factorInput;
    }

    public int GetOneOf(int length)
    {
        return Random.Range(0, length);
    }
}