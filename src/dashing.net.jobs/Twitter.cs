using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;

namespace dashing.net.jobs
{
    public class Twitter : Job
    {
        private const string _searchTerm = "#todayilearned";

        public Twitter(Action<string> sendMessage) : base(sendMessage, "twitter_mentions", TimeSpan.FromSeconds(5))
        {
            Start();
        }

        protected override object GetData()
        {
            using (var client = new WebClient())
            {
                var url = string.Format("http://search.twitter.com/search.json?q={0}", HttpUtility.UrlEncode(_searchTerm));
               
                using (var data = client.OpenRead(url))
                {
                    if (data == null)
                    {
                        return new {};
                    }
                    
                    var reader = new StreamReader(data);

                    var results = JsonConvert.DeserializeObject<SearchResults>(reader.ReadToEnd());

                    return new { comments = results.Results.Select(n => new { name = n.FromUser, body = n.Text, avatar = n.ProfileImageUrl }) };
                }
            }
        }
    }

    public class SearchResults
    {
        [JsonProperty("results")]
        public List<SearchResult> Results { get; set; } 
    }

    public class SearchResult
    {
        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("from_user")]
        public string FromUser { get; set; }

        [JsonProperty("profile_image_url")]
        public string ProfileImageUrl { get; set; }
    }
}
