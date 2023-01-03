namespace DataAccessLayer
{
    public class Column
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public Table Table { get; set; }
        public HashSet<Tuple<string, object>> Constraints { get; set; }
        public Column(string name, string type, Table table)
        {
            Constraints = new HashSet<Tuple<string, object>>();
            Name = name;
            Type = type;
            Table = table;
        }
        public void AddConstraint(Tuple<string, object> constraint)
        {
            Constraints.Add(constraint);
        }
        public override string ToString()
        {
            string container = $"[{Name}] {Type} ";
            foreach (Tuple<string, object> constraint in Constraints)
            {
                switch (constraint.Item1)
                {
                    case "NOT NULL":
                        container += " NOT NULL ";
                        break;
                    case "UNIQUE":
                        container += " UNIQUE ";
                        break;
                    case "PRIMARY KEY":
                        if(constraint.Item2 as string != "first" && constraint.Item2 as string != "second")
                            container += " IDENTITY(1,1) ";
                        break;
                    case "FOREIGN KEY":
                        Column foreignKeyColumn = constraint.Item2 as Column;
                        Table foreignKeyTable = Table.Database.Tables.Where(table => table.Columns.Select(column => column.Name).Contains(foreignKeyColumn.Name)).First();
                        container += $" FOREIGN KEY REFERENCES {foreignKeyTable.Name}({foreignKeyColumn.Name}) ";
                        break;
                    case "CHECK":
                        container += $" CHECK {constraint.Item2 as string} ";
                        break;
                    case "DEFAULT":
                        container += $" DEFAULT {constraint.Item2 as string} ";
                        break;
                    default:
                        break;
                }
            }
            return container + ",\n";
        }
    }
}