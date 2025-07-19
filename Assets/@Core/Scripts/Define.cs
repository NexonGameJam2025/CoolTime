namespace Core.Scripts
{
    public static class Define
    {
        public enum EBuildingType
        {
            None,
            ColdShieldGenerator,
            FrozenBeamLauncher,
            Wall,
        }
        public enum ESceneType
        {
            MainScene,
            PuzzleScene,
            PuzzleSceneJJM,
            
        }
        
        public enum ESoundType
        {
            BGM = 0,
            SFX,
            MAX,
        }

        public enum EHapticType
        {
            Light,
            Medium,
            Heavy
        }
    
        public enum EUIEvent
        {
            PointerDown,
            PointUp,
            Click,
            BeginDrag,
            Drag,
            Drop,
            EndDrag,
            PointEnter,
            PointExit,
        }
    }
}