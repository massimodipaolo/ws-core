using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace core.Extensions.Message
{
    public interface IMessage
    {
        Task SendAsync(string sender, string[][] recipients, string subject, string body);
        Task ReceiveAsync();
    }
}
