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
	/// StateBox.xaml 的交互逻辑
	/// </summary>
	public partial class StateBox : UserControl {


		#region 颜色
		Brush blue=new SolidColorBrush( (Color)ColorConverter.ConvertFromString( "#FF0C6DC1" ) );
		Brush red = new SolidColorBrush( (Color)ColorConverter.ConvertFromString( "#FFB90000" ) );
		#endregion
		/// <summary>
		/// 默认显示模式
		/// </summary>
		/// <param name="content">内容</param>
		public StateBox (string content) {
			InitializeComponent();
			this.TimeLab.Text = DateTime.Now.ToString();
			this.ContentLab.Text = content;
			this.ContentLab.ToolTip = content;
		}


		/// <summary>
		/// 高级显示模式
		/// </summary>
		/// <param name="content">内容</param>
		/// <param name="mod">模式</param>
		public StateBox(string content,StateBoxMod mod) {
			InitializeComponent();
			//显示时间
			this.TimeLab.Text = DateTime.Now.ToString();
			this.ContentLab.Text = content;
			this.ContentLab.ToolTip = content;
			switch (mod) {
				case StateBoxMod.Content: {
						break;
				}
				case StateBoxMod.Information: {
						this.ContentLab.Foreground = blue;
						break;
				}
				case StateBoxMod.Error: {
						this.ContentLab.Foreground = red;
						break;
				}
			}
		}


		public LogPacket GetPacket () {
			return new LogPacket( this.TimeLab.Text,this.ContentLab.Text );
		}

	}



	/// <summary>
	/// StateBox显示状态枚举
	/// </summary>
	public enum StateBoxMod {
		/// <summary>
		/// 普通模式，黑色字体
		/// </summary>
		Content,
		/// <summary>
		/// 信息模式，蓝色字体
		/// </summary>
		Information,
		/// <summary>
		/// 错误模式，红色字体
		/// </summary>
		Error
	}

}
