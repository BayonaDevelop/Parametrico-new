namespace Com.Coppel.SDPC.Application.Models.Services
{
	public class LogVM
	{
		public DateTime Time { get; set; }
		public string Level { get; set; } = string.Empty;
		public string Message { get; set; } = string.Empty;
		public string StackTrace { get; set; } = string.Empty;
	}
}
