using System;
using System.Text;

namespace TwitterTask.Infrastructure.Data
{
	public struct AuthCredential
	{
		public readonly string Key;
		public readonly string Secret;

		public AuthCredential(string key, string secret)
		{
			Key = key;
			Secret = secret;
		}

		public string Credential => Convert.ToBase64String(Encoding.UTF8.GetBytes($"{Key}:{Secret}"));
	}
}