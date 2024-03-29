﻿using BenchmarkDotNet.Attributes;
using Grpc.Core;
using Grpc.Net.Client;

namespace benchmark;

[SimpleJob(warmupCount: 1, launchCount: 1, invocationCount: 2, targetCount: 10)]
[EventPipeProfiler(BenchmarkDotNet.Diagnosers.EventPipeProfile.CpuSampling)] // => https://https://www.speedscope.app/
[MemoryDiagnoser]
//[RPlotExporter]
public class LargeObjectRequest
{
    private const string HOST = "https://localhost:60935";

    [Params(100,1000,10000,100000)]
    public int? TopN;

    [Benchmark]
    public async Task gRPC()
    {
        using var channel = GrpcChannel.ForAddress(HOST, new GrpcChannelOptions { MaxReceiveMessageSize = 100 * 1024 * 1024 });
        var client = new Data.DataClient(channel);
        var reply = await client.EventLogDataAsync(new EventLogQuery { Number = TopN ?? 1, Source = EventLogQuery.Types.Source.Memory });
        Console.WriteLine($"{nameof(gRPC)} size [{reply.CalculateSize()}] items: {reply.Items?.Count}");
    }

    [Benchmark]
    public async Task gRPCStream()
    {
        using var channel = GrpcChannel.ForAddress(HOST);
        var client = new Data.DataClient(channel);
        using var call = client.EventLogDataStream(new EventLogQuery { Number = TopN ?? 1, Source = EventLogQuery.Types.Source.Memory });

        var _size = 0;
        var _items = 0;
        var items = call?.ResponseStream?.ReadAllAsync();
        if (items != null)
        {
            await foreach (var item in items)
            {
                _size += item.CalculateSize();
                _items++;
            }
            Console.WriteLine($"{nameof(gRPCStream)} size [{_size}] items: {_items}");
        }
    }

    [Benchmark]
    public void gRPCStreamFirst()
    {
        using var channel = GrpcChannel.ForAddress(HOST);
        var client = new Data.DataClient(channel);
        using var call = client.EventLogDataStream(new EventLogQuery { Number = TopN ?? 1, Source = EventLogQuery.Types.Source.Memory });
        var _items = 1;
        var _first = call?.ResponseStream?.Current;
        var _size = _first?.CalculateSize();
        Console.WriteLine($"{nameof(gRPCStreamFirst)} size [{_size}] items: {_items}");
    }

    [Benchmark]
    public async Task Rest()
    {
        using var httpClient = new HttpClient();
        var response = await httpClient.GetAsync($"{HOST}/api/data/event-log/{TopN ?? 1}/{EventLogQuery.Types.Source.Memory}");
        var content = await response.Content.ReadAsStreamAsync();
        var items = await System.Text.Json.JsonSerializer.DeserializeAsync<IEnumerable<EventLogItem>>(content);
        Console.WriteLine($"{nameof(Rest)} size [{content.Length}] items: {items?.Count()}");
    }
}