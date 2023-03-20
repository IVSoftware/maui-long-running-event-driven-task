namespace maui_long_running_event_driven_task;

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

public delegate void SecondChangedEventHandler(Object sender, SecondChangedEventArgs e);
public class SecondChangedEventArgs : EventArgs
{
    public SecondChangedEventArgs(DateTime timestamp)
    {
        Timestamp = timestamp;
    }
    public DateTime Timestamp { get; }
}
