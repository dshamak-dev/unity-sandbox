using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    public InputActionAsset assets;

    public Character player;
    public RoadGenerator roadGenerator;
    public CameraManager cameraManager;
    public TextMeshProUGUI distanceText;
    public TextMeshProUGUI finalDistanceText;
    public GameObject pausePanel;
    public GameObject gameOverPanel;
    public Button playButton;
    public Button pauseButton;
    public Button restartButton;
    
    private float distance;
    private bool isPaused = false;
    private bool isGameOver = false;

    public Transform CharacterPrefab;
    public Destroyer destroyerPrefab;


    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (player == null)
        {
            GameObject playerObj = Instantiate(CharacterPrefab, new Vector3(0, 2, 0), Quaternion.identity).gameObject;
            player = playerObj.GetComponent<Character>();

            player.enabled = true;
        }

        roadGenerator = gameObject.AddComponent<RoadGenerator>();

        cameraManager = gameObject.AddComponent<CameraManager>();

        if (destroyerPrefab)
        {
            Destroyer destroyer = Instantiate(destroyerPrefab, new Vector3(0, -10, 0), Quaternion.identity);
            destroyer.follow = player.transform;
            destroyer.offset = new Vector3(0, -10, 0);
            destroyer.tags.Add("Player");
        }
    }

    public InputActionMap GetInputActions(string key)
    {
        return assets.FindActionMap(key);
    }

    void Start()
    {
        if (cameraManager == null)
        { 
            cameraManager = gameObject.AddComponent<CameraManager>();
        }

        // Set up button listeners
        playButton?.onClick.AddListener(ResumeGame);
        pauseButton?.onClick.AddListener(PauseGame);
        restartButton?.onClick.AddListener(RestartGame);

        // Initialize UI
        if (pausePanel != null && gameOverPanel != null) { 
             pausePanel?.SetActive(false);
            gameOverPanel?.SetActive(false);
        }
        
        // Start the game
        Time.timeScale = 1;
    }

    void Update()
    {
        if (player == null)
        {
            return;
        }

        if (!isPaused && !isGameOver && distanceText != null)
        {
            // Update distance
            distance += Time.deltaTime * player.forwardSpeed;
            distanceText.text = "Distance: " + Mathf.FloorToInt(distance) + "m";

            // Pause with Escape key
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (isPaused) ResumeGame();
                else PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0;
        pausePanel.SetActive(true);
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1;
        pausePanel.SetActive(false);
    }

    public void EndGame()
    {
        isGameOver = true;
        finalDistanceText.text = "Final Distance: " + Mathf.FloorToInt(distance) + "m";
        gameOverPanel.SetActive(true);
    }

    public void RestartGame()
    {
        // Reset player
        player.Reset();
        
        // Reset road
        roadGenerator.ResetRoad();
        
        // Reset UI and game state
        distance = 0;
        isPaused = false;
        isGameOver = false;
        pausePanel.SetActive(false);
        gameOverPanel.SetActive(false);
        Time.timeScale = 1;
    }
}
