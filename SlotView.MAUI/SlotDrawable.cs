using IImage = Microsoft.Maui.Graphics.IImage;

namespace SlotView.Maui;

public class SlotDrawable : IDrawable
{
    float _scrollOffset;
    float _speedDecrementIncrement = 0.001f;
    float _speed;
    float _speedInitial;
    int _visibleCount = 3;
    float _drag;
    float _dragInitial = 0.01f;
    bool _isSlowingDown = false;
    DateTime _startTime = DateTime.MinValue;

    internal float Speed
    {
        get => _speedInitial;
        set
        {
            _speedInitial = value;
            _speed = value;
        }
    }

    internal float Drag
    {
        get => _dragInitial;
        set
        {
            _dragInitial = value;
            _drag = value;
        }
    }

    internal int VisibleCount
    {
        get => _visibleCount;
        set
        {
            _visibleCount = value;
        }
    }

    internal float MinimumSpeed { get; set; }
    internal int DragThreshold { get; set; }
    internal int StopIndex { get; set; } = -1;
    internal bool IsSpinning { get; set; }
    internal List<IImage> Images { get; set; }
    internal int ImageCount { get; set; }
    internal float Delay { get; set; }
    internal float Duration { get; set; }
    internal float Width { get; set; }
    internal float Height { get; set; }
    internal SlotDirection Direction { get; set; }

    internal Paint BackgroundPaint { get; set; }
    internal Action Invalidate { get; set; }
    internal Action Finished { get; set; }
    internal Action Paused { get; set; }

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
#if ANDROID
        canvas.ClipRectangle(dirtyRect);
#endif
        canvas.SetFillPaint(BackgroundPaint, dirtyRect);
        canvas.FillRectangle(dirtyRect);
        if (Images is null) return;
        if (Images.Count != ImageCount) return;

        var wh = Direction == SlotDirection.Up || Direction == SlotDirection.Down ? Height : Width;
        var imageSize = wh / VisibleCount;
        var numImagesVisible = (int)Math.Ceiling(Height / imageSize) + 1;

        for (var i = 0; i < numImagesVisible; i++)
        {
            var imageIndex = ((int)Math.Floor(_scrollOffset / imageSize) + i) % Images.Count;

            float x = 0;
            if (Direction == SlotDirection.Left)
            {
                x = i * imageSize - (_scrollOffset % imageSize);
            }
            else if (Direction == SlotDirection.Right)
            {
                x = Width - (i * imageSize) + (_scrollOffset % imageSize) - imageSize;
            }
            else
            {
                x = Width / 2 - imageSize / 2;
            }

            float y = 0;
            if (Direction == SlotDirection.Up)
            {
                y = i * imageSize - (_scrollOffset % imageSize);
            }
            else if (Direction == SlotDirection.Down)
            {
                y = Height - (i * imageSize) + (_scrollOffset % imageSize) - imageSize;
            }
            else
            {
                y = Height / 2 - imageSize / 2;
            }

            canvas.DrawImage(Images[imageIndex], x, y, imageSize, imageSize);
        }

        if (IsSpinning && StopIndex < 0)
        {
            _scrollOffset += wh / 100 * _speed;
            _scrollOffset %= imageSize * Images.Count;

            Invalidate();
        }
        else if (IsSpinning && StopIndex >= 0)
        {
            if (_startTime == DateTime.MinValue)
            {
                _startTime = DateTime.Now;
            }
            var centerIndex = (int)Math.Floor(_scrollOffset / imageSize) + (int)Math.Ceiling(numImagesVisible / 2.0);
            centerIndex--;
            if (centerIndex % Images.Count == StopIndex && _speed <= MinimumSpeed)
            {
                StopIndex = -1;
                _speed = Speed;
                _startTime = DateTime.MinValue;
                _isSlowingDown = false;
                _drag = Drag;
                _speedDecrementIncrement = 0.001f;
                Finished();
            }
            else
            {
                _scrollOffset += Height / 100 * _speed;
                _scrollOffset %= imageSize * Images.Count;

                if (_speed > 0)
                {
                    var check = DateTime.Now.Subtract(_startTime).TotalMilliseconds;
                    if (check > Delay + Duration)
                    {
                        _drag += _speedDecrementIncrement;

                        _speed = Math.Max(MinimumSpeed, _speed - _drag);

                        var distance = centerIndex - StopIndex;
                        if (!_isSlowingDown && distance == DragThreshold)
                        {
                            _isSlowingDown = true;
                        }
                        if (_isSlowingDown)
                        {
                            _speedDecrementIncrement *= 1.05f;
                        }
                    }
                }
            }
            Invalidate();
        }
    }
}