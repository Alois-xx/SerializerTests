using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using Revenj.Serialization;
using Revenj.Serialization.Json;

namespace SerializerTests.Serializers
{
    [SerializerType("https://github.com/ngs-doo/revenj",
                SerializerTypes.Json | SerializerTypes.SupportsVersioning)]
    public class Revenj<T> : TestBase<T, Revenj.Serialization.JsonSerialization>
    {
        class RevenjBinder : SerializationBinder
        {
            public override Type BindToType(string assemblyName, string typeName)
            {
                return null;
            }

            public override void BindToName(Type serializedType, out string assemblyName, out string typeName)
            {
                assemblyName = null;
                typeName = serializedType.FullName;
            }
        }

        public Revenj(Func<int, T> testData, Action<T> toucher) : base(testData, toucher)
        {
            FormatterFactory = () => new JsonSerialization(new RevenjBinder());
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        protected override void Serialize(T obj, Stream stream)
        {
            Formatter.Serialize(obj, stream, minimal:true);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        protected override T Deserialize(Stream stream)
        {
            return (T) Formatter.Deserialize(stream, typeof(T), default(StreamingContext));
        }
    }
}
