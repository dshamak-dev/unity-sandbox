using UnityEngine;

[ExecuteInEditMode]
public class AdvancedRoadController : MonoBehaviour
{
    [Header("References")]
    public Renderer roadRenderer;
    
    [Header("Road Settings")]
    public Color roadColor = new Color(0.2f, 0.2f, 0.2f, 1f);
    public float roadTextureScale = 1f;
    public float roadSmoothness = 0.1f;
    public float roadMetallic = 0.0f;
    
    [Header("Sidewalk Settings")]
    public Color sidewalkColor = new Color(0.8f, 0.8f, 0.8f, 1f);
    public bool hasLeftSidewalk = true;
    public float leftSidewalkWidth = 0.15f;
    public bool hasRightSidewalk = true;
    public float rightSidewalkWidth = 0.15f;
    public float sidewalkTextureScale = 1f;
    public float sidewalkSmoothness = 0.3f;
    public float sidewalkMetallic = 0.0f;
    
    [Header("Road Line Settings")]
    public Color lineColor = Color.white;
    public float lineWidth = 0.02f;
    public int roadLinesAmount = 1;
    public bool hasDashedSeparator = true;
    public float lineRepeat = 5f;
    public float lineDashRatio = 0.5f;
    public float lineEmission = 0.5f;
    public float lineSmoothness = 0.8f;
    public float lineMetallic = 0.0f;
    
    [Header("Border Line Settings")]
    public Color borderLineColor = Color.white;
    public float borderLineWidth = 0.01f;
    public float borderLineEmission = 0.3f;
    public float borderLineSmoothness = 0.7f;
    public float borderLineMetallic = 0.0f;
    
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
        // Clamp values to valid ranges
        leftSidewalkWidth = Mathf.Clamp(leftSidewalkWidth, 0, 0.5f);
        rightSidewalkWidth = Mathf.Clamp(rightSidewalkWidth, 0, 0.5f);
        roadLinesAmount = Mathf.Clamp(roadLinesAmount, 0, 10);
        lineDashRatio = Mathf.Clamp01(lineDashRatio);
        
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
            roadMaterial.SetFloat("_RoadMetallic", roadMetallic);
            
            // Sidewalk properties
            roadMaterial.SetColor("_SidewalkColor", sidewalkColor);
            roadMaterial.SetFloat("_HasLeftSidewalk", hasLeftSidewalk ? 1 : 0);
            roadMaterial.SetFloat("_LeftSidewalkWidth", leftSidewalkWidth);
            roadMaterial.SetFloat("_HasRightSidewalk", hasRightSidewalk ? 1 : 0);
            roadMaterial.SetFloat("_RightSidewalkWidth", rightSidewalkWidth);
            roadMaterial.SetFloat("_SidewalkTextureScale", sidewalkTextureScale);
            roadMaterial.SetFloat("_SidewalkSmoothness", sidewalkSmoothness);
            roadMaterial.SetFloat("_SidewalkMetallic", sidewalkMetallic);
            
            // Line properties
            roadMaterial.SetColor("_LineColor", lineColor);
            roadMaterial.SetFloat("_LineWidth", lineWidth);
            roadMaterial.SetInt("_RoadLinesAmount", roadLinesAmount);
            roadMaterial.SetFloat("_HasDashedSeparator", hasDashedSeparator ? 1 : 0);
            roadMaterial.SetFloat("_LineRepeat", lineRepeat);
            roadMaterial.SetFloat("_LineDashRatio", lineDashRatio);
            roadMaterial.SetFloat("_LineEmission", lineEmission);
            roadMaterial.SetFloat("_LineSmoothness", lineSmoothness);
            roadMaterial.SetFloat("_LineMetallic", lineMetallic);
            
            // Border line properties
            roadMaterial.SetColor("_BorderLineColor", borderLineColor);
            roadMaterial.SetFloat("_BorderLineWidth", borderLineWidth);
            roadMaterial.SetFloat("_BorderLineEmission", borderLineEmission);
            roadMaterial.SetFloat("_BorderLineSmoothness", borderLineSmoothness);
            roadMaterial.SetFloat("_BorderLineMetallic", borderLineMetallic);
            
            // Advanced properties
            roadMaterial.SetFloat("_TransitionSharpness", transitionSharpness);
            roadMaterial.SetFloat("_UVRotation", uvRotation);
        }
    }
    
    // Quick test method to verify lighting is working
    public void TestLighting()
    {
        // Set up materials with different properties to test lighting
        SetRoadColor(new Color(0.3f, 0.3f, 0.3f, 1f));
        SetRoadSmoothness(0.8f);
        SetRoadMetallic(0.1f);
        
        SetSidewalkColor(new Color(0.7f, 0.7f, 0.7f, 1f));
        SetSidewalkSmoothness(0.4f);
        
        Debug.Log("Lighting test applied. Check if road reflects light properly.");
    }
    
    public void SetRoadSmoothness(float smoothness)
    {
        roadSmoothness = Mathf.Clamp01(smoothness);
        UpdateMaterialProperties();
    }
    
    public void SetRoadMetallic(float metallic)
    {
        roadMetallic = Mathf.Clamp01(metallic);
        UpdateMaterialProperties();
    }
    
    public void SetSidewalkSmoothness(float smoothness)
    {
        sidewalkSmoothness = Mathf.Clamp01(smoothness);
        UpdateMaterialProperties();
    }
    
    // Public methods to change properties at runtime
    public void SetLeftSidewalk(bool enabled, float width = 0.15f)
    {
        hasLeftSidewalk = enabled;
        leftSidewalkWidth = Mathf.Clamp(width, 0, 0.5f);
        UpdateMaterialProperties();
    }
    
    public void SetRightSidewalk(bool enabled, float width = 0.15f)
    {
        hasRightSidewalk = enabled;
        rightSidewalkWidth = Mathf.Clamp(width, 0, 0.5f);
        UpdateMaterialProperties();
    }
    
    public void SetRoadLines(int amount, bool dashed = true, float width = 0.02f)
    {
        roadLinesAmount = Mathf.Clamp(amount, 0, 10);
        hasDashedSeparator = dashed;
        lineWidth = Mathf.Clamp(width, 0, 0.1f);
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
    
    // Helper method to quickly configure common road types
    public void ConfigureRoadType(RoadType type)
    {
        switch (type)
        {
            case RoadType.SingleLane:
                SetRoadLines(0, true, 0.02f);
                SetLeftSidewalk(true, 0.15f);
                SetRightSidewalk(true, 0.15f);
                break;
                
            case RoadType.TwoLane:
                SetRoadLines(1, true, 0.02f);
                SetLeftSidewalk(true, 0.15f);
                SetRightSidewalk(true, 0.15f);
                break;
                
            case RoadType.FourLane:
                SetRoadLines(3, true, 0.02f);
                SetLeftSidewalk(true, 0.15f);
                SetRightSidewalk(true, 0.15f);
                break;
                
            case RoadType.Highway:
                SetRoadLines(2, false, 0.03f); // Solid lines for highway
                SetLeftSidewalk(false, 0f);
                SetRightSidewalk(false, 0f);
                break;
                
            case RoadType.Alley:
                SetRoadLines(0, true, 0.02f);
                SetLeftSidewalk(false, 0f);
                SetRightSidewalk(false, 0f);
                break;
        }
    }
}

public enum RoadType
{
    SingleLane,
    TwoLane,
    FourLane,
    Highway,
    Alley
}