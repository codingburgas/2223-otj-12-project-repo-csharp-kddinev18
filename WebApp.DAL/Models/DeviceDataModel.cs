using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

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
                ;
                Data.Add(new List<string>());
                foreach (KeyValuePair<string, object> columnData in row)
                {
                    if (columnData.Key == "Created")
                    {
                        DateTime dateTime = DateTime.ParseExact(columnData.Value.ToString().Substring(0, columnData.Value.ToString().LastIndexOf('.')), "yyyy-MM-dd'T'HH:mm:ss", null);
                        Data.Last().Add(dateTime.ToShortDateString() + " " + dateTime.ToShortTimeString());
                    }
                    else
                        Data.Last().Add(columnData.Value.ToString());
                }
            }
        }
        public DeviceDataModel()
        {
            Infrastructure = new List<string>();
            Data = new List<List<string>>();
            XData = 0;
            YData = 1;
            ChartType = "table";
        }
        public List<string> Infrastructure { get; set; }
        public List<List<string>> Data { get; set; }

        public int XData { get; set; }
        public int YData { get; set; }
        public string ChartType { get; set; }
    }
}
