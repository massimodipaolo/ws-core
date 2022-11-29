namespace Ws.Core.Shared.Serialization;
public static class Util
{
    private static System.Text.Json.JsonSerializerOptions jsonSerializerOptions => new()
    {
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault, 
        WriteIndented = false,
        AllowTrailingCommas = true,
        ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles
    };
    public static T? As<T>(object o) where T : class
    => System.Text.Json.JsonSerializer.Deserialize<T>(System.Text.Json.JsonSerializer.Serialize(o, jsonSerializerOptions), jsonSerializerOptions);

    public static bool IsEqual<T>(T o1, T o2) where T : class
    => System.Text.Json.JsonSerializer.Serialize(o1, jsonSerializerOptions) == System.Text.Json.JsonSerializer.Serialize(o2, jsonSerializerOptions);
}
