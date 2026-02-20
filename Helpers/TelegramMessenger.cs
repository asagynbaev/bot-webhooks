namespace bot_webhooks.Helpers;

public static class TelegramMessenger
{
	private static readonly string? Token = Environment.GetEnvironmentVariable("TELEGRAM_BOT_ID");
	private static readonly string? Channel = Environment.GetEnvironmentVariable("TELEGRAM_CHANNEL_ID");

	public static async Task<HttpResponseMessage> SendMessage(string message)
	{
		if (string.IsNullOrEmpty(Token) || string.IsNullOrEmpty(Channel))
			return new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest);

		using var httpClient = new HttpClient();
		var url = $"https://api.telegram.org/{Token}/sendMessage?chat_id={Channel}&text={Uri.EscapeDataString(message)}";
		return await httpClient.GetAsync(url);
	}
}
