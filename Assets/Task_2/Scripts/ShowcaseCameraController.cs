using UnityEngine;

public class ShowcaseCameraController : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Orbit Settings")]
    public float rotationSpeed = 5f;
    public float zoomSpeed = 2f;
    public float minZoom = 2f;
    public float maxZoom = 10f;

    private float currentYaw = 90f;
    private float currentPitch = 0f;
    private float distance = 0.8f;
    private Vector2 lastTouchPos;

    void LateUpdate()
    {
        HandleInput();
        UpdateCameraPosition();
    }

    void HandleInput()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        if (Input.GetMouseButton(0))
        {
            currentYaw += Input.GetAxis("Mouse X") * rotationSpeed;
            currentPitch -= Input.GetAxis("Mouse Y") * rotationSpeed;
        }

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        distance -= scroll * zoomSpeed;
#endif

#if UNITY_ANDROID || UNITY_IOS
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Moved)
            {
                Vector2 delta = touch.deltaPosition;
                currentYaw += delta.x * rotationSpeed * 0.02f;
                currentPitch -= delta.y * rotationSpeed * 0.02f;
            }
        }

        if (Input.touchCount == 2)
        {
            Touch t0 = Input.GetTouch(0);
            Touch t1 = Input.GetTouch(1);

            Vector2 prevT0 = t0.position - t0.deltaPosition;
            Vector2 prevT1 = t1.position - t1.deltaPosition;

            float prevDistance = Vector2.Distance(prevT0, prevT1);
            float currDistance = Vector2.Distance(t0.position, t1.position);

            float diff = currDistance - prevDistance;
            distance -= diff * zoomSpeed * 0.01f;
        }
#endif

        distance = Mathf.Clamp(distance, minZoom, maxZoom);
        currentPitch = Mathf.Clamp(currentPitch, -30f, 80f);
    }

    void UpdateCameraPosition()
    {
        Quaternion rotation = Quaternion.Euler(currentPitch, currentYaw, 0);
        Vector3 direction = rotation * Vector3.back * distance;

        transform.position = target.position + direction;
        transform.LookAt(target.position);
    }
}
