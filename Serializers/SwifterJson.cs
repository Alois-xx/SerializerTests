#if NETCOREAPP3_0

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
        public SwifterJson(Func<int, T> testData, Action<T> dataToucher) : base(testData, dataToucher)
        {
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        protected override void Serialize(T obj, Stream stream)
        {
            JsonFormatter.SerializeObjectAsync(obj, stream, Encoding.UTF8);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        protected override T Deserialize(Stream stream)
        {
            return JsonFormatter.DeserializeObjectAsync<T>(stream, Encoding.UTF8).Result;
        }
    }
}

#endif
