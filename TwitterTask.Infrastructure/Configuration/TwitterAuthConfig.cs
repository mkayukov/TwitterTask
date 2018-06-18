using System;
using System.Configuration;
using System.Text;
using TwitterTask.Infrastructure.Data;

namespace TwitterTask.Infrastructure.Configuration
{
	public sealed class TwitterAuthConfig : ConfigurationSection
	{
		public static AuthCredential GetCredentials
		{
			get
			{
				var section = ConfigurationManager.GetSection("twitterAuth");
				if (section == null) throw new ConfigurationErrorsException(@"Expected configuration section twitterAuth");
				var twitterAuthConfig = section as TwitterAuthConfig;
				if (twitterAuthConfig == null) throw new ConfigurationErrorsException($"Cannot convert section twitterAuth to type {typeof(TwitterAuthConfig)}");
				return new AuthCredential(twitterAuthConfig.Key, twitterAuthConfig.Secret);
			}
		}

		[ConfigurationProperty("key", IsRequired = true)]
		internal string Key
		{
			get { return (string)this["key"]; }
			set { this["key"] = value; }
		}

		[ConfigurationProperty("secret", IsRequired = true)]
		internal string Secret
		{
			get { return (string)this["secret"]; }
			set { this["secret"] = value; }
		}
	}
}