using System;
using System.ComponentModel;
using System.Diagnostics;

namespace SlotView.MAUI;

public partial class AppShell : Shell, INotifyPropertyChanged
{
	public AppShell()
	{
		InitializeComponent();
	}

    LinearGradientBrush _headerBackground;
    public LinearGradientBrush HeaderBackground
    {
        get { return _headerBackground; }
        set
        {
            _headerBackground = value;
            OnPropertyChanged(nameof(HeaderBackground));
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        Shell.Current.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(Shell.FlyoutIsPresented))
            {
                if (FlyoutIsPresented)
                {
                    RandomizeHeader();
                }
            }
        };
    }


    void RandomizeHeader()
    {
        HeaderBackground = new LinearGradientBrush(new GradientStopCollection()
        {
            new GradientStop(Color.FromHex(String.Format("#{0:X6}", new Random().Next(0x1000000))), 0),
            new GradientStop(Color.FromHex(String.Format("#{0:X6}", new Random().Next(0x1000000))), 1)
        }, new Point(0, 1), new Point(1, 0));
    }
}

