using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using dashing.net.common;
using dashing.net.streaming;

namespace dashing.net.jobs
{
    [Export(typeof(IJob))]
    public class Twitter : IJob
    {
        private const string SearchTerm = "#todayilearned";

        public Lazy<Timer> Timer { get; private set; }

        public Twitter()
        {
            Timer = new Lazy<Timer>(() => new Timer(SendMessage, null, TimeSpan.Zero, TimeSpan.FromMinutes(2)));
        }

        private void SendMessage(object message)
        {
            var authentication = TwitterHelper.Authenticate();

            if (authentication == null)
            {
                Dashing.SendMessage(new { id = "twitter_mentions", comments = new object [] { new { name = "dashing.net", body = "please configure your consumer key/secret", avatar = "" } } });

                return;
            }

            using (var client = new WebClient())
            {
                var url = string.Format("https://api.twitter.com/1.1/search/tweets.json?q={0}", HttpUtility.UrlEncode(SearchTerm));

                client.Headers.Add("Authorization", string.Format("{0} {1}", authentication.TokenType, authentication.AccessToken));

                using (var data = client.OpenRead(url))
                {
                    if (data == null)
                    {
                        return;
                    }
                    
                    var reader = new StreamReader(data);

                    var results = JsonConvert.DeserializeObject<SearchResults>(reader.ReadToEnd());

                    Dashing.SendMessage(new { id = "twitter_mentions", comments = results.Results.Select(n => new { name = n.User.Name, body = n.Text, avatar = n.User.ProfileImageUrl }) });
                }
            }
        }
    }

    /// <summary>
    /// Adopted from the work done by http://stackoverflow.com/a/17071447
    /// </summary>
    public static class TwitterHelper
    {
        public static TwitAuthenticateResponse Authenticate()
        {
            // You need to set your own keys and screen name
            const string oAuthConsumerKey = "YOUR_CONSUMER_KEY";
            const string oAuthConsumerSecret = "YOUR_CONSUMER_SECRET";
            const string oAuthUrl = "https://api.twitter.com/oauth2/token";

            // Do the Authenticate
            const string authHeaderFormat = "Basic {0}";

            var authHeader = string.Format(authHeaderFormat,
                 Convert.ToBase64String(Encoding.UTF8.GetBytes(Uri.EscapeDataString(oAuthConsumerKey) + ":" +
                        Uri.EscapeDataString((oAuthConsumerSecret)))
                        ));

            const string postBody = "grant_type=client_credentials";

            var authRequest = (HttpWebRequest)WebRequest.Create(oAuthUrl);
            authRequest.Headers.Add("Authorization", authHeader);
            authRequest.Method = "POST";
            authRequest.ContentType = "application/x-www-form-urlencoded;charset=UTF-8";
            authRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            using (var stream = authRequest.GetRequestStream())
            {
                byte[] content = ASCIIEncoding.ASCII.GetBytes(postBody);
                stream.Write(content, 0, content.Length);
            }

            authRequest.Headers.Add("Accept-Encoding", "gzip");

            try
            {
                var authResponse = authRequest.GetResponse();

                // deserialize into an object
                TwitAuthenticateResponse twitAuthResponse;
                using (authResponse)
                {
                    using (var reader = new StreamReader(authResponse.GetResponseStream()))
                    {
                        var objectText = reader.ReadToEnd();
                        twitAuthResponse = JsonConvert.DeserializeObject<TwitAuthenticateResponse>(objectText);
                    }
                }

                return twitAuthResponse;
            }
            catch (WebException)
            {
                return null;
            }
        }

        public class TwitAuthenticateResponse 
        {
            public string TokenType { get; set; }
            public string AccessToken { get; set; }
        }
    }
    
    public class SearchResults
    {
        [JsonProperty("statuses")]
        public List<SearchResult> Results { get; set; } 
    }

    public class SearchResult
    {
        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("user")]
        public User User { get; set; }
    }

    public class User
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("profile_image_url")]
        public string ProfileImageUrl { get; set; }
    }
}
