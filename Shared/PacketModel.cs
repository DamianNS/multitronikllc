using System;
using System.Collections.Generic;
using System.Text;

namespace Shared
{
    public class PacketModel(PacketHeader packet, byte[] data)
    {
        public int Id => this.Paket.Id;
        public PacketHeader Paket { get; set; } = packet;
        public EstadoEnum Status { get; set; } = EstadoEnum.Pendiente;
        public byte[] Data { get; set; } = data;
    }
}
