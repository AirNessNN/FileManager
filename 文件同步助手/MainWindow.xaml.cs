using System;
using System.Collections.Generic;
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
using 文件同步助手.Logic;
using 文件同步助手.Control;
using System.IO;

namespace 文件同步助手 {
	/// <summary>
	/// MainWindow.xaml 的交互逻辑
	/// </summary>
	public partial class MainWindow : Window {

		public NavigationControl NavControl;

		#region 文件索引

		#endregion

		#region 页面
		public StatePage State_Page { get { return (StatePage)this.NavControl.BtnState.ControlPage; } }
		public SyncPage Sync_Page { get { return (SyncPage)this.NavControl.BtnSync.ControlPage; } }
		public FilePage File_Page { get { return (FilePage)this.NavControl.BtnFiles.ControlPage; } }
		public SettingPage Setting_Page { get { return (SettingPage)this.NavControl.BtnSetting.ControlPage; } }
		#endregion

		public MainWindow () {
			InitializeComponent();
			//读取设置
			if(File.Exists(ApplicationResoure.SettingPath)) {
				SettingPacket tmpSetting = (SettingPacket)ApplicationResoure.ReadObject( ApplicationResoure.SettingPath );
				tmpSetting.SetAllSetting();
			}
			
			NavControl = new NavigationControl();
			NavControl.Initialize();
		}



		private void Window_Loaded ( object sender, RoutedEventArgs e ) {
			//添加页面
			foreach (ImageButton tmp in NavControl.GetImageButtons()) {
				Navigation.Children.Add( tmp );
			}
		}

		private void Window_Closing ( object sender, System.ComponentModel.CancelEventArgs e ) {
			Sync_Page.SaveList();
			//保存设置
			SettingPacket settingTmp = new SettingPacket(
			 ApplicationResoure.AutoBackup, ApplicationResoure.AutoSync, 
			 ApplicationResoure.BackgroundRunning, ApplicationResoure.ErrorTip, 
			 ApplicationResoure.FileMonitoring, ApplicationResoure.StartingUp, 
			 ApplicationResoure.UsbTrigger, ApplicationResoure.TimeTrigger, 
			 ApplicationResoure.BackupPath, ApplicationResoure.PrintPosition );

			ApplicationResoure.WriteObject( ApplicationResoure.SettingPath, settingTmp );
		}
	}

	/// <summary>
	/// 设置的包装类，可以被序列化
	/// </summary>
	[Serializable]
	public class SettingPacket {
		
		//属性
		public bool AutoBackup { get; set; }

		public bool AutoSync { get; set; }

		public bool BackgroundRunning { get; set; }

		public bool ErrorTip { get; set; }

		public bool FileMonitoring { get; set; }

		public bool StartingUp { get; set; }

		public bool UsbTrigger { get; set; }

		public bool TimeTrigger { get; set; }

		public string PrintPath { get; set; }

		public string BackupPath { get; set; }

		/// <summary>
		/// 封装
		/// </summary>
		/// <param name="autoBackup">自动备份</param>
		/// <param name="autoSync">自动同步</param>
		/// <param name="backgroundRunning">后台运行</param>
		/// <param name="errorTip">错误提示</param>
		/// <param name="fileMonitoring">文件监控</param>
		/// <param name="startingUp">开机启动</param>
		/// <param name="usbTrigger">USB触发</param>
		/// <param name="timeTrigger">时间触发</param>
		/// <param name="backupPath">备份路径</param>
		/// <param name="printPath">打印路径</param>
		public SettingPacket(bool autoBackup,bool autoSync,bool backgroundRunning,bool errorTip,bool fileMonitoring,bool startingUp,bool usbTrigger,bool timeTrigger,string backupPath,string printPath) {
			this.AutoBackup = autoBackup;
			this.AutoSync = autoSync;
			this.BackgroundRunning = backgroundRunning;
			this.ErrorTip = errorTip;
			this.FileMonitoring = fileMonitoring;
			this.StartingUp = startingUp;
			this.UsbTrigger = usbTrigger;
			this.TimeTrigger = timeTrigger;
			this.PrintPath = new FileInfo( printPath ).Directory.FullName;
            this.BackupPath = new DirectoryInfo(backupPath).Parent.FullName;
		}

		/// <summary>
		/// 设置所有属性
		/// </summary>
		public void SetAllSetting() {
			ApplicationResoure.AutoBackup= AutoBackup;
			ApplicationResoure.AutoSync = AutoSync;
			ApplicationResoure.BackgroundRunning = BackgroundRunning;
            if (Directory.Exists(BackupPath)) {
                ApplicationResoure.BackupPath = BackupPath;
            } else {
                MessageBox.Show("选定的备份路径已经失效，已重置为默认路径", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }
			ApplicationResoure.ErrorTip = this.ErrorTip;
			ApplicationResoure.FileMonitoring = this.FileMonitoring;
            if (Directory.Exists(PrintPath)) {
                ApplicationResoure.PrintPosition = PrintPath;
            } else {
                MessageBox.Show("选定的打印路径已经失效，已重置为默认路径", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }
			ApplicationResoure.StartingUp = this.StartingUp;
			ApplicationResoure.TimeTrigger = this.TimeTrigger;
			ApplicationResoure.UsbTrigger = this.UsbTrigger;
		}
	}
}
