using System;
using System.Collections.Generic;
using System.Text;

namespace core
{
    public interface IAppConfiguration
    {
        bool DeveloperExceptionPage { get; set; }
        //AppConfig.RazorEngineOptions RazorEngine { get; set; }
    }

    public class AppConfig : IAppConfiguration
    {
        public bool DeveloperExceptionPage { get; set; } = false;
        /*
        public RazorEngineOptions RazorEngine { get; set; }
        public class RazorEngineOptions : RazorLight.RazorLightOptions
        {
            public string[] Assemblies { get; set; }
        }
        */
    }


}
