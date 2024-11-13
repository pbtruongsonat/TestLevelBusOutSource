using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CustomSO/CarSO", fileName = "CarSO")]
public class CarSO : ScriptableObject
{
    public List<CarDirection> carDirections;
    public List<CarInfoConfig> carConfigs;
    public List<CarSizeConfig> carSlots;
    public List<GarageLight> garageLights;

    public CarInfoConfig GetCarConfig(ObjectColor color, CarSize size)
    {
        var info = carConfigs.Find(i => i.color == color && i.size == size);
        return info;
    }

    public CarDirection GetCarDirection(Direction direction)
    {
        var info = carDirections.Find(i => i.direction == direction);
        return info;
    }

    public ColliderConfig GetColliderConfig(Direction direction, CarSize size)
    {
        if (direction == Direction.Right)
        {
            direction = Direction.Left;
        }
        else if (direction == Direction.TopRight)
        {
            direction = Direction.TopLeft;
        }
        else if (direction == Direction.DownRight)
        {
            direction = Direction.DownLeft;
        }

        var carDirection = GetCarDirection(direction);

        if (carDirection == null) return null;

        var colliderConfig = carDirection.colliderConfigs.Find(c => c.size == size);

        return colliderConfig;
    }

    public List<Vector2> GetCarCollider(Direction direction, CarSize size)
    {
        var colliderConfig = GetColliderConfig(direction, size);

        if (colliderConfig == null) return null;

        return colliderConfig.colliderPoints;
    }

    public CarSizeConfig GetCarSlot(CarSize size)
    {
        var info = carSlots.Find(i => i.size == size);
        return info;
    }

    public Vector2 GetOffsetArrow(Direction direction, CarSize size)
    {
        var carDirection = carDirections.Find(x => x.direction == direction);

        return carDirection.offsetArrows.Find(x => x.size == size).offset;
    }

    public Sprite GetGarageLightSprite(ObjectColor color)
    {
        var garageLight = garageLights.Find(x => x.color == color);

        if (garageLight != null)
        {
            return garageLight.lightSprite;
        }
        else
        {
            return null;
        }
    }
}

[Serializable]
public class CarDirection
{
    public Direction direction;
    public Sprite arrowSprite;
    public List<ColliderConfig> colliderConfigs;
    public List<OffsetArrow> offsetArrows;
}

[Serializable]
public class CarInfoConfig
{
    public ObjectColor color;
    public CarSize size;
    public RuntimeAnimatorController carAnimation;
}

[Serializable]
public class ColliderConfig
{
    public CarSize size;
    public List<Vector2> colliderPoints;
    public List<Vector2> touchCollider;
}

[Serializable]
public class CarSizeConfig
{
    public CarSize size;
    public List<Vector2> slotPositions;
}

[Serializable]
public class OffsetArrow
{
    public CarSize size;
    public Vector2 offset;
}