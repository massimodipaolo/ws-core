using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace web
{
	public class EmailMessage : IMessage
	{
		private IHostingEnvironment _env { get; set; }
		private Configuration _config { get; set; }
		private ILogger<EmailMessage> _logger { get; set; }

		public EmailMessage(IHostingEnvironment env,IOptions<Configuration> opt, ILogger<EmailMessage> logger)
		{
			_env = env;
			_config = opt.Value;
			_logger = logger;
		}

		void IMessage.Send()
		{
			throw new NotImplementedException();
		}

		void IMessage.Receive()
		{
			throw new NotImplementedException();
		}
	}
}
