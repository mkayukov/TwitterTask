using TwitterTask.Infrastructure.Data;

namespace TwitterTask.Infrastructure.Services.Interfaces
{
	public interface ITwitterConsumerService
	{
		Tweet[] GetTweets(string screenName, int maxTweetCount);
	}
}