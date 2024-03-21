using System;

namespace Logical;

public struct Level
{
    #region Fields
    public Block[,] Blocks;
    public int Number;
    public int BallTime;
    public int Time;
    public bool IsTimed;
    public string Name;
    #endregion
}