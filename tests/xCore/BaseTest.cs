using Microsoft.AspNetCore.Mvc.Testing;
using Ws.Core;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using System.Linq;
using Microsoft.Extensions.Logging;
using System;
using xCore.Extensions;

namespace xCore

{
    public class BaseTest : IClassFixture<Program>
    {
        protected readonly Program _factory;
        protected readonly ITestOutputHelper _output;
        public BaseTest(Program factory, ITestOutputHelper output)
        {
            _factory = factory;
            _output = output;
        }
    }
}

namespace xCore.Extensions
{
    public static partial class Extend
    {
        const int _separatorSize = 50;
        static Func<char,string> _line = (separator) => $"{Environment.NewLine}{new string(separator, _separatorSize)}{Environment.NewLine}";
        static Func<string> newLine = () => _line('-');
        static Func<string> endLine = () => _line('#');

        public static void Write(this ITestOutputHelper output, params string[] args)
        {
            output.WriteLine($"" +
                $"{Environment.StackTrace.Split("\r\n")[2]}" +
                newLine() + 
                string.Join(newLine(),args) +
                $"{endLine()}"
                );
        }
    }
}
