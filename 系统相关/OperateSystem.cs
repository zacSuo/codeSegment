using System;

namespace Core
{
    public class OperateSystem
    {
        public static bool Is64Bits
        {
            get
            {
                return Environment.Is64BitOperatingSystem;
            }
        }
    }
}
