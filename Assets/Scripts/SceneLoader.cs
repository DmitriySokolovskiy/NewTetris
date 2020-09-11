using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    /// <summary>Кнопка пользовательского интерфейса, для загрузки сцены первого режима игры</summary>
    [SerializeField] private Button gameModeOne;
    /// <summary>Кнопка пользовательского интерфейса, для загрузки сцены второго режима игры</summary>
    [SerializeField] private Button gameModeTwo;

    void Start()
    {
       
        gameModeOne.onClick.AddListener( () =>
        {
            SceneManager.LoadScene("GameModeOne");
        
        });

        gameModeTwo.onClick.AddListener( () =>
        {
            SceneManager.LoadScene("GameModeTwo");

        });
    }
}
