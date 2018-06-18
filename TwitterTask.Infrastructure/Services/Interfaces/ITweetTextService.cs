using System.Collections.Generic;
using TwitterTask.Infrastructure.Data;

namespace TwitterTask.Infrastructure.Services.Interfaces
{
	public interface ITweetTextService
	{
		Dictionary<char, decimal> GetTweetCharUsageStats(IEnumerable<Tweet> tweets);
		string GetTweetCharUsageStatsMessage(string userName, IEnumerable<Tweet> tweets);
	}
}