using System;
using UnityEngine;

public class GridUpdaterModeTwo : MonoBehaviour, IGridUpdater
{
    /// <summary> Ширина сетки игрового поля </summary>
    private const int GridWidth = 12;
    /// <summary> Высота сетки игрового поля </summary>
    private const int GridHeight = 20;
    /// <summary> Левая граница игрового поля </summary>
    private int leftBorder = 0;
    /// <summary> Нижняя граница игрового поля </summary>
    private int downBorder = 0;
    /// <summary> Необходимое для удаления, число заполненных подряд  строк </summary>
    private int numberOfRowsDel = 2;
    /// <summary> Столкновения фигур, правда - произошло столкновение, ложь - не произошло </summary>
    private bool borderCollision = false;
    /// <summary> Массив сохраняющий состояние объектов игрового поля </summary>
    private Transform[,] gridObjectsArray = new Transform[GridWidth, GridHeight];

    public event Action onDeleteRowOccured;
    public event Action onDownCollisionOccured;

    public bool CheckPosition(Transform currentFigure, Vector3 move)
    {
        foreach (Transform child in currentFigure)
        {
            Vector3 checkedPosition = child.position + move;

            borderCollision = !CheckIsInsideBorder(checkedPosition);

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

            checkedPosition = (Quaternion.AngleAxis(angle, new Vector3(0, 0, 1)) * checkedPosition) + currentFigure.position;

            if (!CheckIsInsideBorder(checkedPosition) || gridObjectsArray[(int)checkedPosition.x, (int)checkedPosition.y] != null)
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
    /// <returns>Правда если объект находится внутри игрового поля, ложь если обьект находится вне игрового поля</returns>
    private bool CheckIsInsideBorder(Vector3 position)
    {
        position = RoundVector(position);

        return ((int)position.x >= leftBorder && (int)position.x < (GridWidth) && (int)position.y >= downBorder);
    }

    /// <summary>
    /// Производит проверку столкновения текущей фигуры с другой фигурой
    /// </summary>
    /// <param name="position">Позиция проверяемой фигуры</param>
    /// <returns>Правда, если в проверяемой позиции присутствует другой объект, ложь, если отсутствует</returns>
    private bool CheckIsFigureCollision(Vector3 position)
    {
        position = RoundVector(position);

        return gridObjectsArray[(int)position.x, (int)position.y] != null;
    }

    /// <summary>
    /// Производит запись игровой фигуры в массив состояния обьектов игрового поля
    /// </summary>
    /// <param name="currentFigure">компонент "transform" текущей фигуры</param>
    private void UpdateGridObjectState(Transform currentFigure)
    {
        foreach (Transform child in currentFigure)
        {
            Vector2 childPos = RoundVector(child.position);

            gridObjectsArray[(int)childPos.x, (int)childPos.y] = child;
        }
    }

    /// <summary>
    /// Обновляет состояние всех строк игрового поля
    /// </summary>
    private void UpdateRowsState()
    {
        for (int i = downBorder; i < GridHeight; ++ i)
        {
            if (IsRowFull(i))
            {
                onDeleteRowOccured?.Invoke();
                DestroyRowObject(i, gridObjectsArray);
                DecreaseRows(i, gridObjectsArray);
                --i;
            }
        }
    }

    /// <summary>
    /// Проверяет на заполненность заданное количество строк (подряд)
    /// </summary>
    /// <param name="rowNumber">Номер проверяемой строки</param>
    /// <returns>Правда, если строки заполнены, ложь, если не заполнены </returns>
    private bool IsRowFull(int rowNumber)
    {
        for (int i = leftBorder; i < GridWidth; ++ i)
        {
            for (int j = rowNumber; j < (rowNumber + numberOfRowsDel); ++ j)
            {
                if (gridObjectsArray[i, j] == null)
                {
                    return false;
                }
            }
        }

        return true;
    }

    /// <summary>
    /// Удаляет фигуры заполненных подряд строк
    /// </summary>
    /// <param name="rowNumber">Номер первой заполненной строки</param>
    private void DestroyRowObject(int rowNumber, Transform[,] gridObjectsArray)
    {
        for (int i = leftBorder; i < GridWidth; ++ i)
        {
            for (int j = rowNumber; j < rowNumber + numberOfRowsDel; ++ j)
            {
                Destroy(gridObjectsArray[i, j].gameObject);

                gridObjectsArray[i, j] = null;
            }
        }
    }

    /// <summary>
    /// Производит сдвиг строк вниз на число удаленных строк
    /// </summary>
    /// <param name="rowNumber">Номер строки выше, которой необходимо осуществить сдвиг строк</param>
    private void DecreaseRows(int rowNumber, Transform[,] gridObjectsArray)
    {
        for (int i = leftBorder; i < GridWidth; ++ i)
        {
            for (int j = rowNumber; j < GridHeight - numberOfRowsDel; ++ j)
            {
                if (gridObjectsArray[i, j + numberOfRowsDel] != null) 
                {
                    gridObjectsArray[i, j] = gridObjectsArray[i, j + numberOfRowsDel];
                    gridObjectsArray[i, j + numberOfRowsDel] = null;
                    gridObjectsArray[i, j].position += new Vector3(0, -1, 0) * numberOfRowsDel;
                }
            }
        }
    }
    
    public void HandleCollision(Transform currentFigure, Vector3 move)
    {

        if (move.x > 0 || move.x < 0)
        {
            if (borderCollision)
            {
                TransferFigureAcrossBorder(currentFigure, move);
            }
        }
        else if (move.y < 0)
        {
            UpdateGridObjectState(currentFigure);
            UpdateRowsState();
            onDownCollisionOccured?.Invoke();
        }
    }

    /// <summary>
    /// Переносит фигуру к противоложной границе игрового поля
    /// </summary>
    /// <param name="currentFigure">Компонент "transform" текущей фигуры</param>
    /// <param name="move">Вектор движения игровой фигуры</param>
    private void TransferFigureAcrossBorder(Transform currentFigure, Vector3 move) 
    {
        foreach (Transform child in currentFigure)
        {
            Vector3 checkedPosChild = child.position + move;

            Vector3 checkedPosParent = currentFigure.position + move;

            if (!CheckIsInsideBorder(checkedPosChild))
            {
                checkedPosChild = child.position - move * GridWidth + move;

                if (!CheckIsFigureCollision(checkedPosChild))
                {
                    child.position = checkedPosChild - move;
                }
                else
                {
                    return;
                }

            }

            if (!CheckIsInsideBorder(checkedPosParent))
            {
                currentFigure.position -= move * GridWidth;

                foreach (Transform child2 in currentFigure)
                {
                    child2.position += move * GridWidth;
                }
            }
        }

        currentFigure.position += move;
    }

    private Vector3 RoundVector(Vector3 position)
    {
        return new Vector3(Mathf.Round(position.x), Mathf.Round(position.y));
    }
}