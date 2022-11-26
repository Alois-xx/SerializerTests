using Google.Protobuf;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace SerializerTests.Serializers
{
    [SerializerType("https://github.com/protocolbuffers/protobuf/",
                    SerializerTypes.Binary | SerializerTypes.ProtocolProtobuf | SerializerTypes.SupportsVersioning)]
    public class GoogleProtobuf<TSerialize> : TestBase<TSerialize, TSerialize, IMessage> where TSerialize : class, IMessage<TSerialize>, new()
    {
        public GoogleProtobuf(Func<int, TSerialize> testData, Action<TSerialize, int, int> touchAndVerify) : base(testData, touchAndVerify)
        {
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        protected override TSerialize Deserialize(Stream stream)
        {
            // Another approach could be to get the generated Parser via reflection, e.g.:
            // var parser = typeof(TDeserialize).GetProperty("Parser", BindingFlags.Public | BindingFlags.Static).GetValue(null, null) as MessageParser<TSerialize>;
            // and then deserialize via:
            // var obj = parser.ParseFrom(stream);
            // Taken from: https://github.com/dotnet/orleans/blob/master/src/Serializers/Orleans.Serialization.Protobuf/ProtobufSerializer.cs

            var obj = new TSerialize();
            obj.MergeFrom(stream);
            return obj;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        protected override void Serialize(TSerialize obj, Stream stream)
        {
            obj.WriteTo(stream);
        }
    }
}
