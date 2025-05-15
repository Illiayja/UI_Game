using UnityEngine;
using UnityEngine.SceneManagement;

public class Settingsforbutton : MonoBehaviour
{

    public string SceneName;
    
    public void ChangeScene()
    {
        SceneManager.LoadScene(SceneName);
    }
}
