using Google.FlatBuffers;
using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace SerializerTests.Serializers
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [SerializerType("https://google.github.io/flatbuffers/",
                    SerializerTypes.Binary | SerializerTypes.SupportsVersioning)]
    class FlatBuffer<TSerialize> : TestBase<TSerialize, TSerialize, ByteBuffer>
    {
        public FlatBuffer(Func<int, TSerialize> testData, Action<TSerialize, int,int> touchAndVerify):base(testData, touchAndVerify)
        {
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        protected override void Serialize(TSerialize obj, Stream stream)
        {
            if(obj is BookShelfFlat bookShelf)
            {
                stream.Write(bookShelf.ByteBuffer.ToFullArray(), bookShelf.ByteBuffer.Position, bookShelf.ByteBuffer.Length - bookShelf.ByteBuffer.Position);
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        protected override TSerialize Deserialize(Stream stream)
        {
            MemoryStream mem = new MemoryStream();
            // Since flatbuffers do not support memory streams we have to copy here
            stream.CopyTo(mem);
            byte[] data = mem.ToArray();
            var bookShelf = BookShelfFlat.GetRootAsBookShelfFlat(new ByteBuffer(data));
            return (TSerialize) (object) bookShelf;
        }
    }
}
