using System;

namespace TwitterTask.Infrastructure.Exceptions
{
	public class PublicException : Exception
	{
		public readonly string Code;
		
		public PublicException(string code, string message, Exception innerException = null) : base(message, innerException)
		{
			Code = code;
		}
	}
}