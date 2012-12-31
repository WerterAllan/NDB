using System;

namespace NDatabase.Odb.Core.Layers.Layer3.IO
{
    /// <summary>
    ///   The basic IO interface for basic IO operation like reading and writing bytes
    /// </summary>
    internal interface IOdbFileStream : IDisposable
    {
        /// <summary>
        ///  Sets the current position of this stream to the given value
        /// </summary>
        /// <param name="position">offset</param>
        void SetPosition(long position);

        void Write(byte value);

        void Write(byte[] buffer, int size);

        long Read(byte[] buffer, int size);

        int Read();

        /// <summary>
        /// Gets the length in bytes of the stream
        /// </summary>
        long Length { get; }
    }
}
