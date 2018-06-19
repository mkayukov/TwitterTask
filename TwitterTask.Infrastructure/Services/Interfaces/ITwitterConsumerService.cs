using System.Threading.Tasks;
using TwitterTask.Infrastructure.Data;

namespace TwitterTask.Infrastructure.Services.Interfaces
{
	public interface ITwitterConsumerService
	{
		/// <summary>
		/// Получает все твиты в неурезанном формате для screenName
		/// </summary>
		/// <param name="screenName"></param>
		/// <param name="maxTweetCount"></param>
		/// <returns></returns>
		Task<Tweet[]> GetTweetsAsync(string screenName, int maxTweetCount);
	}
}