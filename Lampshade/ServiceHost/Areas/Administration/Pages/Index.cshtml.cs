using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace ServiceHost.Areas.Administration.Pages
{
    public class IndexModel : PageModel
    {
        public Chart DoughnutDataset { get; set; }
        public List<Chart> BarLineDataSet { get; set; }
        public void OnGet()
        {
            BarLineDataSet = new List<Chart>
            {
                new()
                {
                    Label = "Apple",
                    Data = new List<int> { 100, 200, 250, 170, 50 },
                    BackgroundColor = new []{"#118ab2"},
                    BorderColor = "#118ab2"
                },
                new()
                {
                    Label = "Samsung",
                    Data = new List<int> { 200, 300, 400, 150, 350 },
                    BackgroundColor = new []{"#fde4cf"},
                    BorderColor = "#118ab2"
                },
                new()
                {
                    Label = "Total",
                    Data = new List<int> { 300, 500, 650, 320, 400 },
                    BackgroundColor = new []{"#2a9d8f"},
                    BorderColor = "#118ab2"
                }
            };
            DoughnutDataset = new Chart
            {
                Label = "Apple",
                Data = new List<int> { 100, 200, 250, -100, 50 },
                BackgroundColor = new []{ "#118ab2", "#fde4cf", "#2a9d8f", "#f72585", "#97a97c" },
                BorderColor = "#ffffff"
            };
        }
    }

    public class Chart
    {
        [JsonProperty(PropertyName = "label")]
        public string Label { get; set; }
        [JsonProperty(PropertyName = "backgroundColor")]
        public string[] BackgroundColor { get; set; }
        [JsonProperty(PropertyName = "borderColor")]
        public string BorderColor { get; set; }
        [JsonProperty(PropertyName = "data")]
        public List<int> Data { get; set; }
    }
}
