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
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using 文件同步助手.Control;
using 文件同步助手.Logic;

namespace 文件同步助手.Control {
	/// <summary>
	/// SyncPage.xaml 的交互逻辑
	/// </summary>
	public partial class SyncPage : UserControl {

		#region 属性和字段
		/// <summary>
		/// 列表控制器
		/// </summary>
		private SyncItemControl itemControl;
		public SyncItemControl ItemControl { get { return itemControl; } }
		#endregion


		/// <summary>
		/// 默认构造函数
		/// </summary>
		public SyncPage () {
			InitializeComponent();
			//初始化任务列表控制器
			itemControl = new SyncItemControl();
			itemControl.Page = this;
			itemControl.ListControl = this.SyncList.Children;
			itemControl.AddAllToPanel();

			//初始化设置
			this.AutoSync.IsChecked = ApplicationResoure.AutoSync;
			this.AutoBackup.IsChecked = ApplicationResoure.AutoBackup;
		}


		#region List方法
		/// <summary>
		/// 向同步列表中添加任务
		/// </summary>
		/// <param name="item"></param>
		public void AddItem(SyncItem item) {
			this.SyncList.Children.Add( item );
		}


		/// <summary>
		/// 保存文件
		/// </summary>
		public void SaveList() {
			this.itemControl.SaveList();
		}

		/// <summary>
		/// 同步任务，在新线程上进行
		/// </summary>
		/// <param name="b">False是全部同步</param>
		public void ThreadTask ( object b ) {
			foreach (var item in itemControl.ItemList) {
				if ((bool)b) {
					if (item.IsSelected) {
						item.Sync();
					}
				} else {
					item.Sync();
				}
			}
		}


		#endregion

		

		#region 按钮事件
		//选择全部
		private void BtnSelectAll_Click ( object sender, RoutedEventArgs e ) {
			itemControl.SetAllItemSelection( true );
		}

		//反选
		private void BtnInversionSelect_Click ( object sender, RoutedEventArgs e ) {
			itemControl.InversionSelection();
		}

		//删除选择
		private void BtnDeleteSelected_Click ( object sender, RoutedEventArgs e ) {
			if(itemControl.GetSelectedItem()==0) {
				MessageBox.Show( "未选择任何项目", "警告", MessageBoxButton.OK, MessageBoxImage.Warning );
				return;
			}
			MessageBoxResult r= MessageBox.Show( "确定要删除这些任务吗？", "提示", MessageBoxButton.YesNo, MessageBoxImage.Asterisk );
			if(r==MessageBoxResult.Yes) {
				itemControl.DeleteSelectedItem();
			}
		}

		//删除全部
		private void BtnDeleteAll_Click ( object sender, RoutedEventArgs e ) {
			MessageBoxResult r = MessageBox.Show( "确定要清空任务列表吗？", "提示", MessageBoxButton.YesNo, MessageBoxImage.Asterisk );
			if(r==MessageBoxResult.Yes) {
				itemControl.DeleteAllItems();
			}
		}

		private void BtnSelectionMod_Click ( object sender, RoutedEventArgs e ) {
			if ((string)BtnSelectionMod.Content == "多选") {
				itemControl.SetAllItemCheckBox( true );
				BtnSelectionMod.Content = "取消";
				BtnSelectAll.Visibility = Visibility.Visible;
				BtnInversionSelect.Visibility = Visibility.Visible;
				itemControl.IsMultiSelect = true;
			} else {
				itemControl.SetAllItemCheckBox( false );
				itemControl.ClearSelectedItemSelection();
				BtnSelectionMod.Content = "多选";
				BtnSelectAll.Visibility = Visibility.Hidden;
				BtnInversionSelect.Visibility = Visibility.Hidden;
				itemControl.IsMultiSelect = false;
			}
		}

		private void BtnNewTask_Click ( object sender, RoutedEventArgs e ) {
			TaskCreateWindow tc = new TaskCreateWindow();
			tc.Owner = ApplicationResoure.Main_Window;
			tc.ShowDialog();
		}

        //同步选择项
		private void BtnSyncSelected_Click ( object sender, RoutedEventArgs e ) {
			if (itemControl.IsMultiSelect) {
				if (itemControl.GetSelectedItem() == 0) {
					MessageBox.Show( "未选择任何任务", "提示", MessageBoxButton.OK, MessageBoxImage.Exclamation );
					return;
				}
				var result = MessageBox.Show( "将会同步选中的项目，同步任务开始之后不能被终止，是否继续？", "提示", MessageBoxButton.YesNo, MessageBoxImage.Information );
				if (result != MessageBoxResult.Yes) {
					return;
				}
                ApplicationResoure.MessageSender("开始同步选中的任务", StateBoxMod.Information);
				new Thread( () =>
				{
                    ThreadTask(true);
                } ).Start();
			}
		}

        //同步全部
		private void BtnSyncAll_Click ( object sender, RoutedEventArgs e ) {
			var result = MessageBox.Show( "将会同步列表中所有项目，同步任务开始之后不能被终止，是否继续？", "提示", MessageBoxButton.YesNo, MessageBoxImage.Information );
			if (result != MessageBoxResult.Yes) {
				return;
			}
			ApplicationResoure.MessageSender("开始同步选中的任务", StateBoxMod.Information);
			new Thread( () =>
			{
                ThreadTask(false);
            } ).Start();
		}

		private void AutoSync_Click ( object sender, RoutedEventArgs e ) {
			ApplicationResoure.AutoSync = this.AutoSync.IsChecked==true;
		}

		private void AutoBackup_Click ( object sender, RoutedEventArgs e ) {
			ApplicationResoure.AutoBackup = this.AutoBackup.IsChecked == true;
		}

		#endregion


	}
}
