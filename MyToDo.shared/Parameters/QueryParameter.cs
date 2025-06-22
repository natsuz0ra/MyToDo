using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyToDo.Shared.Parameters
{
    public class QueryParameter
    {
        public int Page { get; set; }
        public int Size { get; set; }
        public string? Search { get; set; } = "";
    }
}
