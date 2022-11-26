#if NETCOREAPP3_0_OR_GREATER
using System;
using System.IO;
using System.Runtime.CompilerServices;
using BinaryPack;

namespace SerializerTests.Serializers
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [SerializerType("https://github.com/Sergio0694/BinaryPack", SerializerTypes.Binary)]
    class BinaryPack<TSerialize> : TestBase<TSerialize, TSerialize, Program> where TSerialize : class, new()
    {

        public BinaryPack(Func<int, TSerialize> testData, Action<TSerialize, int, int> touchAndVerify) : base(testData, touchAndVerify)
        {
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        protected override void Serialize(TSerialize obj, Stream stream)
        {
            BinaryConverter.Serialize(obj, stream);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        protected override TSerialize Deserialize(Stream stream)
        {
            return BinaryConverter.Deserialize<TSerialize>(stream);
        }
    }
}
#endif
