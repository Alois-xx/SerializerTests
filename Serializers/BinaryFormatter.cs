using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace SerializerTests.Serializers
{
    [SerializerType("https://docs.microsoft.com/en-us/dotnet/api/system.runtime.serialization.formatters.binary.binaryformatter",
                    SerializerTypes.Binary|SerializerTypes.SupportsVersioning)]
    public class BinaryFormatter<TSerialize> : TestBase<TSerialize, TSerialize, System.Runtime.Serialization.Formatters.Binary.BinaryFormatter> where TSerialize : class
    {
        public BinaryFormatter(Func<int, TSerialize> testData, Action<TSerialize,int,int> touchAndVerify) : base(testData, touchAndVerify)
        {
            FormatterFactory = () => new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        protected override void Serialize(TSerialize obj, Stream stream)
        {
#pragma warning disable SYSLIB0011 // Type or member is obsolete
            Formatter.Serialize(stream, obj);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        protected override TSerialize Deserialize(Stream stream)
        {
            return (TSerialize)Formatter.Deserialize(stream);
        }
    }

#pragma warning restore SYSLIB0011 // Type or member is obsolete

}
