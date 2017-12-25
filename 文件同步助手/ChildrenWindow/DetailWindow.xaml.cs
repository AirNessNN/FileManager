using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using 文件同步助手.Control;
using 文件同步助手.Logic;

namespace 文件同步助手.ChildrenWindow {

	/// <summary>
	/// DetailWindow.xaml 的交互逻辑
	/// </summary>
	public partial class DetailWindow : Window {

		#region 属性字段
		/// <summary>
		/// 控制器
		/// </summary>
		private SyncItem ItemControl;
		/// <summary>
		/// 是否编辑过选项
		/// </summary>
		private bool IsEdited { get; set; }
		#endregion



		public DetailWindow (SyncItem syncItem) {
			ItemControl = syncItem;
			InitializeComponent();
			//Item的属性
			TaskNameLab.Content = syncItem.ItemName;
			TitleBox.Text = syncItem.ItemName;
			TypeLab.Content = syncItem.IsDirectory ? "文件夹" : "文件";
			TimeBlock.Text = syncItem.Time.ToShortDateString();
			SizeBlock.Text = syncItem.SizeLab.Text;
			LocalPathBlock.Text = syncItem.LocalPath;
			TargetPathBlock.Text = syncItem.TargetPath;
			//Task属性
			AutoBackup.IsChecked = syncItem.IsAutoBackup;
			AutoSync.IsChecked = syncItem.Task_Item.AutoSync;
			SkipItem.IsChecked = syncItem.Task_Item.IsSkipItem;
			MinSizeBox.Text = syncItem.SkipMinSize.ToString();
			MaxSizeBox.Text = syncItem.SkipMaxSize.ToString();
			BackupFiltrate.IsChecked = syncItem.IsBackupFiltrate;
			//分别处理文件和文件夹情况
			if (syncItem.IsDirectory) {
                //文件夹模式
				NumBlock.Text = syncItem.NumLab.Text;
				BackupFiltrate.IsChecked = syncItem.Task_Item.BackupFiltrate;
				if(BackupFiltrate.IsChecked!=true) {
					textBlock.Visibility = Visibility.Hidden;
					textBlock1.Visibility = Visibility.Hidden;
					textBlock2.Visibility = Visibility.Hidden;
					textBlock3.Visibility = Visibility.Hidden;
					MaxSizeBox.Visibility = Visibility.Hidden;
					MinSizeBox.Visibility = Visibility.Hidden;
				}
                //填充列表
                new Thread(() => {
                   try {
                        List<FileInfo> tmpList = ApplicationResoure.GetFileList(syncItem.LocalPath);
                        Dispatcher.Invoke(() => {
                            foreach (FileInfo info in tmpList) {
                                DetailItem tmp = new DetailItem(info);
                                tmp.Width = FileViewer.Width;
                                this.FileViewer.Children.Add(tmp);
                            }
                        });
                    } catch(DirectoryNotFoundException e) {
                        ApplicationResoure.MessageSender(e.Message, StateBoxMod.Error);
                        Dispatcher.Invoke(() => {
                            ItemControl.SetSyncItemState(false);
                            this.Close();
                        });
                    }
                }).Start();
			}else {
				Viewer.Visibility = Visibility.Hidden;
				label3.Visibility = Visibility.Hidden;
				NumBlock.Visibility = Visibility.Hidden;
				BackupFiltrate.Visibility = Visibility.Hidden;
				Box.Visibility = Visibility.Hidden;
                FileViewer.Visibility = Visibility.Hidden;
				textBlock.Visibility = Visibility.Hidden;
				textBlock1.Visibility = Visibility.Hidden;
				textBlock2.Visibility = Visibility.Hidden;
				textBlock3.Visibility = Visibility.Hidden;
				MaxSizeBox.Visibility = Visibility.Hidden;
				MinSizeBox.Visibility = Visibility.Hidden;
			}
			if(ItemControl.IsInvlidItem) {
				this.MaxSizeBox.Foreground = new SolidColorBrush( Colors.Red );
				this.MinSizeBox.Foreground = new SolidColorBrush( Colors.Red );
			}
		}





		#region 事件

		private void BtnDone_Click ( object sender, RoutedEventArgs e ) {
			if(IsEdited) {
				//对不合法的值取缔
				if(MaxSizeBox.Visibility==Visibility.Visible) {
					try {
						if(MinSizeBox.Text=="") {
							MinSizeBox.Text = "0";
						}
						if(MaxSizeBox.Text=="") {
							MaxSizeBox.Text = "0";
						}
						ItemControl.SkipMaxSize =(long) Math.Pow( Convert.ToInt32( MaxSizeBox.Text ) ,2);
						ItemControl.SkipMinSize = (long)Math.Pow( Convert.ToInt32( MinSizeBox.Text ), 2 );
					} catch {
						System.Windows.MessageBox.Show( "请输入正确的区间值", "错误", MessageBoxButton.OK, MessageBoxImage.Error );
						return;
					}
				}
				//对不合法的路径取缔
				if (new DirectoryInfo( LocalPathBlock.Text ).Parent.FullName == TargetPathBlock.Text) {
					System.Windows.MessageBox.Show( "路径重复，同步将导致意想不到的错误!", "警告", MessageBoxButton.OK, MessageBoxImage.Warning );
					return;
				}
				ItemControl.LocalPath = LocalPathBlock.Text;
				ItemControl.GetNumAndSize( ItemControl.LocalPath );
				
				ItemControl.TargetPath = TargetPathBlock.Text;
			}
			this.Closing -= this.Window_Closing;
			this.Close();
		}

		private void BtnCancel_Click ( object sender, RoutedEventArgs e ) {
			if(IsEdited) {
				var result = System.Windows.MessageBox.Show( "有编辑过的数据，确定关闭窗口吗？","提示",MessageBoxButton.YesNo,MessageBoxImage.Asterisk );
				if(result==MessageBoxResult.Yes) {
					this.Closing -= this.Window_Closing;
					this.Close();
					return;
				}
			}
		}

		private void BtnSyncNow_Click ( object sender, RoutedEventArgs e ) {
			this.ItemControl.SyncInThread();
			this.Close();
		}

		private void AutoSync_Checked ( object sender, RoutedEventArgs e ) {
			ItemControl.Task_Item.AutoSync = AutoSync.IsChecked == true;
		}

		private void SkipItem_Click ( object sender, RoutedEventArgs e ) {
			ItemControl.Task_Item.IsSkipItem = SkipItem.IsChecked == true;
		}

		private void AutoBackup_Click ( object sender, RoutedEventArgs e ) {
			ItemControl.Task_Item.AutoBackup = AutoBackup.IsChecked == true;
		}

		private void BackupFiltrate_Click ( object sender, RoutedEventArgs e ) {
			ItemControl.Task_Item.BackupFiltrate = BackupFiltrate.IsChecked == true;
			if(BackupFiltrate.IsChecked==true) {
				textBlock.Visibility = Visibility.Visible;
				textBlock1.Visibility = Visibility.Visible;
				textBlock2.Visibility = Visibility.Visible;
				textBlock3.Visibility = Visibility.Visible;
				MaxSizeBox.Visibility = Visibility.Visible; 
				MinSizeBox.Visibility = Visibility.Visible;
			}else {
				textBlock.Visibility = Visibility.Hidden;
				textBlock1.Visibility = Visibility.Hidden;
				textBlock2.Visibility = Visibility.Hidden;
				textBlock3.Visibility = Visibility.Hidden;
				MaxSizeBox.Visibility = Visibility.Hidden;
				MinSizeBox.Visibility = Visibility.Hidden;
			}
		}

		private void BtnEditLocalPath_Click ( object sender, RoutedEventArgs e ) {
			if(ItemControl.IsDirectory) {
				FolderBrowserDialog Dialog = new FolderBrowserDialog();
				var result = Dialog.ShowDialog();
				if (result == System.Windows.Forms.DialogResult.Cancel) {
					return;
				}
				LocalPathBlock.Text = Dialog.SelectedPath.Trim();
				LocalPathBlock.Foreground = new SolidColorBrush( Colors.Black );
				IsEdited = true;
			} else {
				var openDialog = new Microsoft.Win32.OpenFileDialog();
				var result = openDialog.ShowDialog();
				if (result == true) {
					LocalPathBlock.Text = openDialog.FileName;
					IsEdited = true;
				}
			}
		}

		private void BtnEditTargetPath_Click ( object sender, RoutedEventArgs e ) {
			FolderBrowserDialog Dialog = new FolderBrowserDialog();
			var result = Dialog.ShowDialog();
			if (result == System.Windows.Forms.DialogResult.Cancel) {
				return;
			}
			TargetPathBlock.Text = Dialog.SelectedPath.Trim();
			TargetPathBlock.Foreground = new SolidColorBrush( Colors.Black );
			IsEdited = true;
		}

		private void BtnDelete_Click ( object sender, RoutedEventArgs e ) {
			ItemControl.ItemControl.DelectThis( ItemControl );
			this.Close();
		}

		private void MinSizeBox_GotFocus ( object sender, RoutedEventArgs e ) {
			this.MinSizeBox.SelectAll();
		}

		private void MaxSizeBox_GotFocus ( object sender, RoutedEventArgs e ) {
			this.MaxSizeBox.SelectAll();
		}

		private void MinSizeBox_TextChanged ( object sender, TextChangedEventArgs e ) {
			if (MinSizeBox.Text != ItemControl.SkipMinSize.ToString()) {
				IsEdited = true;
			} else {
				IsEdited = false;
			}
		}

		private void MaxSizeBox_TextChanged ( object sender, TextChangedEventArgs e ) {
			if (MaxSizeBox.Text != ItemControl.SkipMaxSize.ToString()) {
				IsEdited = true;
			} else {
				IsEdited = false;
			}
		}

		private void Window_Closing ( object sender, System.ComponentModel.CancelEventArgs e ) {
			if (IsEdited) {
				e.Cancel = true;
				var result = System.Windows.MessageBox.Show( "有编辑过的数据，确定关闭窗口吗？", "提示", MessageBoxButton.YesNo, MessageBoxImage.Asterisk );
				if (result == MessageBoxResult.Yes) {
					e.Cancel = false;
					try { this.Close(); }catch {
						return;
					}
				}
			}
			
		}

		#endregion

		#region 标题修改逻辑
		private void TaskNameLab_MouseLeftButtonUp ( object sender, MouseButtonEventArgs e ) {
			this.TaskNameLab.Visibility = Visibility.Hidden;
			this.TitleBox.Visibility = Visibility.Visible;
			this.TitleBox.Focus();
			this.TitleBox.SelectAll();
		}

		private void TitleBox_KeyDown ( object sender, System.Windows.Input.KeyEventArgs e ) {
			if(e.Key==Key.Enter) {
				if(this.TitleBox.Text=="") {
					System.Windows.MessageBox.Show( "请输入标题！", "提示", MessageBoxButton.OK, MessageBoxImage.Exclamation );
                    this.TitleBox.Text =(String) this.TaskNameLab.Content;
					return;
				}
				this.TaskNameLab.Visibility = Visibility.Visible;
				this.TitleBox.Visibility = Visibility.Hidden;
				this.TaskNameLab.Content = TitleBox.Text;
				ItemControl.ItemName = TitleBox.Text;
				
			}
		}

		private void TitleBox_LostFocus ( object sender, RoutedEventArgs e ) {
			if (this.TitleBox.Text == "") {
				System.Windows.MessageBox.Show( "请输入标题！", "提示", MessageBoxButton.OK, MessageBoxImage.Exclamation );
                this.TitleBox.Text = (String)this.TaskNameLab.Content;
                return;
			}
			this.TaskNameLab.Visibility = Visibility.Visible;
			this.TitleBox.Visibility = Visibility.Hidden;
			this.TaskNameLab.Content = TitleBox.Text;
			ItemControl.ItemName = TitleBox.Text;
		}

		#endregion
		
	}
}
