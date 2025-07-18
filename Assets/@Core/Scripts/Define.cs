namespace Core.Scripts
{
    public static class Define
    {
        public enum ESceneType
        {
            MainScene
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