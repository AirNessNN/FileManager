using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using 文件同步助手.Control;
using 文件同步助手.Logic;

namespace 文件同步助手.Logic {

	public class SyncItemControl {

		#region 属性和字段
		/// <summary>
		/// 任务列表
		/// </summary>
		private List<SyncItem> itemList = null;
		public List<SyncItem> ItemList { get { return itemList; } }

		/// <summary>
		/// 该页面的引用
		/// </summary>
		public SyncPage Page;

		/// <summary>
		/// 列表控制器引用
		/// </summary>
		public UIElementCollection ListControl = null;

		/// <summary>
		/// 消息发送器
		/// </summary>
		public StatePage MessageSender = null;

		/// <summary>
		/// 多选模式
		/// </summary>
		public bool IsMultiSelect { get; set; }
		#endregion

		/// <summary>
		/// 默认的列表控制器构造方法
		/// </summary>
		public SyncItemControl() {

			//初始化日志发送器
			MessageSender = (StatePage)((MainWindow)Application.Current.MainWindow).NavControl.BtnState.ControlPage;
			//ListControl = ((SyncPage)((MainWindow)Application.Current.MainWindow).NavControl.BtnSync.ControlPage).SyncList.Children;
			//初始化列表
			itemList = new List<SyncItem>();
			//初始化列表项目
			if (File.Exists(ApplicationResoure.TaskItemPosition)) {
				List<TaskItem> tmp= this.ReadList();
				foreach(TaskItem t in tmp) {
					SyncItem si = new SyncItem( t );
					si.ItemControl = this;

					itemList.Add( si );
				}
			}
			//添加所有项目到容器
		}

		/// <summary>
		/// 保存该列表
		/// </summary>
		public void SaveList() {
			List<TaskItem> tmp = new List<TaskItem>();
			foreach(SyncItem item in itemList) {
				tmp.Add( item.Task_Item );
			}
			ApplicationResoure.WriteObject( ApplicationResoure.TaskItemPosition, tmp);
		}
		//从文件读取列表
		public List<TaskItem> ReadList() {
			return (List<TaskItem>)ApplicationResoure.ReadObject( ApplicationResoure.TaskItemPosition );
		}




		#region 列表选择相关

		/// <summary>
		/// 打开或关闭所有列表项目选择框
		/// </summary>
		/// <param name="b">True打开所有选择框，False关闭所有选择框</param>
		public void SetAllItemCheckBox(bool b) {
			foreach (SyncItem item in itemList) {
				item.SelectionBoxVisablity = b ? Visibility.Visible : Visibility.Hidden;
			}
		}

		/// <summary>
		/// 返回选择的项目数量
		/// </summary>
		/// <returns></returns>
		public int GetSelectedItem() {
			int kase = 0;
			foreach(SyncItem item in itemList) {
				if (item.IsSelected)
					kase++;
			}
			return kase;
		}


		/// <summary>
		/// 恢复列表项选择状态到未选定状态
		/// </summary>
		public void ClearSelectedItemSelection() {
			foreach(SyncItem item in itemList) {
				if(item.IsSelected==true) {
					item.IsSelected = false;
				}
			}
		}

		/// <summary>
		/// 强制设置所有列表项的选择状态
		/// </summary>
		/// <param name="b">True选中，False未选中</param>
		public void SetAllItemSelection(bool b) {
			foreach(SyncItem item in itemList) {
				item.IsSelected = b;
			}
		}

		/// <summary>
		/// 反选列表项
		/// </summary>
		public void InversionSelection() {
			foreach(SyncItem item in itemList) {
				item.IsSelected = !item.IsSelected;
			}
		}


		#endregion


		#region 忽略相关

		/// <summary>
		/// 设置选择项目忽略的状态
		/// </summary>
		/// <param name="b">True为跳过，False为默认</param>
		public void SetSelectedSkipState(bool b) {
			//设置消息文本的格式
			string info = b ? "跳过同步" : "默认同步模式";
			//计数器
			int kase = 0;
			foreach(SyncItem item in itemList) {
				if(item.IsSelected) {
					MessageSender.AddMessage( "设置 " + item.ItemName + " 为 " + info + "。");
					item.IsSkipElement = b;
					kase++;
				}
			}
			//添加消息
			MessageSender.AddMessage( "本次修改为 " + info + " 的项目共有：" + kase.ToString() + " 个。" ,StateBoxMod.Information);
			//取消选择
			ClearSelectedItemSelection();
		}

		#endregion


		#region 增加删除

		/// <summary>
		/// 添加所有项目到容器中
		/// </summary>
		public void AddAllToPanel() {
			int kase = 0;
			foreach (SyncItem item in itemList) {
				item.Width = Page.SyncList.Width;
				ListControl.Add( item );
				kase++;
			}
			MessageSender.AddMessage( "任务列表填充完成，填充数为：" + kase.ToString() + " 个。" ,StateBoxMod.Information);
		}

		/// <summary>
		/// 添加单个项目
		/// </summary>
		public void AddToPanel(SyncItem item) { 
			item.ItemControl = this;
			ListControl.Add( item );
			itemList.Add( item );
			item.Width = Page.SyncList.Width;
			MessageSender.AddMessage( "添加 " + item.ItemName + " 到同步列队。", StateBoxMod.Information );
		}

		/// <summary>
		/// 删除选择的项目
		/// </summary>
		public void DeleteSelectedItem() {
			int kase = 0;
			List<SyncItem> tmp = new List<SyncItem>();
			foreach(SyncItem item in itemList) {
				if(item.IsSelected) {
					MessageSender.AddMessage( "删除 " + item.ItemName + "。" );
					tmp.Add( item );
					kase++;
				}
			}
			foreach(SyncItem item in tmp) {
				itemList.Remove( item );
				ListControl.Remove( item );
			}
			MessageSender.AddMessage( "本次共删除 " + kase.ToString() + " 个项目。" ,StateBoxMod.Information);
		}

		/// <summary>
		/// 删除所有列表项目
		/// </summary>
		public void DeleteAllItems() {
			MessageSender.AddMessage( "清空任务列队共 " + ItemList.Count.ToString() + " 个。", StateBoxMod.Information );
			itemList.Clear();
			ListControl.Clear();
		}

		public void DelectThis(SyncItem item) {
			MessageSender.AddMessage( "删除 " + item.ItemName + "。" );
			itemList.Remove( item );
			ListControl.Remove( item );
		}


		#endregion

	}
}
