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
	/// ImageButton.xaml 的交互逻辑
	/// </summary>
	public partial class ImageButton : UserControl {

		public ImageButton (UserControl cp,string tip) {
			InitializeComponent();
			this.ControlPage = cp;
			//cp.TitleLab.Content = this.ToolTip = tip;
		}

		//选中标记
		public bool IsSelected { get; set; }
		//事件
		//public event EventHandler ClickEvent;
		//所支持的页面
		public UserControl ControlPage { get; set; }
		//窗口
		public MainWindow MW = (MainWindow)Application.Current.MainWindow;

		public EventHandler Event;

		#region 图片资源
		public ImageSource Normal = null;
		public ImageSource Down = null;
		public ImageSource Over = null;
		#endregion
		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="normal">正常状态的图片</param>
		/// <param name="down">按下状态</param>
		/// <param name="over">悬停状态</param>
		public void SetImageSource(ImageSource normal,ImageSource down,ImageSource over) {
			Normal = normal;
			Down = down;
			Over = over;
			this.image.Source = Normal;
		}


		/// <summary>
		/// 清除被点击的状态，回到就绪状态
		/// </summary>
		public void ClearClick() {
			this.IsSelected = false;
			this.image.Source = Normal;
		}

		/// <summary>
		/// 鼠标进入
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void image_MouseEnter ( object sender, MouseEventArgs e ) {
			if(!IsSelected) {
				this.image.Source = Over;
			}
		}

		/// <summary>
		/// 鼠标离开
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void image_MouseLeave ( object sender, MouseEventArgs e ) {
			if(!IsSelected) {
				this.image.Source = Normal;
			}
		}

		/// <summary>
		/// 鼠标点击
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void image_MouseUp ( object sender, MouseButtonEventArgs e ) {
			if (!IsSelected) {
				this.image.Source = Down;
				this.IsSelected = true;
				MW.frame.Content = this.ControlPage;
				//重置上一个点击项的状态
				if(!MW.NavControl.IsPress) {
					MW.NavControl.LastClick = this;
					MW.NavControl.IsPress = true;
				}else {
					MW.NavControl.LastClick.ClearClick();
					MW.NavControl.LastClick = this;
				}
			}
		}
	}
}
