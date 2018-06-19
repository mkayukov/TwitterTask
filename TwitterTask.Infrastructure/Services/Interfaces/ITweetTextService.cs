using System.Collections.Generic;
using TwitterTask.Infrastructure.Data;

namespace TwitterTask.Infrastructure.Services.Interfaces
{
	public interface ITweetTextService
	{
		string GetTweetCharUsageStatsMessage(string userName, IEnumerable<Tweet> tweets);
	}
}