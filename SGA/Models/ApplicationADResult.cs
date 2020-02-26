using System.Collections.Generic;

namespace SGA.Models
{
    public class ApplicationADResult
    {
        public List<string> Columns = new List<string>();

        public void AddColumns(string column)
        {
            Columns.Add(column);
        }
    }
}
