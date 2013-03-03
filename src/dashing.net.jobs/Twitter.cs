using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using dashing.net.streaming;

namespace dashing.net.jobs
{
    public class Twitter : IJob
    {
        private const string SearchTerm = "#todayilearned";

        public Lazy<Timer> Timer { get; private set; }

        public Twitter()
        {
            Timer = new Lazy<Timer>(() => new Timer(SendMessage, null, TimeSpan.Zero, TimeSpan.FromSeconds(2)));

            var start = Timer.Value;
        }

        private void SendMessage(object message)
        {
            using (var client = new WebClient())
            {
                var url = string.Format("http://search.twitter.com/search.json?q={0}", HttpUtility.UrlEncode(SearchTerm));
               
                using (var data = client.OpenRead(url))
                {
                    if (data == null)
                    {
                        return;
                    }
                    
                    var reader = new StreamReader(data);

                    var results = JsonConvert.DeserializeObject<SearchResults>(reader.ReadToEnd());

                    Dashing.SendMessage(new { id = "twitter_mentions", comments = results.Results.Select(n => new { name = n.FromUser, body = n.Text, avatar = n.ProfileImageUrl }) });
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
