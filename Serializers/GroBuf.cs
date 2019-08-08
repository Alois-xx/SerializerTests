using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GroBuf;
using GroBuf.DataMembersExtracters;
using System.Runtime.CompilerServices;

namespace SerializerTests.Serializers
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [SerializerType("https://github.com/skbkontur/GroBuf",
                    SerializerTypes.Binary| SerializerTypes.SupportsVersioning)]
    public class GroBuf<T> : TestBase<T, Serializer> where T : class
    {
        new Serializer Formatter = new Serializer(new AllFieldsExtractor(), options: GroBufOptions.WriteEmptyObjects);

        public GroBuf(Func<int, T> testData, Action<T> dataToucher) : base(testData, dataToucher)
        {
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        protected override void Serialize(T obj, Stream stream)
        {
            var bytes = Formatter.Serialize(obj);
            stream.Write(bytes, 0, bytes.Length);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        protected override T Deserialize(Stream stream)
        {
            // GroBuf does not support deserializing from a stream. 
            // We need to copy the memory which costs some perf here
            MemoryStream mem = new MemoryStream();
            stream.CopyTo(mem);
            return Formatter.Deserialize<T>(mem.GetBuffer(), (int) mem.Length);
        }
    }
}
