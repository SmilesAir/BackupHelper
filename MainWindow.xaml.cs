using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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
		string backupTag = "";
		public string BackupTag
		{
			get { return backupTag; }
			set
			{
				backupTag = value;
				OnPropertyChanged("BackupTag");
			}
		}

		string outputText = "";
		public string OutputText
		{
			get { return outputText; }
			set
			{
				outputText = value;
				OnPropertyChanged("OutputText");
			}
		}
		Brush outputBgColor = Brushes.Transparent;
		public Brush OutputBgColor
		{
			get { return outputBgColor; }
			set
			{
				outputBgColor = value;
				OnPropertyChanged("OutputBgColor");
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
			BackupTag = Properties.Settings.Default.BackupTag;
		}

		private void Window_Closing(object sender, CancelEventArgs e)
		{
			Properties.Settings.Default.EventName = EventName;
			Properties.Settings.Default.BackupPath = BackupPath;
			Properties.Settings.Default.FPAHelperPath = FPAHelperPath;
			Properties.Settings.Default.SpreadsheetsPath = SpreadsheetsPath;
			Properties.Settings.Default.BackupTag = BackupTag;

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
			OutputText = "";

			bool bError = false;
			if (EventName.Length == 0)
			{
				bError = true;
				AppendOutputText("Error: Need to set an Event Name!", Brushes.Salmon);
			}

			if (BackupPath.Length == 0)
			{
				bError = true;
				AppendOutputText("Error: Need to set an Backup Path!", Brushes.Salmon);
			}
			//else if (!Uri.IsWellFormedUriString(BackupPath, UriKind.Absolute))
			//{
			//	bError = true;
			//	AppendOutputText("Error: Backup Path is not valid!", Brushes.Salmon);
			//}

			if (bError)
			{
				return;
			}

			string eventPath = System.IO.Path.Combine(BackupPath, EventName);
			Directory.CreateDirectory(eventPath);

			string newBackupPath = System.IO.Path.Combine(BackupPath, EventName, BackupTag + DateTime.Now.ToString(" yyyy-dd-M--HH-mm-ss"));
			Directory.CreateDirectory(newBackupPath);

			bError |= !BackupFile(newBackupPath, FPAHelperPath, "names.xml");
			bError |= !BackupFile(newBackupPath, FPAHelperPath, "save.xml");
			bError |= !BackupFolder(System.IO.Path.Combine(newBackupPath, "Spreadsheets"), SpreadsheetsPath);

			if (!bError)
			{
				AppendOutputText("Succefully Finished Backup", Brushes.LightGreen);
			}
			else
			{
				AppendOutputText("There were errors trying to backup!", Brushes.Salmon);
			}
		}

		private bool BackupFile(string newBackupPath, string sourceFolder, string sourceFilename)
		{
			string sourceFilePath = System.IO.Path.Combine(sourceFolder, sourceFilename);
			string targetFilePath = System.IO.Path.Combine(newBackupPath, sourceFilename);
			if (!TryCopyFile(sourceFilePath, targetFilePath))
			{
				AppendOutputText("Error: Couldn't copy: " + sourceFilePath, Brushes.Salmon);

				return false;
			}

			AppendOutputText("Copied: " + sourceFilePath);

			return true;
		}

		private bool BackupFolder(string newBackupPath, string sourceFolder)
		{
			Directory.CreateDirectory(newBackupPath);

			return TryCopyFolder(sourceFolder, newBackupPath);
		}

		private bool TryCopyFile(string sourceFilePath, string targetFolderPath)
		{
			if (!File.Exists(sourceFilePath))
			{
				AppendOutputText("Error: Can't find Source: " + sourceFilePath, Brushes.Salmon);
				return false;
			}

			if (File.Exists(targetFolderPath))
			{
				AppendOutputText("Error: Target already exists: " + sourceFilePath, Brushes.Salmon);
				return false;
			}

			try
			{
				File.Copy(sourceFilePath, targetFolderPath);
			}
			catch (Exception e)
			{
				AppendOutputText("Error: " + e.Message, Brushes.Salmon);
			}

			return true;
		}

		private bool TryCopyFolder(string sourcePath, string targetPath)
		{
			if (!Directory.Exists(sourcePath))
			{
				AppendOutputText("Error: Can't find Source: " + sourcePath, Brushes.Salmon);
				return false;
			}

			if (!Directory.Exists(targetPath))
			{
				AppendOutputText("Error: Can't find Target: " + targetPath, Brushes.Salmon);
				return false;
			}

			Copy(sourcePath, targetPath);

			return true;
		}

		public void Copy(string sourceDirectory, string targetDirectory)
		{
			DirectoryInfo diSource = new DirectoryInfo(sourceDirectory);
			DirectoryInfo diTarget = new DirectoryInfo(targetDirectory);

			CopyAll(diSource, diTarget);
		}

		public void CopyAll(DirectoryInfo source, DirectoryInfo target)
		{
			Directory.CreateDirectory(target.FullName);

			// Copy each file into the new directory.
			foreach (FileInfo fi in source.GetFiles())
			{
				fi.CopyTo(System.IO.Path.Combine(target.FullName, fi.Name), true);

				AppendOutputText("Copied: " + fi.Name);
			}

			// Copy each subdirectory using recursion.
			foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
			{
				DirectoryInfo nextTargetSubDir =
					target.CreateSubdirectory(diSourceSubDir.Name);
				CopyAll(diSourceSubDir, nextTargetSubDir);
			}
		}

		private void AppendOutputText(string text)
		{
			OutputText += text + "\r\n";

			OutputTextbox.ScrollToEnd();
		}

		private void AppendOutputText(string text, Brush bgColor)
		{
			OutputText += text + "\r\n";
			OutputBgColor = bgColor;

			OutputTextbox.ScrollToEnd();
		}
	}
}
