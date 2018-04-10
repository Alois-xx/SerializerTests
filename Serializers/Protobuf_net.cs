using ProtoBuf.Meta;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace SerializerTests
{
    // https://github.com/mgravell/protobuf-net
    public class Protobuf_net<T> : TestBase<T, RuntimeTypeModel> where T : class
    {
        public Protobuf_net(Func<int, T> testData, Action<T> dataToucher) : base(testData, dataToucher)
        {
            FormatterFactory = () => RuntimeTypeModel.Create();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        protected override void Serialize(T obj, Stream stream)
        {
            Formatter.Serialize(stream, obj);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        protected override T Deserialize(Stream stream)
        {
            return (T)Formatter.Deserialize(stream, null, typeof(T));
        }
    }
}
