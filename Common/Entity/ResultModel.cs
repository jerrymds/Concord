using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Threading.Tasks;
using Common.Entity.Interface;

namespace Common.Entity
{
    public class ResultModel : IResultModel, IResult 
    {
        /// <summary>
        /// 執行是否成功
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// 輸出訊息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 更新成功的資料列數
        /// </summary>
        public int AffectCount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// Exception object
        /// </summary>
        public Exception Exception { get; set; }

        /// <summary>
        /// 輸出資料集
        /// </summary>
        public DataSet ResultObject { get; set; }

        public ResultModel()
            : this(false)
        {

        }

        public ResultModel(bool isSuccess)
        {
            this.IsSuccess = isSuccess;
        }
    }
}
