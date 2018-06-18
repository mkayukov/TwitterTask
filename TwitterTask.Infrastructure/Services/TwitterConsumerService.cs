using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using TweetSharp;
using TwitterTask.Infrastructure.Data;
using TwitterTask.Infrastructure.Services.Interfaces;

namespace TwitterTask.Infrastructure.Services
{
	public class TwitterConsumerService : ITwitterConsumerService
	{
		private readonly AuthCredential _credentials;

		public TwitterConsumerService(AuthCredential credentials)
		{
			_credentials = credentials;
		}

		private string _accessToken;
		private string AccessToken => _accessToken ?? (_accessToken = GetAccessToken());

		private string GetAccessToken()
		{
			var post = WebRequest.Create("https://api.twitter.com/oauth2/token");
			post.Method = "POST";
			post.ContentType = "application/x-www-form-urlencoded";
			post.Headers[HttpRequestHeader.Authorization] = "Basic " + _credentials.Credential;
			var reqbody = Encoding.UTF8.GetBytes("grant_type=client_credentials");
			post.ContentLength = reqbody.Length;
			using (var req = post.GetRequestStream())
			{
				req.Write(reqbody, 0, reqbody.Length);
			}
			try
			{
				string respbody;
				using (var resp = post.GetResponse().GetResponseStream())//there request sends
				{
					var respR = new StreamReader(resp);
					respbody = respR.ReadToEnd();
				}
				var accessToken = JsonConvert.DeserializeObject<dynamic>(respbody)?.access_token;
				return accessToken;
			}
			catch //if credentials are not valid (403 error)
			{
				throw;
			}
		}

		public Tweet[] GetTweets(string screenName, int maxTweetCount)
		{
			var gettimeline = WebRequest.Create($"https://api.twitter.com/1.1/statuses/user_timeline.json?count={maxTweetCount}&screen_name={screenName}&tweet_mode=extended");
			gettimeline.Method = "GET";
			gettimeline.Headers[HttpRequestHeader.Authorization] = "Bearer " + AccessToken;
			try
			{
				using (var resp = gettimeline.GetResponse().GetResponseStream())//there request sends
				{
					var respR = new StreamReader(resp);
					var respbody = respR.ReadToEnd();

					var tweets = JsonConvert.DeserializeObject<dynamic[]>(respbody)
						.Select(x => new Tweet((string)x.created_at, (string)x.full_text))
						.ToArray();
					return tweets;
				}

			}
			catch //401 (access token invalid or expired)
			{
				throw;
			}
		}
	}
}