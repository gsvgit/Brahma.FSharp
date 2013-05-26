using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace RawIo
{
    public class Raw
    {
        [Flags]
        public enum AccessRights : uint
        {
            GENERIC_READ = (0x80000000),
            GENERIC_WRITE = (0x40000000),
            GENERIC_EXECUTE = (0x20000000),
            GENERIC_ALL = (0x10000000)
        }

        [Flags]
        public enum ShareModes : uint
        {
            FILE_SHARE_READ = 0x00000001,
            FILE_SHARE_WRITE = 0x00000002,
            FILE_SHARE_DELETE = 0x00000004
        }

        public enum CreationDispositions
        {
            CREATE_NEW = 1,
            CREATE_ALWAYS = 2,
            OPEN_EXISTING = 3,
            OPEN_ALWAYS = 4,
            TRUNCATE_EXISTING = 5
        }
        
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool CloseHandle(IntPtr handle);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern IntPtr CreateFile(
            string lpFileName,
            uint dwDesiredAccess,
            uint dwShareMode,
            IntPtr lpSecurityAttributes,
            uint dwCreationDisposition,
            int dwFlagsAndAttributes,
            IntPtr hTemplateFile);

        public static IntPtr CreateFile(int id)
        {
            return CreateFile(string.Format("\\\\.\\PhysicalDrive{0}", id),
                              Convert.ToUInt32(AccessRights.GENERIC_READ),
                              Convert.ToUInt32(ShareModes.FILE_SHARE_READ | ShareModes.FILE_SHARE_WRITE),
                              IntPtr.Zero,
                              Convert.ToUInt32(CreationDispositions.OPEN_EXISTING),
                              0,
                              IntPtr.Zero);
        }
    }
}
