namespace DataAccessLayer
{
    public class Column
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public Table Table { get; set; }
        public HashSet<Tuple<string, object>> Constraints { get; set; }

    }
}