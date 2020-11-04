#if ( NETCOREAPP3_1 || NETCOREAPP3_0 ||  NET5_0)

using SerializerTests.TypesToSerialize;
using SimdJsonSharp;
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
    class SystemTextJson<T> : TestBase<T, JsonSerializerOptions>
    {
        // Enable support for public fields which are only supported since .NET 5.0
        JsonSerializerOptions myOptions =
#if NET5_0
            new JsonSerializerOptions { IncludeFields = true };
#elif (NETCOREAPP3_1 || NETCOREAPP3_0) && !NET5_0
      new JsonSerializerOptions {  };
#endif

        public SystemTextJson(Func<int, T> testData, Action<T> dataToucher) : base(testData, dataToucher)
        {
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        protected override void Serialize(T obj, Stream stream)
        {
            JsonSerializer.SerializeAsync(stream, obj, myOptions).Wait();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        protected override T Deserialize(Stream stream)
        {
            return (T) JsonSerializer.DeserializeAsync(stream, typeof(T), myOptions).Result;
        }
    }
}

#endif
