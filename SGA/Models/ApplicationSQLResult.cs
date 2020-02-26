using System.Collections.Generic;

namespace SGA.Models
{
    public class ApplicationSQLResult
    {
        public List<string> Columns = new List<string>();

        public void AddColumns(string column)
        {
            Columns.Add(column);
        }
    }
}
