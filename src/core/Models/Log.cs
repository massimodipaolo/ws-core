using System;
namespace core.Models
{
	public class Log//: core.Data.Entity
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
            return $"[Log: Message={Message}, Ex={Ex}, Date={Date}]";
        }
	}
}
