using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace backend.Regras
{
    public class Requisicao
    {
        public static async Task<IEnumerable> Consultar<IEnumerable>(string url)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                using (HttpResponseMessage res = await client.GetAsync(url))
                using (HttpContent content = res.Content)
                {
                    return JsonSerializer.Deserialize<IEnumerable>(await content.ReadAsStringAsync());
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
