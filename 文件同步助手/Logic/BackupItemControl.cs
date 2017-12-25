using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using 文件同步助手.Control;

namespace 文件同步助手.Logic {
    public class BackupItemControl {

        /// <summary>
        /// 获取或设置多选模式开关
        /// </summary>
        public bool IsMultiSelect { get; set; }

        /// <summary>
        /// 备份项目列表
        /// </summary>
        public List<BackupItem> BackupItemList { get { return backupItem; } }
        private List<BackupItem> backupItem;


        /// <summary>
        /// 父类页面
        /// </summary>
        private FilePage context;





        /// <summary>
        /// 默认构造函数
        /// </summary>
        public BackupItemControl(FilePage context) {
            this.context = context;
            Initilize();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public void Initilize( ) {

            backupItem = new List<BackupItem>();

            IsMultiSelect = false;

            CheckBackupPath();

        }


        /// <summary>
        /// 检查备份目录
        /// </summary>
        public void CheckBackupPath() {
            DirectoryInfo[] tmpArr = GetDirectoryArray(new DirectoryInfo(ApplicationResoure.BackupPath));
            backupItem = GetBackupItemList(tmpArr);

            //添加到视图
            if (backupItem != null) {
                context.AddAllItem(backupItem);
            }
        }

        /// <summary>
        /// 取得目录数组
        /// </summary>
        /// <param name="info">目标地址</param>
        /// <returns></returns>
        private DirectoryInfo[] GetDirectoryArray(DirectoryInfo info) {
            if (info.Exists) {
                return info.GetDirectories();
            }else {
                return null;
            }
        }

        /// <summary>
        /// 获得备份文件夹的列表项
        /// </summary>
        /// <param name="arr">地址</param>
        /// <returns>返回项目列表</returns>
        private List<BackupItem> GetBackupItemList(DirectoryInfo[] arr) {
            if (arr != null) {
                if (backupItem == null) {
                    backupItem = new List<BackupItem>();
                }
                List<BackupItem> tmpList = new List<BackupItem>();
                foreach(DirectoryInfo d in arr) {
                    BackupItem item = new BackupItem(this, d);
                    
                    tmpList.Add(item);
                }
                return tmpList;
            }
            return null;
        }

        /// <summary>
        /// 设置所有的CheckBox
        /// </summary>
        /// <param name="b">状态</param>
        public void SetAllItemCheckBox(bool b) {
            foreach(BackupItem item in BackupItemList) {
                item.SelectionBox.Visibility = b ? Visibility.Visible : Visibility.Hidden;
            }
        }

        /// <summary>
        /// 删除该项
        /// </summary>
        /// <param name="item">要删除的选项</param>
        public void DeleteItem(BackupItem item) {
            backupItem.Remove(item);
            context.Delete(item);
        }


        public void DeleteAll() {
            foreach(BackupItem item in backupItem) {
                DeleteItem(item);
            }
        }

        /// <summary>
        /// 反选列表项
        /// </summary>
        public void InversionSelection() {
            foreach (BackupItem item in backupItem) {
                item.SetCheckBoxState(!item.GetCheckBoxState());
            }
        }


        /// <summary>
        /// 多选模式的删除选项
        /// </summary>
        public void DeleteSelected() {
            foreach(BackupItem item in backupItem) {
                if (item.GetCheckBoxState()) {
                    item.DeleteThis();
                }
            }
        }
    }
}
