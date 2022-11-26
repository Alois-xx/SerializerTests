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
    class Hyperion<TSerialize> : TestBase<TSerialize, TSerialize, Serializer> where TSerialize : class
    {
        public Hyperion(Func<int, TSerialize> testData, Action<TSerialize,int,int> touchAndVerify, bool refTracking = false) : base(testData, touchAndVerify, refTracking)
        {
            FormatterFactory = () => new Serializer(new SerializerOptions( preserveObjectReferences: RefTracking));
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        protected override void Serialize(TSerialize obj, Stream stream)
        {
            Formatter.Serialize(obj, stream);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        protected override TSerialize Deserialize(Stream stream)
        {
            return Formatter.Deserialize<TSerialize>(stream);
        }
    }
}
