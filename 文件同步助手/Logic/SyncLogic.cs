using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using 文件同步助手.Control;

namespace 文件同步助手.Logic {

	/// <summary>
	/// 同步逻辑和算法
	/// <para>作为每个SyncItem的固有属性，所以可以直接忽略自动同步和跳过项目的状态属性，因此只有一个备份筛选属性</para>
	/// <para>在实例化的时候传入参数，在搜索结束之后，开始备份，代码会索引备份筛选的值选择是否进行备份和是否进行备份筛选</para>
	/// </summary>
	///
	public class SyncLogic {

		#region 字段和属性
		private DirectoryInfo LocalDirectory { get; set; }
		private FileInfo LocalFile { get; set; }

		private DirectoryInfo TargetDirectory { get; set; }
		private FileInfo TargetFile { get; set; }

		private bool IsDirectory { get; set; }

		private SyncItem ItemControl;

        //项目名字
        private string ItemName { get; set; }

		//备份相关
		private bool IsAutoBackup { get; set; }

		private bool IsFiltrateBackup { get; set; }

		private long MaxFiltrate { get; set; }

		private long MinFiltrate { get; set; }

		#endregion


		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="path1">主路径</param>
		/// <param name="path2">目标路径</param>
		/// <param name="isDir">是否是文件夹</param>
		public SyncLogic (string path1,string path2,bool isDir,bool isFiltrate,long max,long min,SyncItem item) {

			if(IsDirectory=isDir) {
				LocalDirectory = new DirectoryInfo( path1 );
				TargetDirectory = new DirectoryInfo( path2 + "\\" + LocalDirectory.Name );
				IsFiltrateBackup = isFiltrate;
				MaxFiltrate = max;
				MinFiltrate = min;
			}else {
				LocalFile = new FileInfo( path1 );
				TargetFile = new FileInfo( path2+"\\"+LocalFile.Name );
			}
			this.ItemControl = item;
            IsAutoBackup = ItemControl.IsAutoBackup;
            ItemName = ItemControl.ItemName;
		}


		/// <summary>
		/// 单个任务的同步逻辑
		/// </summary>
		public void SyncThis() {
			//设置开始标志
			ItemControl.SetSyncFlag( true );
			ApplicationResoure.IsRunning = true;
            ApplicationResoure.SetSyncImg(true);

			//区分模式
			if (IsDirectory) {

				//确认主目录存在
				if (LocalDirectory.Exists) {

					//两者都存在
					if (TargetDirectory.Exists) {
						//目标存在主目录存在	
						ApplicationResoure.MessageSender( ItemName+"：开始填充文件夹。", StateBoxMod.Information );
						//获取到文件夹列队，循环创建文件夹，直到所有文件夹对应为止
						List<DirectoryMap> tmpDirectoryMap = GetDirectoryMap( LocalDirectory, TargetDirectory );
						while (tmpDirectoryMap.Count > 0) {
							//创建不存在的目录
							CreatDirectory( tmpDirectoryMap );
							tmpDirectoryMap = GetDirectoryMap( LocalDirectory, TargetDirectory );
						}

						//获取文件映射表
						List<FileMap> tmpFileMap = GetFileMap( LocalDirectory, TargetDirectory );

						//备份文件
						//全局备份开关打开才开始检查每个任务的备份开关
						if(ApplicationResoure.AutoBackup) {
							if (IsAutoBackup) {
								BackupFile( tmpFileMap );
							}
						}

						ApplicationResoure.MessageSender( ItemName + "：开始复制文件。", StateBoxMod.Information );
						//复制文件
						CreatFile( tmpFileMap );

					} else {
						ApplicationResoure.MessageSender( ItemName + "：未检测到目标文件夹，采用填充模式，将跳过备份。", StateBoxMod.Information );
						//当目标目录不存在，主目录存在时，复制全部文件到目标目录
						//创建目标目录
						TargetDirectory.Create();

						ApplicationResoure.MessageSender( ItemName + "：开始填充文件夹。", StateBoxMod.Information );
						//填充文件夹
						List<DirectoryMap> tmpDirectoryMap = GetDirectoryMap( LocalDirectory, TargetDirectory );
						while (tmpDirectoryMap.Count > 0) {
							CreatDirectory( tmpDirectoryMap );
							tmpDirectoryMap = GetDirectoryMap( LocalDirectory, TargetDirectory );
						}

						ApplicationResoure.MessageSender( ItemName + "：开始复制文件。", StateBoxMod.Information );
						//开始填充文件
						List<FileMap> tmpFileMap = GetFileMap( LocalDirectory, TargetDirectory );
						//复制文件
						CreatFile( tmpFileMap );
					}
				} else {
					//确认目标目录存在
					if (TargetDirectory.Exists) {
						ApplicationResoure.MessageSender( ItemName + "：未检测到主文件夹，采用填充模式，将跳过备份。", StateBoxMod.Information );
						//创建目录
						LocalDirectory.Create();

						ApplicationResoure.MessageSender( ItemName + "：开始填充文件夹。", StateBoxMod.Information );
						//填充文件夹
						List<DirectoryMap> tmpDirectoryMap = GetDirectoryMap( LocalDirectory, TargetDirectory );
						while (tmpDirectoryMap.Count > 0) {
							CreatDirectory( tmpDirectoryMap );
							tmpDirectoryMap = GetDirectoryMap( LocalDirectory, TargetDirectory );
						}

						ApplicationResoure.MessageSender( ItemName + "：开始复制文件。", StateBoxMod.Information );
						//开始填充文件
						List<FileMap> tmpFileMap = GetFileMap( LocalDirectory, TargetDirectory );
						//复制文件
						CreatFile( tmpFileMap );

					} else {
						//当主目录不存在目标目录不存在时，同步失败
						ApplicationResoure.MessageSender( ItemName + "：找不到主目录和目标目录，无法同步，该任务已经无效", Control.StateBoxMod.Error );
						if (ApplicationResoure.ErrorTip) {
							MessageBox.Show( "无法同步 " + ItemName + "，详细情况请查阅日志", "错误", MessageBoxButton.OK, MessageBoxImage.Error );
						}
                        //设为无效项
                        this.ItemControl.SetSyncItemState( false );
                    }
				}

			} else {
				//文件模式==============================================================================================
				if (TargetFile.Exists && LocalFile.Exists) {

					//备份文件
					DirectoryInfo backup = null;
					if (!Directory.Exists( ApplicationResoure.BackupPath )) {
						Directory.CreateDirectory( ApplicationResoure.BackupPath );

						backup = new DirectoryInfo(
						ApplicationResoure.BackupPath + "\\" +
						ItemName + "_" + DateTime.Today.ToLongDateString() + "_" +
						 DateTime.Now.ToShortTimeString().Replace( ":", "点" ) );

						Directory.CreateDirectory( backup.FullName );
						ApplicationResoure.MessageSender( "备份文件夹创建：" + backup.FullName, Control.StateBoxMod.Information );
					}

					//同步文件
					if (LocalFile.LastWriteTime > TargetFile.LastWriteTime) {
						//备份文件
						if(ApplicationResoure.AutoBackup) {
							if(IsAutoBackup) {
								if(IsAutoBackup) {
									try {
										TargetFile.CopyTo( backup.FullName + "\\" + TargetFile.Name );
										ApplicationResoure.MessageSender( "备份文件：" + TargetFile.FullName + " 完成", Control.StateBoxMod.Content );
									}catch(Exception e) {
										ApplicationResoure.MessageSender( TargetFile.FullName + " 备份出现错误，已跳过：" + e.Message, StateBoxMod.Error );
									}
								}
							}
						}

						//复制文件
						try {
							LocalFile.CopyTo( TargetFile.FullName, true );
							ApplicationResoure.MessageSender( "文件同步：" + LocalFile.FullName + " 到 " + TargetFile.FullName, StateBoxMod.Content );
						}catch(Exception e) {
							ApplicationResoure.MessageSender( LocalFile.FullName + " 出错，" + "已跳过同步" + e.Message, StateBoxMod.Error );
						}
					} else {
						//备份文件
						if (ApplicationResoure.AutoBackup) {
							if (IsAutoBackup) {
								if (IsAutoBackup) {
									try {
										LocalFile.CopyTo( backup.FullName + "\\" + LocalFile.Name );
										ApplicationResoure.MessageSender( "备份文件：" + LocalFile.FullName + " 完成", StateBoxMod.Content );
									}catch(Exception e) {
										ApplicationResoure.MessageSender(LocalFile.FullName+" 备份出现错误，已跳过："+ e.Message, StateBoxMod.Error );
									}
								}
							}
						}
						//复制文件
						try {
							TargetFile.CopyTo( LocalFile.FullName, true );
							ApplicationResoure.MessageSender( "文件同步：" + TargetFile.FullName + " 到 " + LocalFile.FullName, StateBoxMod.Content );
						}catch(Exception e) {
							ApplicationResoure.MessageSender( TargetFile.FullName+" 出错，" +"已跳过同步"+ e.Message , StateBoxMod.Error );
						}
					}
				}else if(LocalFile.Exists&&!TargetFile.Exists) {
					LocalFile.CopyTo( TargetFile.FullName, true );
					ApplicationResoure.MessageSender( "文件同步：" + LocalFile.FullName + " 到 " + TargetFile.FullName, StateBoxMod.Content );
				} else if(TargetFile.Exists&&!LocalFile.Exists) {
					TargetFile.CopyTo( LocalFile.FullName, true );
					ApplicationResoure.MessageSender( "文件同步：" + TargetFile.FullName + " 到 " + LocalFile.FullName, StateBoxMod.Content );
				} else {
					ApplicationResoure.MessageSender( ItemName + "：找不到主文件和目标文件，无法同步，该任务已经无效", Control.StateBoxMod.Error );
					if(ApplicationResoure.ErrorTip) {
						MessageBox.Show( "无法同步 " + ItemName + "，详细情况请查阅日志", "错误", MessageBoxButton.OK, MessageBoxImage.Error );
					}
					//设为无效项
					this.ItemControl.SetSyncItemState( false );
				}
				ApplicationResoure.MessageSender( ItemName + " 同步完成", StateBoxMod.Information );
			}
			//结束
			ItemControl.SetSyncFlag( false );
			ApplicationResoure.IsRunning = false;
            ApplicationResoure.SetSyncImg(false);
		}



		#region 文件搜索方法
		/// <summary>
		/// 将两个文件夹内所有文件名相同且修改时间不同的文件打包在映射表中，不存在对应的文件一起打包到映射表，DFS实现
		/// </summary>
		/// <param name="d1">目录1</param>
		/// <param name="d2">目录2</param>
		/// <returns>返回映射表</returns>
		private List<FileMap> GetFileMap(DirectoryInfo d1,DirectoryInfo d2) {

			#region 局部变量

			//用于遍历的Dir对象
			DirectoryInfo[] d1_Dir;
			DirectoryInfo[] d2_Dir;

			//用于对比的File对象
			FileInfo[] d1_File;
			FileInfo[] d2_File;

			//访问标记
			bool[] d1_vis;
			bool[] d2_vis;

			//传递的Map列表
			List<FileMap> tmpMap = new List<FileMap>();
			#endregion

			#region 定义操作
			//获取D1的根目录和根文件
			d1_Dir = d1.GetDirectories();
			d1_File = d1.GetFiles();

			//获取D2的根目录和根文件
			d2_Dir = d2.GetDirectories();
			d2_File = d2.GetFiles();

			//获取同比的访问数组
			d1_vis = new bool[d1_File.Count()];
			d2_vis = new bool[d2_File.Count()];
			#endregion


			//开始递归所有文件夹
			foreach (DirectoryInfo dir1 in d1_Dir) {
				foreach (DirectoryInfo dir2 in d2_Dir) {
					if (dir1.Name == dir2.Name) {
						tmpMap.AddRange( GetFileMap( dir1, dir2 ) );
					}
				}
			}

			//回溯
			#region 目录操作

			#region 剪枝
			//传入的两个目录其中有一个目录无文件
			if (d1_File.Count() == 0 && d2_File.Count() == 0)
				return tmpMap;

			//单方无目录
			if (d1_File.Count() == 0 && d2_File.Count() > 0) {
				foreach (var d in d2_File) {
					tmpMap.Add( new FileMap( d, new FileInfo(d1.FullName+"\\"+d.Name) ) );
				}
				return tmpMap;
			}
			if (d2_File.Count() == 0 && d1_File.Count() > 0) {
				foreach (var d in d1_File) {
					tmpMap.Add( new FileMap( d, new FileInfo( d2.FullName + "\\" + d.Name ) ) );
				}
				return tmpMap;
			}

			#endregion

			//对比
			for (var i=0;i<d1_File.Count();i++) {
				for(var j=0;j<d2_File.Count();j++) {
					if(d2_vis[j]) {
						continue;
					}
					if (d1_File[i].Name == d2_File[j].Name) {
						if(d1_File[i].LastWriteTime > d2_File[j].LastWriteTime) {
							tmpMap.Add( new FileMap( d1_File[i], d2_File[j] ) );
							d1_vis[i] = true;
							d2_vis[j] = true;
							break;
						}else if(d1_File[i].LastWriteTime < d2_File[j].LastWriteTime) {
							tmpMap.Add( new FileMap( d2_File[i], d1_File[j] ) );
							d1_vis[i] = true;
							d2_vis[j] = true;
						}
					}
				}
			}

			//入列
			for (int i = 0; i < d1_vis.Count(); i++) {
				if (!d1_vis[i]) {
					tmpMap.Add( new FileMap( d1_File[i], new FileInfo( d2.FullName + "\\" + d1_File[i].Name) ) );
				}
			}
			for (int i = 0; i < d2_vis.Count(); i++) {
				if (!d2_vis[i]) {
					tmpMap.Add( new FileMap( d2_File[i], new FileInfo( d1.FullName + "\\" + d2_File[i].Name ) ) );
				}
			}


			#endregion
			return tmpMap;
		}


		/// <summary>
		/// 将两个文件夹内所有不对应的文件入列和对应目录一起打包在映射表中，DFS实现
		/// </summary>
		/// <param name="d1">目录1</param>
		/// <param name="d2">目录2</param>
		/// <returns></returns>
		private List<CopyMap>GetCopyMap(DirectoryInfo d1,DirectoryInfo d2) {
			#region 局部变量

			//用于遍历的Dir对象
			DirectoryInfo[] d1_Dir;
			DirectoryInfo[] d2_Dir;

			//用于对比的File对象
			FileInfo[] d1_File;
			FileInfo[] d2_File;

			//传递的Map列表
			List<CopyMap> tmpMap = new List<CopyMap>();
			#endregion

			#region 定义操作
			//获取D1的根目录和根文件
			d1_Dir = d1.GetDirectories();
			d1_File = d1.GetFiles();

			//获取D2的根目录和根文件
			d2_Dir = d2.GetDirectories();
			d2_File = d2.GetFiles();
			#endregion


			//递归所有文件夹
			foreach (DirectoryInfo dir1 in d1_Dir) {
				foreach (DirectoryInfo dir2 in d2_Dir) {
					if (dir1.Name == dir2.Name) {
						tmpMap.AddRange( GetCopyMap( dir1, dir2 ) );
					}
				}
			}

			//回溯
			#region 目录操作
			//当文件数量等于0退出
			if (d1_File.Count()==0&&d2_File.Count()==0)
				return tmpMap;
			
			//入列D1单独的文件
			foreach(FileInfo file1 in d1_File) {
				//文件Flag
				bool flag = false;
				foreach(FileInfo file2 in d2_File) {
					if (file1.Name == file2.Name) {
						flag = true;
						break;
					}
				}
				if(!flag) {
					tmpMap.Add( new CopyMap( file1, d2 ) );
				}
			}

			//入列D2单独的文件
			foreach (FileInfo file1 in d2_File) {
				//文件Flag
				bool flag = false;
				foreach (FileInfo file2 in d1_File) {
					if (file1.Name == file2.Name) {
						flag = true;
						break;
					}
				}
				if (!flag) {
					tmpMap.Add( new CopyMap( file1, d1 ) );
				}
			}
			#endregion
			return tmpMap;
		}

		/// <summary>
		/// 将不匹配的文件夹入列到文件夹映射表中，DFS实现
		/// </summary>
		/// <param name="d1">目录1</param>
		/// <param name="d2">目录2</param>
		/// <returns></returns>
		private List<DirectoryMap>GetDirectoryMap(DirectoryInfo d1,DirectoryInfo d2) {

			//D1中的文件
			DirectoryInfo[] d1_dir;
			//D2中的文件
			DirectoryInfo[] d2_dir;
			//返回的映射表
			List<DirectoryMap> tmpMap = new List<DirectoryMap>();

			d1_dir = d1.GetDirectories();
			d2_dir = d2.GetDirectories();

			//访问表
			bool[] d1_vis = new bool[d1_dir.Count()];
			bool[] d2_vis = new bool[d2_dir.Count()];



			#region 剪枝
			//无目录
			if (d1_dir.Count()==0&&d2_dir.Count()==0) {
				return tmpMap;
			 }

			 //单方无目录
			 if(d1_dir.Count()==0&&d2_dir.Count()>0) {
				foreach(var d in d2_dir) {
					tmpMap.Add( new DirectoryMap( d, d1 ) );
				}
				return tmpMap;
			 }
			 if(d2_dir.Count()==0&&d1_dir.Count()>0) {
				foreach(var d in d1_dir) {
					tmpMap.Add( new DirectoryMap( d, d2 ) );
				}
				return tmpMap;
			 }
			#endregion

			//搜索
			for (int i=0;i<d1_dir.Count();i++) {
				//剪枝
				if(d1_vis[i]) {
					continue;
				}
				for(int j=0;j<d2_dir.Count();j++) {
					//剪枝
					if(d2_vis[j]) {
						continue;
					}
					//判断
					if(d1_dir[i].Name==d2_dir[j].Name) {
						//递归
						tmpMap.AddRange( GetDirectoryMap( d1_dir[i], d2_dir[j] ) );
						d1_vis[i] = true;
						d2_vis[j] = true;
						break;
					}
				}
			 }

			 //入列
			 for(int i=0;i<d1_vis.Count();i++) {
				if(!d1_vis[i]) {
					tmpMap.Add( new DirectoryMap( d1_dir[i], d2 ) );
				}
			 }
			 for(int i=0;i<d2_vis.Count();i++) {
				if(!d2_vis[i]) {
					tmpMap.Add( new DirectoryMap( d2_dir[i], d1 ) );
				}
			 }


			//返回
			return tmpMap;
		}

		#endregion

		#region 写入方法

		/// <summary>
		/// 创建目录
		/// </summary>
		/// <param name="tmpMap">目录映射表</param>
		private void CreatDirectory(List<DirectoryMap> tmpMap) {
			//	循环写入
			foreach(var dir in tmpMap) {
				try {
					dir.Target.Create();
					ApplicationResoure.MessageSender( "目录同步：" +" [ "+dir.Local.FullName+" ] 到 [ " +dir.Target.FullName+" ] ", Control.StateBoxMod.Content );
				}catch(IOException ex) {
					ApplicationResoure.MessageSender( "错误，无法创建目录，将跳过 "+dir.Target.FullName+"。", Control.StateBoxMod.Error );
					if(ApplicationResoure.ErrorTip) {
						MessageBox.Show( ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Error );
					}
				}
			}
		}

		/// <summary>
		/// 文件写入器
		/// </summary>
		/// <param name="tmpMap"></param>
		private void CreatFile(List<FileMap> tmpMap) {
			var sum = 0;
			var fail = 0;
			foreach(var fileMap in tmpMap) {
				try {
					fileMap.Local.CopyTo( fileMap.Target.FullName ,true);
					ApplicationResoure.MessageSender( "文件复制：" + " [ " + fileMap.Local.FullName + " ] 到 [ " + fileMap.Target.FullName + " ] ", Control.StateBoxMod.Content );
					sum++;
				} catch(Exception e) {
					ApplicationResoure.MessageSender( "错误，无法写入文件，将跳过 " + fileMap.Target.FullName + "。", Control.StateBoxMod.Error );
					if (ApplicationResoure.ErrorTip) {
						MessageBox.Show( e.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Error );
					}
					fail++;
				}
			}
			ApplicationResoure.MessageSender( ItemName+" 同步完成：共 " + sum.ToString() + "个文件，失败："+fail.ToString()+" 个。", StateBoxMod.Information );
		}


		private void BackupFile(List<FileMap> tmpMap) {
			//备份地址
			DirectoryInfo backup = null;
			if (!Directory.Exists(ApplicationResoure.BackupPath)) {
				Directory.CreateDirectory( ApplicationResoure.BackupPath );

				backup = new DirectoryInfo( 
				ApplicationResoure.BackupPath + "\\" + 
				ItemName+"_" + DateTime.Today.ToLongDateString() + "_" +
				 DateTime.Now.ToShortTimeString().Replace(":","点") );

				Directory.CreateDirectory( backup.FullName );
				ApplicationResoure.MessageSender( "备份文件夹创建：" + backup.FullName , StateBoxMod.Information );
			}else {
				backup = new DirectoryInfo(
				ApplicationResoure.BackupPath + "\\" +
				ItemName + "_" + DateTime.Today.ToLongDateString() + "_" +
				 DateTime.Now.ToShortTimeString().Replace( ":", "点" ) );

				Directory.CreateDirectory( backup.FullName );
				ApplicationResoure.MessageSender( "备份文件夹创建：" + backup.FullName, StateBoxMod.Information );
			}
			foreach (var item in tmpMap) {
				
				if(IsFiltrateBackup) {
					if (item.Local.Length > MaxFiltrate && item.Local.Length < MinFiltrate) {
						continue;
					}
				}
				item.Local.CopyTo( backup.FullName + "\\" + item.Local.Name, true );
				ApplicationResoure.MessageSender( "文件备份：" + item.Local.FullName ,Control.StateBoxMod.Content);
			}
		}
		#endregion

	}

	/// <summary>
	/// 文件映射表
	/// </summary>
	public class FileMap {

		public FileInfo Local { get; set; }
		public FileInfo Target { get; set; }

		public FileMap(FileInfo f1,FileInfo f2) {
			this.Local = f1;
			this.Target = f2;
		}
	}

	/// <summary>
	/// 单文件映射表
	/// </summary>
	public class CopyMap {

		public FileInfo File { get; set; }
		private DirectoryInfo Directory { get; set; }
		public FileInfo TargetFile { get; set; }

		public CopyMap(FileInfo f,DirectoryInfo d) {
			this.File = f;
			this.Directory = d;
			this.TargetFile = new FileInfo( d.FullName + "\\" + f.Name );
		}
	}

	/// <summary>
	/// 目录映射表 
	/// </summary>
	public class DirectoryMap {
		
		public DirectoryInfo Local { get; set; }
		public DirectoryInfo Target { get; set; }

		public DirectoryMap(DirectoryInfo d1,DirectoryInfo d2) {
			Local = d1;
			Target = new DirectoryInfo( d2.FullName + "\\" + d1.Name );
		}
	}
}
