using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApp.DAL.Models
{
    public class DeviceDataModel
    {
        public DeviceDataModel(IEnumerable<Dictionary<string, object>> data)
        {
            Infrastructure = new List<string>();
            Data = new List<List<string>>();
            foreach (KeyValuePair<string, object> infrasstructure in data.First())
            {
                Infrastructure.Add(infrasstructure.Key);
            }

            foreach (Dictionary<string, object> row in data)
            {
                Data.Add(new List<string>());
                foreach (KeyValuePair<string, object> columnData in row)
                {
                    Data.Last().Add(columnData.Value.ToString());
                }
            }
        }
        public List<string> Infrastructure { get; set; }
        public List<List<string>> Data { get; set; }
    }
}
