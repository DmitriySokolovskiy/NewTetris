using System;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    /// <summary> Параметры игровых фигур, необходимые для создания новой фигуры</summary>
    [SerializeField] private TetrisFigureParameters[] tetrisFiguresParameters;
    /// <summary> Начальная позиция игровой фигуры</summary>
    private Vector3 spawnPos = new Vector3(5, 17, 0);

    [SerializeField] private GameObject gridUpdater;
    private IGridUpdater iGridUpdater;

    void Awake()
    {
        CheckSumSpawnChance();
        iGridUpdater = gridUpdater.GetComponent<IGridUpdater>();
        iGridUpdater.onDownCollisionOccured += SpawnFigure;
    }
    void Start()
    {
        SpawnFigure();
    }

    /// <summary>
    /// Создает фигуру внутри игрового поля в заданной позиции с заданной вероятностью
    /// </summary>
    private void SpawnFigure()
    {
        int randomNumber = UnityEngine.Random.Range(1, 101); 

        int delta = 0;

        for (int i = 0; i < tetrisFiguresParameters.Length; i++)
        {
            if (delta < randomNumber && randomNumber <= tetrisFiguresParameters[i].SpawnChance + delta)
            {
                Instantiate(tetrisFiguresParameters[i].Prefab, spawnPos, Quaternion.identity);
            }

            delta += tetrisFiguresParameters[i].SpawnChance;

        }

    }

    /// <summary>
    /// Производит проверку суммы вероятностей выпадения всех фигур
    /// </summary>
    private void CheckSumSpawnChance()
    {
        int sumSpawnChance = 0;

        for (int i = 0; i < tetrisFiguresParameters.Length; i++)
        {
            sumSpawnChance += tetrisFiguresParameters[i].SpawnChance;
        }

        if (sumSpawnChance > 100)
        {
            throw new Exception("Вероятность выпадения всех фигур превышает 100%");
        }

    }
    
}
