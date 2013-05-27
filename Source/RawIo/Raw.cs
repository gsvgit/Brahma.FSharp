using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;

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
        static extern IntPtr CreateFile(string lpFileName,
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

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool ReadFile(IntPtr hFile,                        // handle to file
                                    byte[] lpBuffer,                // data buffer
                                    int nNumberOfBytesToRead,        // number of bytes to read
                                    ref int lpNumberOfBytesRead,    // number of bytes read
                                    IntPtr lpOverlapped);

        public static int ReadFile(IntPtr hFile,
                                   byte[] lpBuffer,
                                   int nNumberOfBytesToRead,
                                   long offset)
        {
            NativeOverlapped overlapped = new NativeOverlapped();

            byte[] bytes = BitConverter.GetBytes(offset);

            if (BitConverter.IsLittleEndian)
            {
                overlapped.OffsetHigh = BitConverter.ToInt32(bytes, 32);
                overlapped.OffsetLow = BitConverter.ToInt32(bytes, 0);
            }
            else
            {
                overlapped.OffsetHigh = BitConverter.ToInt32(bytes, 0);
                overlapped.OffsetLow = BitConverter.ToInt32(bytes, 32);
            }

            GCHandle handle = GCHandle.Alloc(overlapped, GCHandleType.Pinned);
            int read = 0;

            bool succeeded = ReadFile(hFile, lpBuffer, nNumberOfBytesToRead, ref read, handle.AddrOfPinnedObject());

            handle.Free();

            if (succeeded)
            {
                return read;
            }
            else
            {
                return -1;
            }
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern int GetFileSize(IntPtr hFile,
                                      ref int high);

        public static long GetFileSize(IntPtr hFile)
        {
            int high = 0;

            int low = GetFileSize(hFile, ref high);

            if (BitConverter.IsLittleEndian)
            {
                return (((long)low) << 32) + ((long)high);
            }
            else
            {
                return (((long)high) << 32) + ((long)low);
            }
        }
    }
}
