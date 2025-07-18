using System.Collections.Generic;
using UnityEngine;

public class FrozenBeamLauncher : Building
{
    protected readonly Dictionary<EManaLevel, int> TemperatureInfo = new()
    {
        { EManaLevel.One, 10 },
        { EManaLevel.Two, 20 },
        { EManaLevel.Three, 30 }
    };

    protected readonly List<(int, int)> Directions = new()
    {
        (0, 0), (0, 1), (1, 0), (0, -1), (-1, 0)
    };
    
    public override void OnCollision(EManaLevel manaLevel)
    {
        throw new System.NotImplementedException();
    }
}
