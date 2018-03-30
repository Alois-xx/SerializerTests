using MessagePack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SerializerTests.Serializers
{
    /// <summary>
    /// https://github.com/neuecc/MessagePack-CSharp
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MessagePackSharp<T> : TestBase<T, MessagePack.IFormatterResolver> where T : class
    {
        public MessagePackSharp(Func<int, T> testData, Action<T> dataToucher) : base(testData, dataToucher)
        {
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
