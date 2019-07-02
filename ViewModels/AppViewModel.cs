using Avalonia.Controls;
using DynamicData;
using LaughingLocale.Data;
using LaughingLocale.ViewModel.Menu;
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

namespace LaughingLocale.ViewModel
{
	public enum MenuID
	{
		File,
		Edit,
		Help
	}

	public class AppViewModel : ReactiveObject, IScreen
	{
		public AppSettings Settings { get; private set; } = new AppSettings();


		private RoutingState _router = new RoutingState();

		[DataMember]
		public RoutingState Router
		{
			get => _router;
			set => this.RaiseAndSetIfChanged(ref _router, value);
		}

		public SourceCache<MenuItemViewModel, int> Menu { get; private set; }

		public MenuItemViewModel GetMenu(MenuID id)
		{
			return Menu.Lookup((int)id).Value;
		}

		private MenuItemViewModel AddMenuEntry(MenuID menuID)
		{
			MenuItemViewModel menuItem = new MenuItemViewModel()
			{
				ID = (int)menuID,
				Header = menuID.ToString(),
			};
			Menu.AddOrUpdate(menuItem);
			return menuItem;
		}

		private readonly SourceList<string> _recentFiles = new SourceList<string>();
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

		public AppViewModel()
		{
			Menu = new SourceCache<MenuItemViewModel, int>(m => m.ID);

			OpenCommand = ReactiveCommand.CreateFromTask(Open);
			SaveCommand = ReactiveCommand.Create(Save);
			SaveAsCommand = ReactiveCommand.CreateFromTask(SaveAs);
			OpenRecentCommand = ReactiveCommand.Create<string>(OpenRecent);

			var fileMenu = AddMenuEntry(MenuID.File);

			var recentFiles = RecentFilesConnection.ToCollection().Select(items =>
			{
				List<MenuItemViewModel> list = new List<MenuItemViewModel>();
				foreach (var path in items)
				{
					list.Add(new MenuItemViewModel { Header = path, CommandParameter = path, Command = OpenRecentCommand });
				}
				return list;
			});

			fileMenu.Items = new List<MenuItemViewModel>()
			{
				new MenuItemViewModel { Header = "Open...", Command = OpenCommand},
				new MenuItemViewModel { Header = "Save", Command = SaveCommand},
				new MenuItemViewModel { Header = "Save As...", Command = SaveAsCommand},
				new MenuItemViewModel {
					Header = "Recent Files",
					Items = recentFiles.Wait()
				}
			};
		}
	}
}
