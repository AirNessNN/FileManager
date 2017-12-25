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
using System.IO;
using 文件同步助手.Logic;
using 文件同步助手.ChildrenWindow;
using System.Windows.Media.Effects;
using System.Threading;

namespace 文件同步助手.Control {
	/// <summary>
	/// SyncItem.xaml 的交互逻辑
	/// </summary>
	public partial class SyncItem : UserControl {

		#region 构造
		/// <summary>
		/// 从文件中读取对象的实例化模式
		/// </summary>
		/// <param name="task">读取到的对象</param>
		public SyncItem (TaskItem task) {
			InitializeComponent();
			this.taskItem = task;
			//初始化任务模型
			this.InitializeTask( task );
			if (IsDirectory) {
				if (Directory.Exists( localPath )) {
					DirectoryInfo dir = new DirectoryInfo( localPath );
					List<FileInfo> tmp = this.GetFileList( dir );
					//大小
					long size = 0;
					foreach (FileInfo f in tmp) {
						size += f.Length;
					}
					this.Num = tmp.Count;
					this.Size = size;
				}
			} else {
				FileInfo info = new FileInfo( localPath );
				this.Size = info.Length;
				this.NumLab.Visibility = Visibility.Hidden;
			}
		}

		/// <summary>
		/// 新建任务的实例化模式
		/// </summary>
		/// <param name="path1">本地路径</param>
		/// <param name="path2">目标路径</param>
		/// <param name="size">任务字节大小</param>
		/// <param name="num">任务文件数量</param>
		/// <param name="dateTime">任务创建的时间</param>
		public SyncItem(string path1,string path2, bool isDir,string itemName) {
			InitializeComponent();
			//新建一个任务对象
			this.taskItem = new TaskItem( isDir, path1, path2, itemName );
			InitializeTask( taskItem );

			//新建一个同步控制器

			if(isDir) {
				if(Directory.Exists(path1)) {
					GetNumAndSize( path1 );
				}
			}else {
				FileInfo info = new FileInfo( path1 );
				this.Size = info.Length;
				this.NumLab.Visibility = Visibility.Hidden;
			}
		}

		#endregion






		#region SyncItem属性

		/// <summary>
		/// 任务字节总数
		/// </summary>
		public long Size {
			get { return this.size; }
			set {
				size = value;
				//取消小数，自动转换单位
				if (value < 1024) {
					this.SizeLab.Text = "大小："+value.ToString() + " B";
				} else if (value >= 1024 && value < Math.Pow( 1024, 2 )) {
					this.SizeLab.Text = "大小：" + Convert.ToInt32( (value / 1024) ).ToString() + " KB";
				} else if (value >= Math.Pow( 1024, 2 ) && value < Math.Pow( 1024, 3 )) {
					this.SizeLab.Text = "大小：" + Convert.ToInt32( (value / Math.Pow( 1024, 2 )) ).ToString() + " MB";
				} else {
					this.SizeLab.Text = "大小：" + Convert.ToInt32( (value / Math.Pow( 1024, 3 )) ).ToString() + " GB";
				}
			}
		}
		private long size;

		/// <summary>
		/// 任务包含文件数量
		/// </summary>
		public int Num { get { return this.num; } set { this.NumLab.Text = "数量："+ value.ToString(); this.num = value; } }
		private int num;

		/// <summary>
		/// 本地路径
		/// </summary>
		public string LocalPath { get { return localPath; } set { LocalPathLab.Text = value; localPath = value; LocalPathLab.ToolTip = value; taskItem.LocalPath = value; } }

        /*
         问题：SyncItem多线程读取错误，取不到属性，导致弹出
         新增：DetailWindow文件模式布局重新设定
             
             
             */

		/// <summary>
		/// 目标路径
		/// </summary>
		public string TargetPath { get { return targetPath; } set { targetPath = value; taskItem.TargetPath = value; } }

		/// <summary>
		/// 选择框状态
		/// </summary>
		public bool IsSelected { get { return this.SelectionBox.IsChecked == true; } set { this.SelectionBox.IsChecked = value; } }

		/// <summary>
		/// 任务创建的时间
		/// </summary>
		public DateTime Time { get { return DateTime.Parse( this.TimeLab.Text ); } set { this.TimeLab.Text = value.ToShortDateString(); } }

		/// <summary>
		/// 任务标题
		/// </summary>
		public string ItemName { get { return (string)this.Title.Content; }set { this.Title.Content = value; } }

		/// <summary>
		/// 选择框显示状态
		/// </summary>
		public Visibility SelectionBoxVisablity { set { this.SelectionBox.Visibility = value; } }

		/// <summary>
		/// 该项的列表控制器
		/// </summary>
		public  SyncItemControl ItemControl { get; set; }

		


		#endregion




		#region Task属性和字段

		/// <summary>
		/// 本地的路径
		/// </summary>
		private string localPath;

		/// <summary>
		/// 目标路径
		/// </summary>
		private string targetPath;

		/// <summary>
		/// 是否是忽略的元素
		/// </summary>
		public bool IsSkipElement { get { return taskItem.IsSkipItem; }set { taskItem.IsSkipItem = value; } }

		/// <summary>
		/// 是否是目录
		/// </summary>
		public bool IsDirectory { get { return taskItem.IsDirectory; }set { taskItem.IsDirectory = value; } }

		/// <summary>
		/// 备份开关
		/// </summary>
		public bool IsAutoBackup { get { return taskItem.AutoBackup; }set { taskItem.AutoBackup = value; } }

		/// <summary>
		/// 是否有备份筛选
		/// </summary>
		public bool IsBackupFiltrate { get { return taskItem.BackupFiltrate; }set { taskItem.BackupFiltrate = value; } }

		/// <summary>
		/// 最大文件区间
		/// </summary>
		public long SkipMaxSize { get { return taskItem.SkipMaxSize; }set { taskItem.SkipMaxSize = value; } }

		/// <summary>
		/// 最小文件区间
		/// </summary>
		public long SkipMinSize { get { return taskItem.SkipMinSize; }set { taskItem.SkipMinSize = value; } }

		/// <summary>
		/// 是否是无效项
		/// </summary>
		public bool IsInvlidItem { get { return taskItem.IsInvlidItem; } set { taskItem.IsInvlidItem = value; } }

		/// <summary>
		/// 封装的TaskItem对象
		/// </summary>
		private TaskItem taskItem=null;
		public TaskItem Task_Item { get { return taskItem; } }

		#endregion




		#region 方法
		/// <summary>
		/// 填充所有标签
		/// </summary>
		/// <param name="task">从文件中恢复的对象</param>
		private void InitializeTask(TaskItem task) {
			//恢复对象状态
			LocalPath = task.LocalPath;
			TargetPath = task.TargetPath;
			Time = task.Time;
			ItemName = task.TaskName;
			IsSkipElement = task.IsSkipItem;
			IsDirectory = task.IsDirectory;
			this.IsAutoBackup = task.AutoBackup;
			this.IsBackupFiltrate = task.BackupFiltrate;
			this.SkipMaxSize = task.SkipMaxSize;
			this.SkipMinSize = task.SkipMinSize;
		}


		/// <summary>
		/// 搜索文件夹内的文件
		/// </summary>
		/// <param name="dir"></param>
		/// <returns></returns>
		public List<FileInfo> GetFileList ( DirectoryInfo dir) {
			List<FileInfo> fileList = new List<FileInfo>();
			if (dir.Exists) {
				//得到当前文件和子目录
				try {
					FileInfo[] tmpFiles = dir.GetFiles();
					DirectoryInfo[] tmpDirs = dir.GetDirectories();
					//空文件夹检测
					if (tmpDirs.Count() == 0 && tmpFiles.Count() == 0) {
						return fileList;
					}
					//搜索文件
					foreach (FileInfo tmpF in tmpFiles) {
						fileList.Add( tmpF );
						size += tmpF.Length;
						num += tmpFiles.Count();
					}
					//递归子文件夹
					foreach (DirectoryInfo d in tmpDirs) {
						fileList.AddRange( GetFileList( d) );
					}
				} catch {
					return fileList;
				}
			}
			return fileList;
		}

		/// <summary>
		/// 设置同步标志
		/// </summary>
		/// <param name="b"></param>
		public void SetSyncFlag(bool b) {
            this.Dispatcher.Invoke(() => {
                this.SyncFlagLab.Visibility = b ? Visibility.Visible : Visibility.Hidden;
            });
        }

		/// <summary>
		/// 刷新该项目的大小和数量
		/// </summary>
		/// <param name="path">路径</param>
		public void GetNumAndSize(string path) {
			DirectoryInfo dir = new DirectoryInfo( path );
			List<FileInfo> tmp = this.GetFileList( dir );
			//大小
			long size = 0;
			foreach (FileInfo f in tmp) {
				size += f.Length;
			}
			this.Num = tmp.Count;
			this.Size = size;
		}

		/// <summary>
		/// 设置同步项目的状态，是否有效
		/// </summary>
		/// <param name="b">True有效，False无效</param>
		public void SetSyncItemState(bool b) {

            this.Dispatcher.Invoke(() => {
                this.IsInvlidItem = b;

                if (!b) {
                    //否
                    BlurEffect blue = new BlurEffect();
                    this.ContentGrid.Effect = blue;
                    this.InvlidLab.Visibility = Visibility.Visible;
                } else {
                    //是
                    this.ContentGrid.Effect = null;
                    this.InvlidLab.Visibility = Visibility.Hidden;
                }
            });
		}

		/// <summary>
		/// 在新建线程中启动备份
		/// </summary>
		public void SyncInThread () {
            ThreadTask();
        }

		/// <summary>
		/// 不实例线程启动同步
		/// </summary>
		public void Sync() {
			ThreadTask();
		}

		//线程的托管方法
		public void ThreadTask() {
            SyncLogic syncLogic = new SyncLogic(localPath, targetPath, IsDirectory, IsBackupFiltrate, SkipMaxSize, SkipMinSize, this);
            new Thread(() => {
                syncLogic.SyncThis();
            }).Start();
        }

		/// <summary>
		/// 详情窗口打开方法
		/// </summary>
		private void OpenDetailWindow() {
			DetailWindow dw = new DetailWindow( this );
            dw.Owner = App.Current.MainWindow;
            dw.ShowDialog();
        }

		#endregion



		#region 颜色
		private Brush MouseOverColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString( "#FFB4B4B4" ) );
		private Brush MouseNormalColor = new SolidColorBrush( (Color)ColorConverter.ConvertFromString( "#FFFFFFFF" ) );
		#endregion


		//事件
		#region 效果实现
		private void ContentGrid_MouseEnter ( object sender, MouseEventArgs e ) {
			if(!ItemControl.IsMultiSelect) {
				this.ContentGrid.Background = MouseOverColor;
			}
		}

		private void ContentGrid_MouseLeave ( object sender, MouseEventArgs e ) {
			if(!ItemControl.IsMultiSelect) {
				this.ContentGrid.Background = MouseNormalColor;
			}
		}

		private void ContentGrid_MouseRightButtonDown ( object sender, MouseButtonEventArgs e ) {
			if (!ItemControl.IsMultiSelect) {
				this.ContentGrid.Background = MouseOverColor;
			}
		}
		#endregion



		//双击
		private void UserControl_MouseDoubleClick ( object sender, MouseButtonEventArgs e ) {
			if (!ItemControl.IsMultiSelect) {
				OpenDetailWindow();
			}
		}

		//同步
		private void menuSync_Click ( object sender, RoutedEventArgs e ) {
            if (!IsInvlidItem) {
                SyncInThread();
            }
		}

		//删除
		private void menuDelete_Click ( object sender, RoutedEventArgs e ) {
			ItemControl.DelectThis( this );
		}
		
		//详情
		private void menuDetails_Click ( object sender, RoutedEventArgs e ) {
			OpenDetailWindow();
		}

		
	}
}
