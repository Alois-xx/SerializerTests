using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Salar.Bois;
using System.Runtime.CompilerServices;
using Salar.Bois.LZ4;

namespace SerializerTests.Serializers
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [SerializerType("https://github.com/salarcode/Bois",
                    SerializerTypes.Binary)]
    class Bois<TSerialize> : TestBase<TSerialize, TSerialize, BoisSerializer>
    {
        new BoisSerializer Formatter = new BoisSerializer();
        public Bois(Func<int, TSerialize> testData, Action<TSerialize,int,int> touchAndVerify) : base(testData, touchAndVerify)
        {
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        protected override void Serialize(TSerialize obj, Stream stream)
        {
            Formatter.Serialize<TSerialize>(obj, stream);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        protected override TSerialize Deserialize(Stream stream)
        {
            return Formatter.Deserialize<TSerialize>(stream);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [SerializerType("https://github.com/salarcode/Bois",
	    SerializerTypes.Binary)]
    class Bois_LZ4<TSerialize> : TestBase<TSerialize, TSerialize, BoisLz4Serializer>
    {
	    new BoisLz4Serializer Formatter = new BoisLz4Serializer();
	    public Bois_LZ4(Func<int, TSerialize> testData, Action<TSerialize,int,int> touchAndVerify) : base(testData, touchAndVerify)
	    {
	    }

	    [MethodImpl(MethodImplOptions.NoInlining)]
	    protected override void Serialize(TSerialize obj, Stream stream)
	    {
		    Formatter.Pickle(obj, stream);
	    }

	    [MethodImpl(MethodImplOptions.NoInlining)]
	    protected override TSerialize Deserialize(Stream stream)
	    {
		    return Formatter.Unpickle<TSerialize>(stream);
	    }
    }

}
