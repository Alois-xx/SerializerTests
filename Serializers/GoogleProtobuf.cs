using Google.Protobuf;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace SerializerTests.Serializers
{
    [SerializerType("https://github.com/protocolbuffers/protobuf/",
                    SerializerTypes.Binary | SerializerTypes.ProtocolProtobuf | SerializerTypes.SupportsVersioning)]
    public class GoogleProtobuf<T> : TestBase<T, IMessage> where T : class, IMessage<T>, new()
    {
        public GoogleProtobuf(Func<int, T> testData, Action<T, int, int> touchAndVerify) : base(testData, touchAndVerify)
        {
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        protected override T Deserialize(Stream stream)
        {
            // Another approach could be to get the generated Parser via reflection, e.g.:
            // var parser = typeof(T).GetProperty("Parser", BindingFlags.Public | BindingFlags.Static).GetValue(null, null) as MessageParser<T>;
            // and then deserialize via:
            // var obj = parser.ParseFrom(stream);
            // Taken from: https://github.com/dotnet/orleans/blob/master/src/Serializers/Orleans.Serialization.Protobuf/ProtobufSerializer.cs

            var obj = new T();
            obj.MergeFrom(stream);
            return obj;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        protected override void Serialize(T obj, Stream stream)
        {
            obj.WriteTo(stream);
        }
    }
}
