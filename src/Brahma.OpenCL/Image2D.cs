using System;
using System.Collections.Generic;
using OpenCL.Net;

namespace Brahma.OpenCL
{
    public sealed class Image2D<T> : Brahma.Image2D<T> where T : struct, IImageFormat
    {
        private static readonly T _imageFormat = new T();

        private readonly object _image;
        private readonly int _width;
        private readonly int _height;
        private readonly int _rowPitch = -1;

        public Image2D(ComputeProvider provider, Operations operations, bool hostAccessible, int width, int height, int rowPitch = -1) // Create, no data
        {
            ErrorCode error;
            _image = Cl.CreateImage2D(provider.Context, (MemFlags)operations | (hostAccessible ? MemFlags.AllocHostPtr : 0),
                new ImageFormat(_imageFormat.ChannelOrder, _imageFormat.ChannelType.ChannelType), (IntPtr)width, (IntPtr)height,
                rowPitch == -1 ? (IntPtr)(width * _imageFormat.ComponentCount * _imageFormat.ChannelType.Size) : (IntPtr)rowPitch,
                null, out error);

            if (error != ErrorCode.Success)
                throw new CLException(error);

            _width = width;
            _height = height;
            _rowPitch = rowPitch;
        }

        public Image2D(ComputeProvider provider, Operations operations, Memory memory, int width, int height, T[] data, int rowPitch = -1) // Create and copy/use data from host
        {
            ErrorCode error;
            _image = Cl.CreateImage2D(provider.Context, (MemFlags)operations | (memory == Memory.Host ? MemFlags.UseHostPtr : (MemFlags)memory | MemFlags.CopyHostPtr),
                new ImageFormat(_imageFormat.ChannelOrder, _imageFormat.ChannelType.ChannelType), (IntPtr)width, (IntPtr)height,
                rowPitch == -1 ? (IntPtr)(width * _imageFormat.ComponentCount * _imageFormat.ChannelType.Size) : (IntPtr)rowPitch,
                data, out error);

            if (error != ErrorCode.Success)
                throw new CLException(error);

            _width = width;
            _height = height;
            _rowPitch = rowPitch;
        }

        public int Width
        {
            get
            {
                return _width;
            }
        }

        public int Height
        {
            get
            {
                return _height;
            }
        }

        public int RowPitch
        {
            get
            {
                return _rowPitch;
            }
        }
    }
}
