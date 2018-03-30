using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeroFormatter;

namespace SerializerTests.Serializers
{
    // Index is key of serialization
    [ZeroFormattable]
    public class ZeroFormatterBookShelf
    {
        [Index(0)]
        public virtual List<ZeroFormatterBook> Books
        {
            get;
            set;
        }
    }

    [ZeroFormattable]
    public class ZeroFormatterBook
    {
        private int id;

        [Index(0)]
        public virtual int Id { get => id; set => id = value; }

        [Index(1)]
        public virtual string Title { get; set; }
    }

    [ZeroFormattable]
    public class ZeroFormatterBookShelf1
    {

        [Index(0)]
        public virtual List<ZeroFormatterBook1> Books
        {
            get;
            set;
        }
    }
    [ZeroFormattable]
    public class ZeroFormatterBook1
    {
        private int id;

        [Index(0)]
        public virtual int Id { get => id; set => id = value; }

        [Index(1)]
        public virtual string Title { get; set; }
    }

    [ZeroFormattable]
    public class ZeroFormatterBookShelf2
    {

        [Index(0)]
        public virtual List<ZeroFormatterBook2> Books
        {
            get;
            set;
        }
    }
    [ZeroFormattable]
    public class ZeroFormatterBook2
    {
        private int id;

        [Index(0)]
        public virtual int Id { get => id; set => id = value; }

        [Index(1)]
        public virtual string Title { get; set; }
    }

    [ZeroFormattable]
    public class ZeroFormatterLargeBookShelf
    {
        [Index(0)]
        public virtual List<ZeroFormatterLargeBook> Books
        {
            get;
            set;
        }

        private string _Secret;

        private int _Id;

        private int _Id4;

        private double _Id5;

        private List<float> _Id6;

        private List<float> _Id7;

        private List<float> _Id8;

        private List<float> _Id9;

        private Dictionary<double, string> _Id10;

        private Dictionary<double, string> _Id11;

        private Dictionary<double, string> _Id12;

        private Dictionary<double, string> _Id13;

        [Index(8)] public virtual string Secret { get => _Secret; set => _Secret = value; }
        [Index(9)] public virtual int Id { get => _Id; set => _Id = value; }
        [Index(10)] public virtual int Id4 { get => _Id4; set => _Id4 = value; }
        [Index(11)] public virtual double Id5 { get => _Id5; set => _Id5 = value; }
        [Index(12)] public virtual List<float> Id6 { get => _Id6; set => _Id6 = value; }
        [Index(13)] public virtual List<float> Id7 { get => _Id7; set => _Id7 = value; }
        [Index(14)] public virtual List<float> Id8 { get => _Id8; set => _Id8 = value; }
        [Index(15)] public virtual List<float> Id9 { get => _Id9; set => _Id9 = value; }
        [Index(16)] public virtual Dictionary<double, string> Id10 { get => _Id10; set => _Id10 = value; }
        [Index(17)] public virtual Dictionary<double, string> Id11 { get => _Id11; set => _Id11 = value; }
        [Index(18)] public virtual Dictionary<double, string> Id12 { get => _Id12; set => _Id12 = value; }
        [Index(19)] public virtual Dictionary<double, string> Id13 { get => _Id13; set => _Id13 = value; }

        [Index(1)] public virtual Dictionary<double, string> Id14 { get; set; }
        [Index(2)] public virtual Dictionary<double, string> Id15 { get; set; }
        [Index(3)] public virtual Dictionary<double, string> Id16 { get; set; }
        [Index(4)] public virtual Dictionary<double, string> Id17 { get; set; }
        [Index(5)] public virtual Dictionary<double, string> Id18 { get; set; }
        [Index(6)] public virtual Dictionary<double, string> Id19 { get; set; }
        [Index(7)] public virtual Dictionary<double, string> Id20 { get; set; }


        public ZeroFormatterLargeBookShelf(string secret)
        {
            Secret = secret;
        }

        public ZeroFormatterLargeBookShelf() // Parameterless ctor is needed for every protocol buffer class during deserialization
        { }
    }

    [ZeroFormattable]
    public class ZeroFormatterLargeBook
    {
        private string _Title;
        private int id;
        private int _Id3;
        private int _Id4;
        private double _Id5;
        private List<float> _Id6;
        private List<float> _Id7;
        private List<float> _Id8;
        private List<float> _Id9;
        private Dictionary<double, string> _Id10;
        private Dictionary<double, string> _Id11;
        private Dictionary<double, string> _Id12;
        private Dictionary<double, string> _Id13;

        [Index(19)] public virtual string Title { get => _Title; set => _Title = value; }
        [Index(18)] public virtual int Id { get => id; set => id = value; }
        [Index(17)] public virtual int Id3 { get => _Id3; set => _Id3 = value; }
        [Index(16)] public virtual int Id4 { get => _Id4; set => _Id4 = value; }
        [Index(15)] public virtual double Id5 { get => _Id5; set => _Id5 = value; }
        [Index(14)] public virtual List<float> Id6 { get => _Id6; set => _Id6 = value; }
        [Index(13)] public virtual List<float> Id7 { get => _Id7; set => _Id7 = value; }
        [Index(12)] public virtual List<float> Id8 { get => _Id8; set => _Id8 = value; }
        [Index(11)] public virtual List<float> Id9 { get => _Id9; set => _Id9 = value; }
        [Index(10)] public virtual Dictionary<double, string> Id10 { get => _Id10; set => _Id10 = value; }
        [Index(9)] public virtual Dictionary<double, string> Id11 { get => _Id11; set => _Id11 = value; }
        [Index(8)] public virtual Dictionary<double, string> Id12 { get => _Id12; set => _Id12 = value; }
        [Index(7)] public virtual Dictionary<double, string> Id13 { get => _Id13; set => _Id13 = value; }
        [Index(0)] public virtual Dictionary<double, string> Id14 { get; set; }
        [Index(1)] public virtual Dictionary<double, string> Id15 { get; set; }
        [Index(2)] public virtual Dictionary<double, string> Id16 { get; set; }
        [Index(3)] public virtual Dictionary<double, string> Id17 { get; set; }
        [Index(4)] public virtual Dictionary<double, string> Id18 { get; set; }
        [Index(5)] public virtual Dictionary<double, string> Id19 { get; set; }
        [Index(6)] public virtual Dictionary<double, string> Id20 { get; set; }

        public ZeroFormatterLargeBook()
        {
        }

    }
}
