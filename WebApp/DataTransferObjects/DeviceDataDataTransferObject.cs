using System.Text.Json.Nodes;

namespace WebApp.DataTransferObjects
{
    public class DeviceDataDataTransferObject
    {
        public string Name { get; set; }
        public IEnumerable<string> Infrastructure { get; set; }
        public IEnumerable<JsonObject> Data { get; set; }
    }
}
