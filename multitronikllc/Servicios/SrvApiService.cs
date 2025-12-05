using multitronikllc.Models;
using Shared;
using static System.Net.WebRequestMethods;

namespace multitronikllc.Servicios
{
    public class SrvApiService(HttpClient http, IConfiguration configRoot)
    {
        public int userId {
            get => field; 
            set {
                http.DefaultRequestHeaders.Remove("usuario-id");
                http.DefaultRequestHeaders.Add("usuario-id", value.ToString());                
                field = value;
            } }

        private string url = configRoot.GetValue<string>("serverUrl") ?? "http://mi-api:5000";

        public async Task<Tuple<PacketHeader, string>> LeerPackete(int? id = null)
        {            
            ResponsePacket? response;
            if (id == null)
            {
                try
                {
                    response = await http.GetFromJsonAsync<ResponsePacket>($"{url}/Challenge/get-next-packet");
                }
                catch (Exception ex)
                {
                    throw;
                }
                
            }
            else
            {
                response = await http.GetFromJsonAsync<ResponsePacket>($"{url}/Challenge/retry-packet?packetId={id}");                
            }            
            if(response == null)
            {
                throw new Exception("No se pudo obtener el paquete desde el servidor.");
            }
            //var dataJsonBase64 = await reponse.Content.ReadFromJsonAsync<string>();
            var dataBinary = Convert.FromBase64String(response.content);
            return ProcesarPacket.GetData(dataBinary);
        }

        public async Task Restart()
        {
            await http.GetAsync($"{url}/Challenge/restart?userId={userId}");
        }

        internal async Task Ack(int packetId)
        {
            await http.GetAsync($"{url}/Challenge/ack-packet?packetId={packetId}");
        }
    }
}
