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
using System.Windows.Shapes;

namespace 文件同步助手.ChildrenWindow {
	/// <summary>
	/// HelpWindow.xaml 的交互逻辑
	/// </summary>
	public partial class HelpWindow : Window {
		public HelpWindow (string str) {
			InitializeComponent();
            switch (str)
            {
                case "about":
                    {
                        About.Visibility = Visibility.Visible;
                        Help.Visibility = Visibility.Hidden;
                        this.Title = "关于";
                        break;
                    }
                case "help":
                    {
                        About.Visibility = Visibility.Hidden;
                        Help.Visibility = Visibility.Visible;
                        this.Title = "帮助";
                        break;
                    }
            }
		}
	}
}
