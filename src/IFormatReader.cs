using Dragoman.Intermediate;
using System.IO;

namespace Dragoman
{
    /// <summary>
    /// When implemented, defines a reader object for reading a specific file format
    /// </summary>
    public interface IFormatReader
    {
        IIntermediateWriter ReadBytes(byte[] bytes, int offset);
        IIntermediateWriter ReadStream(Stream data, int offset);
    }
}
