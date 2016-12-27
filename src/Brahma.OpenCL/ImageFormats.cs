using System.Runtime.InteropServices;
using OpenCL.Net;

namespace Brahma.OpenCL
{
    public interface IImageComponentType
    {
        ChannelType ChannelType
        {
            get;
        }

        int Size
        {
            get;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Snorm_Int8 : IImageComponentType
    {
        public int Size
        {
            get
            {
                return 1;
            }
        }

        public ChannelType ChannelType
        {
            get
            {
                return ChannelType.Snorm_Int8;
            }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Snorm_Int16 : IImageComponentType
    {
        public int Size
        {
            get
            {
                return 2;
            }
        }

        public ChannelType ChannelType
        {
            get
            {
                return ChannelType.Snorm_Int16;
            }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Unorm_Int8 : IImageComponentType
    {
        public int Size
        {
            get
            {
                return 1;
            }
        }

        public ChannelType ChannelType
        {
            get
            {
                return ChannelType.Unorm_Int8;
            }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Unorm_Short565 : IImageComponentType
    {
        public int Size
        {
            get
            {
                return 2;
            }
        }

        public ChannelType ChannelType
        {
            get
            {
                return ChannelType.Unorm_Short565;
            }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Unorm_Short555 : IImageComponentType
    {
        public int Size
        {
            get
            {
                return 2;
            }
        }

        public ChannelType ChannelType
        {
            get
            {
                return ChannelType.Unorm_Short555;
            }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Unorm_Int101010 : IImageComponentType
    {
        public int Size
        {
            get
            {
                return 4;
            }
        }

        public ChannelType ChannelType
        {
            get
            {
                return ChannelType.Unorm_Int101010;
            }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Signed_Int8 : IImageComponentType
    {
        public int Size
        {
            get
            {
                return 1;
            }
        }

        public ChannelType ChannelType
        {
            get
            {
                return ChannelType.Signed_Int8;
            }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Signed_Int16 : IImageComponentType
    {
        public int Size
        {
            get
            {
                return 2;
            }
        }

        public ChannelType ChannelType
        {
            get
            {
                return ChannelType.Signed_Int16;
            }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Signed_Int32 : IImageComponentType
    {
        public int Size
        {
            get
            {
                return 4;
            }
        }

        public ChannelType ChannelType
        {
            get
            {
                return ChannelType.Signed_Int32;
            }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Unsigned_Int8 : IImageComponentType
    {
        public int Size
        {
            get
            {
                return 1;
            }
        }

        public ChannelType ChannelType
        {
            get
            {
                return ChannelType.Unsigned_Int8;
            }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Unsigned_Int16 : IImageComponentType
    {
        public int Size
        {
            get
            {
                return 2;
            }
        }

        public ChannelType ChannelType
        {
            get
            {
                return ChannelType.Unsigned_Int16;
            }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Unsigned_Int32 : IImageComponentType
    {
        public int Size
        {
            get
            {
                return 4;
            }
        }

        public ChannelType ChannelType
        {
            get
            {
                return ChannelType.Unsigned_Int32;
            }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct HalfFloat : IImageComponentType
    {
        public int Size
        {
            get
            {
                return 2;
            }
        }

        public ChannelType ChannelType
        {
            get
            {
                return ChannelType.HalfFloat;
            }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Float : IImageComponentType
    {
        public int Size
        {
            get
            {
                return 4;
            }
        }

        public ChannelType ChannelType
        {
            get
            {
                return ChannelType.Float;
            }
        }
    }

    public interface IImageFormat : Brahma.IImageFormat
    {
        ChannelOrder ChannelOrder
        {
            get;
        }

        IImageComponentType ChannelType
        {
            get;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct R<T> : IImageFormat where T : struct, IImageComponentType
    {
        public T r;

        public int ComponentCount
        {
            get
            {
                return 1;
            }
        }

        public ChannelOrder ChannelOrder
        {
            get
            {
                return ChannelOrder.R;
            }
        }

        private static readonly T _channelType = new T();

        public IImageComponentType ChannelType
        {
            get
            {
                return _channelType;
            }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct A<T> : IImageFormat where T : struct, IImageComponentType
    {
        public T a;

        public int ComponentCount
        {
            get
            {
                return 1;
            }
        }

        public ChannelOrder ChannelOrder
        {
            get
            {
                return ChannelOrder.A;
            }
        }

        private static readonly T _channelType = new T();

        public IImageComponentType ChannelType
        {
            get
            {
                return _channelType;
            }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct RG<T> : IImageFormat where T : struct, IImageComponentType
    {
        public T r;
        public T g;

        public int ComponentCount
        {
            get
            {
                return 2;
            }
        }

        public ChannelOrder ChannelOrder
        {
            get
            {
                return ChannelOrder.RG;
            }
        }

        private static readonly T _channelType = new T();

        public IImageComponentType ChannelType
        {
            get
            {
                return _channelType;
            }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct RA<T> : IImageFormat where T : struct, IImageComponentType
    {
        public T r;
        public T a;

        public int ComponentCount
        {
            get
            {
                return 2;
            }
        }

        public ChannelOrder ChannelOrder
        {
            get
            {
                return ChannelOrder.RA;
            }
        }

        private static readonly T _channelType = new T();

        public IImageComponentType ChannelType
        {
            get
            {
                return _channelType;
            }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct RGB<T> : IImageFormat where T : struct, IImageComponentType
    {
        public T r;
        public T g;
        public T b;

        public int ComponentCount
        {
            get
            {
                return 3;
            }
        }

        public ChannelOrder ChannelOrder
        {
            get
            {
                return ChannelOrder.RGB;
            }
        }

        private static readonly T _channelType = new T();

        public IImageComponentType ChannelType
        {
            get
            {
                return _channelType;
            }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct RGBA<T> : IImageFormat where T : struct, IImageComponentType
    {
        public T r;
        public T g;
        public T b;
        public T a;

        public int ComponentCount
        {
            get
            {
                return 4;
            }
        }

        public ChannelOrder ChannelOrder
        {
            get
            {
                return ChannelOrder.RGBA;
            }
        }

        private static readonly T _channelType = new T();

        public IImageComponentType ChannelType
        {
            get
            {
                return _channelType;
            }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct BGRA<T> : IImageFormat where T : struct, IImageComponentType
    {
        public T b;
        public T g;
        public T r;
        public T a;

        public int ComponentCount
        {
            get
            {
                return 4;
            }
        }

        public ChannelOrder ChannelOrder
        {
            get
            {
                return ChannelOrder.RGBA;
            }
        }

        private static readonly T _channelType = new T();

        public IImageComponentType ChannelType
        {
            get
            {
                return _channelType;
            }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ARGB<T> : IImageFormat where T : struct, IImageComponentType
    {
        public T a;
        public T r;
        public T g;
        public T b;

        public int ComponentCount
        {
            get
            {
                return 4;
            }
        }

        public ChannelOrder ChannelOrder
        {
            get
            {
                return ChannelOrder.ARGB;
            }
        }

        private static readonly T _channelType = new T();

        public IImageComponentType ChannelType
        {
            get
            {
                return _channelType;
            }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Intensity<T> : IImageFormat where T : struct, IImageComponentType
    {
        public T k;

        public int ComponentCount
        {
            get
            {
                return 1;
            }
        }

        public ChannelOrder ChannelOrder
        {
            get
            {
                return ChannelOrder.Intensity;
            }
        }

        private static readonly T _channelType = new T();

        public IImageComponentType ChannelType
        {
            get
            {
                return _channelType;
            }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Luminance<T> : IImageFormat where T : struct, IImageComponentType
    {
        public T l;

        public int ComponentCount
        {
            get
            {
                return 1;
            }
        }

        public ChannelOrder ChannelOrder
        {
            get
            {
                return ChannelOrder.Luminance;
            }
        }

        private static readonly T _channelType = new T();

        public IImageComponentType ChannelType
        {
            get
            {
                return _channelType;
            }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ReadOnly: IImageModifier
    {
        public bool Modifier
        {
            get
            {
                return true;
            }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct WriteOnly : IImageModifier
    {
        public bool Modifier
        {
            get
            {
                return false;
            }
        }
    }
}
