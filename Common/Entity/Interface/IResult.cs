using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Entity.Interface
{
    public interface IResult
    {
        bool IsSuccess { get; set; }

        string Message { get; set; }

        int AffectCount { get; set; }

        int TotalCount { get; set; }

        Exception Exception { get; set; }
    }
}
