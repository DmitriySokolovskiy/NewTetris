using UnityEngine;
using UnityEngine.UI;


public class ScoreManager: MonoBehaviour
{
    /// <summary> Игровые очки </summary>
    private int score;
    /// <summary> Отображение игровых очков  </summary>
    [SerializeField] private Text textScore;

    [SerializeField] private GameObject gridUpdater;
    private IGridUpdater iGridUpdater;


    void Start()
    {
        iGridUpdater = gridUpdater.GetComponent<IGridUpdater>();
        iGridUpdater.onDeleteRowOccured += ScoreCalculate;
    }

    /// <summary>
    /// Производит подсчет очков
    /// </summary>
    public void ScoreCalculate() 
    {
        score++;
        textScore.text ="Очки: " + score;
    }
}
