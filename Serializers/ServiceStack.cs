using ServiceStack.Text;
using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace SerializerTests.Serializers
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [SerializerType("https://github.com/ServiceStack/ServiceStack.Text",
                    SerializerTypes.Json | SerializerTypes.SupportsVersioning)]
    class ServiceStack<TSerialize> : TestBase<TSerialize, TSerialize, Tracer> where TSerialize : class
    {
        public ServiceStack(Func<int, TSerialize> testData, Action<TSerialize,int,int> touchAndVerify) : base(testData, touchAndVerify)
        {
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        protected override void Serialize(TSerialize obj, Stream stream)
        {
            JsonSerializer.SerializeToStream<TSerialize>(obj, stream);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        protected override TSerialize Deserialize(Stream stream)
        {
            return JsonSerializer.DeserializeFromStream<TSerialize>(stream);
        }
    }
}
