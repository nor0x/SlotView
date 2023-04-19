namespace SlotView.MAUI;

public partial class MainPage : ContentPage
{

	public MainPage()
	{
		InitializeComponent();
	}

    void StartButton_Clicked(System.Object sender, System.EventArgs e)
    {
        mySlotView.StartAnimation();
    }

    void StopButton_Clicked(System.Object sender, System.EventArgs e)
    {
        mySlotView.StopAnimation(3);
    }

    private void PauseButton_Clicked(object sender, EventArgs e)
    {
        mySlotView.PauseAnimation();

    }

    private void GraphicsStartButton_Clicked(object sender, EventArgs e)
    {
        myGraphicsSlotView.StartAnimation();
    }

    private void GraphicsPauseButton_Clicked(object sender, EventArgs e)
    {
        myGraphicsSlotView.PauseAnimation();
    }

    private void GraphicsStopButton_Clicked(object sender, EventArgs e)
    {
        myGraphicsSlotView.StopAnimation(3);
    }
}


