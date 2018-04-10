using FlatBuffers;
using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace SerializerTests.Serializers
{
    /// <summary>
    /// https://google.github.io/flatbuffers/
    /// </summary>
    /// <typeparam name="T"></typeparam>
    class FlatBuffer<T> : TestBase<BookShelfFlat, ByteBuffer>
    {
        public FlatBuffer(Func<int, BookShelfFlat> testData, Action<BookShelfFlat> toucher):base(testData, toucher)
        {
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        protected override void Serialize(BookShelfFlat obj, Stream stream)
        {
            stream.Write(obj.ByteBuffer.Data, obj.ByteBuffer.Position, obj.ByteBuffer.Length - obj.ByteBuffer.Position);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        protected override BookShelfFlat Deserialize(Stream stream)
        {
            MemoryStream mem = new MemoryStream();
            // Since flatbuffers do not support memory streams we have to copy here
            stream.CopyTo(mem);
            byte[] data = mem.ToArray();
            var bookShelf = BookShelfFlat.GetRootAsBookShelfFlat(new ByteBuffer(data));
            return bookShelf;
        }
    }
}
