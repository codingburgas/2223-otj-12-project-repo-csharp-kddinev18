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
    }
}