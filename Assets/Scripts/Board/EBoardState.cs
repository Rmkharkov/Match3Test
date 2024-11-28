namespace Board
{
    public enum EBoardState
    {
        Default,
        WaitForFill,
        CheckingGemsMatch,
        DestroyGems,
        DestroyBombs,
        AfterDestroy,
    }
}