using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Shared
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct PacketHeader
    {
        public int Id;
        public byte Size;
        public byte Checksum;
    }
}
