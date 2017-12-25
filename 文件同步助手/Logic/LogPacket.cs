using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 文件同步助手.Logic {
	/// <summary>
	/// 日志数据包，用于输出日志
	/// </summary>
	public class LogPacket {
		
		public string Content { get; set; }

		public LogPacket(string time,string content) {
			Content = time + "  " + content;
		}
	}
}
