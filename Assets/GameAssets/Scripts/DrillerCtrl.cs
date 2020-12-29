using DG.Tweening; 
using UnityEngine; 
using UnityEngine.Animations;
using System.Collections;

public class DrillerCtrl : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem confettiParticle;

    private StatusController statusController; 
    private DestructibleObject destructibleObj;
    private DrillerMovement drillerMovement;
    private DOTweenAnimation[] wheelAnims;
    private UIController uiController;
    private bool tutorialComplete=false,swipeDownComplete=false;
    private void Start()
    {
        uiController = FindObjectOfType<UIController>();
        wheelAnims = GetComponentsInChildren<DOTweenAnimation>();
        drillerMovement = GetComponent<DrillerMovement>();
        statusController = FindObjectOfType<StatusController>();  
    } 
    private void Update()
    {
        if (uiController.playedBefore == 0)
        {
            if (!tutorialComplete)
            {
                if (transform.position.y <= -8.5f && !swipeDownComplete)//-8,5
                {
                    uiController.CloseSwipeDown.Invoke();
                    Invoke("OpenSwipeUp", 1.5f);
                    swipeDownComplete = true;
                }
                if (swipeDownComplete && transform.position.y >= -4.5f)
                {
                    uiController.TutorialEnd.Invoke();
                    tutorialComplete = true;
                }
            }
        }
    }
    private void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("StartTrigger"))
        {
            drillerMovement.allowMovement = true;
            if (uiController.playedBefore==0)
            {
                uiController.TutorialBegin.Invoke(/*StandbyTime*/);
            }
        }
        if (col.CompareTag("FinishTrigger"))
        {
            if (uiController.playedBefore==0)
            {
                PlayerPrefs.SetInt("firstPlay", 1);
            }
            LevelSuccess();
        } 
    } 
    
    public void WheelAnimsStart()
    {
        foreach (DOTweenAnimation wheelAnim in wheelAnims)
        {
            wheelAnim.DOPlay();
        }
    }
    public void WheelAnimsStop()
    {
        foreach (DOTweenAnimation wheelAnim in wheelAnims)
        {
            wheelAnim.DOKill();
        }
    }
    public void GameOver()
    {
        Camera.main.GetComponent<LookAtConstraint>().enabled = false;
        Camera.main.GetComponent<PositionConstraint>().enabled = false; 
        statusController.LevelFailed();
    }

    public void LevelSuccess()
    {
        confettiParticle.Play();
        statusController.LevelComplete();
        WheelAnimsStop();
        drillerMovement.speedLimit = 0;
    }
    void OpenSwipeUp()
    { 
        uiController.OpenSwipeUp.Invoke();
    }
}
