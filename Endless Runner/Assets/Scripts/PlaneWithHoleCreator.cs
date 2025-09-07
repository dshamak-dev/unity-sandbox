using UnityEngine;

public class PlaneWithSquareHole : MonoBehaviour
{
    [Header("Plane Settings")]
    public float planeSize = 10f;
    public int segments = 10;

    [Header("Hole Settings")]
    public float holeSize = 3f;
    public Vector2 holeOffset = Vector2.zero;

    public bool randomOffset = false;

    [Header("Rendering")]
    public Material planeMaterial;
    public Color color = Color.white;
    public bool generateOnStart = true;
    public bool addCollider = true;

    void Start()
    {
        if (randomOffset)
        {
            Vector2 offset = new Vector2(
                Mathf.Round(Random.Range(-planeSize/2 + holeSize/2, planeSize/2 - holeSize/2)),
                Mathf.Round(Random.Range(-planeSize/2 + holeSize/2, planeSize/2 - holeSize/2))
            ); 

            holeOffset = offset;
        }

        GeneratePlaneWithHole();
    }
    
    public void GeneratePlaneWithHole()
    {
        // Create mesh filter and renderer if they don't exist
        MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
        if (meshFilter == null)
            meshFilter = gameObject.AddComponent<MeshFilter>();
            
        MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
        if (meshRenderer == null)
            meshRenderer = gameObject.AddComponent<MeshRenderer>();

        // Generate the mesh
        Mesh mesh = CreatePlaneMeshWithHole(planeSize, segments, holeSize, holeOffset);
        meshFilter.mesh = mesh;

        // Apply material
        if (planeMaterial != null)
        {
            meshRenderer.material = planeMaterial;
            meshRenderer.material.color = color;
        }
        else
        {
            // Create a default material if none provided
            meshRenderer.material = new Material(Shader.Find("Standard"));
            meshRenderer.material.color = color == null ? Color.gray : color;
        }

        // Add collider if requested
        if (addCollider)
        {
            MeshCollider collider = gameObject.GetComponent<MeshCollider>();
            if (collider == null)
                collider = gameObject.AddComponent<MeshCollider>();
            collider.sharedMesh = mesh;
        }
    }

    private Mesh CreatePlaneMeshWithHole(float size, int segs, float holeSize, Vector2 holeOffset)
    {
        Mesh mesh = new Mesh();
        mesh.name = "PlaneWithHole";

        int vertexCount = (segs + 1) * (segs + 1);
        int holeVertexCount = 4; // 4 vertices for the hole
        int totalVertices = vertexCount + holeVertexCount;

        // Create vertices
        Vector3[] vertices = new Vector3[totalVertices];
        Vector2[] uv = new Vector2[totalVertices];
        Vector3[] normals = new Vector3[totalVertices];

        float halfSize = size * 0.5f;
        float segmentSize = size / segs;

        // Generate grid vertices
        for (int z = 0; z <= segs; z++)
        {
            for (int x = 0; x <= segs; x++)
            {
                int index = z * (segs + 1) + x;
                vertices[index] = new Vector3(
                    x * segmentSize - halfSize,
                    0,
                    z * segmentSize - halfSize
                );
                uv[index] = new Vector2((float)x / segs, (float)z / segs);
                normals[index] = Vector3.up;
            }
        }

        // Add hole vertices (counter-clockwise order)
        int holeStartIndex = vertexCount;
        vertices[holeStartIndex] = new Vector3(-holeSize/2 + holeOffset.x, 0, holeSize/2 + holeOffset.y);
        vertices[holeStartIndex + 1] = new Vector3(holeSize/2 + holeOffset.x, 0, holeSize/2 + holeOffset.y);
        vertices[holeStartIndex + 2] = new Vector3(holeSize/2 + holeOffset.x, 0, -holeSize/2 + holeOffset.y);
        vertices[holeStartIndex + 3] = new Vector3(-holeSize/2 + holeOffset.x, 0, -holeSize/2 + holeOffset.y);
        
        // Set UVs and normals for hole vertices
        for (int i = 0; i < 4; i++)
        {
            uv[holeStartIndex + i] = new Vector2(
                (vertices[holeStartIndex + i].x + halfSize) / size,
                (vertices[holeStartIndex + i].z + halfSize) / size
            );
            normals[holeStartIndex + i] = Vector3.up;
        }

        // Create triangles - we need to create triangles around the hole
        int triangleCount = segs * segs * 6; // Standard plane triangles
        triangleCount += 12; // Additional triangles for the hole (4 quads = 12 triangles)
        
        int[] triangles = new int[triangleCount];
        int triIndex = 0;

        // Create standard plane triangles (excluding the area where the hole will be)
        for (int z = 0; z < segs; z++)
        {
            for (int x = 0; x < segs; x++)
            {
                int vertexIndex = z * (segs + 1) + x;
                
                // Check if this quad is inside the hole area
                Vector3 quadCenter = new Vector3(
                    (vertices[vertexIndex].x + vertices[vertexIndex + 1].x + 
                     vertices[vertexIndex + segs + 1].x + vertices[vertexIndex + segs + 2].x) / 4,
                    0,
                    (vertices[vertexIndex].z + vertices[vertexIndex + 1].z + 
                     vertices[vertexIndex + segs + 1].z + vertices[vertexIndex + segs + 2].z) / 4
                );
                
                // Skip quads that are inside the hole
                if (Mathf.Abs(quadCenter.x - holeOffset.x) < holeSize/2 && 
                    Mathf.Abs(quadCenter.z - holeOffset.y) < holeSize/2)
                {
                    continue;
                }

                // Create two triangles for the quad
                triangles[triIndex++] = vertexIndex;
                triangles[triIndex++] = vertexIndex + segs + 1;
                triangles[triIndex++] = vertexIndex + 1;

                triangles[triIndex++] = vertexIndex + 1;
                triangles[triIndex++] = vertexIndex + segs + 1;
                triangles[triIndex++] = vertexIndex + segs + 2;
            }
        }

        // Create triangles connecting the hole to the plane
        // We need to find which grid vertices are closest to the hole vertices
        // This is a simplified approach - for a more accurate mesh, we'd need to do proper triangulation
        
        // For each hole vertex, find the closest grid vertex and create triangles
        for (int i = 0; i < 4; i++)
        {
            int holeVertexIndex = holeStartIndex + i;
            int nextHoleVertexIndex = holeStartIndex + ((i + 1) % 4);
            
            // Find closest grid vertex to this hole vertex
            int closestGridVertex = FindClosestGridVertex(vertices, holeVertexIndex, segs);
            
            // Create triangles connecting hole vertex to grid
            triangles[triIndex++] = holeVertexIndex;
            triangles[triIndex++] = closestGridVertex;
            triangles[triIndex++] = nextHoleVertexIndex;
        }

        // Assign arrays to mesh
        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.normals = normals;
        mesh.triangles = triangles;

        // Recalculate for good measure
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.RecalculateTangents();

        return mesh;
    }

    private int FindClosestGridVertex(Vector3[] vertices, int holeVertexIndex, int segs)
    {
        int closestIndex = 0;
        float closestDistance = float.MaxValue;
        
        // Only check the grid vertices (not the hole vertices we added)
        for (int i = 0; i < (segs + 1) * (segs + 1); i++)
        {
            float distance = Vector3.Distance(vertices[i], vertices[holeVertexIndex]);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestIndex = i;
            }
        }
        
        return closestIndex;
    }

    // Editor method to regenerate the plane
    #if UNITY_EDITOR
    [ContextMenu("Regenerate Plane")]
    private void RegeneratePlane()
    {
        GeneratePlaneWithHole();
    }
    #endif
}