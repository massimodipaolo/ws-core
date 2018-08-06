using Microsoft.CodeAnalysis;
using RazorLight;
using RazorLight.Caching;
using RazorLight.Razor;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
 
namespace core
{
    public interface IRazorEngineBuilder
    {
        RazorLightEngineBuilder AddDefaultNamespaces(params string[] namespaces);
        RazorLightEngineBuilder AddDynamicTemplates(IDictionary<string, string> dynamicTemplates);
        RazorLightEngineBuilder AddMetadataReferences(params MetadataReference[] metadata);
        RazorLightEngine Build();
        RazorLightEngineBuilder SetOperatingAssembly(Assembly assembly);
        RazorLightEngineBuilder UseCachingProvider(ICachingProvider provider);        
        RazorLightEngineBuilder UseMemoryCachingProvider();        
    }

    public class RazorEngineBuilder : RazorLight.RazorLightEngineBuilder, IRazorEngineBuilder{}
}