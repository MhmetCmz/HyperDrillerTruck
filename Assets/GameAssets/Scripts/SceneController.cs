using UnityEngine; 

public class SceneController : MonoBehaviour
{
    [SerializeField] private LevelLoader levelLoader; 
    void Start()
    {
        levelLoader.LoadFirstLevel(); 
    }  
}
