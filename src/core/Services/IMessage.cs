using System;
namespace core
{
	public interface IMessage
	{
		void Send();
		void Receive();
	}
}
