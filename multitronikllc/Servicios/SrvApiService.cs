using Shared;
using static System.Net.WebRequestMethods;

namespace multitronikllc.Servicios
{
    public class SrvApiService(HttpClient http, IConfiguration configRoot)
    {
        public int userId { get; set {
                http.DefaultRequestHeaders.Remove("usuario-id");
                http.DefaultRequestHeaders.Add("usuario-id", field.ToString());
            } }

        private string url = configRoot.GetValue<string>("serverUrl") ?? "http://mi-api:5000";

        public async Task<Tuple<PacketHeader, string>> LeerPackete(int? id = null)
        {            
            HttpResponseMessage reponse;
            if (id == null)
            {
                try
                {
                    reponse = await http.GetAsync($"{url}/Challenge/get-next-packet");
                }
                catch (Exception)
                {
                    throw;
                }
                
            }
            else
            {
                reponse = await http.GetAsync($"{url}/Challenge/retry-packet?packetId={id}");                
            }            
            var dataJsonBase64 = await reponse.Content.ReadFromJsonAsync<string>();
            var dataBinary = Convert.FromBase64String(dataJsonBase64);
            return ProcesarPacket.GetData(dataBinary);
        }

        public async Task Restart()
        {
            await http.GetAsync($"{url}/Challenge/restart?userId={userId}");
        }
    }
}
