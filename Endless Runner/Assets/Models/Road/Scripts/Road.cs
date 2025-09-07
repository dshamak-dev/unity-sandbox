using UnityEngine;

public class Road : MonoBehaviour
{
    public RoadMaterialControllerURP roadController;
    public float animationSpeed = 1f;
    
    void Update()
    {
        // Example animation - moving lines
        // if (roadController != null)
        // {
        //     float newOffset = roadController.lineOffset + Time.deltaTime * animationSpeed;
        //     roadController.SetLineOffset(newOffset);
        // }
    }
    
    // GUI for testing in Play Mode
    void OnGUI()
    {
        if (roadController != null)
        {
            GUILayout.BeginArea(new Rect(10, 10, 300, 400));
            GUILayout.Label("Road Shader Controls");
            
            // Sidewalk width
            GUILayout.Label("Sidewalk Width: " + roadController.sidewalkWidth.ToString("F2"));
            roadController.sidewalkWidth = GUILayout.HorizontalSlider(roadController.sidewalkWidth, 0f, 0.5f);
            
            // Line pattern
            GUILayout.Label("Line Repeat: " + roadController.lineRepeat.ToString("F1"));
            roadController.lineRepeat = GUILayout.HorizontalSlider(roadController.lineRepeat, 1f, 20f);
            
            GUILayout.Label("Line Dash Ratio: " + roadController.lineDashRatio.ToString("F2"));
            roadController.lineDashRatio = GUILayout.HorizontalSlider(roadController.lineDashRatio, 0f, 1f);
            
            // UV Rotation
            GUILayout.Label("UV Rotation: " + roadController.uvRotation.ToString("F0"));
            roadController.uvRotation = GUILayout.HorizontalSlider(roadController.uvRotation, 0f, 360f);
            
            if (GUILayout.Button("Random Colors"))
            {
                roadController.SetRoadColor(new Color(Random.value * 0.3f, Random.value * 0.3f, Random.value * 0.3f));
                roadController.SetSidewalkColor(new Color(0.5f + Random.value * 0.5f, 0.5f + Random.value * 0.5f, 0.5f + Random.value * 0.5f));
                roadController.SetLineColor(new Color(Random.value, Random.value, Random.value));
            }
            
            GUILayout.EndArea();
        }
    }
}