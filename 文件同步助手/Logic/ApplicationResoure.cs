using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using static System.Environment;
using 文件同步助手.Control;
using System.Windows.Threading;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Threading;

namespace 文件同步助手.Logic {
	public class ApplicationResoure {


		#region 对象引用

		/// <summary>
		/// 主窗口引用
		/// </summary>
		public static MainWindow Main_Window { get {return (MainWindow)Application.Current.MainWindow; } }


        /// <summary>
        /// 日志传送器
        /// </summary>
        /// <param name="str">输入字符</param>
        /// <param name="mod">模式</param>
        public static void MessageSender(string str,StateBoxMod mod) {
            App.Current.Dispatcher.Invoke(() => {
                ((MainWindow)Application.Current.MainWindow).State_Page.AddMessage(str, mod);
            });
        }

        public static void SetSyncImg(bool b) {
            App.Current.Dispatcher.Invoke(() => {
                ((MainWindow)Application.Current.MainWindow).State_Page.SetSyncImgState(b);
            });
        }

        /// <summary>
        /// 获取扩展名相对的图片资源
        /// </summary>
        /// <param name="rex"></param>
        /// <returns></returns>
        public static ImageSource GetTypeImage(string rex) {

            string str = new FileInfo(rex).Extension;
            str = str.Trim('.');
            ImageSource tmpImg;
            try {
                if (!File.Exists(@"E:\Document\Visual Studio 2015\Projects\文件同步助手\文件同步助手\Resoure"+"\\"+str+"_s.png")) {
                    return tmpImg = new BitmapImage(new Uri(@"/Resoure/unknow_s.png", UriKind.Relative));
                }
                tmpImg = new BitmapImage(new Uri(@"/Resoure/" + str + "_s.png", UriKind.Relative));
                
            } catch(IOException) {
                tmpImg = new BitmapImage(new Uri(@"/Resoure/unknow_s.png", UriKind.Relative));
            }
            return tmpImg;
        }

		#endregion

		#region 文件保存位置引用
		//打印位置
		private static string printPos = Environment.GetFolderPath( SpecialFolder.Desktop);
		/// <summary>
		/// 打印位置
		/// </summary>
		public static string PrintPosition { get { return printPos+"\\File_Log.txt";  } set { printPos = value; } }

		//任务列表保存位置
		private static string taskItemPos=Environment.CurrentDirectory;
		/// <summary>
		/// 任务列表保存位置，储存TaskItem的List对象
		/// </summary>
		public static string TaskItemPosition { get { return taskItemPos + "\\task_item_save.items"; } }


		private static string setting = Environment.CurrentDirectory;
		/// <summary>
		/// 设置保存的路径
		/// </summary>
		public static string SettingPath { get { return setting + "\\setting.set"; } }


		//备份位置引用
		private static string backupPath = Environment.CurrentDirectory;
		/// <summary>
		/// 备份位置，默认为工程目录
		/// </summary>
		public static string BackupPath { get { return backupPath + "\\Backup"; }set { backupPath = value; } }

		#endregion

		#region 全局变量
		/// <summary>
		/// 错误提示设置
		/// </summary>
		public static bool ErrorTip { get; set; }

		/// <summary>
		/// 全局自动同步
		/// </summary>
		public static bool AutoSync { get; set; }

		/// <summary>
		/// 全局自动备份
		/// </summary>
		public static bool AutoBackup { get; set; }

		/// <summary>
		/// 后台运行
		/// </summary>
		public static  bool BackgroundRunning { get; set; }

		/// <summary>
		/// 文件监控
		/// </summary>
		public static  bool FileMonitoring { get; set; }

		/// <summary>
		/// 开机启动
		/// </summary>
		public static  bool StartingUp { get; set; }

		/// <summary>
		/// USB触发
		/// </summary>
		public static  bool UsbTrigger { get; set; }

		/// <summary>
		/// 时间触发
		/// </summary>
		public static bool TimeTrigger { get; set; }

		/// <summary>
		/// 设置或返回一个布尔值，表示是否有任务在运行
		/// </summary>
		public static bool IsRunning { get; set; }
		#endregion



		#region 公用静态方法

		#region 对象读取写入

		/// <summary>
		/// 对象读取器
		/// </summary>
		/// <param name="fileName">文件名称</param>
		/// <returns></returns>
		public static object ReadObject ( string fileName ) {
				try {
					FileInfo tmpObject = new FileInfo( fileName );
					if (tmpObject.Exists) {
						Stream stream = new FileStream( tmpObject.FullName, FileMode.Open, FileAccess.Read );
						BinaryFormatter bF = new BinaryFormatter();
						object readedObject = (object)bF.Deserialize( stream );
						stream.Close();
						return readedObject;
					}
					return null;
				} catch(Exception ex) {
				MessageBox.Show( ex.ToString(), "错误", MessageBoxButton.OK, MessageBoxImage.Error );
					return null;
				}
			}

			/// <summary>
			/// 对象写入器
			/// </summary>
			/// <param name="fileName">写入文件的名称</param>
			/// <param name="obj">希望写入的对象</param>
			/// <returns></returns>
			public static bool WriteObject ( string fileName, Object obj ) {
				try {
					Stream stream = new FileStream( fileName, FileMode.Create, FileAccess.Write );
					BinaryFormatter bF = new BinaryFormatter();
					bF.Serialize( stream, obj );
					stream.Close();
					return true;
				} catch (Exception ex) {
					MessageBox.Show( ex.ToString(), "错误", MessageBoxButton.OK, MessageBoxImage.Error );
					return false;
				}
			}
        #endregion

        #region 文件查找
        /// <summary>
        /// 查找文件，并返回LIst
        /// </summary>
        /// <param name="root">查找的根目录</param>
        /// <returns>返回List《FileInfo》</returns>
        public static List<FileInfo> GetFileList(string root) {
            DirectoryInfo rootDir = new DirectoryInfo(root);
            List<FileInfo> tmpList = new List<FileInfo>();
            DirectoryInfo[] dirList;
            try {
                dirList = rootDir.GetDirectories();
                foreach (DirectoryInfo info in dirList) {
                    tmpList.AddRange(GetFileList(info.FullName));
                }
                tmpList.AddRange(rootDir.GetFiles());
                return tmpList;
            } catch(DirectoryNotFoundException e) {
                throw e;
            }
        }

        /// <summary>
        /// 删除文件夹和里面的所有文件
        /// </summary>
        /// <param name="root">根目录</param>
        public static void DeleteDirectory(DirectoryInfo root) {
            FileInfo[] tmpList = root.GetFiles();
            DirectoryInfo[] dirList;
            try {
                dirList = root.GetDirectories();
                foreach (DirectoryInfo info in dirList) {
                    DeleteDirectory(info);
                }
                foreach(FileInfo f in tmpList) {
                    f.Delete();
                }
                root.Delete();
            } catch (DirectoryNotFoundException e) {
                throw e;
            }
        }
        #endregion


        #endregion




    }
}
