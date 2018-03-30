using MsgPack;
using MsgPack.Serialization;
using System;
using System.IO;

namespace SerializerTests.Serializers
{
    /// <summary>
    /// https://github.com/msgpack/msgpack-cli
    /// </summary>
    /// <typeparam name="T"></typeparam>
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

        protected override void Serialize(T obj, Stream stream)
        {
            // Creates serializer.
            Formatter.Pack(stream, obj);
        }

        protected override T Deserialize(Stream stream)
        {
            return (T) Formatter.Unpack(stream);
        }
    }
}
