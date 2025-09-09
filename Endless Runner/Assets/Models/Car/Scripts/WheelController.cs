using UnityEngine;

public class WheelController : MonoBehaviour
{
    [Header("Settings")]
    public bool isPowered = false;
    public float maxAngle = 30f;
    public float offset = 0f;

    [Header("Debug")]
    public float turnAngle;
    public WheelCollider wcol;
    public Transform mesh;
    void Start()
    {
        wcol = transform.Find("Collider").GetComponent<WheelCollider>();
        mesh = transform.Find("Mesh");
    }

    public void Steer(float steerInput)
    {
        if (wcol == null)
        {
            return;
        }

        turnAngle = steerInput * maxAngle + offset;
        wcol.steerAngle = turnAngle;
        // mesh.localRotation = Quaternion.Euler(0, turnAngle, 0);
    }

    public void Accelerate(float powerInput)
    {
        if (wcol == null)
        {
            return;
        }

        if (isPowered)
        {
            wcol.motorTorque = powerInput;
        }
        else
        {
            wcol.motorTorque = 0;
            wcol.brakeTorque = 0;
        }
    }

    public void UpdatePosition()
    {
        if (mesh == null || wcol == null)
        {
            return;
        }

        Vector3 pos = transform.position;
        Quaternion rot = transform.rotation;

        wcol.GetWorldPose(out pos, out rot);

        mesh.transform.position = pos;
        mesh.transform.rotation = rot;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
