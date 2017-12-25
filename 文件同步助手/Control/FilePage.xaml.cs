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

namespace 文件同步助手.Control {
	/// <summary>
	/// FilePage.xaml 的交互逻辑
	/// </summary>
	public partial class FilePage : UserControl {

        private BackupItemControl itemControl;


		public FilePage () {
			InitializeComponent();
            Initizilize();
		}

        /// <summary>
        /// 初始化
        /// </summary>
        public void Initizilize() {
            itemControl = new BackupItemControl(this);
        }


        public void AddItem(BackupItem item) {
            this.FileViewer.Children.Add(item);
        }

        public void AddAllItem(List<BackupItem> list) {
            foreach(BackupItem item in list) {
                FileViewer.Children.Add(item);
            }
        }

        public void Delete(BackupItem item) {
            FileViewer.Children.Remove(item);
        }








        /// <summary>
        /// 设置多选模式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSelectionMod_Click(object sender, RoutedEventArgs e) {
            if ((string)BtnSelectionMod.Content == "多选") {
                BtnSelectionMod.Content = "取消";
                BtnSelectAll.Visibility = Visibility.Visible;
                BtnInversionSelect.Visibility = Visibility.Visible;
                itemControl.SetAllItemCheckBox(true);
            } else {
                BtnSelectionMod.Content = "多选";
                BtnSelectAll.Visibility = Visibility.Hidden;
                BtnInversionSelect.Visibility = Visibility.Hidden;
                itemControl.SetAllItemCheckBox(false);
            }
        }

        private void BtnSelectAll_Click(object sender, RoutedEventArgs e) {
            itemControl.SetAllItemCheckBox(true);
        }

        private void BtnInversionSelect_Click(object sender, RoutedEventArgs e) {
            itemControl.InversionSelection();
        }

        private void BtnUpdata_Click(object sender, RoutedEventArgs e) {
            itemControl.Initilize();
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e) {
            itemControl.DeleteAll();
        }
    }






}
