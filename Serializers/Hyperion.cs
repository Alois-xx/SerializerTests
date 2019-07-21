using Hyperion;
using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace SerializerTests.Serializers
{
    /// <summary>
    /// Not active anymore since it does not work with .NET Core 3
    /// https://github.com/akkadotnet/Hyperion/
    /// </summary>
    class Hyperion<T> : TestBase<T, Serializer> where T : class
    {
        public Hyperion(Func<int, T> testData, Action<T> dataToucher, bool refTracking = false) : base(testData, dataToucher, refTracking)
        {
            FormatterFactory = () => new Serializer(new SerializerOptions( preserveObjectReferences: RefTracking));
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        protected override void Serialize(T obj, Stream stream)
        {
            Formatter.Serialize(obj, stream);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        protected override T Deserialize(Stream stream)
        {
            return Formatter.Deserialize<T>(stream);
        }
    }
}
