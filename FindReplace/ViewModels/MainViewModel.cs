using System;
using System.Collections.Generic;
using FindReplace.Annotations;
using FindReplace.Models;
using FindReplace.Utils;
using GalaSoft.MvvmLight.Command;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FindReplace.ViewModels
{
	class MainViewModel : INotifyPropertyChanged
	{
		private string _dir;
		private string _mask;
		private string _excludeMask;
		private string _findText;
		private string _replaceText;
		private int _numberOfItems;
		private int _itemsInProcess;
		private bool _isIncludeSubDir;
		private bool _isInterfaceBlocked;
		private CancellationTokenSource cancellationSource;

		public string Dir
		{
			get { return _dir; }
			set
			{
				_dir = value;
				OnPropertyChanged(nameof(Dir));
			}
		}
		public string Mask
		{
			get { return _mask; }
			set
			{
				_mask = value;
				OnPropertyChanged(nameof(Mask));
			}
		}
		public string ExcludeMask
		{
			get { return _excludeMask; }
			set
			{
				_excludeMask = value; 
				OnPropertyChanged(nameof(ExcludeMask));
			}
		}
		public string FindText
		{
			get { return _findText; }
			set
			{
				_findText = value; 
				OnPropertyChanged(nameof(FindText));
			}
		}
		public string ReplaceText
		{
			get { return _replaceText; }
			set
			{
				_replaceText = value;
				OnPropertyChanged(nameof(ReplaceText));
			}
		}
		public int NumberOfItems
		{
			get { return _numberOfItems; }
			set
			{
				_numberOfItems = value; 
				OnPropertyChanged(nameof(NumberOfItems));
			}
		}
		public int ItemsInProcess
		{
			get { return _itemsInProcess; }
			set
			{
				_itemsInProcess = value; 
				OnPropertyChanged(nameof(ItemsInProcess));
			}
		}
		public bool IsIncludeSubDir
		{
			get { return _isIncludeSubDir; }
			set
			{
				_isIncludeSubDir = value;
				OnPropertyChanged(nameof(IsIncludeSubDir));
			}
		}

		public ObservableCollection<FileItem> FilesCollection { get; set; }
		public ObservableCollection<string> DebugStringCollection { get; set; }

		public ICommand FindCommand { get; }
		public ICommand ReplaceCommand { get; }
		public ICommand BrowseFilesCommand { get; }
		public ICommand CancelFindCommand { get; }


		public MainViewModel()
		{
			Mask = "*.*";
			IsIncludeSubDir = false;
			_isInterfaceBlocked = false;
			FilesCollection = new ObservableCollection<FileItem>();
			DebugStringCollection = new ObservableCollection<string>();

			BrowseFilesCommand = new RelayCommand(BrowseFiles, () => !_isInterfaceBlocked);

			FindCommand = new RelayCommand(() =>
			{
				cancellationSource = new CancellationTokenSource();
				Task.Run(Find,cancellationSource.Token).ContinueWith((x) => _isInterfaceBlocked = false);
			}, () => !_isInterfaceBlocked);

			ReplaceCommand = new RelayCommand(() =>
			{
				cancellationSource = new CancellationTokenSource();
				Task.Run(ReplaceTextInFiles, cancellationSource.Token).ContinueWith((x) => _isInterfaceBlocked = false);
			}, () => !_isInterfaceBlocked);

			CancelFindCommand = new RelayCommand(CancelFind);
		}

		public void BrowseFiles()
		{
			var dialog = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();
			var result = dialog.ShowDialog();
			if (result == true)
			{
				Dir = dialog.SelectedPath;
			}
		}

		public void Find()
		{
			_isInterfaceBlocked = true;
			App.Current.Dispatcher.Invoke(() =>
			{
				DebugStringCollection.Add($"Search started in directory: {Dir}.");
				FilesCollection.Clear();
			});
			ItemsInProcess = 0;

			List<string> files = new List<string>();
			
			if (!String.IsNullOrWhiteSpace(ExcludeMask))
			{
				try
				{
					files = FileFinder.FindFiles(Dir, Mask,ExcludeMask,
						IsIncludeSubDir ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
				}
				catch (Exception e)
				{
					App.Current.Dispatcher.Invoke(() => DebugStringCollection.Add($"Error: {e.Message}"));
					return;
				}
			}
			else
			{
				try
				{
					files = FileFinder.FindFiles(Dir, Mask,
						IsIncludeSubDir ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
				}
				catch (Exception e)
				{
					App.Current.Dispatcher.Invoke(() => DebugStringCollection.Add($"Error: {e.Message}"));
					return;
				}
			}

			NumberOfItems = files.Count;
			foreach (var file in files)
			{
				if (cancellationSource.Token.IsCancellationRequested)
				{
					App.Current.Dispatcher.Invoke(() => DebugStringCollection.Add($"Search canceled."));
					return;
				}

				int count;
				try
				{
					count = TextFinder.CountMatches(FindText, file);
				}
				catch (Exception e)
				{
					App.Current.Dispatcher.Invoke(() => DebugStringCollection.Add(e.Message));
					return;
				}

				if (count != 0)
				{
					App.Current.Dispatcher.Invoke(() =>
					{
						FilesCollection.Add(new FileItem(file.Substring(Dir.Length),count));
					});
				}

				ItemsInProcess++;
			}
			App.Current.Dispatcher.Invoke(() => DebugStringCollection.Add($"Search completed."));
		}

		public void ReplaceTextInFiles()
		{
			if (FilesCollection.Count <= 0)
			{
				App.Current.Dispatcher.Invoke(() => DebugStringCollection.Add("Missing files to replace."));
				return;
			}
			_isInterfaceBlocked = true;
			App.Current.Dispatcher.Invoke(() =>
			{
				DebugStringCollection.Add($"Replacement started.");
			});
			ItemsInProcess = 0;
			NumberOfItems = FilesCollection.Count;

			foreach (var fileItem in FilesCollection)
			{
				if (cancellationSource.Token.IsCancellationRequested)
				{
					App.Current.Dispatcher.Invoke(() => DebugStringCollection.Add($"Replace canceled."));
					return;
				}

				try
				{
					Replacer.Replace(Dir + fileItem.Path, FindText, ReplaceText);
				}
				catch (Exception e)
				{
					App.Current.Dispatcher.Invoke(() => DebugStringCollection.Add(e.Message));
					return;
				}

				ItemsInProcess++;
			}
			App.Current.Dispatcher.Invoke(() => DebugStringCollection.Add($"Replacement completed."));
		}

		public void CancelFind()
		{
			cancellationSource.Cancel();
			ItemsInProcess = 0;
			NumberOfItems = 0;
			_isInterfaceBlocked = false;
		}

		public event PropertyChangedEventHandler PropertyChanged;

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
