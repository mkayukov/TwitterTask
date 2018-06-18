namespace TwitterTask.Infrastructure.Data
{
	public struct Tweet
	{
		public readonly string Date;
		public readonly string Text;

		public Tweet(string date, string text)
		{
			Date = date;
			Text = text;
		}

		public override string ToString() => $"[{Date}] {Text}";
	}
}