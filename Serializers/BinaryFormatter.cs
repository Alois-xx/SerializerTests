using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace SerializerTests.Serializers
{
    [SerializerType("https://docs.microsoft.com/en-us/dotnet/api/system.runtime.serialization.formatters.binary.binaryformatter",
                    SerializerTypes.Binary|SerializerTypes.SupportsVersioning)]
    public class BinaryFormatter<T> : TestBase<T, System.Runtime.Serialization.Formatters.Binary.BinaryFormatter> where T : class
    {
        public BinaryFormatter(Func<int, T> testData, Action<T,int,int> touchAndVerify) : base(testData, touchAndVerify)
        {
            FormatterFactory = () => new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        protected override void Serialize(T obj, Stream stream)
        {
#pragma warning disable SYSLIB0011 // Type or member is obsolete
            Formatter.Serialize(stream, obj);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        protected override T Deserialize(Stream stream)
        {
            return (T)Formatter.Deserialize(stream);
        }
    }

#pragma warning restore SYSLIB0011 // Type or member is obsolete

}
