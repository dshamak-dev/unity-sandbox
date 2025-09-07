using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterMovement : MonoBehaviour
{
    private Character character;

    public GameManager gameManager;

    private InputAction moveLeftAction;
    private InputAction moveRightAction;

    private InputAction moveAction;

    bool canMove
    {
        get
        {
            return character != null && character.isOnGround;
        }
    }

    float forwardSpeed
    {
        get
        {
            return character.forwardSpeed;
        }
        
        set {
            character.forwardSpeed = value;
        }
    }

    int currentLane
    {
        get
        {
            return character.currentLane;
        }
        
        set {
            character.currentLane = value;
        }
    }

    float[] lanePositions
    {
        get
        {
            return GameManager.Instance.roadGenerator.lanePositions;
        }
    
    }

    Vector3 targetPosition
    {
        get
        {
            return character.targetPosition;
        }
        
         set {
            character.targetPosition = value;
        }
    }

    void Start()
    {
        gameManager = GameManager.Instance;
    }

    public void setCharacter(Character character)
    {
        this.character = character;

        character.targetPosition = transform.position;
        character.isRunning = true;

        if (character.input != null) { 
            moveAction = character.input.FindAction("Move");
        }
    }

    void Update()
    {
        if (!character.isRunning || !canMove) return;

        forwardSpeed = Mathf.Lerp(forwardSpeed, character.maxForwardSpeed, character.forwardAcceleration * Time.deltaTime);

        // Move forward continuously
        transform.Translate(Vector3.forward * character.forwardSpeed * Time.deltaTime);

        // Handle lane switching
        if (moveAction != null && moveAction.triggered)
        {
            Vector2 moveInput = moveAction.ReadValue<Vector2>();

            ChangeLane((int)moveInput.x);
        }

        // Smoothly move to target lane position
        if (transform.position != character.targetPosition)
        {
            float sideSpeed = Mathf.Min(character.maxSideSpeed, 
                forwardSpeed * character.sideSpeedFactor);

            Vector3 targetPos = targetPosition;

            targetPos.z = transform.position.z; // Keep current z position
            targetPos.y = transform.position.y; // Keep current y position

            transform.position = Vector3.MoveTowards(transform.position, 
                targetPos, sideSpeed * Time.deltaTime);
        }
    }

    void ChangeLane(int direction)
    {
        int newLane = currentLane + direction;
        if (newLane >= 0 && newLane < lanePositions.Length)
        {
            currentLane = newLane;
            targetPosition = new Vector3(lanePositions[currentLane], 
                transform.position.y, transform.position.z);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            character.isRunning = false;
            if (gameManager != null)
            {
                gameManager.EndGame();
            }
        }
    }
}
