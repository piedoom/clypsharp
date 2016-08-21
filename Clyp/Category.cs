using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clyp
{
    public class Category
    {
        [JsonProperty(PropertyName = "Title")]
        public string Title { get; set; }

        [JsonProperty(PropertyName = "Location")]
        public Flurl.Url Url { get; set; }

        public Category(Flurl.Url url, string title = null)
        {
            Url = url;
            Title = title;
        }
    }
}
