// https://stackoverflow.com/q/75778432/5438626

Your question **how to launch a long-running, event-driven task**. A clean, minimal example of this would be a clock updater that (without blocking the UI thread) samples the current time ~ 10x a second and fires an event when the second changes. This code snippet shows how one might launch this task in the `App` constructor for example. This general approach could be easily adapted to the Accelerometer monitoring you want your app to do.

	public partial class App : Application
	{
		public App()
		{
			InitializeComponent();

			MainPage = new AppShell();
			_ = Task.Run(() => longRunningEventDrivenTask(LongRunningTskCanceller.Token));
		}
		CancellationTokenSource LongRunningTskCanceller { get; } = new CancellationTokenSource();

		private async Task longRunningEventDrivenTask(CancellationToken token)
		{
			int prevSecond = -1;
			while(!LongRunningTskCanceller.IsCancellationRequested)
			{
				var now = DateTime.Now;
				if (now.Second != prevSecond)
				{
					OnSecondChanged(new SecondChangedEventArgs(timestamp: now));
				}
				await Task.Delay(TimeSpan.FromSeconds(0.1));
			}
		}
		public event SecondChangedEventHandler SecondChanged;
		protected virtual void OnSecondChanged(SecondChangedEventArgs e)
		{
			SecondChanged?.Invoke(this, e);
		}
	}

***
**Event**

	public delegate void SecondChangedEventHandler(Object sender, SecondChangedEventArgs e);
	public class SecondChangedEventArgs : EventArgs
	{
		public SecondChangedEventArgs(DateTime timestamp)
		{
			Timestamp = timestamp;
		}
		public DateTime Timestamp { get; }
	}

***
**Consume the event (Main Page)**

To show how the event might be connsumed, the `MainPage` subscribes to the `SecondChanged` event and the event handler modifies the bound `Clock` property.

XAML

	<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
				 xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
				 x:Class="maui_long_running_event_driven_task.MainPage">
		<ScrollView>
			<VerticalStackLayout
				Spacing="25"
				Padding="30,0"
				VerticalOptions="Center">
				<Image
					Source="dotnet_bot.png"
					SemanticProperties.Description="Cute dot net bot waving hi to you!"
					HeightRequest="200"
					HorizontalOptions="Center" />
				<Label
					Text="{Binding Clock}"
					SemanticProperties.HeadingLevel="Level1"
					FontSize="32"
					HorizontalOptions="Center" />
			</VerticalStackLayout>
		</ScrollView>
	</ContentPage>

C#
 
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