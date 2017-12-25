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
	/// StatePage.xaml 的交互逻辑
	/// </summary>
	public partial class StatePage : UserControl {

		//程序主窗口引用
		private bool IsUsbOnline { get; set; }
		private bool IsRunning { get; set; }
		private bool IsOnline { get; set; }

		#region 图片资源
		ImageSource ImgUsbOn = new BitmapImage( new Uri( @"\Resoure\usbIn.png", UriKind.Relative ) );
		ImageSource ImgUsbOff= new BitmapImage( new Uri( @"\Resoure\usbOut.png", UriKind.Relative ) );
		ImageSource ImgSyncOn = new BitmapImage( new Uri( @"\Resoure\syncing.png", UriKind.Relative ) );
		ImageSource ImgSyncOff = new BitmapImage( new Uri( @"\Resoure\notSync.png", UriKind.Relative ) );
		ImageSource ImgCludeOn = new BitmapImage( new Uri( @"\Resoure\onLine.png", UriKind.Relative ) );
		ImageSource ImgCludeOff = new BitmapImage( new Uri( @"\Resoure\offLine.png", UriKind.Relative ) );
		#endregion


		/// <summary>
		/// 构造
		/// </summary>
		public StatePage () {
			InitializeComponent();
			Initialize();
		}

		private void Initialize() {
			//初始化欢迎语句
			AddMessage( "欢迎使用文件同步助手。", StateBoxMod.Information );
			SetUsbImgState( false );
			SetSyncImgState( false );
			SetCludeImgState( false );
		}


		#region 信息显示
		/// <summary>
		/// 添加信息到状态页面的MessageStack中
		/// </summary>
		/// <param name="content">内容</param>
		/// <param name="mod">模式</param>
		public void AddMessage ( string content, StateBoxMod mod ) {
            StateBox tmp = new StateBox(content, mod);
            tmp.Width = this.MessageStack.Width;
            this.MessageStack.Children.Add(tmp);
            this.Viewer.ScrollToHome();
        }
		/// <summary>
		/// 添加信息到状态页面的MessageStack中
		/// </summary>
		/// <param name="content">内容</param>
		public void AddMessage(string content) {
			AddMessage( content, StateBoxMod.Content );
		}
		#endregion

		#region 程序状态设置
		//检测第一次运行，因为USB插拔需要输出到日志，第一次启动程序不需要写入日志
		private bool flag = false;

		public void SetUsbImgState(bool b) {
            new Thread(() => {
                this.Dispatcher.Invoke(() => {
                    if (b) {
                        this.UsbState.Source = ImgUsbOn;
                        this.UsbState.ToolTip = "目标USB已连接";
                        AddMessage("USB已经连接", StateBoxMod.Information);
                        this.IsUsbOnline = true;
                    } else {
                        this.UsbState.Source = ImgUsbOff;
                        this.UsbState.ToolTip = "目标USB未连接";
                        this.IsUsbOnline = false;
                        if (flag) {
                            AddMessage("USB已经移除", StateBoxMod.Information);
                        }
                        flag = true;
                    }
                });
            }).Start();
        }

		public void SetSyncImgState ( bool b ) {
            new Thread(() => {
                Dispatcher.Invoke(() => {
                    if (b) {
                        this.SyncState.Source = ImgSyncOn;
                        this.SyncState.ToolTip = "同步任务进行中，请不要关闭程序";
                        this.IsRunning = true;
                    } else {
                        this.SyncState.Source = ImgSyncOff;
                        this.SyncState.ToolTip = "无同步任务进行";
                        IsRunning = false;
                    }
                });
            }).Start();
        }

		public void SetCludeImgState ( bool b ) {
			
            new Thread(() => {
                this.Dispatcher.Invoke(() => {
                    if (b) {
                        this.CludeState.Source = ImgCludeOn;
                        this.CludeState.ToolTip = "云端连接成功";
                        IsOnline = true;
                    } else {
                        this.CludeState.Source = ImgCludeOff;
                        this.CludeState.ToolTip = "云端未连接";
                        IsOnline = false;
                    }
                });
            }).Start();
        }

		#endregion










		//打印日志
		private void BtnPrint_Click ( object sender, RoutedEventArgs e ) {
			List<LogPacket> tmpList = new List<LogPacket>();
			foreach(StateBox s in MessageStack.Children) {
				tmpList.Add( s.GetPacket() );
			}
			if(tmpList.Count>0) {
				tmpList.RemoveAt( 0 );
			}
			try {
				Stream stream = new FileStream( ApplicationResoure.PrintPosition, FileMode.Create );
				StreamWriter sw = new StreamWriter( stream );
				foreach (LogPacket log in tmpList) {
					sw.WriteLine( log.Content );
				}
				sw.Close();
				stream.Close();
				MessageBox.Show( "日志打印完成，位置在：" + ApplicationResoure.PrintPosition, "提示", MessageBoxButton.OK, MessageBoxImage.Information );
			}catch(IOException ex) {
				MessageBox.Show( ex.ToString(), "错误", MessageBoxButton.OK, MessageBoxImage.Error );
			}
		}
		//清空日志
		private void BtnClear_Click ( object sender, RoutedEventArgs e ) {
			MessageBoxResult req= MessageBox.Show( "清空消息栈会导致当前日志丢失，确定这样做吗？" ,"提示",MessageBoxButton.YesNo,MessageBoxImage.Asterisk);
			if (req==MessageBoxResult.Yes) {
				this.MessageStack.Children.Clear();
			}
		}
	}
}
