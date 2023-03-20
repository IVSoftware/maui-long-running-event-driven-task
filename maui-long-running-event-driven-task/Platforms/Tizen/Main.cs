using System;
using Microsoft.Maui;
using Microsoft.Maui.Hosting;

namespace maui_long_running_event_driven_task;

class Program : MauiApplication
{
	protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

	static void Main(string[] args)
	{
		var app = new Program();
		app.Run(args);
	}
}
