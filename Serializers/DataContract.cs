using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.CompilerServices;

namespace SerializerTests.Serializers
{
    [SerializerType("https://docs.microsoft.com/en-us/dotnet/api/system.runtime.serialization.datacontractserializer",
                    SerializerTypes.Xml | SerializerTypes.SupportsVersioning)]
    public class DataContract<T> : TestBase<T, DataContractSerializer> where T : class
    {
        public DataContract(Func<int, T> testData, Action<T,int,int> touchAndVerify, bool refTracking = false) : base(testData, touchAndVerify, refTracking)
        {
            FormatterFactory = () => new DataContractSerializer(typeof(T), new DataContractSerializerSettings
                                                                          { PreserveObjectReferences = RefTracking } );
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        protected override void Serialize(T obj, Stream stream)
        {
            Formatter.WriteObject(stream, obj);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        protected override T Deserialize(Stream stream)
        {
            return (T)Formatter.ReadObject(stream);
        }
    }
}
