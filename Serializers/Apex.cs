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
    public sealed class ApexSerializer<T> : TestBase<T, IBinary> where T : class
    {
        public ApexSerializer(Func<int, T> testData, Action<T> dataToucher) : base(testData, dataToucher)
        {
            FormatterFactory = () => Binary.Create(new Settings { SerializationMode = RefTracking ? Mode.Graph : Mode.Tree });
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        protected override void Serialize(T obj, Stream stream)
        {
            Formatter.Write(obj, stream);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        protected override T Deserialize(Stream stream)
        {
            return Formatter.Read<T>(stream);
        }
    }
}
