using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Model
{
    [Serializable]
    public class PageModel
    {
        public int PageSize { get; set; }

        public string Order { get; set; }

        public int? CurrentPage { get; set; }
    }
}
