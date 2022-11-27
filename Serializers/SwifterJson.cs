#if ( NET5_0_OR_GREATER )

namespace SerializerTests.Serializers
{
    using System;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Text;
    using Swifter.Json;

    [SerializerType("https://github.com/Dogwei/Swifter.Json",
                    SerializerTypes.Json)]
    class SwifterJson<TSerialize> : TestBase<TSerialize, TSerialize, JsonFormatter>
    {
        public SwifterJson(Func<int, TSerialize> testData, Action<TSerialize,int,int> touchAndVerify) : base(testData, touchAndVerify)
        {
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        protected override void Serialize(TSerialize obj, Stream stream)
        {
            JsonFormatter.SerializeObjectAsync(obj, stream, Encoding.UTF8).AsTask().Wait();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        protected override TSerialize Deserialize(Stream stream)
        {
            return JsonFormatter.DeserializeObjectAsync<TSerialize>(stream, Encoding.UTF8).Result;
        }
    }
}

#endif
