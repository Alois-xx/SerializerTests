#if NET7_0_OR_GREATER

using SerializerTests.TypesToSerialize;
using System;
using System.Buffers;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SerializerTests.Serializers
{
    [JsonSerializable(typeof(BookShelf))]
    internal partial class MyJsonContext : JsonSerializerContext { }

    /// <summary>
    /// Source Generators for .NET were added with .NET 7.0 which generate de/serialization code during compilation
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [SerializerType("https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/source-generation See docu for Source Generators",
                    SerializerTypes.Json | SerializerTypes.SupportsVersioning)]
    class SystemTextJsonSourceGen<T> : TestBase<T, JsonSerializerOptions>
    {
        // Enable support for public fields which are only supported since .NET 5.0
        JsonSerializerOptions myOptions =new(){ IncludeFields = true, TypeInfoResolver = MyJsonContext.Default };
        public SystemTextJsonSourceGen(Func<int, T> testData, Action<T, int, int> touchAndVerify) : base(testData, touchAndVerify) { }

        [MethodImpl(MethodImplOptions.NoInlining)]
        protected override void Serialize(T obj, Stream stream) => JsonSerializer.Serialize(obj, myOptions);

        [MethodImpl(MethodImplOptions.NoInlining)]
        protected override T Deserialize(Stream stream) => JsonSerializer.Deserialize<T>(stream, myOptions);
    }
}
#endif