using UnityEngine;
using System;

public class GridUpdater : MonoBehaviour, IGridUpdater
{
    /// <summary> Ширина сетки игрового поля </summary>
    private const int GridWidth = 10;
    /// <summary> Высота сетки игрового поля </summary>
    private const int GridHeight = 20;
    /// <summary> Левая граница игрового поля </summary>
    private int leftBorder = 0;
    /// <summary> Нижняя граница игрового поля </summary>
    private int downBorder = 0;
    /// <summary> Необходимое для удаления число подряд заполненных строк </summary>
    private int numberOfRowsDel = 1;
    /// <summary> Массив, сохраняющий состояние объектов игрового поля </summary>
    private Transform[,] gridObjectsStatus = new Transform[GridWidth, GridHeight];

    public event Action onDeleteRowOccured;
    public event Action onDownCollisionOccured;

    public bool CheckPosition(Transform currentFigure, Vector3 move)
    {
        foreach (Transform child in currentFigure) 
        {
            Vector3 checkedPosition = child.position + move;

            if (!CheckIsInsideBorder(checkedPosition) || CheckIsFigureCollision(checkedPosition))
            {
                return false;
            }
        }
        
        return true;
    }

    public bool CheckRotation(Transform currentFigure, int angle)
    {
        foreach (Transform child in currentFigure)
        {
            Vector3 checkedPosition = child.position - currentFigure.position;

            checkedPosition = (Quaternion.AngleAxis(90, new Vector3(0, 0, 1)) * checkedPosition) + currentFigure.position;
 
            if (!CheckIsInsideBorder(checkedPosition) || CheckIsFigureCollision(checkedPosition))
            {
                return false;
            }

        }

        return true;
    }


    /// <summary>
    /// Производит проверку на нахождение игрового объекта внутри границ игрового поля
    /// </summary>
    /// <param name="position">Позиция проверяемой фигуры</param>
    /// <returns>Правда, если объект находится внутри игрового поля, ложь, если обьект находится вне игрового поля</returns>
    private bool CheckIsInsideBorder(Vector3 position)
    {
        position = RoundVector(position);

        return ((int)position.x >= leftBorder && (int)position.x < (GridWidth) && (int)position.y >= downBorder);
    }

    /// <summary>
    /// Производит проверку столкновения текущей фигуры с другой фигурой
    /// </summary>
    /// <param name="position">Позиция проверяемой фигуры</param>
    /// <returns>Парвда, если в проверяемой позиции присутствует другой объект, ложь если отсутствует</returns>
    private bool CheckIsFigureCollision(Vector3 position) 
    {
        position = RoundVector(position);

        return gridObjectsStatus[(int)position.x, (int)position.y] != null;
    }

    /// <summary>
    /// Производит запись игровой фигуры в массив состояния обьектов игрового поля
    /// </summary>
    /// <param name="currentFigure">Компонент "transform" текущей фигуры</param>
    private void UpdateGridObjectStatus(Transform currentFigure)
    {
        foreach (Transform child in currentFigure)
        {
            Vector3 childPos = RoundVector(child.position);

            gridObjectsStatus[(int)childPos.x, (int)childPos.y] = child;
        }
    }

    /// <summary>
    /// Обновляет состояние всех строк игрового поля
    /// </summary>
    private void UpdateRowsState()
    {
        for (int i = downBorder; i < GridHeight; ++ i)
        {
            if (IsRowsFull(i))
            {
                onDeleteRowOccured?.Invoke();
                DestroyRowObject(i);
                DownShiftRows(i);
                --i;
            }
        }
    }

    /// <summary>
    /// Проверяет на заполненность заданное количество строк (подряд)
    /// </summary>
    /// <param name="rowNumber">Номер проверяемой строки</param>
    /// <returns>Правда, если строки заполнены, ложь, если не заполнены </returns>
    private bool IsRowsFull(int rowNumber)
    {
        for (int i = leftBorder; i < GridWidth; ++ i)
        {
            for (int j = rowNumber; j < rowNumber + numberOfRowsDel; ++ j)
            {
                if (gridObjectsStatus[i, j] == null)
                {
                    return false;
                }
            }

        }

        return true;
    }

    /// <summary>
    /// Удаляет фигуры заполненных подряд  строк
    /// </summary>
    /// <param name="rowNumber">Номер первой заполненной строки</param>
    private void DestroyRowObject(int rowNumber)
    {        
        for (int i = leftBorder; i < GridWidth; ++ i)
        {
            for (int j = rowNumber; j < rowNumber + numberOfRowsDel; ++ j) 
            {
                Destroy(gridObjectsStatus[i, j].gameObject);

                gridObjectsStatus[i, j] = null;
            }                
        }
    }

    /// <summary>
    /// Производит сдвиг строк вниз на число удаленных строк
    /// </summary>
    /// <param name="rowNumber">Номер строки, выше которой необходимо осуществить сдвиг строк</param>
    private void DownShiftRows(int rowNumber)
    {
        for (int i = leftBorder; i < GridWidth; ++ i)
        {
            for (int j = rowNumber; j < GridHeight - numberOfRowsDel; ++ j)
            {
                if (gridObjectsStatus[i, j + numberOfRowsDel] != null)
                {
                    gridObjectsStatus[i, j] = gridObjectsStatus[i, j + numberOfRowsDel];
                    gridObjectsStatus[i, j + numberOfRowsDel] = null;
                    gridObjectsStatus[i, j].position += new Vector3(0, -1, 0) * numberOfRowsDel;
                }
            }
        }
    }

    public void HandleCollision(Transform currentFigure, Vector3 move) 
    {
        if (move.y < 0) 
        {
            UpdateGridObjectStatus(currentFigure);
            UpdateRowsState();
            onDownCollisionOccured?.Invoke();
        }
    }

    /// <summary>
    /// Округляет координаты вектора
    /// </summary>
    /// <param name="roundedVector"></param>
    /// <returns>Вектор с округленными координатами</returns>
    private Vector3 RoundVector(Vector3 roundedVector)
    {
        return new Vector3(Mathf.Round(roundedVector.x), Mathf.Round(roundedVector.y));
    }

}