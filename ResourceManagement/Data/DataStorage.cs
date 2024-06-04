using ResourceManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
namespace ResourceManagement.Data
{
    public class DataStorage
    {
        private const string FileName = "data.json";

        public List<Shortage> LoadData()
        {
            if (!File.Exists(FileName))
            {
                return [];
            }

            var json = File.ReadAllText(FileName);
            return JsonConvert.DeserializeObject<List<Shortage>>(json);
        }

        public void SaveData(List<Shortage> shortages)
        {
            var json = JsonConvert.SerializeObject(shortages);
            File.WriteAllText(FileName, json);
        }
    }
}
