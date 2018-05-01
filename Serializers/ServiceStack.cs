using ServiceStack.Text;
using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace SerializerTests.Serializers
{
    /// <summary>
    /// https://github.com/ServiceStack/ServiceStack.Text
    /// </summary>
    /// <typeparam name="T"></typeparam>
    class ServiceStack<T> : TestBase<T, Tracer> where T : class
    {
        public ServiceStack(Func<int, T> testData, Action<T> dataToucher) : base(testData, dataToucher)
        {
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        protected override void Serialize(T obj, Stream stream)
        {
            JsonSerializer.SerializeToStream<T>(obj, stream);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        protected override T Deserialize(Stream stream)
        {
            return JsonSerializer.DeserializeFromStream<T>(stream);
        }
    }
}
