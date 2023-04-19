using System;
using SkiaSharp;
using System.Diagnostics;
using SkiaSharp.Views.Maui.Controls;
using SkiaSharp.Views.Maui;

namespace SlotView.MAUI
{
    public class SlotView : SKCanvasView
    {
        int imageCount = 9;

        private async void LoadImages()
        {
            for (int i = 0; i < imageCount; i++)
            {
                var filename = $"octocat{i}.png";

                Debug.WriteLine("loading filename: " + filename);
                var stream = await ImageLoading.LoadImageStreamAsync(filename, new CancellationToken());
                images.Add(SKBitmap.Decode(stream));
            }
        }

        private readonly List<SKBitmap> images = new List<SKBitmap>();
        private float scrollOffset;
        private SKPaint paint = new SKPaint();
        private bool isAnimating;
        private int stopIndex = -1;
        private float speedInitial = 15.0f;
        private float speed = 15.0f;
        private float speedDecrement = 0.01f;
        private float maxSpeedDecrement = 0.2f;
        private float speedDecrementIncrement = 0.01f;

        private int visibleCount = 3;


        public SlotView()
        {
            paint.IsAntialias = true;
            paint.FilterQuality = SKFilterQuality.High;
            LoadImages();
        }

        public void AddImage(SKBitmap image)
        {
            images.Add(image);
        }

        public void StartAnimation()
        {
            isAnimating = true;
            InvalidateSurface();
        }

        public void PauseAnimation()
        {
            isAnimating = false;
        }

        public void StopAnimation(int index)
        {
            if (index >= images.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            stopIndex = index;
            StartAnimation();
        }

        protected override void OnPaintSurface(SKPaintSurfaceEventArgs args)
        {
            if (images.Count != imageCount) return;
            base.OnPaintSurface(args);

            var surface = args.Surface;
            var canvas = surface.Canvas;

            canvas.Clear(SKColors.White);

            var width = canvas.LocalClipBounds.Width;
            var height = canvas.LocalClipBounds.Height;

            // Draw the images that are visible on the canvas
            var imageHeight = height / visibleCount;
            var numImagesVisible = (int)Math.Ceiling(height / imageHeight) + 1;
            var imageWidth = imageHeight;

            for (var i = 0; i < numImagesVisible; i++)
            {
                var imageIndex = ((int)Math.Floor(scrollOffset / imageHeight) + i) % images.Count;
                var imageY = i * imageHeight - (scrollOffset % imageHeight);
                var x = width / 2 - imageWidth / 2;
                var imageRect = new SKRect(x, imageY, x + imageWidth, imageY + imageHeight);


                canvas.DrawBitmap(images[imageIndex], imageRect, paint);
            }

            // Update the scroll offset if animating
            if (isAnimating && stopIndex < 0)
            {
                scrollOffset += height / 100 * speed;
                scrollOffset %= imageHeight * images.Count;


                InvalidateSurface();
            }
            else if (isAnimating && stopIndex >= 0)
            {
                var centerIndex = (int)Math.Floor(scrollOffset / imageHeight) + (int)Math.Ceiling(numImagesVisible / 2.0);
                Debug.WriteLine("CENTER INDEX: " + centerIndex);
                Debug.WriteLine("CENTER INDEX % COUNT: " + centerIndex % images.Count);
                Debug.WriteLine("STOP INDEX: " + stopIndex);
                if (centerIndex % images.Count == stopIndex && speed <= 2.0f)
                {
                    stopIndex = -1;
                    speed = speedInitial;
                    PauseAnimation();
                    Debug.WriteLine("STOPPED!");
                }
                else
                {
                    scrollOffset += height / 100 * speed;
                    scrollOffset %= imageHeight * images.Count;

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
                    if (speed > 0)
                    {
                        speedDecrement += speedDecrementIncrement;
                        Debug.WriteLine("speed decrement: " + speedDecrement);
                        speedDecrement = Math.Min(speedDecrement, maxSpeedDecrement);

                        speed = Math.Max(2.0f, speed - speedDecrement);
                        Debug.WriteLine("speed: " + speed);


                    }
                    speedDecrementIncrement *= 1.05f;


                    InvalidateSurface();
                }
            }
        }
    }
}
