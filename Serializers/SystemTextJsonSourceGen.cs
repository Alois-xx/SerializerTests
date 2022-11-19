#if NET7_0_OR_GREATER

using SerializerTests.TypesToSerialize;
using ServiceStack.Text.Pools;
using System;
using System.Buffers;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SerializerTests.Serializers
{
    [JsonSourceGenerationOptions(GenerationMode = JsonSourceGenerationMode.Default, IncludeFields = true)]
    [JsonSerializable(typeof(BookShelf))]
    [JsonSerializable(typeof(BookShelf1))]
    [JsonSerializable(typeof(BookShelf2))]
    [JsonSerializable(typeof(LargeBookShelf))]
    internal partial class MyJsonContext : JsonSerializerContext
    {
    }

    /// <summary>
    /// Source Generators for .NET were added with .NET 7.0 which generate de/serialization code during compilation
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [SerializerType("https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/source-generation See docu for Source Generators",
                    SerializerTypes.Json | SerializerTypes.SupportsVersioning)]
    class SystemTextJsonSourceGen<T> : TestBase<T, JsonSerializerOptions>
    {
        public SystemTextJsonSourceGen(Func<int, T> testData, Action<T, int, int> touchAndVerify) : base(testData, touchAndVerify)
        {
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        protected override void Serialize(T obj, Stream stream)
        {
            // Use overload which uses precompiled serialization code
            string dat = JsonSerializer.Serialize(obj, MyJsonContext.Default.Options);

            using IMemoryOwner<byte> utf8_Owner = MemoryPool<byte>.Shared.Rent(dat.Length*2);

            // we still need to convert back to UTF8 or we will write UTF-16 with double size to disk
            int bytes = Encoding.UTF8.GetBytes(dat, utf8_Owner.Memory.Span);
            Span<byte> utf8Span = utf8_Owner.Memory.Span[..bytes];
                        
            stream.Write(utf8Span);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        protected override T Deserialize(Stream stream)
        {
            // Currently we need to use the MetaData approach for deserialization. 
            // Precompiled Deserialization seems not to be supported yet? 

            if (typeof(T) == typeof(BookShelf))
            {
                return (T) (object) JsonSerializer.Deserialize(stream, MyJsonContext.Default.BookShelf);
            }
            else if (typeof(T) == typeof(BookShelf1))
            {
                return (T) JsonSerializer.Deserialize(stream, MyJsonContext.Default.BookShelf1.Type, MyJsonContext.Default);
            }
            else if (typeof(T) == typeof(BookShelf2))
            {
                return (T) JsonSerializer.Deserialize(stream, MyJsonContext.Default.BookShelf2.Type, MyJsonContext.Default);
            }
            else if (typeof(T) == typeof(LargeBookShelf))
            {
                return (T)JsonSerializer.Deserialize(stream, MyJsonContext.Default.LargeBookShelf.Type, MyJsonContext.Default);
            }

            throw new NotSupportedException($"No source generator for type {typeof(T).Name} declared.");

        }
    }
}

#endif
