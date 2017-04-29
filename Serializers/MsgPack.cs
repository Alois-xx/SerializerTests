using MsgPack;
using System;
using System.IO;

namespace SerializerTests.Serializers
{
    class MsgPack<T> : TestBase<T, CompiledPacker> where T : class
    {
        public MsgPack(Func<int, T> testData)
        {
            base.CreateNTestData = testData;
            FormatterFactory = () =>
            {
                var lret = new CompiledPacker(true);
                return lret;
            };
        }

        protected override void Serialize(T obj, Stream stream)
        {
            Formatter.Pack(stream, obj);
        }

        protected override T Deserialize(Stream stream)
        {
            return Formatter.Unpack<T>(stream);
        }
    }
}
