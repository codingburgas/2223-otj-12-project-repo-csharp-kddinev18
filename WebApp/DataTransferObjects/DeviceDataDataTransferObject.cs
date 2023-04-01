using System.Text.Json.Nodes;

namespace WebApp.DataTransferObjects
{
    public class DeviceDataDataTransferObject
    {
        public IEnumerable<string> Infrastructure { get; set; }
        public IEnumerable<JsonObject> Data { get; set; }
    }
}
