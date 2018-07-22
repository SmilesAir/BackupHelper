using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BackupHelper
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window, INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;
		protected void OnPropertyChanged(string name)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null)
			{
				handler(this, new PropertyChangedEventArgs(name));
			}
		}

		string eventName = "";
		public string EventName
		{
			get { return eventName; }
			set
			{
				eventName = value;
				OnPropertyChanged("EventName");
			}
		}
		string backupPath = "";
		public string BackupPath
		{
			get { return backupPath; }
			set
			{
				backupPath = value;
				OnPropertyChanged("BackupPath");
			}
		}
		string fpaHelperPath = "";
		public string FPAHelperPath
		{
			get { return fpaHelperPath; }
			set
			{
				fpaHelperPath = value;
				OnPropertyChanged("FPAHelperPath");
			}
		}
		string spreadsheetsPath = "";
		public string SpreadsheetsPath
		{
			get { return spreadsheetsPath; }
			set
			{
				spreadsheetsPath = value;
				OnPropertyChanged("SpreadsheetsPath");
			}
		}

		public MainWindow()
		{
			InitializeComponent();

			TopLevelGrid.DataContext = this;
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			EventName = Properties.Settings.Default.EventName;
			BackupPath = Properties.Settings.Default.BackupPath;
			FPAHelperPath = Properties.Settings.Default.FPAHelperPath;
			SpreadsheetsPath = Properties.Settings.Default.SpreadsheetsPath;
		}

		private void Window_Closing(object sender, CancelEventArgs e)
		{
			Properties.Settings.Default.EventName = EventName;
			Properties.Settings.Default.BackupPath = BackupPath;
			Properties.Settings.Default.FPAHelperPath = FPAHelperPath;
			Properties.Settings.Default.SpreadsheetsPath = SpreadsheetsPath;

			Properties.Settings.Default.Save();
		}

		private void Exit_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}

		private void BrowseBackupPath_Click(object sender, RoutedEventArgs e)
		{
			using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
			{
				if (BackupPath.Length == 0)
				{
					dialog.RootFolder = Environment.SpecialFolder.Desktop;
				}
				else
				{
					dialog.SelectedPath = BackupPath;
				}
				if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
				{
					BackupPath = dialog.SelectedPath;
				}
			}
		}

		private void BrowseFPAPath_Click(object sender, RoutedEventArgs e)
		{
			using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
			{
				if (FPAHelperPath.Length == 0)
				{
					dialog.RootFolder = Environment.SpecialFolder.Desktop;
				}
				else
				{
					dialog.SelectedPath = FPAHelperPath;
				}
				if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
				{
					FPAHelperPath = dialog.SelectedPath;
				}
			}
		}

		private void BrowseSpreadsheetsPath_Click(object sender, RoutedEventArgs e)
		{
			using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
			{
				if (SpreadsheetsPath.Length == 0)
				{
					dialog.RootFolder = Environment.SpecialFolder.Desktop;
				}
				else
				{
					dialog.SelectedPath = SpreadsheetsPath;
				}
				if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
				{
					SpreadsheetsPath = dialog.SelectedPath;
				}
			}
		}

		private void Backup_Click(object sender, RoutedEventArgs e)
		{

		}
	}
}
