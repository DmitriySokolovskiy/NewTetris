using UnityEngine;

[System.Serializable]
public class TetrisFigureParameters
{
    /// <summary> Префаб фигуры </summary>
    [SerializeField] private GameObject prefab;
    /// <summary> Шанс выпадения фигуры </summary>
    [SerializeField] private int spawnChance;

    public int SpawnChance 
    {
        get 
        {
            return spawnChance;
        } 
    }

    public GameObject Prefab 
    {
        get
        {
            return prefab;
        }
    }
}
