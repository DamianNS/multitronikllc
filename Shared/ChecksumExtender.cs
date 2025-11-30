namespace Shared
{
    public static class ChecksumExtender
    {
        public static byte CalculateChecksum(this byte[] data)
        {            
            int sumaTotal = 0;
            if(data == null || data.Length == 0)
            {
                return 0;
            }
            foreach (byte b in data)
            {
                sumaTotal += b;
            }
            return (byte)sumaTotal;
        }
    }
}
