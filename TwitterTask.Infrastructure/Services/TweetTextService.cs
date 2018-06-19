using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using TwitterTask.Infrastructure.Data;
using TwitterTask.Infrastructure.Exceptions;
using TwitterTask.Infrastructure.Services.Interfaces;

namespace TwitterTask.Infrastructure.Services
{
	public class TweetTextService : ITweetTextService
	{
		const int MAX_TWEET_SYMBOL_COUNT = 280;

		public string GetTweetCharUsageStatsMessage(string userName, IEnumerable<Tweet> tweets)
		{
			var twitterUserName = new Regex("@.+").IsMatch(userName) ? userName : $"@{userName}";
			var usageStats = GetTweetCharUsageStats(tweets);

			var outputStatsMessage = FillMessage(twitterUserName, usageStats);

			if (outputStatsMessage.Length > MAX_TWEET_SYMBOL_COUNT)
				outputStatsMessage = TruncateMessage(twitterUserName, usageStats);

			return outputStatsMessage;
		}

		private Dictionary<char, decimal> GetTweetCharUsageStats(IEnumerable<Tweet> tweets)
		{
			var cleanTweets = tweets
				.Select(x => GetTextWithoutTweetUrlLinks(x.Text))
				.Select(GetOnlyLetters)
				.ToArray();
			var letterCounts = new Dictionary<char, int>();

			foreach (var letter in cleanTweets.SelectMany(c => c))
				if (letterCounts.ContainsKey(letter))
					letterCounts[letter] += 1;
				else
					letterCounts[letter] = 1;

			var allLetterCount = cleanTweets.SelectMany(c => c).Count();
			var charUsage = letterCounts
				.Select(x => new { x.Key, Value = (decimal)x.Value / allLetterCount })
				.OrderByDescending(x => x.Value)
				.ToDictionary(x => x.Key, x => decimal.Round(x.Value, 6));

			return charUsage;
		}

		/// <summary>
		/// т.к. в твитере есть ограничение на кол-во символов, пытаемся с минимальными потерями урезать сообщение
		/// </summary>
		/// <param name="userName"></param>
		/// <param name="usageStats"></param>
		/// <returns></returns>
		private string TruncateMessage(string userName, Dictionary<char, decimal> usageStats)
		{
			var precision = 4;
			var statsLength = usageStats.Count;
			var outputStatsMessage = FillMessage(userName, usageStats);
			while (outputStatsMessage.Length > MAX_TWEET_SYMBOL_COUNT)
			{
				if (precision > 2)
				{
					var roundPrecision = precision--;
					usageStats = usageStats.ToDictionary(x => x.Key, x => decimal.Round(x.Value, roundPrecision));
				}
				else if (statsLength > 0)
				{
					usageStats = usageStats
						.OrderByDescending(x => x.Value)
						.Take(statsLength--)
						.ToDictionary(x => x.Key, x => x.Value);
				}
				else
					throw new PublicException("1001", $"Message cannot be truncated to max tweet size of {MAX_TWEET_SYMBOL_COUNT}");

				outputStatsMessage = FillMessage(userName, usageStats);
			}
			return outputStatsMessage;
		}

		private string FillMessage(string userName, Dictionary<char, decimal> usageStats)
			=> $"{userName}, статистика для последних 5 твитов: {JsonConvert.SerializeObject(usageStats)}";

		private string GetOnlyLetters(string text) => new Regex(@"[\W\d]").Replace(text, "");

		private string GetTextWithoutTweetUrlLinks(string text)
		{
			var cleanTweetText = new Regex(@"https://t.co/[\w\d]+").Replace(text, "");
			return cleanTweetText;
		}
	}
}
