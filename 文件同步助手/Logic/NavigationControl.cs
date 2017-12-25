using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using 文件同步助手.Control;

namespace 文件同步助手.Logic {

	public class NavigationControl {

		//主程序的窗口
		MainWindow MW = (MainWindow)Application.Current.MainWindow;
		//上个点击
		public ImageButton LastClick = null;
		public bool IsPress = false;

		#region 按钮
		public ImageButton BtnState /*= new ImageButton( new StatePage(), "通知中心" );*/;
		public ImageButton BtnSync /*= new ImageButton( new SyncPage(), "同步中心" )*/;
		public ImageButton BtnFiles /*= new ImageButton( new FilePage(), "文件管理" )*/;
		public ImageButton BtnSetting /*= new ImageButton( new SettingPage(), "设置中心" )*/;
		#endregion

		public void Initialize() {
			BtnState = new ImageButton( new StatePage(), "通知中心" );
			BtnState.SetImageSource(
			new BitmapImage( new Uri( @"\Resoure\stateNormal.png", UriKind.Relative ) ),
			new BitmapImage( new Uri( @"\Resoure\stateSelected.png", UriKind.Relative ) ),
			new BitmapImage( new Uri( @"\Resoure\stateSelecting.png", UriKind.Relative ) ) );
			//BtnState.ClickEvent += new EventHandler(BtnStateClick);
			BtnState.ToolTip = "通知中心";

			BtnSync = new ImageButton( new SyncPage(), "同步中心" );
			BtnSync.SetImageSource(
			new BitmapImage( new Uri( @"\Resoure\syncNormal.png", UriKind.Relative ) ),
			new BitmapImage( new Uri( @"\Resoure\syncSelected.png", UriKind.Relative ) ),
			new BitmapImage( new Uri( @"\Resoure\syncSelecting.png", UriKind.Relative ) ) );
			//BtnSync.ClickEvent += new EventHandler( BtnSyncClick );
			BtnSync.ToolTip = "同步中心";

			BtnFiles = new ImageButton( new FilePage(), "文件管理" );
			BtnFiles.SetImageSource(
			new BitmapImage( new Uri( @"\Resoure\fileNormal.png", UriKind.Relative ) ),
			new BitmapImage( new Uri( @"\Resoure\fileSelected.png", UriKind.Relative ) ),
			new BitmapImage( new Uri( @"\Resoure\fileSelecting.png", UriKind.Relative ) ) );
			//BtnFiles.ClickEvent += new EventHandler( BtnFilesClick );
			BtnFiles.ToolTip = "文件管理";

			BtnSetting = new ImageButton( new SettingPage(), "设置中心" );
			BtnSetting.SetImageSource(
			new BitmapImage( new Uri( @"\Resoure\settingNormal.png", UriKind.Relative ) ),
			new BitmapImage( new Uri( @"\Resoure\settingSelected.png", UriKind.Relative ) ),
			new BitmapImage( new Uri( @"\Resoure\settingSelecting.png", UriKind.Relative ) ) );
			//BtnSetting.ClickEvent += new EventHandler( BtnSettingClick );
			BtnSetting.ToolTip = "设置中心";

			//设置首页
			BtnState.IsSelected = true;
			BtnState.image.Source = BtnState.Down;
			BtnState.MW.frame.Content = BtnState.ControlPage;
			this.LastClick = BtnState;
			this.IsPress = true;
		}

		/// <summary>
		/// 获取导航的按钮组
		/// </summary>
		/// <returns>返回List对象</returns>
		public List<ImageButton> GetImageButtons() {
			List<ImageButton> tmp = new List<ImageButton>();
			tmp.Add( BtnState );
			tmp.Add( BtnSync );
			tmp.Add( BtnFiles );
			tmp.Add( BtnSetting );
			return tmp;
		}

	}
}
