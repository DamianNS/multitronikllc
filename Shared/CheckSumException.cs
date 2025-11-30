
namespace Shared
{
    [Serializable]
    public class CheckSumException : Exception
    {
        public PacketHeader Packet { get; set; }

        public CheckSumException(PacketHeader Packet)
        {
            this.Packet = Packet;
        }
    }
}