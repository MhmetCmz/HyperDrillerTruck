using Lean.Touch;
using UnityEngine;

public class DrillerMovement : MonoBehaviour
{
    public float rotationSpeed = 5, speedLimit = 0, drillerSpeed, sensitivityMultiplier, screenDelta;
    private LeanFingerFilter use = new LeanFingerFilter(false);
    private bool isFingerDown;
    private float maxYPos;


    private Vector3 clampedRotation;
    public bool allowMovement;

    private void OnEnable()
    {
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
        if (allowMovement)
        {
            if (isFingerDown)
            {
                var screenFrom = LeanGesture.GetStartScreenCenter(fingers);
                var screenTo = LeanGesture.GetScreenCenter(fingers);
                screenDelta = screenTo.y - screenFrom.y;
                screenDelta *= LeanTouch.ScreenFactor;
                Debug.Log(screenDelta);
            }
            else screenDelta = Mathf.Lerp(screenDelta, 0, 0.06f);
            Horizontal(screenDelta);
        } 
        Acceleration();
    }

    private void Horizontal(float screenDelta)
    {
        var upturnLimit =Mathf.Clamp( (maxYPos - transform.position.y)*20 ,0,60);
        var ClampedNextX =Mathf.Clamp(screenDelta * sensitivityMultiplier,-60,upturnLimit);
        var currentRot = transform.localRotation.eulerAngles;
        currentRot.x = ClampedNextX;
        clampedRotation = currentRot;  
        transform.localRotation=
            Quaternion.Lerp(transform.localRotation, Quaternion.Euler(clampedRotation), rotationSpeed);
    }

    private void Acceleration()
    {
        drillerSpeed = Mathf.Lerp(drillerSpeed, speedLimit, .15f);
        transform.Translate(transform.right * -drillerSpeed * Time.deltaTime);
        Vector3 clampedPos = transform.position;
        clampedPos.y = Mathf.Clamp(transform.position.y, -50, maxYPos);
        transform.position = clampedPos;
    }

    private void OnDisable()
    {
        LeanTouch.OnFingerDown -= OnFingerDown;
        LeanTouch.OnFingerUp -= OnFingerUp;
    }
}