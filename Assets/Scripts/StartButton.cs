using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartButton : MonoBehaviour
{
    [SerializeField]
    private Button startButton;

    /*private void Start()
    {
        startButton.onClick.AddListener(OnClickStart);
    }*/

    public void OnClickStart()
    {
        SceneManager.LoadScene(1);
    }

}
