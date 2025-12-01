using Shared;
using static System.Net.WebRequestMethods;

namespace multitronikllc.Servicios
{
    public class SrvApiService(HttpClient http)
    {
        public int userId { set {
                http.DefaultRequestHeaders.Remove("usuario-id");
                http.DefaultRequestHeaders.Add("usuario-id", value.ToString());
            } }

        public async Task<Tuple<PacketHeader, string>> LeerPackete(int? id = null)
        {
            HttpResponseMessage reponse;
            if (id == null)
            {
                try
                {
                    reponse = await http.GetAsync("http://web:5000/Challenge/get-next-packet");
                }
                catch (Exception)
                {
                    throw;
                }
                
            }
            else
            {
                reponse = await http.GetAsync($"http://web:5000/Challenge/retry-packet?packetId={id}");                
            }            
            var dataJsonBase64 = await reponse.Content.ReadFromJsonAsync<string>();
            var dataBinary = Convert.FromBase64String(dataJsonBase64);
            return ProcesarPacket.GetData(dataBinary);
        }
    }
}
