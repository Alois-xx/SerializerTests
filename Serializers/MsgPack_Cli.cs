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
    class MsgPack_Cli<TSerialize> : TestBase<TSerialize, TSerialize, MessagePackSerializer> where TSerialize : class
    {
        public MsgPack_Cli(Func<int, TSerialize> testData, Action<TSerialize,int,int> touchAndVerify) : base(testData, touchAndVerify)
        {
            FormatterFactory = () =>
            {
                var lret = MessagePackSerializer.Get<TSerialize>(); 
                return lret;
            };
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        protected override void Serialize(TSerialize obj, Stream stream)
        {
            // Creates serializer.
            Formatter.Pack(stream, obj);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        protected override TSerialize Deserialize(Stream stream)
        {
            return (TSerialize) Formatter.Unpack(stream);
        }
    }
}
