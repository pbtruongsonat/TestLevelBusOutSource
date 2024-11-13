using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SaveCollider : MonoBehaviour
{
    [SerializeField] private PolygonCollider2D polygonCollider;
    [SerializeField] private Direction direction;
    [SerializeField] private CarSize size;
    [SerializeField] private CarSO carSO;

    public void SaveMainCollider()
    {
        List<Vector2> points = polygonCollider.points.ToList();

        CarDirection dataDirection = carSO.GetCarDirection(direction);

        foreach (var colliderConfig in dataDirection.colliderConfigs)
        {
            if (colliderConfig.size == size)
            {
                colliderConfig.colliderPoints = points;
                return;
            }
        }
    }

    public void LoadMainCollider()
    {
        List<Vector2> points = new();

        CarDirection dataDirection = carSO.GetCarDirection(direction);

        foreach (var colliderConfig in dataDirection.colliderConfigs)
        {
            if (colliderConfig.size == size)
            {
                points = colliderConfig.colliderPoints;
                break;
            }
        }

        polygonCollider.points = points.ToArray();
    }

    public void SaveTouchCollider()
    {
        List<Vector2> points = polygonCollider.points.ToList();

        CarDirection dataDirection = carSO.GetCarDirection(direction);

        foreach (var colliderConfig in dataDirection.colliderConfigs)
        {
            if (colliderConfig.size == size)
            {
                colliderConfig.touchCollider = points;
                return;
            }
        }
    }

    public void LoadTouchCollider()
    {
        List<Vector2> points = new();

        CarDirection dataDirection = carSO.GetCarDirection(direction);

        foreach (var colliderConfig in dataDirection.colliderConfigs)
        {
            if (colliderConfig.size == size)
            {
                points = colliderConfig.touchCollider;
                break;
            }
        }

        polygonCollider.points = points.ToArray();
    }
}
