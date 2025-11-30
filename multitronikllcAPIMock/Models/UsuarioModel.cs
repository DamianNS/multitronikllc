using Shared;

namespace multitronikllcAPIMock.Models
{
    public class UsuarioModel
    {
        public int Id { get; set; }
        public List<PacketHeader> PaketesTotal { get; set; } = [];
    }
}
