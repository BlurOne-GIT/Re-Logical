using System;
using System.Collections.Generic;
using System.Linq;

public class Animation<T>
{
    #region Fields
    private T[] _frames;
    private int pos;
    private bool isLooped;
    public bool IsFinished { get => _frames.Length - 1 == pos; }
    #endregion

    public Animation(T[] frames, bool looped)
    {
        _frames = frames;
        isLooped = looped;
        pos = _frames.Length - 1;
    }

    public void Start() => pos = 0;

    public T NextFrame()
    {
        if (IsFinished)
        {
            if (isLooped)
                pos = 0;
            else
                return _frames[pos];
        }

        return _frames[pos++];
    }
}