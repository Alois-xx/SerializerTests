#if NETCOREAPP3_1_OR_GREATER

namespace SerializerTests.Serializers
{
    using System;
    using System.IO;
    using System.Runtime.CompilerServices;
    using SpanJson;
    using SpanJson.Resolvers;

    [SerializerType("https://github.com/Tornhoof/SpanJson",
                    SerializerTypes.Json)]
    class SpanJson<TSerialize> : TestBase<TSerialize, TSerialize, SpanJsonOptions>
    {
        public SpanJson(Func<int, TSerialize> testData, Action<TSerialize,int,int> touchAndVerify) : base(testData, touchAndVerify)
        {
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        protected override void Serialize(TSerialize obj, Stream stream)
        {
            JsonSerializer.Generic.Utf8.SerializeAsync(obj, stream).GetAwaiter().GetResult();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        protected override TSerialize Deserialize(Stream stream)
        {
            return JsonSerializer.Generic.Utf8.DeserializeAsync<TSerialize>(stream).Result;
        }
    }
}

#endif
