namespace Logical;

public class Animation<T>
{
    #region Fields
    private readonly T[] _frames;
    private int _pos;
    private readonly bool _isLooped;
    public bool IsFinished => _frames.Length - 1 == _pos;

    #endregion

    public Animation(T[] frames, bool looped)
    {
        _frames = frames;
        _isLooped = looped;
        _pos = _frames.Length - 1;
    }

    public void Start() => _pos = 0;

    public T NextFrame()
    {
        if (!IsFinished) 
            return _frames[_pos++];
            
        if (_isLooped)
            _pos = 0;
        else
            return _frames[_pos];

        return _frames[_pos++];
    }
}