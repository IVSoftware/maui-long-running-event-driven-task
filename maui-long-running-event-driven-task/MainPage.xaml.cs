using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace maui_long_running_event_driven_task;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        BindingContext = new MainPageBindingContext();
        InitializeComponent();
    }
}
class MainPageBindingContext : INotifyPropertyChanged
{
    public MainPageBindingContext()
    {
		var app = App.Current as App;
		app.SecondChanged += onAppSecondChanged;
	}

    private void onAppSecondChanged(object sender, SecondChangedEventArgs e)
    {
        Clock = e.Timestamp.ToString(@"hh\:mm\:ss");
    }
    string _clock = string.Empty;
    public string Clock
    {
        get => _clock;
        set
        {
            if (!Equals(_clock, value))
            {
                _clock = value;
                OnPropertyChanged();
            }
        }
    }
    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

