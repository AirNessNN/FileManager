using System.IO;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using 文件同步助手.Logic;

namespace 文件同步助手.Control {
    /// <summary>
    /// BackupItem.xaml 的交互逻辑
    /// </summary>
    public partial class BackupItem : UserControl {

        #region 字段和属性
        /// <summary>
        /// 列表控制器
        /// </summary>
        private BackupItemControl itemControl;

        /// <summary>
        /// 备份对象
        /// </summary>
        private DirectoryInfo backupItem = null;


         #endregion


        /// <summary>
        /// 构造一个包装备份文件夹的类
        /// </summary>
        /// <param name="itemControl">父类对象</param>
        /// <param name="dir">包装的BackupDIrectory</param>
        public BackupItem(BackupItemControl itemControl,DirectoryInfo dir) {
            InitializeComponent();

            this.itemControl = itemControl;
            backupItem = dir;
            //更新名字和时间
            Update(dir);
        }

        /// <summary>
        /// 更新该列表项的所有数据
        /// </summary>
        public void Update(DirectoryInfo dir) {
            string name = dir.Name.Substring(0, dir.Name.IndexOf('_'));
            NameLab.Content = name;
            TimeBlo.Text = dir.LastWriteTime.ToString();
        }

        /// <summary>
        /// 设置多选框选择的状态
        /// </summary>
        /// <param name="b"></param>
        public void SetCheckBoxState(bool b) {
            SelectionBox.IsChecked = b;
        }

        /// <summary>
        /// 返回多选框选择的状态
        /// </summary>
        /// <returns>返回状态</returns>
        public bool GetCheckBoxState() {
            return SelectionBox.IsChecked==true ? true : false;
        }

        /// <summary>
        /// 删除本项目
        /// </summary>
        public void DeleteThis() {
            itemControl.DeleteItem(this);
            //删除文件夹
            if (backupItem != null) {
                ApplicationResoure.DeleteDirectory(backupItem);
            }
        }

        private void ContentGrid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) {

        }

        private void ContentGrid_MouseRightButtonUp(object sender, MouseButtonEventArgs e) {

        }






        #region 颜色
        private Brush MouseOverColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFB4B4B4"));
        private Brush MouseNormalColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFFFF"));
        #endregion


        //事件
        #region 效果实现
        private void ContentGrid_MouseEnter(object sender, MouseEventArgs e) {
            if (!itemControl.IsMultiSelect) {
                this.ContentGrid.Background = MouseOverColor;
            }
        }

        private void ContentGrid_MouseLeave(object sender, MouseEventArgs e) {
            if (!itemControl.IsMultiSelect) {
                this.ContentGrid.Background = MouseNormalColor;
            }
        }

        private void ContentGrid_MouseRightButtonDown(object sender, MouseButtonEventArgs e) {
            if (!itemControl.IsMultiSelect) {
                this.ContentGrid.Background = MouseOverColor;
            }
        }
        #endregion

        private void MenuItem_Click(object sender, System.Windows.RoutedEventArgs e) {
            DeleteThis();
        }
    }
}
