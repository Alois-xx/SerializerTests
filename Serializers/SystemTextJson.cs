#if NETCOREAPP3_1_OR_GREATER

using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace SerializerTests.Serializers
{
    /// <summary>
    /// Was added as part of .NET Core 3.0
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [SerializerType("https://docs.microsoft.com/en-us/dotnet/core/whats-new/dotnet-core-3-0 See Fast built-in JSON support",
                    SerializerTypes.Json | SerializerTypes.SupportsVersioning)]
    class SystemTextJson<TSerialize> : TestBase<TSerialize, TSerialize, JsonSerializerOptions>
    {
        // Enable support for public fields which are only supported since .NET 5.0
        JsonSerializerOptions myOptions =
#if NET5_0_OR_GREATER
            new JsonSerializerOptions { IncludeFields = true };
#elif NETCOREAPP3_1
      new JsonSerializerOptions {  };
#endif

        public SystemTextJson(Func<int, TSerialize> testData, Action<TSerialize,int,int> touchAndVerify) : base(testData, touchAndVerify)
        {
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        protected override void Serialize(TSerialize obj, Stream stream)
        {
            JsonSerializer.SerializeAsync(stream, obj, myOptions).Wait();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        protected override TSerialize Deserialize(Stream stream)
        {
            return (TSerialize) JsonSerializer.DeserializeAsync(stream, typeof(TSerialize), myOptions).Result;
        }
    }
}

#endif
