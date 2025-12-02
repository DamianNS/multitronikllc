using Microsoft.AspNetCore.Mvc;
using multitronikllcAPIMock.Services;
using Shared;
using System.Net.Sockets;

namespace multitronikllcAPIMock.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ChallengeController(UsuariosService us) : ControllerBase
    {
        private readonly Random _random = new();

        [HttpGet("restart")]
        public void Restart()
        {
            var userId = GetUsuarioIdFromHeaders();
            if (userId == -1)
            {
                Response.StatusCode = 400; // Bad Request                
            }
            else
            {
                us.EliminarUsuario(userId);
            }
        }

        private int GetUsuarioIdFromHeaders()
        {
            if (HttpContext.Request.Headers.TryGetValue("usuario-id", out var usuarioIdValues))
            {
                if (int.TryParse(usuarioIdValues.FirstOrDefault(), out var usuarioId))
                {
                    return usuarioId;
                }
            }
            return -1; // Indica que no se encontró un ID válido
        }

        [HttpGet("get-next-packet")]
        public ResponsePacket? GetNextPacket()
        {
            //Task.Delay(100).Wait(); // Simula un retardo de 2 segundos
            var userId = GetUsuarioIdFromHeaders();
            if (userId == -1)
            {
                Response.StatusCode = 400; // Bad Request
                return null;
            }

            PacketModel packet;
            if (_random.Next(5) == 1)
            {
                packet = us.GetPackageError(userId);
            }
            else
            {
                packet = us.GetPackage(userId);
            }


            // emulo una trasmision incorrecta
            var data = ProcesarPacket.GetRawPaket(packet.Paket, packet.Data);
            var ret = new ResponsePacket()
            {
                content = Convert.ToBase64String(data)
            };
            return ret;
        }

        [HttpGet("retry-packet")]
        public ResponsePacket? RetryPacket([FromQuery] int packetId)
        {
            var userId = GetUsuarioIdFromHeaders();
            if (userId == -1)
            {
                Response.StatusCode = 400; // Bad Request
                return null;
            }
            else
            {
                var p = us.RetryPackage(userId, packetId);
                
                var data = ProcesarPacket.GetRawPaket(p.Paket, p.Data);
                var ret = new ResponsePacket()
                {
                    content = Convert.ToBase64String(data)
                };
                return ret;
            }            
        }

        [HttpGet("ack-packet")]
        public void AckPacket([FromQuery] int packetId)
        {
            var userId = GetUsuarioIdFromHeaders();
            if (userId == -1)
            {
                Response.StatusCode = 400; // Bad Request                
            }
            else
            {
                us.AckPackage(userId, packetId);
            }
        }
    }
}
