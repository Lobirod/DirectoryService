using System.Data;
using System.Text.Json;
using Dapper;

namespace Shared.Dapper;

public class JsonTypeHandler<T> : SqlMapper.TypeHandler<T>
{
    public override void SetValue(IDbDataParameter parameter, T? value)
    {
        parameter.Value = JsonSerializer.Serialize(value);
    }

    public override T Parse(object value)
    {
        if (value is string jsonString)
        {
            return JsonSerializer.Deserialize<T>(jsonString)!;
        }

        return default!;
    }
}