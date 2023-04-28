#if !WINDOWS
using Microsoft.Maui.Graphics.Platform;
using System;
#endif
using System.Diagnostics;
using System.Windows.Input;
using IImage = Microsoft.Maui.Graphics.IImage;

namespace SlotView.MAUI
{
    public class SlotView : GraphicsView
    {
        public static readonly BindableProperty ImagesProperty =
            BindableProperty.Create(nameof(Images), typeof(string[]), typeof(string[]), null, propertyChanged: (bindableObject, oldValue, newValue) =>
            {
                if (newValue is String[] images && bindableObject is SlotView slotView)
                {
                    slotView.LoadImages(images);
                    slotView.Invalidate();
                }
            });

        public static readonly BindableProperty SpeedProperty =
            BindableProperty.Create(nameof(Speed), typeof(float), typeof(float), 15.0f, propertyChanged: (bindableObject, oldValue, newValue) =>
            {
                if (newValue is float speed && bindableObject is SlotView slotView)
                {
                    slotView.Slot.Speed = speed;
                    slotView.Invalidate();
                }
            });

        public static readonly BindableProperty VisibleCountProperty =
            BindableProperty.Create(nameof(VisibleCount), typeof(int), typeof(int), 3, propertyChanged: (bindableObject, oldValue, newValue) =>
            {
                if (newValue is int visibleCount && bindableObject is SlotView slotView)
                {
                    slotView.Slot.VisibleCount = visibleCount;
                    slotView.Invalidate();
                }
            });

        public static readonly BindableProperty DelayProperty =
            BindableProperty.Create(nameof(Delay), typeof(float), typeof(float), 0.0f, propertyChanged: (bindableObject, oldValue, newValue) =>
            {
                if (newValue is float delay && bindableObject is SlotView slotView)
                {
                    slotView.Slot.Delay = delay;
                    slotView.Invalidate();
                }
            });

        public static readonly BindableProperty DurationProperty =
            BindableProperty.Create(nameof(Duration), typeof(float), typeof(float), 1000f, propertyChanged: (bindableObject, oldValue, newValue) =>
            {
                if (newValue is float duration && bindableObject is SlotView slotView)
                {
                    slotView.Slot.Duration = duration;
                    slotView.Invalidate();
                }
            });
        public static readonly BindableProperty StopIndexProperty =
            BindableProperty.Create(nameof(StopIndex), typeof(int), typeof(int), -1, propertyChanged: (bindableObject, oldValue, newValue) =>
            {
                if (newValue is int stopIndex && oldValue is int oldIndex && bindableObject is SlotView slotView)
                {
                    slotView.Invalidate();
                }
            });

        public static readonly BindableProperty DirectionProperty =
            BindableProperty.Create(nameof(Direction), typeof(SlotDirection), typeof(SlotDirection), SlotDirection.Down, propertyChanged: (bindableObject, oldValue, newValue) =>
            {
                if (newValue is SlotDirection dir && bindableObject is SlotView slotView)
                {
                    slotView.Slot.Direction = dir;
                    slotView.Invalidate();
                }
            });
        public static readonly BindableProperty IsSpinningProperty =
            BindableProperty.Create(nameof(IsSpinning), typeof(bool), typeof(bool), false, propertyChanged: (bindableObject, oldValue, newValue) =>
            {
                if (newValue is bool IsSpinning && oldValue is bool WasSpinning && bindableObject is SlotView slotView)
                {
                    slotView.Slot.IsSpinning = IsSpinning;
                    if (IsSpinning && !WasSpinning)
                    {
                        if (slotView.Delay > 0)
                        {
                            {
                                Task.Delay(TimeSpan.FromMilliseconds(slotView.Delay)).ContinueWith((t) =>
                                {
                                    slotView.StartAnimation();
                                });
                            }
                        }
                        else
                        {
                            slotView.StartAnimation();
                        }
                    }
                    else if(IsSpinning && WasSpinning)
                    {
                        slotView.PauseAnimation();
                    }
                    slotView.Invalidate();
                }
            });

        public static readonly new BindableProperty BackgroundColorProperty =
    BindableProperty.Create(nameof(BackgroundColor), typeof(Color), typeof(SlotView), null,
        propertyChanged: (bindableObject, oldValue, newValue) =>
        {
            if (newValue != null && bindableObject is SlotView slotView)
            {
                slotView.UpdateBackground();
            }
        });

        public new Color BackgroundColor
        {
            get => (Color)GetValue(BackgroundColorProperty);
            set => SetValue(BackgroundColorProperty, value);
        }

        public string[] Images
        {
            get => (string[])GetValue(ImagesProperty);
            set => SetValue(ImagesProperty, value);
        }

        public float Speed
        {
            get => (float)GetValue(SpeedProperty);
            set => SetValue(SpeedProperty, value);
        }

        public int VisibleCount
        {
            get => (int)GetValue(VisibleCountProperty);
            set => SetValue(VisibleCountProperty, value);
        }

        public float Delay
        {
            get => (float)GetValue(DelayProperty);
            set => SetValue(DelayProperty, value);
        }        
        
        public float Duration
        {
            get => (float)GetValue(DurationProperty);
            set => SetValue(DurationProperty, value);
        }

        public int StopIndex
        {
            get => (int)GetValue(StopIndexProperty);
            set => SetValue(StopIndexProperty, value);
        }

        public bool IsSpinning
        {
            get => (bool)GetValue(IsSpinningProperty);
            set => SetValue(IsSpinningProperty, value);
        }

        public SlotDirection Direction
        {
            get => (SlotDirection)GetValue(DirectionProperty);
            set => SetValue(DirectionProperty, value);
        }

        public event EventHandler ImagesLoaded;
        public event EventHandler Started;
        public event EventHandler Paused;
        public event EventHandler Finished;

        bool _isLoaded;

        protected SlotDrawable Slot { get; set; }

        public SlotView()
        {
            Drawable = Slot = new SlotDrawable();
            Slot.Invalidate += Slot_Invalidate;
            Slot.Finished += Slot_Finished;
            Slot.Speed = Speed;
            Slot.VisibleCount = VisibleCount;
            Slot.Delay = Delay;
            Slot.Duration = Duration;

            SizeChanged += SlotView_SizeChanged;
            Background = new SolidColorBrush(Colors.Red);
        }

        private void Slot_Finished()
        {
            Finished?.Invoke(this, EventArgs.Empty);
            IsSpinning = false;
        }

        private void Slot_Invalidate()
        {
            Invalidate();
        }

        private void SlotView_SizeChanged(object sender, EventArgs e)
        {
            if (Slot is null) return;
            if (Slot.Width != (float)Width || Slot.Height != (float)Height)
            {
                Slot.Width = (float)Width;
                Slot.Height = (float)Height;
                Invalidate();
            }
        }

        async Task LoadImages(string[] sources)
        {
            Slot.ImageCount = sources.Length;
            foreach (var source in sources)
            {
                using var stream = await ImageLoading.LoadImageStreamAsync(source, new CancellationToken());

                IImage image = null;
#if WINDOWS
                var service = new Microsoft.Maui.Graphics.Win2D.W2DImageLoadingService();
                image = service.FromStream(stream);

#else
                image = PlatformImage.FromStream(stream);
#endif
                if (Slot.Images is null) Slot.Images = new List<IImage>();
                Slot.Images.Add(image);
            }
            _isLoaded = Slot.Images.Count == sources.Length;
            if (_isLoaded)
            {
                ImagesLoaded?.Invoke(this, EventArgs.Empty);
            }

            Slot.Invalidate();


        }

        void UpdateBackground()
        {
            if (Slot == null)
                return;

            var background = new SolidPaint { Color = BackgroundColor };
            Slot.BackgroundPaint = background;

            Invalidate();
        }

        public async void StartAnimation()
        {
            if (!_isLoaded) return;
            if (IsSpinning) return;
            if (Delay > 0)
            {
                Task.Delay(TimeSpan.FromMilliseconds(Delay)).ContinueWith((t) =>
                {

                    Slot.IsSpinning = true;
                    Slot.StopIndex = StopIndex;
                    Started?.Invoke(this, EventArgs.Empty);
                    Invalidate();
                    IsSpinning = true;
                });
            }
            else
            {
                Slot.IsSpinning = true;
                Slot.StopIndex = StopIndex;
                Started?.Invoke(this, EventArgs.Empty);
                Invalidate();
                IsSpinning = true;
            }

        }

        public void PauseAnimation()
        {
            if (!_isLoaded) return;
            if (IsSpinning != false)
            {
                IsSpinning = false;
                Slot.IsSpinning = false;
                Paused?.Invoke(this, EventArgs.Empty);
            }
        }

        public void StopAnimation(int index)
        {
            if (!_isLoaded) return;
            if (index >= Images.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }
            if (Delay > 0)
            {
                Task.Delay(TimeSpan.FromMilliseconds(Delay)).ContinueWith((t) =>
                {
                    StopIndex = index;
                    Slot.StopIndex = index;
                    StartAnimation();
                });
            }
            else
            {
                StopIndex = index;
                Slot.StopIndex = index;
                StartAnimation();
            }
        }
    }

    public class SlotDrawable : IDrawable
    {

        private float scrollOffset;
        private float maxSpeedDecrement = 0.2f;
        private float speedDecrementIncrement = 0.001f;
        private float minSpeed = 6.0f;
        public Paint BackgroundPaint { get; set; }

        public Action Invalidate { get; set; }
        public Action Finished { get; set; }
        public Action Paused { get; set; }

        float _speed;
        float _speedInitial;        
        
        float _drag;
        float _dragInitial = 0.01f;

        int _visibleCount = 3;
        DateTime _startTime = DateTime.MinValue;
        public float Speed
        {
            get => _speedInitial;
            set
            {
                _speedInitial = value;
                _speed = value;
            }
        }        
        
        
        public float Drag
        {
            get => _dragInitial;
            set
            {
                _dragInitial = value;
                _drag = value;
            }
        }

        public int DragThreshold { get; set; } = 3;


        public int VisibleCount
        {
            get => _visibleCount;
            set
            {
                _visibleCount = value;
            }
        }
        public int StopIndex { get; set; } = -1;
        public bool IsSpinning { get; set; }
        public List<IImage> Images { get; set; }
        public int ImageCount { get; set; }
        public float Delay { get; set; }
        public float Duration { get; set; }


        public float Width { get; set; }
        public float Height { get; set; }

        public SlotDirection Direction { get; set; }

        public SlotDrawable()
        {

        }

        bool _isSlowingDown = false;
        
        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            canvas.SetFillPaint(BackgroundPaint, dirtyRect);
            canvas.FillRectangle(dirtyRect);
            if (Images is null) return;
            if (Images.Count != ImageCount) return;

            var wh = Direction == SlotDirection.Up || Direction == SlotDirection.Down ? Height : Width;
            var imageSize = wh / VisibleCount;
            var numImagesVisible = (int)Math.Ceiling(Height / imageSize) + 1;

            for (var i = 0; i < numImagesVisible; i++)
            {
                var imageIndex = ((int)Math.Floor(scrollOffset / imageSize) + i) % Images.Count;

                float x = 0;
                if (Direction == SlotDirection.Left)
                {
                    x = i * imageSize - (scrollOffset % imageSize);
                }
                else if (Direction == SlotDirection.Right)
                {
                    x = Width - (i * imageSize) + (scrollOffset % imageSize) - imageSize;
                }
                else
                {
                    x = Width / 2 - imageSize / 2;
                }

                float y = 0;
                if (Direction == SlotDirection.Up)
                {
                    y = i * imageSize - (scrollOffset % imageSize);
                }
                else if (Direction == SlotDirection.Down)
                {
                    y = Height - (i * imageSize) + (scrollOffset % imageSize) - imageSize;
                }
                else
                {
                    y = Height / 2 - imageSize / 2;
                }

                canvas.DrawImage(Images[imageIndex], x, y, imageSize, imageSize);
            }

            if (IsSpinning && StopIndex < 0)
            {
                scrollOffset += wh / 100 * _speed;
                scrollOffset %= imageSize * Images.Count;

                Invalidate();
            }
            else if (IsSpinning && StopIndex >= 0)
            {
                if (_startTime == DateTime.MinValue)
                {
                    _startTime = DateTime.Now;
                }
                var centerIndex = (int)Math.Floor(scrollOffset / imageSize) + (int)Math.Ceiling(numImagesVisible / 2.0);
                centerIndex--;
                Debug.WriteLine("centerindex: " + centerIndex + " stopindex " + StopIndex);
                if (centerIndex % Images.Count == StopIndex && _speed <= minSpeed)
                {
                    StopIndex = -1;
                    _speed = Speed;
                    _startTime = DateTime.MinValue;
                    _isSlowingDown = false;
                    _drag = Drag;
                    speedDecrementIncrement = 0.001f;
                    Finished();
                }
                else
                {
                    scrollOffset += Height / 100 * _speed;
                    scrollOffset %= imageSize * Images.Count;

                    if (_speed > 0)
                    {
                        var check = DateTime.Now.Subtract(_startTime).TotalMilliseconds;
                        if (check > Delay + Duration)
                        {
                            _drag += speedDecrementIncrement;

                            _speed = Math.Max(minSpeed, _speed - _drag);

                            var distance = centerIndex - StopIndex;
                            if(!_isSlowingDown && distance == DragThreshold)
                            {
                                _isSlowingDown = true;
                            }
                            if(_isSlowingDown)
                            {
                                speedDecrementIncrement += 1.0f;
                            }
                            /*
                            Debug.WriteLine("distance: " + distance);
                            Debug.WriteLine("duration: " + Duration);
                            Debug.WriteLine("delay: " + Delay);
                            Debug.WriteLine("check: " + check);
                            Debug.WriteLine("speed: " + _speed);
                            Debug.WriteLine("drag: " + _drag);
                            Debug.WriteLine("speedDecrementIncrement: " + speedDecrementIncrement);
                            */
                        }
                    }
                    Invalidate();
                }
            }
        }
    }

    public enum SlotDirection
    {
        Up,
        Down,
        Left,
        Right
    }
}