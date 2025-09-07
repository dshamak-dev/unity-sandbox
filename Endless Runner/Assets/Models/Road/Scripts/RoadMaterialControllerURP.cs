using UnityEngine;

[ExecuteInEditMode]
public class RoadMaterialControllerURP : MonoBehaviour
{
    [Header("References")]
    public Renderer roadRenderer;
    
    [Header("Road Settings")]
    public Color roadColor = new Color(0.2f, 0.2f, 0.2f, 1f);
    public float roadTextureScale = 1f;
    public float roadSmoothness = 0.1f;
    
    [Header("Sidewalk Settings")]
    public Color sidewalkColor = new Color(0.8f, 0.8f, 0.8f, 1f);
    public float sidewalkWidth = 0.15f;
    public float sidewalkTextureScale = 1f;
    public float sidewalkSmoothness = 0.3f;
    
    [Header("Line Settings")]
    public Color lineColor = Color.white;
    public float lineWidth = 0.02f;
    public float lineRepeat = 5f;
    public float lineDashRatio = 0.5f;
    public float lineEmission = 0.5f;
    public float lineSmoothness = 0.8f;
    
    [Header("Border Line Settings")]
    public Color borderLineColor = Color.white;
    public float borderLineWidth = 0.01f;
    public float borderLineEmission = 0.3f;
    public float borderLineSmoothness = 0.7f;
    
    [Header("Advanced Settings")]
    public float transitionSharpness = 5f;
    public float uvRotation = 0f;
    
    private Material roadMaterial;
    
    void Start()
    {
        if (roadRenderer != null)
        {
            // Create a material instance to avoid affecting the original material
            roadMaterial = new Material(roadRenderer.sharedMaterial);
            roadRenderer.material = roadMaterial;
            UpdateMaterialProperties();
        }
    }
    
    void OnValidate()
    {
        UpdateMaterialProperties();
    }
    
    void OnDestroy()
    {
        // Clean up the material instance
        if (roadRenderer != null && roadMaterial != null)
        {
            if (Application.isPlaying)
            {
                Destroy(roadMaterial);
            }
            else
            {
                DestroyImmediate(roadMaterial);
            }
        }
    }
    
    public void UpdateMaterialProperties()
    {
        if (roadMaterial == null && roadRenderer != null)
        {
            roadMaterial = roadRenderer.material;
        }
        
        if (roadMaterial != null)
        {
            // Road properties
            roadMaterial.SetColor("_RoadColor", roadColor);
            roadMaterial.SetFloat("_RoadTextureScale", roadTextureScale);
            roadMaterial.SetFloat("_RoadSmoothness", roadSmoothness);
            
            // Sidewalk properties
            roadMaterial.SetColor("_SidewalkColor", sidewalkColor);
            roadMaterial.SetFloat("_SidewalkWidth", sidewalkWidth);
            roadMaterial.SetFloat("_SidewalkTextureScale", sidewalkTextureScale);
            roadMaterial.SetFloat("_SidewalkSmoothness", sidewalkSmoothness);
            
            // Line properties
            roadMaterial.SetColor("_LineColor", lineColor);
            roadMaterial.SetFloat("_LineWidth", lineWidth);
            roadMaterial.SetFloat("_LineRepeat", lineRepeat);
            roadMaterial.SetFloat("_LineDashRatio", lineDashRatio);
            roadMaterial.SetFloat("_LineEmission", lineEmission);
            roadMaterial.SetFloat("_LineSmoothness", lineSmoothness);
            
            // Border line properties
            roadMaterial.SetColor("_BorderLineColor", borderLineColor);
            roadMaterial.SetFloat("_BorderLineWidth", borderLineWidth);
            roadMaterial.SetFloat("_BorderLineEmission", borderLineEmission);
            roadMaterial.SetFloat("_BorderLineSmoothness", borderLineSmoothness);
            
            // Advanced properties
            roadMaterial.SetFloat("_TransitionSharpness", transitionSharpness);
            roadMaterial.SetFloat("_UVRotation", uvRotation);
        }
    }
    
    // Public methods to change properties at runtime
    public void SetSidewalkWidth(float width)
    {
        sidewalkWidth = Mathf.Clamp(width, 0, 0.5f);
        UpdateMaterialProperties();
    }
    
    public void SetLinePattern(float repeat, float dashRatio)
    {
        lineRepeat = repeat;
        lineDashRatio = Mathf.Clamp01(dashRatio);
        UpdateMaterialProperties();
    }
    
    public void SetRoadColor(Color color)
    {
        roadColor = color;
        UpdateMaterialProperties();
    }
    
    public void SetSidewalkColor(Color color)
    {
        sidewalkColor = color;
        UpdateMaterialProperties();
    }
    
    public void SetLineColor(Color color)
    {
        lineColor = color;
        UpdateMaterialProperties();
    }
    
    public void SetBorderLineColor(Color color)
    {
        borderLineColor = color;
        UpdateMaterialProperties();
    }
    
    public void SetBorderLineWidth(float width)
    {
        borderLineWidth = Mathf.Clamp(width, 0, 0.05f);
        UpdateMaterialProperties();
    }
    
    public void SetUVRotation(float rotation)
    {
        uvRotation = rotation;
        UpdateMaterialProperties();
    }
}