using System;

public static class GameConfigs
{ 

}

public enum EScreen
{
    Lose,
    Win,
    Ingame,
    Home,
}

public enum ObjectColor
{
    None = -1,
    Blue = 0,
    OceanBlue = 1,
    Green = 2,
    Orange = 3,
    Pink = 4,
    Red = 5,
    Purple = 6,
    Yellow = 7,
    Grey = 8,
    Black = 9,
    Brown = 10,
}

public enum CarSize
{
    Small4Slots = 0,
    Medium6Slots = 1,
    Big10Slots = 2
}

public enum Direction
{
    Top = 0,
    TopRight = 1,
    Right = 2,
    DownRight = 3,
    Down = 4,
    DownLeft = 5,
    Left = 6,
    TopLeft = 7
}

public enum GuestMotion
{
    StandDown = 0,
    StandLeft = 1,
    SitTop = 2,
}

public enum CarArea
{
    OnMap = 0,
    InGarage = 1,
    InConveyorBelt = 2
}

public enum ParkingSpaceStatus
{
    Lock = 0,
    Empty = 1,
    Busy = 2
}

public enum GameplayState
{
    Pause = 0,
    Play = 1,
}

[Serializable]
public enum GameState
{
    None = -1,
    Ingame = 0,
    Home = 1,
}

public enum BoosterType
{
    SortingGuest = 0,
    ShuffleCar = 1,
    ParkingVip = 2,
    Magnet = 3,
}

public enum Difficulty
{
    Normal = 0,
    Hard = 1,
}

public enum BgTheme
{
    None = -1,
    Restaurant = 0,
    Airport = 1,
    Beach = 2,
}