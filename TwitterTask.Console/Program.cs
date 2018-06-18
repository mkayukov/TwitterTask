using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TweetSharp;
using TwitterTask.Infrastructure.Configuration;
using TwitterTask.Infrastructure.Services;
using TwitterTask.Infrastructure.Services.Interfaces;

namespace TwitterTask
{
	class Program
	{
		const string EXIT = "exit";
		const string CONSOLE_STATS = "console stats";
		const string CONSOLE_STATS_SMALL = "cs";
		const string TWEET_STATS = "tweet stats";
		const string TWEET_STATS_SMALL = "ts";

		private static ITwitterConsumerService _twitterConsumerService;
		private static ITweetTextService _tweetTextService;
		private static ITwitterService _twitterService;
		private static bool _isUserAuthenticated;

		static void Main(string[] args)
		{
			var credentials = TwitterAuthConfig.GetCredentials;
			_twitterConsumerService = new TwitterConsumerService(credentials);
			_tweetTextService = new TweetTextService();
			_twitterService = new TwitterService(credentials.Key, credentials.Secret);

			RunApplication();
		}

		static void RunApplication()
		{
			while (true)
			{
				Console.Write($"Choose operation '{CONSOLE_STATS}'('cs') or '{TWEET_STATS}'('ts'): ");
				var operation = Console.ReadLine();
				if (operation == CONSOLE_STATS || operation == CONSOLE_STATS_SMALL)
				{
					Console.Write("Write twitter userName: ");
					ConsoleLetterUsageStats(Console.ReadLine());
				}
				else if (operation == TWEET_STATS || operation == TWEET_STATS_SMALL)
				{
					Console.Write("Write twitter userName: ");
					TweetLetterUsageStats(Console.ReadLine());
				}
				else if (operation == EXIT)
					break;

				Console.WriteLine("----------------------------------------------------------------");
			}
		}

		static void ConsoleLetterUsageStats(string userName)
		{
			var tweets = _twitterConsumerService.GetTweets(userName, 5);
			var usageStatsMessage = _tweetTextService.GetTweetCharUsageStatsMessage(userName, tweets);
			Console.WriteLine(usageStatsMessage);
		}

		static void TweetLetterUsageStats(string userName)
		{
			var tweets = _twitterConsumerService.GetTweets(userName, 5);
			var usageStatsMessage = _tweetTextService.GetTweetCharUsageStatsMessage(userName, tweets);

			if (!_isUserAuthenticated) InitUserAuthentication();

			var twitterStatus = _twitterService.SendTweet(new SendTweetOptions { Status = usageStatsMessage });
			Console.WriteLine(twitterStatus != null ? "Сообщение отправлено успешно" : "Сообщение не было отправлено");
		}

		static void InitUserAuthentication()
		{
			var requestToken = _twitterService.GetRequestToken();
			var uri = _twitterService.GetAuthorizationUri(requestToken);
			Process.Start(uri.ToString());
			Console.Write("For using your twitter account in application please enter verification code from twitter auth: ");
			var verifier = Console.ReadLine();
			var userAccessToken = _twitterService.GetAccessToken(requestToken, verifier);
			_twitterService.AuthenticateWith(userAccessToken.Token, userAccessToken.TokenSecret);
			_isUserAuthenticated = true;
		}
	}
}
