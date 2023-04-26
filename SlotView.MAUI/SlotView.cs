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
                    if (delay > 0)
                    {
                        Task.Delay(TimeSpan.FromMilliseconds(delay)).ContinueWith((t) =>
                        {
                            slotView.StopAnimation(slotView.StopIndex);
                        });
                    }

                    slotView.Invalidate();
                }
            });

        public static readonly BindableProperty StopIndexProperty =
            BindableProperty.Create(nameof(StopIndex), typeof(int), typeof(int), -1, propertyChanged: (bindableObject, oldValue, newValue) =>
            {
                if (newValue is int stopIndex && oldValue is int oldIndex && bindableObject is SlotView slotView)
                {
                    slotView.StopAnimation(stopIndex);
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

        public static readonly new BindableProperty BackgroundProperty =
            BindableProperty.Create(nameof(Background), typeof(Brush), typeof(SlotView), null,
                propertyChanged: (bindableObject, oldValue, newValue) =>
                {
                    if (newValue != null && bindableObject is SlotView slotView)
                    {
                        slotView.UpdateBackground();
                    }
                });

        public new Brush Background
        {
            get => (Brush)GetValue(BackgroundProperty);
            set => SetValue(BackgroundProperty, value);
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

            if (Background != null)
                Slot.BackgroundPaint = Background;
            else
            {
                var background = new SolidPaint { Color = BackgroundColor };
                Slot.BackgroundPaint = background;
            }

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

            StopIndex = index;
            Slot.StopIndex = index;
            StartAnimation();
        }
    }

    public class SlotDrawable : IDrawable
    {

        private float scrollOffset;
        private float speedDecrement = 0.01f;
        private float maxSpeedDecrement = 0.2f;
        private float speedDecrementIncrement = 0.01f;
        public Paint BackgroundPaint { get; set; }

        public Action Invalidate { get; set; }
        public Action Finished { get; set; }
        public Action Paused { get; set; }

        float _speed;
        float _speedInitial;
        public float Speed
        {
            get => _speedInitial;
            set
            {
                _speedInitial = value;
                _speed = value;
            }
        }
        public int VisibleCount { get; set; } = 3;
        public int StopIndex { get; set; } = -1;
        public bool IsSpinning { get; set; }
        public List<IImage> Images { get; set; }
        public int ImageCount { get; set; }
        public float Delay { get; set; }


        public float Width { get; set; }
        public float Height { get; set; }

        public SlotDrawable()
        {

        }

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            canvas.SetFillPaint(BackgroundPaint, dirtyRect);
            canvas.FillRectangle(dirtyRect);
            if (Images is null) return;
            if (Images.Count != ImageCount) return;

            var imageHeight = Height / VisibleCount;
            var numImagesVisible = (int)Math.Ceiling(Height / imageHeight) + 1;
            var imageWidth = imageHeight;

            for (var i = 0; i < numImagesVisible; i++)
            {
                var imageIndex = ((int)Math.Floor(scrollOffset / imageHeight) + i) % Images.Count;
                var imageY = i * imageHeight - (scrollOffset % imageHeight);
                var x = Width / 2 - imageWidth / 2;

                canvas.DrawImage(Images[imageIndex], x, imageY, imageWidth, imageHeight);
            }

            if (IsSpinning && StopIndex < 0)
            {
                scrollOffset += Height / 100 * _speed;
                scrollOffset %= imageHeight * Images.Count;


                Invalidate();
            }
            else if (IsSpinning && StopIndex >= 0)
            {
                var centerIndex = (int)Math.Floor(scrollOffset / imageHeight) + (int)Math.Ceiling(numImagesVisible / 2.0);
                centerIndex--;
                Debug.WriteLine("centerindex: " + centerIndex + " stopindex " + StopIndex);
                if (centerIndex % Images.Count == StopIndex && _speed <= 2.0f)
                {
                    StopIndex = -1;
                    _speed = Speed;
                    Finished();
                }
                else
                {
                    scrollOffset += Height / 100 * _speed;
                    scrollOffset %= imageHeight * Images.Count;

                    if (_speed > 0)
                    {
                        if (Math.Abs(centerIndex - StopIndex) < 3)
                        {
                            speedDecrement += speedDecrementIncrement;
                            speedDecrement = Math.Min(speedDecrement, maxSpeedDecrement);
                            _speed = Math.Max(2.0f, _speed - speedDecrement);

                            //speedDecrementIncrement *= 1.05f;
                        }
                    }


                    Invalidate();
                }
            }
        }
    }
}
