using MsgPack;
using MsgPack.Serialization;
using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace SerializerTests.Serializers
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [SerializerType("https://github.com/msgpack/msgpack-cli",
                    SerializerTypes.Binary | SerializerTypes.ProtocolMessagePack | SerializerTypes.SupportsVersioning)]
    class MsgPack_Cli<T> : TestBase<T, MessagePackSerializer> where T : class
    {
        public MsgPack_Cli(Func<int, T> testData, Action<T> dataToucher) : base(testData, dataToucher)
        {
            FormatterFactory = () =>
            {
                var lret = MessagePackSerializer.Get<T>(); 
                return lret;
            };
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        protected override void Serialize(T obj, Stream stream)
        {
            // Creates serializer.
            Formatter.Pack(stream, obj);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        protected override T Deserialize(Stream stream)
        {
            return (T) Formatter.Unpack(stream);
        }
    }
}
