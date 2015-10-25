using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Model
{
    /// 20120731 Created By Jerry
    /// <summary>
    /// 用來對應Model Property 與多重 Table 欄位
    /// </summary>
    public class TransferModel
    {
        /// <summary>
        /// Model 的 Property 名稱
        /// </summary>
        public string PropertyName
        {
            get;
            set;
        }

        /// <summary>
        /// Table 的欄位名稱
        /// </summary>
        public string[] ColumnName
        {
            get;
            set;
        }

        /// <summary>
        /// 格式化字串
        /// </summary>
        public string Format
        {
            get;
            set;
        }
    }
}
