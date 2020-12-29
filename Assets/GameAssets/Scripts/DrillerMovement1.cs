using Lean.Touch;
using UnityEngine;

public class DrillerMovement1 : MonoBehaviour
{
    [SerializeField] private float tiltAngle = 10; //15
    [SerializeField] private float tiltDuration = 5f; //0.2
    [SerializeField] private float rotateDuration = 5f; //0.1
    [SerializeField] private Transform cabinRoot;
    public float movementSpeed = 5, speedLimit = 0, drillerSpeed, sensitivityMultiplier = 1.5f;
    private LeanFingerFilter use = new LeanFingerFilter(false);
    private bool isFingerDown;
    private Vector3 clampedRotation;
    private Camera camera;
    private float firstZPos;
    private float maxYPos;
    private float tiltAmount;
    private float tiltVelocity;
    private float difPos;
    public bool allowMovement;
    private void OnEnable()
    {
        allowMovement = false;
        firstZPos = transform.position.z;
        camera = Camera.main;
        clampedRotation = transform.localEulerAngles;
        maxYPos = transform.position.y;
        LeanTouch.OnFingerDown += OnFingerDown;
        LeanTouch.OnFingerUp += OnFingerUp;
    }

    private void OnFingerUp(LeanFinger obj)
    {
        isFingerDown = false;
    }

    private void OnFingerDown(LeanFinger obj)
    {
        isFingerDown = true;
    }

    private void Update()
    {
        var fingers = use.GetFingers();
        var screenDelta = LeanGesture.GetScreenDelta(fingers);
        if (isFingerDown)
        {
            if (allowMovement)
            {
                if (screenDelta != Vector2.zero)
                {
                    Vertical(screenDelta);
                }
            }
        }

        if (allowMovement) Tilt(screenDelta);
        Acceleration();
    }

    private void Vertical(Vector2 screenDelta)
    {
        var screenPoint = camera.WorldToScreenPoint(transform.position);
        screenPoint += (Vector3) screenDelta;
        Vector3 position = camera.ScreenToWorldPoint(screenPoint);
        position.y = Mathf.Clamp(position.y, -50f, maxYPos);
        position.x = transform.position.x;
        position.z = firstZPos;
        if (position.y > transform.position.y) difPos *= -1;
        else difPos *= 1;
        transform.position = Vector3.Lerp(transform.position, position, movementSpeed);
    }

    private void Acceleration()
    {
        drillerSpeed = Mathf.Lerp(drillerSpeed, speedLimit, .15f);
        transform.Translate(Vector3.right * drillerSpeed * Time.deltaTime);
        Vector3 clampedPos = transform.position;
        clampedPos.y = Mathf.Clamp(transform.position.y, -50, maxYPos);
        transform.position = clampedPos;
    }

    private void Tilt(Vector3 position)
    {
        tiltAmount = Mathf.SmoothDamp(tiltAmount,
            position.y / movementSpeed * tiltAngle, ref tiltVelocity,
            tiltDuration);
        Vector3 desireRot = cabinRoot.localEulerAngles;
        tiltAmount = Mathf.Clamp(tiltAmount, -tiltAngle, tiltAngle);
        desireRot.z = tiltAmount;
        if (cabinRoot.position.y < maxYPos)
            cabinRoot.localRotation =
                Quaternion.Lerp(cabinRoot.localRotation, Quaternion.Euler(desireRot), rotateDuration);
        else
            cabinRoot.localRotation =
                Quaternion.Lerp(cabinRoot.localRotation, Quaternion.Euler(Vector3.zero), rotateDuration);
    }

    private void OnDisable()
    {
        LeanTouch.OnFingerDown -= OnFingerDown;
        LeanTouch.OnFingerUp -= OnFingerUp;
    }
}