using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using 文件同步助手.ChildrenWindow;
using 文件同步助手.Logic;

namespace 文件同步助手.Control {
	/// <summary>
	/// SettingPage.xaml 的交互逻辑
	/// </summary>
	public partial class SettingPage : System.Windows.Controls.UserControl {

		public SettingPage () {
			InitializeComponent();
			//初始化选项
			LogPrintPathBox.Text = ApplicationResoure.PrintPosition;
			BackupPathBox.Text = ApplicationResoure.BackupPath;
			//初始化选项
			this.IsBackupgroundRunning.IsChecked = ApplicationResoure.BackgroundRunning;
			this.IsErrorTip.IsChecked = ApplicationResoure.ErrorTip;
			this.IsFileMonitoring.IsChecked = ApplicationResoure.FileMonitoring;
			this.IsStartingUp.IsChecked = ApplicationResoure.StartingUp;
			this.TimeTriggerRadio.IsChecked = ApplicationResoure.TimeTrigger;
			this.UsbTriggerRadio.IsChecked = ApplicationResoure.UsbTrigger;
		}




        #region 按钮事件

        private void BtnEditPrintPath_Click ( object sender, RoutedEventArgs e ) {
			FolderBrowserDialog Dialog = new FolderBrowserDialog();
			var result = Dialog.ShowDialog();
			if(result==DialogResult.Cancel) {
				return;
			}
			ApplicationResoure.PrintPosition = Dialog.SelectedPath;
			this.LogPrintPathBox.Text = ApplicationResoure.PrintPosition;
		}

		private void BtnEditBackupPath_Click ( object sender, RoutedEventArgs e ) {
			FolderBrowserDialog Dialog = new FolderBrowserDialog();
			var result = Dialog.ShowDialog();
			if (result == DialogResult.Cancel) {
				return;
			}
			ApplicationResoure.BackupPath = Dialog.SelectedPath;
			this.BackupPathBox.Text = ApplicationResoure.BackupPath;
		}

		private void BtnHelp_Click ( object sender, RoutedEventArgs e ) {
			HelpWindow hw = new HelpWindow("help");
			hw.Owner = ApplicationResoure.Main_Window;
			hw.ShowDialog();
		}

		private void BtnAbout_Click ( object sender, RoutedEventArgs e ) {
            HelpWindow hw = new HelpWindow("about");
            hw.Owner = ApplicationResoure.Main_Window;
            hw.ShowDialog();
        }

        #endregion

        private void IsBackupgroundRunning_Click(object sender, RoutedEventArgs e) {
            ApplicationResoure.BackgroundRunning = !ApplicationResoure.BackgroundRunning;
        }

        private void IsErrorTip_Click(object sender, RoutedEventArgs e) {
            ApplicationResoure.ErrorTip = !ApplicationResoure.ErrorTip;
        }
    }
}
