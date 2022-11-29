using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text.Json;
using Ws.Core;

namespace ws.bom.oven.web.services
{
    public class PayloadCms
    {
        public static code.AppConfig.GatewayConfig.PayloadCmsConfig _gateway { get; set; } = AppInfo.Config?.GetSection($"appconfig:gateway:{nameof(ws.bom.oven.web.services.PayloadCms)}")?.Get<code.AppConfig.GatewayConfig.PayloadCmsConfig>();
        private static readonly Ws.Core.Extensions.Base.Util.Locker _mutexLogin = new();
        private static readonly Ws.Core.Extensions.Base.Util.Locker _mutexStore = new();
        private static readonly PayloadCms _instance = new();
        private static ILogger<PayloadCms>? _logger { get; set; }
        private static string? _token { get; set; }
        private static ConcurrentDictionary<string, dynamic[]?>? _store { get; set; }
        private static IList<code.PayloadCms.CollectionDto>? _collectionPageKey { get; set; }
        public PayloadCms() : this(_logger ?? AppInfo.Logger<PayloadCms>()) { }

        public PayloadCms(ILogger<PayloadCms> logger)
        {
            _logger = logger;
            Login();
        }

        private static HttpClient Client()
        {
            HttpClientHandler handler = new();
            handler.AutomaticDecompression = System.Net.DecompressionMethods.Deflate | System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Brotli;
            var _hc = new HttpClient(handler);

            if (_token != null)
                _hc.DefaultRequestHeaders.Add("Authorization", $"JWT {_token}");

            return _hc;
        }

        public static void Login()
        {
            try
            {
                if (_token == null)
                    using (_mutexLogin.Lock())
                        if (_token == null)
                            AccessToken();
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Login failure");
            }
        }

        #region http

        #region auth
        // https://payloadcms.com/docs/authentication/operations
        private static void AccessToken()
        {
            var uri = new Uri($"{_gateway.Host}/api/{_gateway.Slugs.Auth}/{nameof(Login)}");
            using var _hc = Client();
            var _rs = _hc.PostAsJsonAsync(uri, new { email = _gateway.UserName, password = _gateway.Password }).Result;
            switch (_rs.StatusCode)
            {
                case System.Net.HttpStatusCode.OK:
                    var _content = _rs.Content.ReadFromJsonAsync<code.PayloadCms.AuthLoginRs>().Result;
                    _token = _content?.token;
                    _ = RefreshTokenTask(_content?.exp);
                    _logger?.LogInformation("Auth {endpoint} rs status {status} for user {user} with token {token}", nameof(Login), _rs.StatusCode, _gateway.UserName, _token);
                    break;
                default:
                    _logger?.LogWarning("Auth {endpoint} rs status {status} for user {user}", nameof(Login), _rs.StatusCode, _gateway.UserName);
                    break;
            }
        }
        private async Task<code.PayloadCms.AuthLoginRs?> Me()
        {
            var uri = new Uri($"{_gateway.Host}/api/{_gateway.Slugs.Auth}/{nameof(Me)}");
            using var _hc = Client();
            var _rs = await _hc.GetAsync(uri);
            switch (_rs.StatusCode)
            {
                case System.Net.HttpStatusCode.OK:
                    var _content = await _rs.Content.ReadFromJsonAsync<code.PayloadCms.AuthLoginRs>();
                    return _content;
                default:
                    _logger?.LogWarning("Auth {endpoint} rs status {status}", nameof(Me), _rs.StatusCode);
                    break;
            }
            return null;
        }
        public static async Task Logout()
        {
            var uri = new Uri($"{_gateway.Host}/api/{_gateway.Slugs.Auth}/{nameof(Logout)}");
            using var _hc = Client();
            var _rs = await _hc.PostAsync(uri, null);
            switch (_rs.StatusCode)
            {
                case System.Net.HttpStatusCode.OK:
                    var _content = await _rs.Content.ReadFromJsonAsync<code.PayloadCms.AuthLoginRs>();
                    _token = _content?.token;
                    break;
                default:
                    _logger?.LogWarning("Auth {endpoint} rs status {status}", nameof(Logout), _rs.StatusCode);
                    break;
            }
        }
        public static async Task RefreshToken()
        {
            var uri = new Uri($"{_gateway.Host}/api/{_gateway.Slugs.Auth}/refresh-token");
            using var _hc = Client();
            var _rs = await _hc.PostAsync(uri, null);
            switch (_rs.StatusCode)
            {
                case System.Net.HttpStatusCode.OK:
                    var _content = await _rs.Content.ReadFromJsonAsync<code.PayloadCms.AuthLoginRs>();
                    _token = _content?.refreshedToken;
                    _ = RefreshTokenTask(_content?.exp);
                    _logger?.LogInformation("Auth {endpoint} rs status {status} with token {token}: {message}", nameof(RefreshToken), _rs.StatusCode, _token, _content.message);
                    break;
                default:
                    _logger?.LogWarning("Auth {endpoint} rs status {status}", nameof(RefreshToken), _rs.StatusCode);
                    break;
            }
        }
        #endregion auth

        #region collection
        private static async Task<List<code.PayloadCms.CollectionDto>?> AuthAccess()
        {
            Func<string, JsonElement, List<code.PayloadCms.CollectionDto>?> _parse = (type, element) =>
            {
                var collections = new List<code.PayloadCms.CollectionDto>();
                foreach (JsonProperty p in element.EnumerateObject())
                {
                    if (_hasCollectionPermission(p.Name)
                        && (JsonSerializer.Deserialize<code.PayloadCms.AuthAccessRs.Operator>(p.Value) is code.PayloadCms.AuthAccessRs.Operator op && op.read.permission)
                        )
                    {
                        var fields = new List<string>();
                        if (op.fields is JsonElement eFields)
                            foreach (JsonProperty f in eFields.EnumerateObject())
                                if (JsonSerializer.Deserialize<code.PayloadCms.AuthAccessRs.Operator>(f.Value) is code.PayloadCms.AuthAccessRs.Operator opField && opField.read.permission)
                                    fields.Add(f.Name);
                        collections.Add(new code.PayloadCms.CollectionDto() { id = p.Name, type = type, fields = fields.ToArray() });
                    }
                }
                return collections;
            };

            var uri = new Uri($"{_gateway.Host}/api/access");
            using var _hc = Client();
            var _rs = await _hc.GetAsync(uri);
            switch (_rs.StatusCode)
            {
                case System.Net.HttpStatusCode.OK:
                    var authAccess = await _rs.Content.ReadFromJsonAsync<code.PayloadCms.AuthAccessRs>();
                    if (authAccess != null && authAccess.collections.ValueKind != JsonValueKind.Undefined)
                    {
                        var collections = _parse(nameof(authAccess.collections), authAccess.collections);
                        if (authAccess.globals.ValueKind != JsonValueKind.Undefined)
                            collections?.AddRange(_parse(nameof(authAccess.globals), authAccess.globals));
                        _collectionPageKey = collections?.Where(_ => _.isPage).ToList();
                        return collections;
                    }
                    break;
                default:
                    _logger?.LogWarning("Auth {endpoint} with uri {uri} rs status {status}", nameof(AuthAccess), uri, _rs.StatusCode);
                    break;
            }
            return null;
        }
        public static async Task<string?> Store()
        {
            return System.Text.Json.JsonSerializer.Serialize(await MemoryStore());
        }

        private static async Task<ConcurrentDictionary<string, dynamic[]?>?> LiveStore()
        {
            var collections = await AuthAccess();
            if (collections != null)
            {
                return await FillStore(collections, new ConcurrentDictionary<string, dynamic[]?>(), new CancellationToken());
            }
            else return null;
        }

        private static async Task<ConcurrentDictionary<string, dynamic[]?>?> MemoryStore()
        {
            if (_store == null)
                using (_mutexStore.Lock())
                    if (_store == null)
                        _store = await LiveStore();
            return _store;
        }

        private static async Task<IEnumerable<code.PayloadCms.Route>> _route() => (await MemoryStore())["route"] as IEnumerable<code.PayloadCms.Route>;
        public static async Task<string?> Route()
        => System.Text.Json.JsonSerializer.Serialize(await _route());

        public static async Task<code.PayloadCms.Route?> RouteById(string id)
        => (await _route()).FirstOrDefault(_ => _.id == id);

        readonly static Func<string, bool> _hasCollectionPermission = (slug) =>
            !_gateway.Slugs.ExcludeFromStore?.AsEnumerable().Select(_ => _.ToLower()).Contains(slug.ToLower()) == true
            && _gateway.Slugs.Auth.ToLower() != slug.ToLower();

        private static async Task<ConcurrentDictionary<string, dynamic[]?>> FillStore(IList<code.PayloadCms.CollectionDto> collections, ConcurrentDictionary<string, dynamic[]?> store, CancellationToken cancellationToken)
        {
            await Parallel.ForEachAsync(collections, async (collection, cancellationToken) =>
            {
                if (collection.type == nameof(collections))
                    store[collection.id] = (await Collection(collection.id, $"limit={int.MaxValue}&depth=10&locale=*&draft=false&richText=false"))?.docs;
                else
                    store[collection.id] = await Global(collection.id);
            })
                // route
                .ContinueWith(_ => store["route"] = ComputeRoute(store));
            return store;
        }

        public static async Task<bool> RefreshRoute()
        {
#warning todo
            /*
             check changes based on createdAt and updatedAt:   
            - market
            - locale
            - category
            _ collectionPages (recompute by api/access)
             */
            //_store["route"] = ...
            return await Task.FromResult<bool>(true);
        }
        private static code.PayloadCms.Route[]? ComputeRoute(ConcurrentDictionary<string, dynamic[]?> store, IList<code.PayloadCms.CollectionDto>? collections = null)
        {
            IEnumerable<code.PayloadCms.Page> pages = (collections ?? _collectionPageKey)
                ?.SelectMany(_ =>
                {
                    try
                    {
                        code.PayloadCms.Page[] _pages = Ws.Core.Shared.Serialization.Util.As<code.PayloadCms.Page[]>(store[_.id]);
                        return _pages.Select(p => { p.schema = _.id; return p; });
                    }
                    catch { return Array.Empty<code.PayloadCms.Page>(); }
                }
            )?.Where(_ => !string.IsNullOrEmpty(_.id))
            ?.ToList();
            IEnumerable<code.PayloadCms.Market> markets = Ws.Core.Shared.Serialization.Util.As<code.PayloadCms.Market[]>(store[_gateway.Slugs.Market.ToLower()]).AsEnumerable();
            IEnumerable<code.PayloadCms.Locale> locales = Ws.Core.Shared.Serialization.Util.As<code.PayloadCms.Locale[]>(store[_gateway.Slugs.Locale.ToLower()]).AsEnumerable();
            IList<code.PayloadCms.Category> categories = Ws.Core.Shared.Serialization.Util.As<code.PayloadCms.Category[]>(store[_gateway.Slugs.Category.ToLower()])
                .AsEnumerable()
                .OrderByDescending(_ => _.category == null)
                .ToList();
            List<code.PayloadCms.Route> routes = new();
            List<Dictionary<string, string>> _recurseCategory(code.PayloadCms.Category category, List<Dictionary<string, string>> paths)
            {
                Dictionary<string, string> _path = category.slug;
                paths.Add(_path);
                var parent = categories.FirstOrDefault(_ => _.id == category.category?.id);
                if (parent != null)
                    paths = _recurseCategory(parent, paths);
                return paths;
            }
            foreach (code.PayloadCms.Category category in categories)
            {
                List<Dictionary<string, string>> paths = _recurseCategory(category, new List<Dictionary<string, string>>());
                paths.Reverse();
                category.slugs = paths;
            }
            var _markets = markets.Where(_ => _.isActive)?.ToList();
            bool _isSingleMarket = _markets?.Count() == 1;
            var _locales = locales?.Where(_ => _.isActive)?.ToList();
            bool _isSingleLocale = _locales?.Count() == 1;
            string _localeDefault = _locales?.FirstOrDefault(_ => _.isDefault)?.id ?? "en";
            Func<Dictionary<string, string>?, code.PayloadCms.Market, code.PayloadCms.Locale, string?> _getSlug = (dictionary, market, locale) => dictionary?
                                            .OrderByDescending(_ => _.Key == locale.id)
                                            .ThenByDescending(_ => _.Key == (string.IsNullOrEmpty(market.defaultLanguage) ? _localeDefault : market.defaultLanguage))
                                            .FirstOrDefault().Value;
            foreach (code.PayloadCms.Market market in _markets)
                foreach (code.PayloadCms.Locale locale in _locales?.Where(_ => market.languages?.Any() == false || market.languages?.Contains(_.id) == true))
                    foreach (code.PayloadCms.Category category in categories)
                        foreach (code.PayloadCms.Page page in pages.Where(_ => _._category == category.id && (_._markets is null || _._markets.Contains(market.id))))
                        {
                            var _culture = @$"{(_isSingleLocale ? "" : locale.id)}" +
                                           @$"{(_isSingleMarket || market.isDefault ? "" : (_isSingleLocale ? "" : "-") + market.id)}";
                            var _category = string.Join('/', category.slugs.Select(_ => _getSlug(_, market, locale)).Where(_ => !string.IsNullOrEmpty(_)));
                            var _page = page.isDefault ? "" : _getSlug(page.slug, market, locale);
                            var route = new code.PayloadCms.Route()
                            {
                                id = @$"{_culture}" +
                                     @$"{(string.IsNullOrEmpty(_culture) || string.IsNullOrEmpty(_category) ? "" : "/") + _category}" +
                                     @$"{(string.IsNullOrEmpty($"{_culture}{_category}") ? "" : "/") + _page}",
                                page = page.id,
                                category = page._category,
                                schema = page.schema,
                                template = page._template,
                                market = market.id,
                                locale = locale.id,
                                createdAt = page.createdAt,
                                updatedAt = page.updatedAt
                            };
                            if (route.id.EndsWith('/'))
                                route.id = route.id.Remove(route.id.Length - 1);
                            routes.Add(route);
                        }

            return routes.ToArray();
        }

        public static async Task<code.PayloadCms.CollectionRs?> Collection(string slug, string? qs = null)
        {
            if (!_hasCollectionPermission(slug)) return null;
            var uri = new Uri($"{_gateway.Host}/api/{slug}?{qs}");
            using var _hc = Client();
            var _rs = await _hc.GetAsync(uri);
            switch (_rs.StatusCode)
            {
                case System.Net.HttpStatusCode.OK:
                    var _content = await _rs.Content.ReadFromJsonAsync<code.PayloadCms.CollectionRs>();
                    return _content;
                case System.Net.HttpStatusCode.TooManyRequests:
                    await Task.Delay(5000);
                    return await Collection(slug, qs);
                default:
                    _logger?.LogWarning("Auth {endpoint} with uri {uri} rs status {status}", nameof(Collection), uri, _rs.StatusCode);
                    break;
            }
            return null;
        }
        public static async Task<string?> CollectionById(string slug, string id)
        {
            if (!_hasCollectionPermission(slug)) return null;
            var uri = new Uri($"{_gateway.Host}/api/{slug}/{id}");
            using var _hc = Client();
            var _rs = await _hc.GetAsync(uri);
            switch (_rs.StatusCode)
            {
                case System.Net.HttpStatusCode.OK:
                    var _content = await _rs.Content.ReadAsStringAsync();
                    return _content;
                default:
                    _logger?.LogWarning("Auth {endpoint} with uri {uri} rs status {status}", nameof(CollectionById), uri, _rs.StatusCode);
                    break;
            }
            return null;
        }
        #endregion collection

        #region global
        public static async Task<dynamic[]?> Global(string slug)
        {
            if (!_hasCollectionPermission(slug)) return null;
            var uri = new Uri($"{_gateway.Host}/api/globals/{slug}");
            using var _hc = Client();
            var _rs = await _hc.GetAsync(uri);
            switch (_rs.StatusCode)
            {
                case System.Net.HttpStatusCode.OK:
                    var _content = await _rs.Content.ReadFromJsonAsync<code.PayloadCms.GlobalRs>();
                    return _content.items;
                case System.Net.HttpStatusCode.TooManyRequests:
                    await Task.Delay(5000);
                    return await Global(slug);
                default:
                    _logger?.LogWarning("Auth {endpoint} with uri {uri} rs status {status}", nameof(Global), uri, _rs.StatusCode);
                    break;
            }
            return null;
        }
        #endregion global

        #endregion http

        #region task
        internal static async Task<bool> Sync() => await Task.FromResult(true);
#warning todo: cron jon interface for enqueu, schedule 
        internal static string RefreshTokenTask(long? exp) => Hangfire.BackgroundJob.Schedule(() => RefreshToken(), TimeSpan.FromSeconds((exp ?? long.MaxValue) - DateTimeOffset.UtcNow.ToUnixTimeSeconds() - 600));        
        #endregion task

    }
}
