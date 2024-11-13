using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GarageInfo
{
    public Vector2Serialized position;
    public Direction direction;
    public List<CarData> cars;
}

[Serializable]
public class GarageLight
{
    public ObjectColor color;
    public Sprite lightSprite;
}