using Avalonia;
using Avalonia.Markup.Xaml;

namespace LaughingLocale
{
	public class App : Application
	{
		public override void Initialize()
		{
			AvaloniaXamlLoader.Load(this);
		}
	}
}
