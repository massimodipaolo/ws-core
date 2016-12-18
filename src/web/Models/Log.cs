using System;
namespace web
{
	public class Log
	{
		public Log()
		{

		}
		public Log(string message)
		{
			Message = message;
		}
		public string Message { get; set; }
		public Exception Ex { get; set; }
		public DateTime Date { get; set; } = DateTime.Now;
		public override string ToString()
		{
			return string.Format("[Log: Message={0}, Ex={1}, Date={2}]", Message, Ex, Date);
		}
	}
}
