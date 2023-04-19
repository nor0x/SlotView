#if !WINDOWS
using Microsoft.Maui.Graphics.Platform;
#endif
using System.Diagnostics;
using IImage = Microsoft.Maui.Graphics.IImage;

namespace SlotView.MAUI
{
    public class GraphicsSlotView : GraphicsView
    {
        int imageCount = 9;

        public static readonly BindableProperty ImagesProperty =
   BindableProperty.Create(nameof(Images), typeof(string[]), typeof(string[]), null, propertyChanged: (bindableObject, oldValue, newValue) =>
   {
       if (newValue is String[] images && bindableObject is GraphicsSlotView slotView)
       {
           slotView.LoadImages(images);
           slotView.Invalidate();
       }
   });



        private async void LoadImages(string[] sources)
        {
            Slot.ImageCount = sources.Length;
            foreach(var source in sources)
            {
                Debug.WriteLine("loading filename: " + source);
                using var stream = await ImageLoading.LoadImageStreamAsync(source, new CancellationToken());

                IImage image = null;
#if WINDOWS
                var service = new Microsoft.Maui.Graphics.Win2D.W2DImageLoadingService();
                image = service.FromStream(stream);

#else
                image = PlatformImage.FromStream(stream);
#endif
                if(Slot.Images is null) Slot.Images = new List<IImage>();
                Slot.Images.Add(image);
            }
        }

        public string[] Images
        {
            get => (string[])GetValue(ImagesProperty);
            set => SetValue(ImagesProperty, value);
        }


        private float scrollOffset;
        private bool isAnimating;
        private int stopIndex = -1;
        private float Speed = 15.0f;
        private float speed = 15.0f;
        private float speedDecrement = 0.01f;
        private float maxSpeedDecrement = 0.2f;
        private float speedDecrementIncrement = 0.01f;

        private int visibleCount = 3;

        protected SlotDrawable Slot { get; set; }

        public GraphicsSlotView()
        {
            Drawable = Slot = new SlotDrawable();
            Slot.Invalidate += Slot_Invalidate;
            Slot.Finished += Slot_Finished;
            Slot.Speed = Speed;
            SizeChanged += GraphicsSlotView_SizeChanged;

        }

        private void Slot_Finished()
        {

        }

        private void Slot_Invalidate()
        {
            Invalidate();
        }

        private void GraphicsSlotView_SizeChanged(object sender, EventArgs e)
        {
            if(Slot is null) return;
            if(Slot.Width != (float)Width || Slot.Height != (float)Height)
            {
                Slot.Width = (float)Width;
                Slot.Height = (float)Height;
                Invalidate();
            }
        }

        public void StartAnimation()
        {
            isAnimating = true;
            Slot.IsAnimating = true;
            Invalidate();
        }

        public void PauseAnimation()
        {
            isAnimating = false;
            Slot.IsAnimating = false;
        }

        public void StopAnimation(int index)
        {
            if (index >= Images.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            stopIndex = index;
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

        private int visibleCount = 3;

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
        public int StopIndex { get; set; } = -1;
        public bool IsAnimating { get; set; }
        public List<IImage> Images { get; set; }
        public int ImageCount { get; set; }
        public float Duration { get; set; }


        public float Width { get; set; }
        public float Height { get; set; }

        public SlotDrawable()
        {

        }

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            if (Images is null) return;
            if (Images.Count != ImageCount) return;
            // Draw the images that are visible on the canvas
            var imageHeight = Height / visibleCount;
            var numImagesVisible = (int)Math.Ceiling(Height / imageHeight) + 1;
            var imageWidth = imageHeight;

            for (var i = 0; i < numImagesVisible; i++)
            {
                var imageIndex = ((int)Math.Floor(scrollOffset / imageHeight) + i) % Images.Count;
                var imageY = i * imageHeight - (scrollOffset % imageHeight);
                var x = Width / 2 - imageWidth / 2;

                canvas.DrawImage(Images[imageIndex], x, imageY, imageWidth, imageHeight);
            }

            // Update the scroll offset if animating
            if (IsAnimating && StopIndex < 0)
            {
                scrollOffset += Height / 100 * _speed;
                scrollOffset %= imageHeight * Images.Count;


                Invalidate();
            }
            else if (IsAnimating && StopIndex >= 0)
            {
                var centerIndex = (int)Math.Floor(scrollOffset / imageHeight) + (int)Math.Ceiling(numImagesVisible / 2.0);
                Debug.WriteLine("CENTER INDEX: " + centerIndex);
                Debug.WriteLine("CENTER INDEX % COUNT: " + centerIndex % Images.Count);
                Debug.WriteLine("STOP INDEX: " + StopIndex);
                if (centerIndex % Images.Count == StopIndex && _speed <= 2.0f)
                {
                    StopIndex = -1;
                    _speed = Speed;
                    Finished();
                    Debug.WriteLine("STOPPED!");
                }
                else
                {
                    scrollOffset += Height / 100 * _speed;
                    scrollOffset %= imageHeight * Images.Count;

                    // Decrease the speed of the animation
                    //if(speed > 0 && speed <= 3.0) 
                    //{
                    //    Debug.WriteLine("INCREASE DECREMENT INCREMENT " + speedDecrementIncrement);
                    //    speedDecrementIncrement *= 1.05f;
                    //    speedDecrement += speedDecrementIncrement;
                    //    speedDecrement = Math.Min(speedDecrement, maxSpeedDecrement);

                    //    speed = Math.Max(1.5f, speed - speedDecrement);
                    //}
                    //else
                    if (_speed > 0)
                    {
                        speedDecrement += speedDecrementIncrement;
                        Debug.WriteLine("speed decrement: " + speedDecrement);
                        speedDecrement = Math.Min(speedDecrement, maxSpeedDecrement);

                        _speed = Math.Max(2.0f, _speed - speedDecrement);
                        Debug.WriteLine("speed: " + _speed);


                    }
                    speedDecrementIncrement *= 1.05f;


                    Invalidate();
                }
            }
        }
    }
}
