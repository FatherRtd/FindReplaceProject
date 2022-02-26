using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;
using FindReplace.Annotations;
using GalaSoft.MvvmLight.Command;

namespace FindReplace
{
	class MainViewModel : INotifyPropertyChanged
	{
		private string _dir;
		private string _mask;
		private string _excludeMask;
		private string _findText;
		private string _replaceText;
		private bool _isUseRegEx;
		private bool _isIncludeSubDir;

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
		public bool IsUseRegEx
		{
			get { return _isUseRegEx; }
			set
			{
				_isUseRegEx = value; 
				OnPropertyChanged(nameof(IsUseRegEx));
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

		public ICommand FindCommand { get; }
		public ICommand ReplaceCommand { get; }


		public MainViewModel()
		{

		}

		public event PropertyChangedEventHandler PropertyChanged;

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
