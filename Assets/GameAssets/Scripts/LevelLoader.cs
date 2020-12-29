using Sirenix.OdinInspector; 
using UnityEngine;
using UnityEngine.SceneManagement; 

[CreateAssetMenu]
public class LevelLoader : ScriptableObject
{
    private const string pref = "level";  
    public int CurrentLevelIndex { get; private set; }

    public void LoadFirstLevel()
    { 
        LoadLevelIndex();
        if (!SceneManager.GetSceneByName($"Level{CurrentLevelIndex}").isLoaded)  
            SceneManager.LoadScene($"Level{CurrentLevelIndex}", LoadSceneMode.Additive); 
        
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene("Game");
        SceneManager.UnloadSceneAsync($"Level{CurrentLevelIndex}");
        SceneManager.LoadScene($"Level{CurrentLevelIndex}", LoadSceneMode.Additive);
    }
    public void Load()
    {
        SceneManager.LoadScene("Game");
        SceneManager.UnloadSceneAsync($"Level{CurrentLevelIndex}");
        SceneManager.LoadScene($"Level{CurrentLevelIndex + 1}", LoadSceneMode.Additive); 
    }
[Button(ButtonSizes.Large)]
    public void LoadNewLevel()
    {
        Load();
        IncreaseLevelIndex();
        SaveLevelIndex(); 
    }
     

    private void IncreaseLevelIndex()
    { 
        CurrentLevelIndex += 1;

        if (CurrentLevelIndex + 1 >= SceneManager.sceneCountInBuildSettings)
            CurrentLevelIndex = 0;
    }

   

    private void SaveLevelIndex()
    {
        PlayerPrefs.SetInt(pref, CurrentLevelIndex);
    }

    private void LoadLevelIndex()
    {
        CurrentLevelIndex = PlayerPrefs.GetInt(pref, 0);
    }

    #region Editor
    private void DecreaseLevelIndex()
    {
        CurrentLevelIndex -= 1;
    }
    [Button(ButtonSizes.Large)]

    public void LoadPreviousLevel()
    {
        DecreaseLevelIndex();
        SaveLevelIndex();
        Load(); 
    }

    #endregion
}