using System;

namespace Agents.CSharp
{
  public interface IDataSource : IDisposable
  {
    void Read(byte[] buffer, int offset, int count);
    byte ReadByte();

    long Length { get; }
    long Position { get; }
  }
}
