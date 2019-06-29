using Avalonia;
using Avalonia.Markup.Xaml;

namespace LaughingLocaleGUI
{
	public class App : Application
	{
		public override void Initialize()
		{
			AvaloniaXamlLoader.Load(this);
		}
	}
}
