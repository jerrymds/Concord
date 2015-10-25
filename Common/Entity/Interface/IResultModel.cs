using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Threading.Tasks;

namespace Common.Entity.Interface
{
    public interface IResultModel : IResult
    {
        DataSet ResultObject { get; set; }
    }
}
