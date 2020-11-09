#if ( NETCOREAPP3_1 || NETCOREAPP3_0 )

namespace SerializerTests.Serializers
{
    using System;
    using System.IO;
    using System.Runtime.CompilerServices;
    using SpanJson;
    using SpanJson.Resolvers;

    [SerializerType("https://github.com/Tornhoof/SpanJson",
                    SerializerTypes.Json)]
    class SpanJson<T> : TestBase<T, SpanJsonOptions>
    {
        public SpanJson(Func<int, T> testData, Action<T,int,int> touchAndVerify) : base(testData, touchAndVerify)
        {
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        protected override void Serialize(T obj, Stream stream)
        {
            JsonSerializer.Generic.Utf8.SerializeAsync(obj, stream).GetAwaiter().GetResult();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        protected override T Deserialize(Stream stream)
        {
            return JsonSerializer.Generic.Utf8.DeserializeAsync<T>(stream).Result;
        }
    }
}

#endif
