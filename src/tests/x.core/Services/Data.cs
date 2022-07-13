using x.core;
using Google.Protobuf;
using Grpc.Core;
using System.Runtime.Serialization.Formatters.Binary;

namespace x.core.Services
{
    public class Data : x.core.Data.DataBase
    {
        private readonly IWebHostEnvironment _env;
        private static IEnumerable<EventLogItem> _memoryData = null;
        private readonly static System.Text.Json.JsonSerializerOptions _msJsonOpt = new() { PropertyNameCaseInsensitive = true };
        public Data(IWebHostEnvironment env)
        {
            _env = env;
        }

        private static IEnumerable<EventLogItem> _getTop(int number, EventLogQuery.Types.Source source, IWebHostEnvironment env)
        => _deserialize(source, env)?.Take(number);

        /// <summary>
        /// Deserialize object from source
        /// </summary>
        /// <param name="source"></param>
        /// <param name="env"></param>
        /// <returns></returns>
        private static IEnumerable<EventLogItem> _deserialize(EventLogQuery.Types.Source source, IWebHostEnvironment env)
        {
            Func<string> _path = () => System.IO.Path.Combine(env?.ContentRootPath ?? Path.GetFullPath(Directory.GetCurrentDirectory()), "data", $"event-log.{(source == EventLogQuery.Types.Source.Buf ? "buf" : "json")}");
            Func<string> _jsonString = () =>
            {
                using FileStream stream = File.Open(_path(), FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                using var reader = new StreamReader(stream);
                return reader.ReadToEnd();
            };

            switch (source)
            {
                case EventLogQuery.Types.Source.Njson:
                    return Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<EventLogItem>>(_jsonString());
                case EventLogQuery.Types.Source.Msjson:
                    return System.Text.Json.JsonSerializer.Deserialize<IEnumerable<EventLogItem>>(_jsonString(), _msJsonOpt);
                case EventLogQuery.Types.Source.Buf:
                    {
                        using var input = File.OpenRead(_path());
                        return EventLogResponse.Parser.ParseFrom(input).Items;
                    }
                default:
                    {
                        if (_memoryData == null)
                            _memoryData = _deserialize(EventLogQuery.Types.Source.Msjson, env);
                        return _memoryData;
                    }
            }
        }

        private static void _serialize(IEnumerable<EventLogItem> data, EventLogQuery.Types.Source source)
        {            
            Action<string> _write = (s) =>
            {
                using MemoryStream stream = new();
                using var writer = new StreamWriter(stream);
                writer.Write(s);
            };

            switch (source)
            {
                case EventLogQuery.Types.Source.Njson:
                    _write(Newtonsoft.Json.JsonConvert.SerializeObject(data));
                    break;
                case EventLogQuery.Types.Source.Msjson:
                    _write(System.Text.Json.JsonSerializer.Serialize(data, _msJsonOpt));
                    break;
                case EventLogQuery.Types.Source.Buf:
                    {
                        var response = new EventLogResponse();
                        response.Items.AddRange(data);
                        using var output = new MemoryStream();
                        response.WriteTo(output);
                    }
                    break;
                default:
                    break;
            }
        }

        public override async Task<EventLogResponse> EventLogData(EventLogQuery request, ServerCallContext context)
        {
            var data = _getTop(request.Number, request.Source, _env);
            var response = new EventLogResponse();
            response.Items.AddRange(data);
            return await Task.FromResult<EventLogResponse>(response);
        }

        public override async Task EventLogDataStream(EventLogQuery request, IServerStreamWriter<EventLogItem> stream, ServerCallContext context)
        {
            var data = _getTop(request.Number, request.Source, _env);
            foreach (var item in data)
                await stream.WriteAsync(item);
            /*
            var i = 0;
            while (!context.CancellationToken.IsCancellationRequested && i++<data.Length)
                await stream.WriteAsync(data[i]);
            */
        }

        public static IEnumerable<EventLogItem> GetEventLogDataApi(IWebHostEnvironment env, int number, EventLogQuery.Types.Source source)
        => _getTop(number, source, env);

        public static void PostEventLogDataApi(IEnumerable<EventLogItem> data, EventLogQuery.Types.Source source)
        => _serialize(data, source);

    }
}