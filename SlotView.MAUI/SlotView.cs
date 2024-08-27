#if !WINDOWS
using Microsoft.Maui.Graphics.Platform;
using System;
#endif
using System.Collections.Concurrent;
using System.Threading.Tasks;
using IImage = Microsoft.Maui.Graphics.IImage;
using System.Collections;


namespace SlotView.Maui;

public class SlotView : GraphicsView
{
	public static readonly BindableProperty ImagesProperty =
		BindableProperty.Create(nameof(Images), typeof(IList), typeof(SlotView), default(IList), propertyChanged: async (bindableObject, oldValue, newValue) =>
		{
			if (newValue is String[] images && bindableObject is SlotView slotView)
			{
				await slotView.LoadImages(images);
				slotView.Invalidate();
			}
		});

	public static readonly BindableProperty SpeedProperty =
		BindableProperty.Create(nameof(Speed), typeof(float), typeof(SlotView), 15.0f, propertyChanged: (bindableObject, oldValue, newValue) =>
		{
			if (newValue is float speed && bindableObject is SlotView slotView)
			{
				slotView.Slot.Speed = speed;
				slotView.Invalidate();
			}
		});

	public static readonly BindableProperty MinimumSpeedProperty =
		BindableProperty.Create(nameof(MinimumSpeed), typeof(float), typeof(SlotView), 4.0f, propertyChanged: (bindableObject, oldValue, newValue) =>
		{
			if (newValue is float speed && bindableObject is SlotView slotView)
			{
				slotView.Slot.MinimumSpeed = speed;
				slotView.Invalidate();
			}
		});

	public static readonly BindableProperty DragProperty =
		BindableProperty.Create(nameof(Drag), typeof(float), typeof(SlotView), 0.01f, propertyChanged: (bindableObject, oldValue, newValue) =>
		{
			if (newValue is float drag && bindableObject is SlotView slotView)
			{
				slotView.Slot.Drag = drag;
				slotView.Invalidate();
			}
		});

	public static readonly BindableProperty DragThresholdProperty =
		BindableProperty.Create(nameof(DragThreshold), typeof(int), typeof(SlotView), 3, propertyChanged: (bindableObject, oldValue, newValue) =>
		{
			if (newValue is int dragthreshold && bindableObject is SlotView slotView)
			{
				slotView.Slot.DragThreshold = dragthreshold;
				slotView.Invalidate();
			}
		});

	public static readonly BindableProperty VisibleCountProperty =
		BindableProperty.Create(nameof(VisibleCount), typeof(int), typeof(SlotView), 3, propertyChanged: (bindableObject, oldValue, newValue) =>
		{
			if (newValue is int visibleCount && bindableObject is SlotView slotView)
			{
				slotView.Slot.VisibleCount = visibleCount;
				slotView.Invalidate();
			}
		});

	public static readonly BindableProperty DelayProperty =
		BindableProperty.Create(nameof(Delay), typeof(float), typeof(SlotView), 0.0f, propertyChanged: (bindableObject, oldValue, newValue) =>
		{
			if (newValue is float delay && bindableObject is SlotView slotView)
			{
				slotView.Slot.Delay = delay;
				slotView.Invalidate();
			}
		});

	public static readonly BindableProperty DurationProperty =
		BindableProperty.Create(nameof(Duration), typeof(float), typeof(SlotView), 1000f, propertyChanged: (bindableObject, oldValue, newValue) =>
		{
			if (newValue is float duration && bindableObject is SlotView slotView)
			{
				slotView.Slot.Duration = duration;
				slotView.Invalidate();
			}
		});

	public static readonly BindableProperty StopIndexProperty =
		BindableProperty.Create(nameof(StopIndex), typeof(int), typeof(SlotView), -1, propertyChanged: (bindableObject, oldValue, newValue) =>
		{
			if (newValue is int stopIndex && oldValue is int oldIndex && bindableObject is SlotView slotView)
			{
				slotView.Invalidate();
			}
		});

	public static readonly BindableProperty DirectionProperty =
		BindableProperty.Create(nameof(Direction), typeof(SlotDirection), typeof(SlotView), SlotDirection.Down, propertyChanged: (bindableObject, oldValue, newValue) =>
		{
			if (newValue is SlotDirection dir && bindableObject is SlotView slotView)
			{
				slotView.Slot.Direction = dir;
				slotView.Invalidate();
			}
		});

	public static readonly BindableProperty IsSpinningProperty =
		BindableProperty.Create(nameof(IsSpinning), typeof(bool), typeof(SlotView), false, propertyChanged: (bindableObject, oldValue, newValue) =>
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
								_ = slotView.StartAnimation();
							});
						}
					}
					else
					{
						_ = slotView.StartAnimation();
					}
				}
				else if (IsSpinning && WasSpinning)
				{
					slotView.PauseAnimation();
				}
				slotView.Invalidate();
			}
		});

	public static readonly new BindableProperty BackgroundColorProperty =
		BindableProperty.Create(nameof(BackgroundColor), typeof(Color), typeof(SlotView), null, propertyChanged: (bindableObject, oldValue, newValue) =>
		{
			if (newValue != null && bindableObject is SlotView slotView)
			{
				slotView.UpdateBackground();
			}
		});


	/// <summary>
	/// Gets or sets the background color of the SlotView.
	/// </summary>
	public new Color BackgroundColor
	{
		get => (Color)GetValue(BackgroundColorProperty);
		set => SetValue(BackgroundColorProperty, value);
	}

	/// <summary>
	/// Gets or sets the array of image paths to display in the SlotView.
	/// Works with both local and remote images.
	/// Works best with square images with equal width and height.
	/// </summary>
	public IList Images
	{
		get => (IList)GetValue(ImagesProperty);
		set => SetValue(ImagesProperty, value);
	}

	/// <summary>
	/// Gets or sets the speed of the image animation in the SlotView.
	/// </summary>
	public float Speed
	{
		get => (float)GetValue(SpeedProperty);
		set => SetValue(SpeedProperty, value);
	}

	/// <summary>
	/// Gets or sets the minimum speed that the animation can slow down to during deceleration.
	/// </summary>
	public float MinimumSpeed
	{
		get => (float)GetValue(MinimumSpeedProperty);
		set => SetValue(MinimumSpeedProperty, value);
	}

	/// <summary>
	/// Gets or sets the deceleration factor when the SlotView is slowing down.
	/// 
	/// </summary>
	public float Drag
	{
		get => (float)GetValue(DragProperty);
		set => SetValue(DragProperty, value);
	}

	/// <summary>
	/// Gets or sets the threshold distance that must be exceeded before the SlotView begins slowing down.
	/// </summary>
	public int DragThreshold
	{
		get => (int)GetValue(DragThresholdProperty);
		set => SetValue(DragThresholdProperty, value);
	}

	/// <summary>
	/// Gets or sets the number of images visible in the SlotView at once.
	/// Works best with an odd number of visible images.
	/// </summary>
	public int VisibleCount
	{
		get => (int)GetValue(VisibleCountProperty);
		set => SetValue(VisibleCountProperty, value);
	}

	/// <summary>
	/// Gets or sets the delay time before the SlotView starts animating.
	/// </summary>
	public float Delay
	{
		get => (float)GetValue(DelayProperty);
		set => SetValue(DelayProperty, value);
	}

	/// <summary>
	/// Gets or sets the duration of the image animation in the SlotView before it slows down.
	/// </summary>
	public float Duration
	{
		get => (float)GetValue(DurationProperty);
		set => SetValue(DurationProperty, value);
	}

	/// <summary>
	/// Gets or sets the index of the image to stop the animation on.
	/// -1 will keep the animation going indefinitely.
	/// </summary>
	public int StopIndex
	{
		get => (int)GetValue(StopIndexProperty);
		set => SetValue(StopIndexProperty, value);
	}

	/// <summary>
	/// Gets or sets a value indicating whether the SlotView is currently spinning.
	/// </summary>
	public bool IsSpinning
	{
		get => (bool)GetValue(IsSpinningProperty);
		set => SetValue(IsSpinningProperty, value);
	}

	/// <summary>
	/// Gets or sets the direction of the image animation in the SlotView.
	/// </summary>
	public SlotDirection Direction
	{
		get => (SlotDirection)GetValue(DirectionProperty);
		set => SetValue(DirectionProperty, value);
	}

	/// <summary>
	/// Invoked when the images have finished loading.
	/// </summary>
	public event EventHandler ImagesLoaded;

	/// <summary>
	/// Invoked when the image animation has started.
	/// </summary>
	public event EventHandler Started;

	/// <summary>
	/// Invoked when the image animation has been paused.
	/// </summary>
	public event EventHandler Paused;

	/// <summary>
	/// Invoked when the image animation has finished.
	/// </summary>
	public event EventHandler Finished;


	bool _isLoaded;

	protected SlotDrawable Slot { get; set; }

	public SlotView()
	{
		Drawable = Slot = new SlotDrawable();
		Slot.Invalidate += Slot_Invalidate;
		Slot.Finished += Slot_Finished;
		Slot.Speed = Speed;
		Slot.MinimumSpeed = MinimumSpeed;
		Slot.Drag = Drag;
		Slot.DragThreshold = DragThreshold;
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

	async Task LoadImages(String[] sources)
	{

		Slot.ImageCount = sources.Length;

		//create sources with indexes
		var indexedSources = new List<(int index, string source)>();
		for (int i = 0; i < sources.Length; i++)
		{
			indexedSources.Add((i, sources[i]));
		}


		var result = new IImage[sources.Length];

		if (Slot.Images is null) Slot.Images = new List<IImage>();
		ParallelOptions parallelOptions = new() { MaxDegreeOfParallelism = 3 };
#if WINDOWS
#endif
		await Parallel.ForEachAsync(indexedSources, parallelOptions, async (item, token) =>
		{
			using var stream = await ImageLoading.GetImageStream(item.source, token);
			IImage image = null;
#if WINDOWS
			var service = new Microsoft.Maui.Graphics.Win2D.W2DImageLoadingService();
			image = service.FromStream(stream);
#else
			image = PlatformImage.FromStream(stream);
#endif
			result[item.index] = image;
		});

		//check if every item the array is set
		if (!result.Any(x => x is null))
		{
			_isLoaded = true;
			Slot.Images = result.ToList();
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

	public async ValueTask StartAnimation()
	{
		if (!_isLoaded) return;
		if (IsSpinning) return;
		if (Delay > 0)
		{
			await Task.Delay(TimeSpan.FromMilliseconds(Delay)).ContinueWith((t) =>
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
		if (index >= Images.Count)
		{
			throw new ArgumentOutOfRangeException(nameof(index));
		}
		if (Delay > 0)
		{
			Task.Delay(TimeSpan.FromMilliseconds(Delay)).ContinueWith((t) =>
			{
				StopIndex = index;
				Slot.StopIndex = index;
				_ = StartAnimation();
			});
		}
		else
		{
			StopIndex = index;
			Slot.StopIndex = index;
			_ = StartAnimation();
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
