using MessagePack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace SerializerTests.Serializers
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [SerializerType("https://github.com/neuecc/MessagePack-CSharp",
                    SerializerTypes.Binary | SerializerTypes.ProtocolMessagePack | SerializerTypes.SupportsVersioning)]
    public class MessagePackSharp<TSerialize> : TestBase<TSerialize, TSerialize, MessagePack.IFormatterResolver> where TSerialize : class
    {
        public MessagePackSharp(Func<int, TSerialize> testData, Action<TSerialize,int,int> touchAndVerify) : base(testData, touchAndVerify)
        {
            FormatterFactory = () => MessagePack.Resolvers.StandardResolver.Instance;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        protected override void Serialize(TSerialize obj, Stream stream)
        {
            MessagePackSerializer.Serialize(typeof(TSerialize), stream, obj);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        protected override TSerialize Deserialize(Stream stream)
        {
            return MessagePackSerializer.Deserialize<TSerialize>(stream);
        }
    }
}
