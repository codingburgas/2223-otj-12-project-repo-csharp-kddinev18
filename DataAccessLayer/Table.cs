using System.Data.Common;

namespace DataAccessLayer
{
    public class Table
    {
        public string Name { get; set; }
        public HashSet<Column> Columns { get; set; }

        public Table(string name)
        {
            Name = name;
            Columns = new HashSet<Column>();
        }

    }
}