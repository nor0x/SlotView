namespace SlotView.Maui.Demo;

public partial class SlotMachinePage : ContentPage
{

	public SlotMachinePage()
	{
		InitializeComponent();
	}

	private void Button_Clicked(object sender, EventArgs e)
	{
		mySlotView0.StopAnimation(7);
		mySlotView1.StopAnimation(7);
		mySlotView2.StopAnimation(7);
	}
}


