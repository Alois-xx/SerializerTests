using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GroBuf;
using GroBuf.DataMembersExtracters;

namespace SerializerTests.Serializers
{
    // https://github.com/skbkontur/GroBuf
    public class GroBuf<T> : TestBase<T, Serializer> where T : class
    {
        new Serializer Formatter = new Serializer(new AllFieldsExtractor(), options: GroBufOptions.WriteEmptyObjects);

        public GroBuf(Func<int, T> testData, Action<T> dataToucher) : base(testData, dataToucher)
        {
        }

        protected override void Serialize(T obj, Stream stream)
        {
            var bytes = Formatter.Serialize(obj);
            stream.Write(bytes, 0, bytes.Length);
        }

        protected override T Deserialize(Stream stream)
        {
            MemoryStream mem = new MemoryStream();
            stream.CopyTo(mem);
            return Formatter.Deserialize<T>(mem.GetBuffer(), (int) mem.Length);
        }
    }
}
