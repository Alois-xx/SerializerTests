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
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [SerializerType("https://github.com/mgravell/protobuf-net",
                    SerializerTypes.Binary | SerializerTypes.ProtocolProtobuf | SerializerTypes.SupportsVersioning)]
    public class Protobuf_net<TSerialize> : TestBase<TSerialize, TSerialize, RuntimeTypeModel> where TSerialize : class
    {
        public Protobuf_net(Func<int, TSerialize> testData, Action<TSerialize,int,int> touchAndVerify) : base(testData, touchAndVerify)
        {
            FormatterFactory = () => RuntimeTypeModel.Create();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        protected override void Serialize(TSerialize obj, Stream stream)
        {
            Formatter.Serialize(stream, obj);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        protected override TSerialize Deserialize(Stream stream)
        {
            return (TSerialize)Formatter.Deserialize(stream, null, typeof(TSerialize));
        }
    }
}
