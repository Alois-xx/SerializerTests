using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeroFormatter;
using System.Runtime.CompilerServices;

namespace SerializerTests.Serializers
{
    /// <summary>
    /// Project was abandoned by maintainer in favor of MessagePack-CSharp see https://github.com/neuecc/ZeroFormatter/issues/104
    /// https://github.com/neuecc/ZeroFormatter
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class ZeroFormatter<TSerialize> : TestBase<TSerialize, TSerialize, ZeroFormattableAttribute>
    {
        public ZeroFormatter(Func<int, TSerialize> testData, Action<TSerialize,int,int> touchAndVerify) : base(testData, touchAndVerify)
        {
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        protected override TSerialize Deserialize(Stream stream)
        {
            return ZeroFormatterSerializer.Deserialize<TSerialize>(stream);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        protected override void Serialize(TSerialize obj, Stream stream)
        {
            ZeroFormatterSerializer.Serialize<TSerialize>(stream, obj);
        }
    }
}
