using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace OrderService.Handlers
{
    public class JsonHandler
    {
        public static async Task<T> DeserializeAsync<T>(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        
    }
}