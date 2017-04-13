using ProtoBuf;

namespace SerializerTests
{

    [ProtoContract]
    class User
    {
        [ProtoMember(1)]
        public IAbstraction Inst1 = null;

        [ProtoMember(2)]
        public IAbstraction Inst2 = null;
    }


    [ProtoContract]
    public interface IAbstraction
    {
        [ProtoMember(1)]
        string Name { get; set; }
    }


    [ProtoContract]
    class ImplA : IAbstraction
    {
        [ProtoMember(1)]
        public string Name { get; set; }

        public override string ToString()
        {
            return $"Name: {Name}";
        }
    }

    [ProtoContract]
    class BaseClass
    {
        [ProtoMember(1)]
        internal string NameBase { get; set; }
    }

    [ProtoContract]
    class ImplB : BaseClass, IAbstraction
    {
        [ProtoMember(1)]
        public string NameB { get; set; }

        [ProtoMember(2)]
        public string NameB2 { get; set; }
        public string Name { get; set; }

        /// <summary>
        /// When an interface is de/serialized (IAbstraction) then only the concrete type but NOT its base class
        /// proto contract members are de/serialized. 
        /// One can work around by adding to the derived class the corresponding getter/setters with the Proto members
        /// </summary>
        [ProtoMember(3)]
        public new string NameBase
        {
            get
            {
                return base.NameBase;
            }

            set
            {
                base.NameBase = value;
            }
        }

        public override string ToString()
        {
            return $"Name: {Name}, NameB: {NameB}, NameB2: {NameB2}";
        }
    }

}
