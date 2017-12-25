using System;
using System.IO;
using System.Windows;
using System.Windows.Forms;

namespace 文件同步助手 {
	/// <summary>
	/// TaskCreateWindow.xaml 的交互逻辑
	/// </summary>
	public partial class TaskCreateWindow : Window {

		public TaskCreateWindow () {
			InitializeComponent();
			DirectoryRadio.IsChecked = true;
		}

		/// <summary>
		/// 取消按钮
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void BtnCancel_Click ( object sender, RoutedEventArgs e ) {
			this.Close();
		}

		/// <summary>
		/// 确认按钮
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void BtnDone_Click ( object sender, RoutedEventArgs e ) {
			
			if (FileRadio.IsChecked == false && DirectoryRadio.IsChecked == false) {
				System.Windows.MessageBox.Show( "未选择类型!", "提示", MessageBoxButton.OK, MessageBoxImage.Exclamation );
				return;
			}
			if(LocalPath.Text=="未选择路径"||TargetPath.Text=="未选择路径") {
				System.Windows.MessageBox.Show( "未选择路径!", "提示", MessageBoxButton.OK, MessageBoxImage.Exclamation );
				return;
			}
			if(new DirectoryInfo(LocalPath.Text).Parent.FullName==TargetPath.Text) {
				System.Windows.MessageBox.Show( "路径重复，同步将导致意想不到的错误!", "警告", MessageBoxButton.OK, MessageBoxImage.Warning );
				return;
			}
			if (TaskNameBox.Text == "") {
				var res=System.Windows.MessageBox.Show( "将使用默认标题：" + new FileInfo( LocalPath.Text ).Name + "  是否确定？", "提示", MessageBoxButton.YesNo, MessageBoxImage.Asterisk );
				if(res==MessageBoxResult.No) {
					return;
				}
				TaskNameBox.Text = new FileInfo( LocalPath.Text ).Name;
			}
			
			MainWindow mw = (MainWindow)this.Owner;
			Control.SyncItem item = new Control.SyncItem( LocalPath.Text, TargetPath.Text, DirectoryRadio.IsChecked == true, TaskNameBox.Text );
			if (BackupSkip.IsChecked == true) {
				try {
					long min = Convert.ToInt32( MinBox.Text );
					min = (long)Math.Pow( min, 2 );
					long max = Convert.ToInt32( MaxBox.Text );
					max = (long)Math.Pow( max, 2 );
					item.Task_Item.SetBackupSkipSize( min, max );
				} catch {
					System.Windows.MessageBox.Show( "请输入正确的区间值！", "错误", MessageBoxButton.OK, MessageBoxImage.Error );
					return;
				}
			}
			item.Task_Item.SetBackupSkipState( AutoBackup.IsChecked == true, BackupSkip.IsChecked == true, AutoSync.IsChecked == true );
			mw.Sync_Page.ItemControl.AddToPanel(item );
			this.Close();
		}

		/// <summary>
		/// 路径1选择器
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void BtnLocalPath_Click ( object sender, RoutedEventArgs e ) {
			if(DirectoryRadio.IsChecked==true) {
				FolderBrowserDialog Dialog = new FolderBrowserDialog();
				var result = Dialog.ShowDialog();
				if(result==System.Windows.Forms.DialogResult.Cancel) {
					return;
				}
				LocalPath.Text = Dialog.SelectedPath.Trim();
			} else {
				var openDialog = new Microsoft.Win32.OpenFileDialog();
				var result = openDialog.ShowDialog();
				if (result == true) {
					LocalPath.Text = openDialog.FileName;
				}
			}
		}

		/// <summary>
		/// 路径2选择器
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void BtnTargetPath_Click ( object sender, RoutedEventArgs e ) {
			FolderBrowserDialog Dialog = new FolderBrowserDialog();
			var result = Dialog.ShowDialog();
			if (result == System.Windows.Forms.DialogResult.Cancel) {
				return;
			}
			TargetPath.Text = Dialog.SelectedPath.Trim();
		}

		/// <summary>
		/// 筛选按钮
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void BackupSkip_Click ( object sender, RoutedEventArgs e ) {
			if(BackupSkip.IsChecked==true) {
				MinBox.Visibility = Visibility.Visible;
				MaxBox.Visibility = Visibility.Visible;
				lab1.Visibility = Visibility.Visible;
				lab2.Visibility = Visibility.Visible;
			}else {
				MinBox.Visibility = Visibility.Hidden;
				MaxBox.Visibility = Visibility.Hidden;
				lab1.Visibility = Visibility.Hidden;
				lab2.Visibility = Visibility.Hidden;
			}
		}

		private void FileRadio_Checked ( object sender, RoutedEventArgs e ) {
			BackupSkip.Visibility = Visibility.Hidden;
			BackupSkip.IsChecked = false;
			MinBox.Visibility = Visibility.Hidden;
			MaxBox.Visibility = Visibility.Hidden;
			lab1.Visibility = Visibility.Hidden;
			lab2.Visibility = Visibility.Hidden;
		}

		private void DirectoryRadio_Checked ( object sender, RoutedEventArgs e ) {
			BackupSkip.Visibility = Visibility.Visible;
		}
	}
}
