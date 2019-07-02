using System;
using Avalonia;
using Avalonia.Logging.Serilog;
using DynamicData;
using LaughingLocale.ViewModels;
using LaughingLocale.Views;

namespace LaughingLocale
{
	class Program
	{
		// Initialization code. Don't use any Avalonia, third-party APIs or any
		// SynchronizationContext-reliant code before AppMain is called: things aren't initialized
		// yet and stuff might break.
		public static void Main(string[] args) => BuildAvaloniaApp().Start(AppMain, args);

		// Avalonia configuration, don't remove; also used by visual designer.
		public static AppBuilder BuildAvaloniaApp()
			=> AppBuilder.Configure<App>()
				.UsePlatformDetect()
				.LogToDebug()
				.UseReactiveUI();

		// Your application's entry point. Here you can initialize your MVVM framework, DI
		// container, etc.
		private static void AppMain(Application app, string[] args)
		{
			var window = new MainWindow
			{
				DataContext = new MainWindowViewModel(),
			};

			if(window.DataContext is MainWindowViewModel vm)
			{
				vm.AddMenuEntry(7, "Test");
				vm._recentFiles.Add("test.txt");
				vm._recentFiles.Add("test2.txt");
				vm._recentFiles.Add("test3.txt");
				vm._recentFiles.Add("test4.txt");
			}

			app.Run(window);
		}
	}
}
