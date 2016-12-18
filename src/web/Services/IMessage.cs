using System;
namespace web
{
	public interface IMessage
	{
		void Send();
		void Receive();
	}
}
