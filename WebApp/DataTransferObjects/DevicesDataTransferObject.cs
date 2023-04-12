using System.Text.Json.Nodes;

namespace WebApp.DataTransferObjects
{
    public class DevicesDataTransferObject
    {
        public IEnumerable<string> DeviceNames { get; set; }

        public string Name { get; set; }
        public IEnumerable<string> Infrastructure { get; set; }
        public IEnumerable<JsonObject> Data { get; set; }
    }
}
