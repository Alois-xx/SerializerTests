using Ceras;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace SerializerTests.Serializers
{
    [SerializerType("https://github.com/rikimaru0345/Ceras",
                   SerializerTypes.Binary | SerializerTypes.SupportsVersioning)]
    class Ceras<TSerialize> : TestBase<TSerialize, TSerialize, CerasSerializer> where TSerialize : class
    {
        byte[] mySerializerBuffer = null;

        public Ceras(Func<int, TSerialize> testData, Action<TSerialize,int,int> dataToucher) : base(testData, dataToucher)
        {
            FormatterFactory = () => new CerasSerializer();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        protected override void Serialize(TSerialize obj, Stream stream)
        {
            int nBytesWritten = Formatter.Serialize(obj, ref mySerializerBuffer);
            stream.Write(mySerializerBuffer, 0, nBytesWritten);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        protected override TSerialize Deserialize(Stream stream)
        {
            // Ceras does not support Streams so we need to copy the data out of the stream
            // This causes some overhead
            byte[] pooledArray = ArrayPool<byte>.Shared.Rent((int) stream.Length);
            stream.Read(pooledArray, 0, (int)stream.Length);
            TSerialize ret = default;
            int offset = 0;
            Formatter.Deserialize<TSerialize>(ref ret, pooledArray, ref offset, (int)stream.Length);
            ArrayPool<byte>.Shared.Return(pooledArray);
            return ret;
        }
    }
}
