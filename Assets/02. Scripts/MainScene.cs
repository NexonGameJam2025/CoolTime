using Core.Scripts;
using UnityEngine;

public class MainScene : BaseScene
{
    protected override void Awake()
    {
        base.Awake();

        Init();
    }
    
    public override void Init()
    {
        base.Init();
        
        Debug.Log("MainScene initialized.");
    }
}
