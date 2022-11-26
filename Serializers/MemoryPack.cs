#if !NET48
using MessagePack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using MemoryPack;

namespace SerializerTests.Serializers
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [SerializerType("https://github.com/Cysharp/MemoryPack", SerializerTypes.Binary | SerializerTypes.SupportsVersioning)]
    public class MemoryPack<TSerialize> : TestBase<TSerialize, TSerialize, string> where TSerialize : class
    {
        public MemoryPack(Func<int, TSerialize> testData, Action<TSerialize,int,int> touchAndVerify) : base(testData, touchAndVerify)
        {
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        protected override void Serialize(TSerialize obj, Stream stream)
        {
            var valueTask = MemoryPackSerializer.SerializeAsync(stream, obj);
            valueTask.GetAwaiter().GetResult();
            if (!valueTask.IsCompletedSuccessfully)
            {
                throw new Exception("MemoryPack.Serialize should complete synchronously!");
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        protected override TSerialize Deserialize(Stream stream)
        {
            var valueTask = MemoryPackSerializer.DeserializeAsync<TSerialize>(stream);
            valueTask.GetAwaiter().GetResult();
            if (!valueTask.IsCompletedSuccessfully)
            {
                throw new Exception("MemoryPack.Deserialize should complete synchronously!");
            }
            return valueTask.Result;
        }
    }
}
#endif