using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Utf8Json;

namespace SerializerTests.Serializers
{
	/// <summary>
	/// https://github.com/neuecc/Utf8Json
	/// </summary>
	/// <typeparam name="T"></typeparam>
	class Utf8JsonSerializer<T> : TestBase<T, IJsonFormatter<T>> where T : class
	{
		public Utf8JsonSerializer(Func<int, T> testData, Action<T> data, bool refTracking = false) : base(testData, data, refTracking)
		{
		}

		protected override void Serialize(T obj, Stream stream)
		{
			JsonSerializer.Serialize(stream, obj);
		}

		protected override T Deserialize(Stream stream)
		{
			return JsonSerializer.Deserialize<T>(stream);
		}
	}
}
