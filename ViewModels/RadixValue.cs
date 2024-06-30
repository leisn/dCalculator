using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;


namespace dCalculator.Bases
{
    [DebuggerDisplay("{Name},{Radix}")]
    public partial class RadixValue : ObservableObject
    {
        public byte Radix { get; }

        public long Value => _value;
        private long _value;
        [ObservableProperty]
        private string _display = "0";
        [ObservableProperty]
        private string _name;
        private byte _shiftBits = 0;

        private readonly Func<long, string, long> _addValueFunc;
        private readonly Func<long, long> _delValueFunc;
        private readonly Func<long, string> _formatter;

        public RadixValue(byte raidx, string name, Func<long, string> formatter)
        {
            Radix = raidx;
            _shiftBits = GetShiftBits(raidx);
            _name = name;
            _formatter = formatter ?? DefaultFormatter;
            _addValueFunc = DefaultAddValueFunc;
            _delValueFunc = DefaultDelValueFunc;
        }
        public RadixValue(byte raidx, string name, Func<long, string, long> addValueFunc, Func<long, long> delValueFunc, Func<long, string> formatter)
        {
            Radix = raidx;
            _shiftBits = GetShiftBits(raidx);
            _name = name;
            _formatter = formatter ?? DefaultFormatter;
            _addValueFunc = addValueFunc ?? DefaultAddValueFunc;
            _delValueFunc = delValueFunc ?? DefaultDelValueFunc;
        }

        private long DefaultAddValueFunc(long value, string number) => (value << _shiftBits) + Convert.ToInt32(number, 16);
        private long DefaultDelValueFunc(long value) => value >> _shiftBits;
        private string DefaultFormatter(long value) => Convert.ToString(value, Radix).ToUpperInvariant();


        public long AppendValue(string number)
        {
            return _addValueFunc(_value, number);
        }
        public long DelValue()
        {
            return _delValueFunc(_value);
        }

        public void SwitchSign()
        {
            _value = -_value;
            Display = Format(_value);
        }
        public void SetValue(long value)
        {
            _value = value;
            Display = Format(value);
        }

        public string Format(long value)
        {
            return _formatter(value);
        }


        public static string InsertString(string str, int everyCount, string ch)
        {
            if (everyCount <= 0)
                return str;

            var copy = str;
            int end = 1;
            if (str.StartsWith('-'))
                end = 2;
            for (int i = str.Length - everyCount; i >= end; i -= everyCount)
            {
                copy = copy.Insert(i, ch);
            }
            return copy;
        }
        public static byte GetShiftBits(byte radix) => radix switch
        {
            16 => 4,
            10 => 0,
            8 => 3,
            2 => 1,
            _ => throw new InvalidDataException(),
        };
    }





}
