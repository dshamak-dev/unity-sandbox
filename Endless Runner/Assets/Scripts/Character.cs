using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class Character : MonoBehaviour
{
    public float forwardSpeed = 10f;
    public float maxForwardSpeed = 30f;
    public float forwardAcceleration = 0.01f;

    public float maxSideSpeed = 5f;
    public float sideSpeedFactor = 0.5f;
    
    public int currentLane = 1; // Middle lane
    public bool isRunning = false;

    [SerializeField] public bool isOnGround
    {
        get { return colliders.Count > 0; }
    }
    public Vector3 targetPosition;

    private Rigidbody rb;

    private CharacterMovement moveCtrl;
    public InputActionMap input;

    private List<Transform> colliders = new List<Transform>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {   
        rb = GetComponent<Rigidbody>();

        if (GameManager.Instance != null)
        {
            input = GameManager.Instance.GetInputActions("Player");
        }

        moveCtrl = gameObject.AddComponent<CharacterMovement>();

        moveCtrl.setCharacter(this);
    }
    
    private void FixedUpdate() {
       
    }

    private void OnCollisionEnter(Collision other) {
        if (other.gameObject.CompareTag("Ground"))
        {
            colliders.Add(other.transform);
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            colliders.Remove(other.transform);
        }
    }

   public void Reset()
    {
        currentLane = 1;
        transform.position = new Vector3(0,
            transform.position.y, transform.position.z);
        targetPosition = transform.position;
        isRunning = true;
    }
}
