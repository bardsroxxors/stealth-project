using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering;
//using UnityEngine.Rendering.Universal;

public class CameraFollow : MonoBehaviour
{
    public GameObject playerObject;
    private Transform target; // The object the camera should follow
    private PlayerController playerController;
    private Vector2 playerMovementVector = Vector2.zero;

    public float leadingFactor = 1.5f;
    public float smoothing = 5f; // The speed with which the camera catches up with the target

    public Vector2 offset = new Vector2(0, 1);
    public Vector2 targetPos = new Vector2(0, 0);
    public float xSmoothing = 5f;
    public float ySmoothing = 3f;

    public float playerHeightCutoff = 3;

    public float gridSize = 64;


    //public UniversalRenderPipelineAsset pipeline;

    private Camera cameraComponent;



    void Start()
    {
        cameraComponent = GetComponent<Camera>();
        target = playerObject.transform;
        playerController = playerObject.GetComponent<PlayerController>();
        /*var rendererFeatures = pipeline.scriptableRenderer.GetType()
        .GetProperty("rendererFeatures", BindingFlags.NonPublic | BindingFlags.Instance)
        ?.GetValue(pipeline.scriptableRenderer, null) as List<ScriptableRendererFeature>;*/


        //UpdateScreenHeight();
    }

    void FixedUpdate()
    {
        playerMovementVector = playerController.inputVector;

        Vector3 targetCamPos = (Vector2)target.position + offset + (playerMovementVector * leadingFactor);
        float newX = Mathf.Lerp(transform.position.x, targetCamPos.x, smoothing * Time.deltaTime);
        float newY = Mathf.Lerp(transform.position.y, targetCamPos.y, smoothing * Time.deltaTime);
        transform.position = new Vector3(newX, newY, transform.position.z);

        //SnapToGrid();

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

        Vector3 snap = new Vector4(transform.position.x - (transform.position.x % gridDistance),
                                    transform.position.y - (transform.position.y % gridDistance), -10);
        transform.position = snap;
    }
}