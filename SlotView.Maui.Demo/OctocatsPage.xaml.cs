namespace SlotView.Maui.Demo;

public partial class OctocatsPage : ContentPage
{

    public OctocatsPage()
    {
        InitializeComponent();
        StopStepper.Minimum = -1;
        StopStepper.Maximum = mySlotView.Images.Count;

        VisibleCountStepper.Minimum = 1;
        VisibleCountStepper.Maximum = mySlotView.Images.Count;
        BackgroundColorEntry.Text = String.Format("#{0:X6}", new Random().Next(0x1000000));
        mySlotView.StopIndex = 0;
        mySlotView.MinimumSpeed = 4;
    }

    void StartButton_Clicked(System.Object sender, System.EventArgs e)
    {
        _ = mySlotView.StartAnimation();
        StatusLabel.Text = "Started";
    }

    void StopButton_Clicked(System.Object sender, System.EventArgs e)
    {
        mySlotView.StopAnimation(mySlotView.StopIndex);
        StatusLabel.Text = $"Stopping at {mySlotView.StopIndex}";
    }

    private void PauseButton_Clicked(object sender, EventArgs e)
    {
        mySlotView.PauseAnimation();
        StatusLabel.Text = "Paused";
    }

    private void mySlotView_Finished(object sender, EventArgs e)
    {
        StatusLabel.Text = "Finished";
    }

    private void SpeedSlider_ValueChanged(object sender, ValueChangedEventArgs e)
    {
        mySlotView.Speed = (float)e.NewValue;
    }

    private void DurationSlider_ValueChanged(object sender, ValueChangedEventArgs e)
    {
        mySlotView.Duration = (float)e.NewValue;
    }

    private void StopStepper_ValueChanged(object sender, ValueChangedEventArgs e)
    {
        mySlotView.StopIndex = (int)e.NewValue;
    }

    private void DelaySlider_ValueChanged(object sender, ValueChangedEventArgs e)
    {
        mySlotView.Delay = (float)e.NewValue;
    }

    private void DirectionPicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        mySlotView.Direction = (SlotDirection)DirectionPicker.SelectedIndex;
    }

    private void VisibleCountStepper_ValueChanged(object sender, ValueChangedEventArgs e)
    {
        mySlotView.VisibleCount = (int)e.NewValue;
    }


    private void SetBackgroundButton_Clicked(object sender, EventArgs e)
    {
        if (Color.TryParse(BackgroundColorEntry.Text, out var color))
        {
            mySlotView.BackgroundColor = color;
        }
        else
        {
            mySlotView.BackgroundColor = Color.FromArgb(String.Format("#{0:X6}", new Random().Next(0x1000000)));
        }
    }

    private void MinimumSpeedSlider_ValueChanged(object sender, ValueChangedEventArgs e)
    {
        mySlotView.MinimumSpeed = (float)e.NewValue;
    }

    private void DragSlider_ValueChanged(object sender, ValueChangedEventArgs e)
    {
        mySlotView.Drag = (float)e.NewValue;
    }

    private void DragThresholdSlider_ValueChanged(object sender, ValueChangedEventArgs e)
    {
        mySlotView.DragThreshold = (int)e.NewValue;
    }
}


