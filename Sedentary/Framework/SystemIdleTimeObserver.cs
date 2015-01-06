using System;
using System.Runtime.InteropServices;

namespace Sedentary.Framework
{
    public static class SystemInfo
    {
        [DllImport("user32.dll")]
        static extern bool GetLastInputInfo(out LASTINPUTINFO plii);

        [StructLayout(LayoutKind.Sequential)]
        struct LASTINPUTINFO
        {
            public static readonly int SizeOf =
                   Marshal.SizeOf(typeof(LASTINPUTINFO));

            [MarshalAs(UnmanagedType.U4)]
            public int cbSize;
            [MarshalAs(UnmanagedType.U4)]
            public int dwTime;
        }

        public static TimeSpan IdleTime
        {
            get { return GetIdleTime(); }
        }

        private static TimeSpan GetIdleTime()
        {
            int idleTimeMs = 0;
            LASTINPUTINFO lastInputInfo = new LASTINPUTINFO();
            lastInputInfo.cbSize = Marshal.SizeOf(lastInputInfo);
            lastInputInfo.dwTime = 0;

            int envMs = Environment.TickCount;

            if (GetLastInputInfo(out lastInputInfo))
            {
                int lastInputMs = lastInputInfo.dwTime;
                idleTimeMs = envMs - lastInputMs;
            }

            return TimeSpan.FromMilliseconds(Math.Max(idleTimeMs, 0));
        }
    }
}