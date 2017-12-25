using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 文件同步助手.Logic {
	/// <summary>
	/// 包装类，封装任务的各种信息，用于保存任务数据
	/// </summary>
	[Serializable]
	public  class TaskItem {
		#region 属性

		/// <summary>
		/// 基准是主路径的属性
		/// </summary>
		public bool IsDirectory { get; set; }

		/// <summary>
		/// 任务的标题
		/// </summary>
		public string TaskName { get; set; }

		/// <summary>
		/// 主路径，可以是文件也可以是文件夹
		/// </summary>
		
		public string LocalPath { get; set; }

		/// <summary>
		/// 目标路径，只能是路径
		/// </summary>
		public string TargetPath { get; set; }

		/// <summary>
		/// 判断主路径的文件或文件夹是否存在，目标路径是否存在
		/// </summary>
		public bool Exists { get {
				if (IsDirectory) {
					if (Directory.Exists( LocalPath ) && Directory.Exists( TargetPath )) {
						return true;
					} else {
						return false;
					}
				} else {
					if (File.Exists( LocalPath ) && Directory.Exists( TargetPath )) {
						return true;
					}else {
						return false;
					}
				}
		} }

		/// <summary>
		/// 是否是跳过的项目
		/// </summary>
		public bool IsSkipItem { get; set; }

		/// <summary>
		/// 创建日期
		/// </summary>
		public DateTime Time { get; set; }


		////======================================备份相关

		/// <summary>
		/// 跳过最大
		/// </summary>
		public long SkipMaxSize { get; set; }

		/// <summary>
		/// 跳过最小
		/// </summary>
		public long SkipMinSize { get; set; }

		/// <summary>
		/// 备份是否打开
		/// </summary>
		public bool AutoBackup { get; set; }

		/// <summary>
		/// 备份筛选
		/// </summary>
		public bool BackupFiltrate { get; set; }

		/// <summary>
		/// 自动同步
		/// </summary>
		public bool AutoSync { get; set; }

		/// <summary>
		/// 无效项
		/// </summary>
		public bool IsInvlidItem { get; set; }


		#endregion

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="isDirectory">是否是文件夹</param>
		/// <param name="mainPath">主路径</param>
		/// <param name="targetPath">目标路径</param>
		/// <param name="taskName">标题</param>
		public TaskItem(bool isDirectory,string mainPath, string targetPath,string taskName) {
			IsDirectory = isDirectory;
			LocalPath = mainPath;
			TargetPath = targetPath;
			TaskName = taskName;
			Time = DateTime.Today;
		}

		/// <summary>
		/// 设置筛选备份的区间
		/// </summary>
		/// <param name="min">最小区间</param>
		/// <param name="max">最大区间</param>
		public void SetBackupSkipSize(long min,long max) {
			SkipMaxSize = max;
			SkipMinSize = min;
		}
		/// <summary>
		/// 设置备份开关和备份筛选开关
		/// </summary>
		/// <param name="backup">备份开关</param>
		/// <param name="isSkip">筛选开关</param>
		public void SetBackupSkipState(bool backup,bool isSkip,bool sync) {
			AutoBackup = backup;
			BackupFiltrate = isSkip;
			AutoSync = sync;
		}

	}
}
