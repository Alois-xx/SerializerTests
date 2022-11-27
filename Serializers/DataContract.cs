using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.CompilerServices;

namespace SerializerTests.Serializers
{
    [SerializerType("https://docs.microsoft.com/en-us/dotnet/api/system.runtime.serialization.datacontractserializer",
                    SerializerTypes.Xml | SerializerTypes.SupportsVersioning)]
    public class DataContract<TSerialize> : TestBase<TSerialize, TSerialize, DataContractSerializer> where TSerialize : class
    {
        public DataContract(Func<int, TSerialize> testData, Action<TSerialize,int,int> touchAndVerify, bool refTracking = false) : base(testData, touchAndVerify, refTracking)
        {
            FormatterFactory = () => new DataContractSerializer(typeof(TSerialize), new DataContractSerializerSettings
                                                                          { PreserveObjectReferences = RefTracking } );
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        protected override void Serialize(TSerialize obj, Stream stream)
        {
            Formatter.WriteObject(stream, obj);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        protected override TSerialize Deserialize(Stream stream)
        {
            return (TSerialize)Formatter.ReadObject(stream);
        }
    }
}
