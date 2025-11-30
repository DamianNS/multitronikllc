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
        public byte[]? GetNextPacket()
        {
            Task.Delay(100).Wait(); // Simula un retardo de 2 segundos
            var userId = GetUsuarioIdFromHeaders();
            if (userId == -1)
            {
                Response.StatusCode = 400; // Bad Request
                return null;
            }
            var packet = us.GetPackage(userId);
            return ProcesarPacket.GetRawPaket(packet.Paket, packet.Data);
        }

        [HttpGet("retry-packet")]
        public byte[]? RetryPacket([FromQuery] int packetId)
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
                return ProcesarPacket.GetRawPaket(p.Paket, p.Data);
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
