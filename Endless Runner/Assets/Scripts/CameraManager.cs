using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using Unity.Cinemachine;

public class CameraManager : MonoBehaviour
{
    public InputActionMap input;

    public GameObject liveCamera;
    public CinemachineOrbitalFollow cameraFollow;

    public List<Vector3> presets = new List<Vector3>()
    {
        new Vector3(-26, 12, 2),
        new Vector3(26, 12, 2)
    };

    public int activePresetIndex = -1;

    public Vector3 targetPosition;

    InputAction changePositionAction;

    public void SetNextCameraPosition()
    {

        if (presets.Count > 0)
        {
            activePresetIndex = (activePresetIndex + 1) % presets.Count;
            targetPosition = presets[activePresetIndex];
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Transform traget = GameManager.Instance.player.transform;

        CinemachineBrain brain = Camera.main.GetComponent<CinemachineBrain>();

        liveCamera = GameObject.FindGameObjectWithTag("Cinemachine");

        CinemachineCamera camera = liveCamera.GetComponent<CinemachineCamera>();

        camera.Follow = traget;

        cameraFollow = liveCamera.GetComponent<CinemachineOrbitalFollow>();

        targetPosition = cameraFollow.TargetOffset;

        if (GameManager.Instance != null)
        {
            input = GameManager.Instance.GetInputActions("Camera");

            changePositionAction = input.FindAction("SwitchPosition");
        }

        SetNextCameraPosition();
    }

    // Update is called once per frame
    void Update()
    {
        if (cameraFollow == null)
        {
            return;
        }

        if (changePositionAction != null && changePositionAction.triggered)
        {
            SetNextCameraPosition();
        }

        if (targetPosition != cameraFollow.TargetOffset)
        {
            cameraFollow.TargetOffset = Vector3.Lerp(cameraFollow.TargetOffset, targetPosition, Time.deltaTime);
        }
    }
}
