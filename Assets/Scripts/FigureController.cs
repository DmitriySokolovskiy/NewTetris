using UnityEngine;
using System.Collections;

public class FigureController : MonoBehaviour
{
    /// <summary> Шаг фигуры </summary>
    [SerializeField] private int moveStep = 1;
    /// <summary> Скорость перемещения фигуры </summary>
    [SerializeField] private float speed = 5f;
    private IGridUpdater gridUpdater;
 
    void Start()
    {
        gridUpdater = GameObject.FindGameObjectWithTag("GridUpdater").GetComponent<IGridUpdater>();
        StartCoroutine(MoveFigureDown());
    }

    void Update()
    {
        Move();
    }
    
    /// <summary>
    /// Производит перемещение игровой фигуры, если нажата соответствующая кнопка, если позиция, в которую будет перемещна фигура, корректна.
    /// Если позиция не корректна запускается метод обработки столкновений.  
    /// </summary>
    public void Move() 
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow)) 
        {
            if (gridUpdater.CheckPosition(transform, Vector3Int.left * moveStep))
            {
                transform.position += Vector3Int.left * moveStep;
            }
            else
            {
                gridUpdater.HandleCollision(transform, Vector3Int.left * moveStep);
            }
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (gridUpdater.CheckPosition(transform, Vector3Int.right * moveStep))
            {            
                 transform.position += Vector3Int.right * moveStep;
            }
            else
            {
                gridUpdater.HandleCollision(transform, Vector3Int.right * moveStep);
            }
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            if (gridUpdater.CheckRotation(transform, 90))
            {
                transform.Rotate(0, 0, 90);
            }
        }

    }

    /// <summary>
    /// Производит движение фигуры вниз, если  позиция, в которую будет перемещена фигура, корректна, иначе запускается метод обработки столкновений 
    /// </summary>
    /// <returns></returns>
    IEnumerator MoveFigureDown() 
    {
        while (true) 
        {
            if (gridUpdater.CheckPosition(transform, Vector3Int.down * moveStep))
            {
                transform.position += Vector3Int.down * moveStep;
            }
            else
            {
                gridUpdater.HandleCollision(transform, Vector3Int.down * moveStep);
                StopAllCoroutines();
                enabled = false; 

            }

            yield return new WaitForSeconds(1/speed);
        }
    }
}