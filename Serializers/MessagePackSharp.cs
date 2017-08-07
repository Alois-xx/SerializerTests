using MessagePack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SerializerTests.Serializers
{
    public class MessagePackSharp<T> : TestBase<T, MessagePack.IFormatterResolver> where T : class
    {
        public MessagePackSharp(Func<int, T> testData)
        {
            base.CreateNTestData = testData;
            FormatterFactory = () => MessagePack.Resolvers.StandardResolver.Instance;
        }

        protected override void Serialize(T obj, Stream stream)
        {
            MessagePackSerializer.Serialize(stream, obj, Formatter);
        }

        protected override T Deserialize(Stream stream)
        {
            return MessagePackSerializer.Deserialize<T>(stream, Formatter);
        }
    }
}
