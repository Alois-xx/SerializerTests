#if NETCOREAPP3_1 || NETCOREAPP3_0 || NET5_0
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
    class BinaryPack<T> : TestBase<T, Program> where T : class, new()
    {

        public BinaryPack(Func<int, T> testData, Action<T, int, int> touchAndVerify) : base(testData, touchAndVerify)
        {
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        protected override void Serialize(T obj, Stream stream)
        {
            BinaryConverter.Serialize(obj, stream);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        protected override T Deserialize(Stream stream)
        {
            return BinaryConverter.Deserialize<T>(stream);
        }
    }
}
#endif
