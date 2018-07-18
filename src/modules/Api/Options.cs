using core.Extensions.Base;

namespace core.Extensions.Api
{
    public class Options : IOptions
    {
        public SessionOptions Session { get; set; }

        public class SessionOptions
        {             
            public CookieOptions Cookie { get; set; }
            public int IdleTimeoutInMinutes { get; set; } = 20;

            public class CookieOptions
            {
                public string Name { get; set; } = ".api.Session";
                public string Path { get; set; }
                public string Domain { get; set; }
                public bool HttpOnly { get; set; } = true;                
            }
        }
    }
}
