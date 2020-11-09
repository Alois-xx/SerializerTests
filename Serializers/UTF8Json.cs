using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using Utf8Json;

namespace SerializerTests.Serializers
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [SerializerType("https://github.com/neuecc/Utf8Json", 
                     SerializerTypes.Json | SerializerTypes.SupportsVersioning)]
    class Utf8JsonSerializer<T> : TestBase<T, IJsonFormatter<T>> where T : class
	{
		public Utf8JsonSerializer(Func<int, T> testData, Action<T,int,int> touchAndVerify, bool refTracking = false) : base(testData, touchAndVerify, refTracking)
		{
		}

        [MethodImpl(MethodImplOptions.NoInlining)]
        protected override void Serialize(T obj, Stream stream)
		{
			JsonSerializer.Serialize(stream, obj);
		}

        [MethodImpl(MethodImplOptions.NoInlining)]
        protected override T Deserialize(Stream stream)
		{
			return JsonSerializer.Deserialize<T>(stream);
		}
	}
}
