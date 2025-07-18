using System.Collections.Generic;
using UnityEngine;

public class ColdShieldGenerator : Building
{
    protected readonly Dictionary<EManaLevel, int> TimerInfo = new()
    {
        { EManaLevel.One, 20 },
        { EManaLevel.Two, 30 },
        { EManaLevel.Three, 40 }
    };
    
    protected readonly Dictionary<EManaLevel, int> TemperatureInfo = new()
    {
        { EManaLevel.One, 7 },
        { EManaLevel.Two, 13 },
        { EManaLevel.Three, 18 }
    };
    
    protected override void OnCollision(EManaLevel manaLevel)
    {
        var timer = TimerInfo[manaLevel];
        var temperature = TemperatureInfo[manaLevel];

        switch (manaLevel)
        {
            case EManaLevel.One:
                foreach (var (x, y) in FourDirections)
                {
                    
                    _tileNodeSystem.TileNodeGrid[x, y].ApplyTemperature(temperature, timer);
                }
                break;
            
            case EManaLevel.Two:
            case EManaLevel.Three:
                foreach (var (x, y) in NineDirections)
                {
                    _tileNodeSystem.TileNodeGrid[x, y].ApplyTemperature(temperature, timer);
                }
                break;
            default:
                Debug.Log($"Invalid mana level: {manaLevel}");
                break;
        }
        
    }
}
