using System;
using System.Collections.Generic;
using System.IO;
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
	/// DetailItem.xaml 的交互逻辑
	/// </summary>
	public partial class DetailItem : UserControl {

		public DetailItem (FileInfo file) {
			InitializeComponent();
           // this.ImageLab.Source= new BitmapImage(new Uri(@"/Resoure/unknow_s.png", UriKind.Relative));
            try {
                FileNameLab.Content = file.Name;
                PositionLab.Text = file.DirectoryName;
                PositionLab.ToolTip = file.DirectoryName;
                Size = file.Length;
                TimeLab.Text = "时间：" + file.LastWriteTime.ToString();
                this.ImageLab.Source = ApplicationResoure.GetTypeImage(file.Name);
            } catch(Exception e) {
                FileNameLab.Content = e.Message;
                PositionLab.Text = e.Message;
                Size = 0;
                TimeLab.Text = "读取失败";
            }
		}

        #region 属性和字段
        public long Size {
            get { return this.size; }
            set {
                size = value;
                //取消小数，自动转换单位
                if (value < 1024) {
                    this.SizeLab.Text = "大小：" + value.ToString() + " B";
                } else if (value >= 1024 && value < Math.Pow(1024, 2)) {
                    this.SizeLab.Text = "大小：" + Convert.ToInt32((value / 1024)).ToString() + " KB";
                } else if (value >= Math.Pow(1024, 2) && value < Math.Pow(1024, 3)) {
                    this.SizeLab.Text = "大小：" + Convert.ToInt32((value / Math.Pow(1024, 2))).ToString() + " MB";
                } else {
                    this.SizeLab.Text = "大小：" + Convert.ToInt32((value / Math.Pow(1024, 3))).ToString() + " GB";
                }
            }
        }
        private long size;


        #endregion


        #region 颜色
        private Brush MouseOverColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFB4B4B4"));
        private Brush MouseNormalColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFFFF"));
        #endregion

        #region 效果实现
        private void ContentGrid_MouseEnter(object sender, MouseEventArgs e) {
            ContentGrid.Background = MouseOverColor;
        }

        private void ContentGrid_MouseLeave(object sender, MouseEventArgs e) {
            ContentGrid.Background = MouseNormalColor;
        }

        private void ContentGrid_MouseRightButtonDown(object sender, MouseButtonEventArgs e) {
            ContentGrid.Background = MouseOverColor;
        }
        #endregion
    }
}
