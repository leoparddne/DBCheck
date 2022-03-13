using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBCheck.ViewModel
{
    public class CheckViewModel: BaseViewModel
    {
        /// <summary>
        /// 数据库校验字段
        /// </summary>
        public string CheckFieldName { get;  set; } = "IS_ENABLED";

        /// <summary>
        /// 结果表列表
        /// </summary>
        public ObservableCollection<string> ResultTableList { get; set; } //= new ObservableCollection<string>() { "a", "x" };
    }
}
