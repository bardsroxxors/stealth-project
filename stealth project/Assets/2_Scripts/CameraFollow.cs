using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
//using UnityEngine.Rendering.Universal;

public class CameraFollow : MonoBehaviour
{
    public GameObject playerObject;
    private Transform target; // The object the camera should follow
    private PlayerController playerController;
    private Vector2 playerMovementVector = Vector2.zero;

    [Range(0, 1)]
    public float leadingFactor = 1.5f;
    public float smoothing = 5f; // The speed with which the camera catches up with the target

    public Vector2 mousePullStrength = Vector2.zero;
    public Vector2 extendedPullStrength = Vector2.zero;


    public Vector2 offset = new Vector2(0, 1);
    public Vector2 targetPos = new Vector2(0, 0);
    public float xSmoothing = 5f;
    public float ySmoothing = 3f;

    public float playerHeightCutoff = 3;

    [Range(16, 100)]
    public float gridSize = 64;

    private Vector2Int screenSize = new Vector2Int(Screen.width, Screen.height);
    private Vector2Int screenCenter = new Vector2Int(0, 0);
    private Vector3 mousePosition = Vector3.zero;


    private Camera cameraComponent;

    private bool f_viewExtend = false;



    void Start()
    {
        cameraComponent = GetComponent<Camera>();
        target = playerObject.transform;
        playerController = playerObject.GetComponent<PlayerController>();

        screenCenter.x = screenSize.x / 2;
        screenCenter.y = screenSize.y / 2;

        playerObject = GameObject.Find("Player");
    }

    private void Update()
    {
        mousePosition = Input.mousePosition;
    }

    void FixedUpdate()
    {
        playerMovementVector = playerController.movementVector;

        Vector3 targetCamPos = (Vector2)target.position + offset + (playerMovementVector * leadingFactor);
        targetCamPos = targetCamPos + GetMousePosVector();
        float newX = Mathf.Lerp(transform.position.x, targetCamPos.x, xSmoothing * Time.deltaTime);
        float newY = Mathf.Lerp(transform.position.y, targetCamPos.y, ySmoothing * Time.deltaTime);
        transform.position = new Vector3(newX, newY, transform.position.z);

        

        SnapToGrid();

    }

    // gets the vector from the screen center to mouse position
    Vector3 GetMousePosVector()
    {
        Vector3 center = new Vector3(screenCenter.x, screenCenter.y, 0);
        Vector3 mouseNormalised = new Vector3(  mousePosition.x / screenSize.x,
                                                mousePosition.y / screenSize.y,
                                                0);

        Vector3 centerToMouse = mouseNormalised - new Vector3(0.5f, 0.5f, 0);

        if(f_viewExtend) return centerToMouse * extendedPullStrength;
        else return centerToMouse * mousePullStrength;

    }

    private void UpdateScreenHeight()
    {
        //Debug.Log("screen height");
        //pixelFeature.settings.screenHeight = (int)(cameraComponent.orthographicSize * 2 * 64);
        //int h = Screen.height;
        //float c = cameraComponent.orthographicSize;
        //pixelFeature.settings.screenHeight = (int)(h / (c*2));
    }

    public void SnapToGrid()
    {

        float gridDistance = 1 / gridSize;

        Vector3 snap = new Vector3(transform.position.x - (transform.position.x % gridDistance),
                                    transform.position.y - (transform.position.y % gridDistance), -10);
        transform.position = snap;
    }


    void OnExtendView(InputValue value)
    {
        f_viewExtend = true;
    }
    void OnExtendViewRelease(InputValue value)
    {
        f_viewExtend = false;
    }
}