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

    private void mySlotView_Finished(object sender, EventArgs e)
    {

    }
}


