using System;
using System.Data;
using System.Text.Json.Serialization;
using Dapper;
using Newtonsoft.Json;

namespace ProductService.Handlers;

public class JSONTypeHandler<T> : SqlMapper.TypeHandler<T> where T : class
{
public override void SetValue(IDbDataParameter parameter, T? value)
        {
            parameter.Value = (value == null) ? (object)DBNull.Value : JsonConvert.SerializeObject(value);
        }


        public override T Parse(object value)
        {
            if (value == null || value == DBNull.Value)
            {
                return null;
            }
            return JsonConvert.DeserializeObject<T>((string)value)!;
        }
}
