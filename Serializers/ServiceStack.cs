using ServiceStack.Text;
using System;
using System.IO;

namespace SerializerTests.Serializers
{
    class ServiceStack<T> : TestBase<T, Tracer> where T : class
    {
        public ServiceStack(Func<int, T> testData, Action<T> dataToucher) : base(testData, dataToucher)
        {
        }

        protected override void Serialize(T obj, Stream stream)
        {
            JsonSerializer.SerializeToStream<T>(obj, stream);
        }

        protected override T Deserialize(Stream stream)
        {
            return JsonSerializer.DeserializeFromStream<T>(stream);
        }
    }
}
