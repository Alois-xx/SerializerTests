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
    class SwifterJson<T> : TestBase<T, JsonFormatter>
    {
        public SwifterJson(Func<int, T> testData, Action<T,int,int> touchAndVerify) : base(testData, touchAndVerify)
        {
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        protected override void Serialize(T obj, Stream stream)
        {
            JsonFormatter.SerializeObjectAsync(obj, stream, Encoding.UTF8).AsTask().Wait();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        protected override T Deserialize(Stream stream)
        {
            return JsonFormatter.DeserializeObjectAsync<T>(stream, Encoding.UTF8).Result;
        }
    }
}

#endif
