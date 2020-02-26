using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGA.Models
{
    public class ApplicationRestResult
    {
        public List<string> Columns = new List<string>();

        public void AddColumns(string column)
        {
            Columns.Add(column);
        }
    }
}
