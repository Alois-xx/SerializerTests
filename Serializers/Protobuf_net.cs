using ProtoBuf.Meta;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SerializerTests
{
    // https://github.com/mgravell/protobuf-net
    public class Protobuf_net<T> : TestBase<T, RuntimeTypeModel> where T : class
    {
        public Protobuf_net(Func<int,T> testData)
        {
            base.CreateNTestData = testData;
            FormatterFactory = () => RuntimeTypeModel.Create();
        }

        protected override void Serialize(T obj, Stream stream)
        {
            Formatter.Serialize(stream, obj);
        }

        protected override T Deserialize(Stream stream)
        {
            return (T)Formatter.Deserialize(stream, null, typeof(T));
        }
    }
}
