using FlatBuffers;
using System;
using System.IO;

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

        protected override void Serialize(BookShelfFlat obj, Stream stream)
        {
            stream.Write(obj.ByteBuffer.Data, obj.ByteBuffer.Position, obj.ByteBuffer.Length - obj.ByteBuffer.Position);
        }

        protected override BookShelfFlat Deserialize(Stream stream)
        {
            MemoryStream mem = new MemoryStream();
            stream.CopyTo(mem);
            byte[] data = mem.ToArray();
            var bookShelf = BookShelfFlat.GetRootAsBookShelfFlat(new ByteBuffer(data));
            return bookShelf;
        }
    }
}
