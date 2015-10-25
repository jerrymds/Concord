using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Model
{
    public class JsonResultModel
    {
        public bool IsSuccess { get; set; }

        public string Message { get; set; }

        public object Data { get; set; }

        public int DataTotalCount { get; set; }
    }
}
