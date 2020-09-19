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
    public class MessagePackSharp<T> : TestBase<T, MessagePack.IFormatterResolver> where T : class
    {
        public MessagePackSharp(Func<int, T> testData, Action<T> dataToucher) : base(testData, dataToucher)
        {
            FormatterFactory = () => MessagePack.Resolvers.StandardResolver.Instance;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        protected override void Serialize(T obj, Stream stream)
        {
            MessagePackSerializer.Serialize(typeof(T), stream, obj);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        protected override T Deserialize(Stream stream)
        {
            return MessagePackSerializer.Deserialize<T>(stream);
        }
    }
}
