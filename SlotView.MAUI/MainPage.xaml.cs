namespace SlotView.MAUI;

public partial class MainPage : ContentPage
{
    List<string> letters;
    public MainPage()
    {
        InitializeComponent();
        letters = new List<string>()
        {
            "a_letter.png", "b_letter.png", "c_letter.png", "d_letter.png", "e_letter.png", "f_letter.png", "g_letter.png", "h_letter.png", "i_letter.png", "j_letter.png", "k_letter.png", "l_letter.png", "m_letter.png", "n_letter.png", "o_letter.png", "p_letter.png", "q_letter.png", "r_letter.png", "s_letter.png", "t_letter.png", "u_letter.png", "v_letter.png", "w_letter.png", "x_letter.png", "y_letter.png", "z_letter.png"

        };

        hSlot.Images = letters.Shuffle().ToArray();
        hSlot.StopIndex = hSlot.Images.ToList().IndexOf("h_letter.png");
        hSlot.Duration = 1000;

        eSlot.Images = letters.Shuffle().ToArray();
        eSlot.StopIndex = eSlot.Images.ToList().IndexOf("e_letter.png");
        eSlot.Duration = 1100;

        lSlot.Images = letters.Shuffle().ToArray();
        lSlot.StopIndex = lSlot.Images.ToList().IndexOf("l_letter.png");
        lSlot.Duration = 1200;

        l1Slot.Images = letters.Shuffle().ToArray();
        l1Slot.StopIndex = l1Slot.Images.ToList().IndexOf("l_letter.png");
        l1Slot.Duration = 1300;

        oSlot.Images = letters.Shuffle().ToArray();
        oSlot.StopIndex = oSlot.Images.ToList().IndexOf("o_letter.png");
        oSlot.Duration = 1400;

        wSlot.Images = letters.Shuffle().ToArray();
        wSlot.StopIndex = wSlot.Images.ToList().IndexOf("w_letter.png");
        wSlot.Duration = 1100;

        o1Slot.Images = letters.Shuffle().ToArray();
        o1Slot.StopIndex = o1Slot.Images.ToList().IndexOf("o_letter.png");
        o1Slot.Duration = 1200;

        rSlot.Images = letters.Shuffle().ToArray();
        rSlot.StopIndex = rSlot.Images.ToList().IndexOf("r_letter.png");
        rSlot.Duration = 1300;

        l2Slot.Images = letters.Shuffle().ToArray();
        l2Slot.StopIndex = l2Slot.Images.ToList().IndexOf("l_letter.png");
        l2Slot.Duration = 1400;

        dSlot.Images = letters.Shuffle().ToArray();
        dSlot.StopIndex = dSlot.Images.ToList().IndexOf("d_letter.png");
        dSlot.Duration = 1500;

    }


    private void Hello_Clicked(object sender, EventArgs e)
    {
        hSlot.StartAnimation();
        eSlot.StartAnimation();
        lSlot.StartAnimation();
        l1Slot.StartAnimation();
        oSlot.StartAnimation();
        wSlot.StartAnimation();
        o1Slot.StartAnimation();
        rSlot.StartAnimation();
        l2Slot.StartAnimation();
        dSlot.StartAnimation();
    }
}


