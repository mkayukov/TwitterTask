using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CSharp.RuntimeBinder;
using Newtonsoft.Json;
using TweetSharp;
using TwitterTask.Infrastructure.Data;
using TwitterTask.Infrastructure.Exceptions;
using TwitterTask.Infrastructure.Services.Interfaces;

namespace TwitterTask.Infrastructure.Services
{
	public class TwitterConsumerService : ITwitterConsumerService
	{
		private readonly HttpClient _httpClient;
		private readonly AuthCredential _credentials;
		private string _accessToken;

		public TwitterConsumerService(AuthCredential credentials)
		{
			_credentials = credentials;
			_httpClient = new HttpClient();
		}

		private async Task<string> GetAccessTokenAsync()
		{
			if (_accessToken != null) return _accessToken;

			var request = new HttpRequestMessage(HttpMethod.Post, "https://api.twitter.com/oauth2/token");
			request.Content = new StringContent("grant_type=client_credentials", Encoding.UTF8, "application/x-www-form-urlencoded");
			request.Headers.Authorization = new AuthenticationHeaderValue("Basic", _credentials.Credential);
			try
			{
				var response = await _httpClient.SendAsync(request);
				var responseBody = await response.Content.ReadAsStringAsync();
				_accessToken = (string) JsonConvert.DeserializeObject<dynamic>(responseBody)?.access_token;
				return _accessToken;
			}
			catch (RuntimeBinderException exception)
			{
				throw new PublicException("1010", "Cannot parse access token result", exception);
			}
			catch (WebException exception)
			{
				switch ((exception.Response as HttpWebResponse)?.StatusCode)
				{
					case HttpStatusCode.Forbidden:
						throw new PublicException("1012", "Attempted to get token too frequently, try again later.");
					default:
						throw new PublicException("1011", "Unexpected web error");
				}
			}
		}

		public async Task<Tweet[]> GetTweetsAsync(string screenName, int maxTweetCount)
		{
			if (screenName.Length > 50) // Точный макс. размер логина неизвестен, поэтому 50 хватит с запасом
				throw new PublicException("1000", $"{nameof(screenName)} length is above 50 symbols");

			var request = new HttpRequestMessage(HttpMethod.Get, $"https://api.twitter.com/1.1/statuses/user_timeline.json?count={maxTweetCount}&screen_name={screenName}&tweet_mode=extended");
			request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", await GetAccessTokenAsync());
			try
			{
				var response = await _httpClient.SendAsync(request);
				var responseBody = await response.Content.ReadAsStringAsync();
				var tweets = JsonConvert.DeserializeObject<dynamic[]>(responseBody)
						.Select(x => new Tweet((string)x.created_at, (string)x.full_text))
						.ToArray();
				return tweets;
			}
			catch (WebException exception)
			{
				switch ((exception.Response as HttpWebResponse)?.StatusCode)
				{
					case HttpStatusCode.NotFound:
						throw new PublicException("1012", "Server unavailable or userName doesnt exist");
					default:
						throw new PublicException("1011", "Unexpected web error");
				}
			}
		}
	}
}