using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Shared
{
    public static class ProcesarPacket
    {
        public static Tuple<PacketHeader, string> GetData(byte[] rawPacketData)
        {
            Span<byte> packetSpan = rawPacketData;
            int headerSize = Marshal.SizeOf<PacketHeader>();
            if (packetSpan.Length < headerSize)
            {
                throw new ProcessException("Error: El paquete es demasiado corto para el encabezado.");                
            }
            PacketHeader header = MemoryMarshal.Read<PacketHeader>(packetSpan);
            int expectedDataSize = header.Size;

            Console.WriteLine($"ID del paquete: {header.Id}");
            Console.WriteLine($"Tamaño de los datos (indicado en el header): {expectedDataSize} bytes");

                        
            int minimumTotalSize = headerSize + expectedDataSize;

            if (packetSpan.Length < minimumTotalSize)
            {
                throw new ProcessException($"Error: El paquete está incompleto. Esperaba {minimumTotalSize} bytes, recibí {packetSpan.Length}.");
            }
            
            Span<byte> variableDataSpan = packetSpan.Slice(headerSize, expectedDataSize);
            var check = variableDataSpan.ToArray().CalculateChecksum();
            if (check != header.Checksum)
            {
                throw new CheckSumException(header);
            }
            string data = Encoding.ASCII.GetString(variableDataSpan);
            return new Tuple<PacketHeader, string>(header, data);
        }

        public static Tuple<PacketHeader, byte[]> GeneratePackages(int id, string? texto = null)
        {
            var data = texto == null ? [] : Encoding.UTF8.GetBytes(texto);
            var header = new PacketHeader
            {
                Id = id,
                Size = (byte)data.Length,
                Checksum = data.CalculateChecksum()
            };
            return new Tuple<PacketHeader, byte[]>(header, data);         
        }

        public static byte[] GetRawPaket(PacketHeader header, byte[] data)
        {
            int headerSize = Marshal.SizeOf<PacketHeader>();
            int dataSize = data?.Length ?? 0;
            int totalSize = headerSize + dataSize;
            byte[] rawPacket = new byte[totalSize];
            Span<byte> headerSpan = new(rawPacket, 0, headerSize);
            MemoryMarshal.Write(headerSpan, in header);
            if (dataSize > 0)
            {                
                Buffer.BlockCopy( data, 0, rawPacket, headerSize, dataSize);
            }
            return rawPacket;
        }
    }
}
