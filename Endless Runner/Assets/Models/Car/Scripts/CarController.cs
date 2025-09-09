using UnityEngine;

public enum DriveType
{
    FULL,
    Front,
    Back
}

public enum WheelLocation
{
    FrontLeft,
    FrontRight,
    BackLeft,
    BackRight
}

public class CarWheel
{
    public WheelCollider collider;
    public WheelLocation location;
    public Transform transform;
}

public class CarController : MonoBehaviour
{
    [Header("Settings")]
    public DriveType driveType = DriveType.FULL;
    public Vector3 size = Vector3.one;
    public float power = 1500f;
    // public float torque = 500f;
    // public float gravity = 9.81f;
    public Transform gravityTarget;

    public bool isAutoOrient = false;
    public float autoOrientSpeed = 1f;
    public Rigidbody rb;
    public Transform wheelPrefab;

    [Header("Debug")]
    public float horInput = 0;
    public float verInput = 0;
    public float steerAngle = 0;

    public Car car;
    public WheelController[] wheels;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (car == null)
        {
            car = GetComponent<Car>();
        }

        rb = GetComponent<Rigidbody>();
    }

    public void Accelerate(Vector2 vector, float powerInput)
    {
        horInput = vector.x;
        verInput = vector.y;

        power = powerInput;
    }

    void FixedUpdate()
    {
        UpdateForces();
        UpdateGravity();
	}

	// Update is called once per frame
	void Update()
    {

    }

    void UpdateForces()
    {
        if (wheels == null)
        {
            Debug.Log("No wheels found");
            return;
        }

        foreach (WheelController w in wheels)
        {
            w.Steer(horInput);
            w.Accelerate(verInput * power);
            // w.UpdatePosition();
        }
    }
    void UpdateGravity() { }

    void SetAutoOrient(Vector3 input) { }
}
