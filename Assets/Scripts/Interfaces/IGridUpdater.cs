using UnityEngine;
using System;

public interface IGridUpdater
{
    /// <summary>
    /// Производит проверку текущей позиции игрового обьекта
    /// </summary>
    /// <param name="currentFigure">компонент "transform" проверяемой фигуры </param>
    /// <param name="move">Вектор движения игровой фигуры</param>
    /// <returns>Ложь, если фигура после движения столкнулась с одной из границ игрового поля или другой фигурой, иначе, правда</returns>
    bool CheckPosition(Transform currentFigure, Vector3 moveVector);

    /// <summary>
    /// Производит проверку вращения игрового обьекта 
    /// </summary>
    /// <param name="currentFigure">Компонент "transform" проверяемой фигуры</param>
    /// <param name="angle">Угол поворота фигуры</param>
    /// <returns>Ложь, если фигура после вращения столкнулась с одной из границ игрового поля или другой фигурой. В остальных случаях правда</returns>
    bool CheckRotation(Transform currentFigure, int angle);

    /// <summary>
    /// Производит обработку столкновения игровой фигуры
    /// </summary>
    /// <param name="currentFigure">компнент "transform" текущей фигуры</param>
    /// <param name="move">вектор движения фигуры</param>
    void HandleCollision(Transform currentFigure, Vector3 moveStep);

    /// <summary>
    /// Событие происходящее при удалении заполненных строк
    /// </summary>
    event Action onDeleteRowOccured;

    /// <summary>
    /// Событие происходящее при столкновении, когда фигура движется вниз 
    /// </summary>
    event Action onDownCollisionOccured;


}
