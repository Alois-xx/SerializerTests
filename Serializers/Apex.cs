#if NETCOREAPP3_0
using Apex.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace SerializerTests.Serializers
{
    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [SerializerType("https://github.com/dbolin/Apex.Serialization",
                    SerializerTypes.Binary)]
    public sealed class ApexSerializer<TSerialize> : TestBase<TSerialize, TSerialize, IBinary> where TSerialize : class
    {
        public ApexSerializer(Func<int, TSerialize> testData, Action<TSerialize,int,int> touchAndVerify) : base(testData, touchAndVerify)
        {
            FormatterFactory = () => Binary.Create(new Settings { SerializationMode = RefTracking ? Mode.Graph : Mode.Tree });
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        protected override void Serialize(TSerialize obj, Stream stream)
        {
            Formatter.Write(obj, stream);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        protected override TDeserialize Deserialize(Stream stream)
        {
            return Formatter.Read<TSerialize>(stream);
        }
    }
}
#endif