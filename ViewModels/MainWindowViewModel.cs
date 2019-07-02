using Avalonia.Controls;
using DynamicData;
using LaughingLocale.Data;
using LaughingLocale.ViewModels.Menu;
using ReactiveUI;
using System;
using System.Collections.Generic;
using Alphaleonis.Win32.Filesystem;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Collections.ObjectModel;
using DynamicData.Binding;

namespace LaughingLocale.ViewModels
{
	public enum MenuID
	{
		File,
		Edit,
		Help
	}

	public class MainWindowViewModel : HistoryViewModel, IScreenViewModel
	{
		public string Greeting => "Hello World!";

		public AppSettings Settings { get; private set; } = new AppSettings();

		private static SourceCache<MenuItemViewModel, int> testMenu()
		{
			var m = new SourceCache<MenuItemViewModel, int>(s => s.ID);
			m.AddOrUpdate(new MenuItemViewModel { Header = "File" });
			return m;
		}

		public SourceCache<MenuItemViewModel, int> Menu { get; set; }

		public ReadOnlyObservableCollection<MenuItemViewModel> TopMenu { get; set; }

		public IReadOnlyList<MenuItemViewModel> TestMenu { get; set; } = new [] { new MenuItemViewModel { Header = "File" } };

		public MenuItemViewModel GetMenu(MenuID id)
		{
			return Menu.Lookup((int)id).Value;
		}

		public MenuItemViewModel AddMenuEntry(MenuID menuID)
		{
			MenuItemViewModel menuItem = new MenuItemViewModel()
			{
				ID = (int)menuID,
				Header = menuID.ToString(),
			};
			Menu.AddOrUpdate(menuItem);
			return menuItem;
		}

		public MenuItemViewModel AddMenuEntry(int menuID, string header)
		{
			MenuItemViewModel menuItem = new MenuItemViewModel()
			{
				ID = menuID,
				Header = header,
			};
			Menu.AddOrUpdate(menuItem);
			return menuItem;
		}

		public SourceList<string> _recentFiles = new SourceList<string>();
		public IObservable<IChangeSet<string>> RecentFilesConnection => _recentFiles.Connect();

		private Window win;

		#region Commands
		public ReactiveCommand<Unit, Unit> OpenCommand { get; }
		public ReactiveCommand<Unit, Unit> SaveCommand { get; }
		public ReactiveCommand<Unit, Unit> SaveAsCommand { get; }
		public ReactiveCommand<string, Unit> OpenRecentCommand { get; }

		public List<FileDialogFilter> OpenFileFilters { get; set; } = CommonFilters.DefaultFilters;

		public async Task Open()
		{
			var dialog = new OpenFileDialog()
			{
				AllowMultiple = true,
				Title = "Open Localization Files...",
				Filters = OpenFileFilters,
				InitialDirectory = Settings.LastDirectory
			};
			var result = await dialog.ShowAsync(win);

			if (result != null)
			{
				foreach (var path in result)
				{
					System.Diagnostics.Debug.WriteLine($"Opened: {path}");
				}
			}
		}

		public void Save()
		{
			System.Diagnostics.Debug.WriteLine("Save");
		}

		public async Task SaveAs()
		{
			var dialog = new SaveFileDialog()
			{
				Title = "Open Localization Files...",
				Filters = OpenFileFilters,
				InitialDirectory = Settings.LastDirectory,
				InitialFileName = Path.GetFileNameWithoutExtension(Settings.LastFile)
			};
			var result = await dialog.ShowAsync(win);

			if (result != null)
			{
				System.Diagnostics.Debug.WriteLine($"Saving file to: {result}");
			}
		}

		public void OpenRecent(string path)
		{
			System.Diagnostics.Debug.WriteLine($"Open recent: {path}");
		}
		#endregion
		private readonly ReadOnlyObservableCollection<MenuItemViewModel> recentFiles;

		public MainWindowViewModel()
		{
			Menu = new SourceCache<MenuItemViewModel, int>(m => m.ID);

			OpenCommand = ReactiveCommand.CreateFromTask(Open);
			SaveCommand = ReactiveCommand.Create(Save);
			SaveAsCommand = ReactiveCommand.CreateFromTask(SaveAs);
			OpenRecentCommand = ReactiveCommand.Create<string>(OpenRecent);

			var fileMenu = AddMenuEntry(MenuID.File);

			var f = RecentFilesConnection.Transform(item =>
			{
				return new MenuItemViewModel { Header = item, CommandParameter = item, Command = OpenRecentCommand };
			}).Bind(out recentFiles).Subscribe();

			fileMenu.Items = new List<MenuItemViewModel>()
			{
				new MenuItemViewModel { Header = "Open...", Command = OpenCommand},
				new MenuItemViewModel { Header = "Save", Command = SaveCommand},
				new MenuItemViewModel { Header = "Save As...", Command = SaveAsCommand},
				new MenuItemViewModel {
					Header = "Recent Files",
					Items = recentFiles
				}
			};

			var editMenu = AddMenuEntry(MenuID.Edit);

			Notify("Menu");

			ReadOnlyObservableCollection<MenuItemViewModel> displayMenu;

			var cancellation = Menu.Connect().Bind(out displayMenu).DisposeMany().Subscribe();

			TopMenu = displayMenu;
		}
	}
}
