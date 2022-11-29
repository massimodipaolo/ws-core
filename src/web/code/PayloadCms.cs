using System.Text.Json;
using Ws.Core.Shared.Serialization;

namespace ws.bom.oven.web.code;

public class PayloadCms
{
    #region models
    public class AuthLoginRs
    {
        public dynamic user { get; set; }
        [SensitiveData]
        public string token { get; set; }
        [SensitiveData]
        public string refreshedToken { get; set; }
        public string message { get; set; }
        public long exp { get; set; }
    }

    public class AuthAccessRs
    {
        public JsonElement collections { get; set; }
        public JsonElement globals { get; set; }
        public JsonElement notFound { get; set; }
        public class Operator
        {
            public dynamic fields { get; set; }
            public Permission read { get; set; }
            public struct Permission
            {
                public bool permission { get; set; }
            }
        }
    }
    public class CollectionDto
    {
        public string id { get; set; }
        public string[] fields { get; set; }
        public string type { get; set; }
        public bool isPage => new[] { "category", "template", "slug", "meta" }.All(_ => fields.Contains(_));
    }
    public class CollectionRs
    {
        public dynamic[] docs { get; set; }
        public int totalDocs { get; set; }
        public int limit { get; set; }
        public int totalPages { get; set; }
        public int page { get; set; }
        public int pagingCounter { get; set; }
        public bool hasPrevPage { get; set; }
        public bool hasNextPage { get; set; }
        public int? prevPage { get; set; }
        public int? nextPage { get; set; }
    }
    public record GlobalRs : Entity
    {
        public dynamic[] items { get; set; }
        public string globalType { get; set; }
    }
    public record Entity
    {
        public string id { get; set; }
#warning todo: exclude serialize?
        public DateTimeOffset createdAt { get; set; }
#warning todo: exclude serialize?
        public DateTimeOffset updatedAt { get; set; }
    }
    public record Locale : Entity
    {
        public bool isDefault { get; set; }
        public bool isActive { get; set; }
    }

    public record Market : Entity
    {
        public string[]? languages { get; set; }
        public string defaultLanguage { get; set; }
        public bool isDefault { get; set; }
        public bool isActive { get; set; }
    }
    public record Category : Entity
    {
        public Dictionary<string, string> title { get; set; }
        public Dictionary<string, string> slug { get; set; }
        public List<Dictionary<string, string>> slugs { get; set; }
        public Category category { get; set; }
    }

    public record Page : Entity
    {
        public Dictionary<string, string> slug { get; set; }
        public Entity[]? markets { get; set; }
        public IEnumerable<string>? _markets => markets?.Select(_ => _.id);
        public Entity? category { get; set; }
        public string _category => category?.id;
        public string schema { get; set; }
        public Entity? template { get; set; }
        public string _template => template?.id;
        public bool isDefault { get; set; }
    }

    public record Route : Entity
    {
        public string market { get; set; }
        public string locale { get; set; }
        public string page { get; set; }
        public string category { get; set; }
        public string schema { get; set; }
        public string template { get; set; }
    }
    #endregion models
}
