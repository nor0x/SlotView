using System;
using System.ComponentModel;
using System.Diagnostics;

namespace SlotView.Maui.Demo;

public partial class AppShell : Shell, INotifyPropertyChanged
{
    public AppShell()
    {
        InitializeComponent();
    }

    LinearGradientBrush _headerBackground;
    float _headerRotation;
    public LinearGradientBrush HeaderBackground
    {
        get { return _headerBackground; }
        set
        {
            _headerBackground = value;
            OnPropertyChanged(nameof(HeaderBackground));
        }
    }

    public float HeaderRotation
    {
        get { return _headerRotation; }
        set
        {
            _headerRotation = value;
            OnPropertyChanged(nameof(HeaderRotation));
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
        HeaderRotation = new Random().Next(-20, 20);
        HeaderBackground = new LinearGradientBrush(new GradientStopCollection()
        {
            new GradientStop(Color.FromArgb(String.Format("#{0:X6}", new Random().Next(0x1000000))), 0),
            new GradientStop(Color.FromArgb(String.Format("#{0:X6}", new Random().Next(0x1000000))), 1)
        }, new Point(0, 1), new Point(1, 0));
    }
}

