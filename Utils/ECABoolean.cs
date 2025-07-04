using Serilog;
using System;


namespace ECARules4All_DLL.Utils
{
    [System.Serializable]
    public class ECABoolean : IComparable<ECABoolean>
    {
        public static readonly ECABoolean YES = new ECABoolean(BoolType.YES);
        public static readonly ECABoolean ON = new ECABoolean(BoolType.ON);
        public static readonly ECABoolean TRUE = new ECABoolean(BoolType.TRUE);
        public static readonly ECABoolean NO = new ECABoolean(BoolType.NO);
        public static readonly ECABoolean OFF = new ECABoolean(BoolType.OFF);
        public static readonly ECABoolean FALSE = new ECABoolean(BoolType.FALSE);

        public enum BoolType
        {
            YES,
            ON,
            TRUE,
            NO,
            OFF,
            FALSE
        }

        public BoolType choice;

        public ECABoolean(BoolType type)
        {
            choice = type;
        }

        public ECABoolean(bool type)
        {
            choice = type ? BoolType.TRUE : BoolType.FALSE;
        }

        public BoolType GetBoolType()
        {
            return choice;
        }

        public void Assign(ECABoolean boolean)
        {
            choice = boolean.choice;
        }

        public void Assign(BoolType boolean)
        {
            choice = boolean;
        }

        public static bool operator ==(ECABoolean one, ECABoolean two)
        {
            return (one.choice <= BoolType.TRUE) == (two.choice <= BoolType.TRUE);
        }

        public static bool operator !=(ECABoolean one, ECABoolean two)
        {
            return (one.choice <= BoolType.TRUE) != (two.choice <= BoolType.TRUE);
        }

        public static bool operator ==(ECABoolean one, bool two)
        {
            return (one.choice <= BoolType.TRUE) == two;
        }

        public static bool operator !=(ECABoolean one, bool two)
        {
            return (one.choice <= BoolType.TRUE) != two;
        }

        public static bool operator ==(bool one, ECABoolean two)
        {
            return (two.choice <= BoolType.TRUE) == one;
        }

        public static bool operator !=(bool one, ECABoolean two)
        {
            return (two.choice <= BoolType.TRUE) != one;
        }

        //This line of code lets us check if the value is true or not just like a classic boolean 
        public static implicit operator bool(ECABoolean one) => one.choice <= BoolType.TRUE;

        public override string ToString()
        {
            return choice.ToString().ToLower();
        }

        public static ECABoolean FromString(string value)
        {
            Log.Information($"ECABOOLEAN FROM STRING - VALORE RICEVUTO: {value}");
            switch (value.ToLower())
            {
                case "yes": return ECABoolean.YES;
                case "on": return ECABoolean.ON;
                case "true": return ECABoolean.TRUE;
                case "no": return ECABoolean.NO;
                case "off": return ECABoolean.OFF;
                case "false": return ECABoolean.FALSE;
                default: throw new ArgumentException($"[{value}] is not a valid string for ECABoolean");
            }
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ECABoolean)) return false;
            return this.choice == ((ECABoolean)obj).choice;
        }

        public static ECABoolean Invert(ECABoolean b)
        {
            if (b == ECABoolean.YES) return ECABoolean.NO;
            if (b == ECABoolean.ON) return ECABoolean.OFF;
            if (b == ECABoolean.TRUE) return ECABoolean.FALSE;
            if (b == ECABoolean.NO) return ECABoolean.YES;
            if (b == ECABoolean.OFF) return ECABoolean.ON;
            if (b == ECABoolean.FALSE) return ECABoolean.TRUE;
            throw new ArgumentException("Invalid ECABoolean");
        }

        ///////////// TODO 4th July 25 - J NEW ///////////
        public int CompareTo(ECABoolean other)
        {
            if (other == null) return 1;
            return this.Equals(other) ? 1 : 0;
        }

        public int CompareTo(object obj)
        {
            if (obj == null) return 1;
            if (obj is ECABoolean other)
                return CompareTo(other);
            throw new ArgumentException($"Object must be of type {nameof(ECABoolean)}");
        }

        // Don't forget to override GetHashCode when you override Equals
        public override int GetHashCode()
        {
            return choice.GetHashCode();
        }
        ///////////////////////////////
    }

    [System.Serializable]
    public class YesNo : ECABoolean
    {
        public YesNo(BoolType type) : base(type)
        {
        }

        public YesNo(bool type) : base(type)
        {
        }
    }

    [System.Serializable]
    public class OnOff : ECABoolean
    {
        public OnOff(BoolType type) : base(type)
        {
        }

        public OnOff(bool type) : base(type)
        {
        }
    }

    [System.Serializable]
    public class TrueFalse : ECABoolean
    {
        public TrueFalse(BoolType type) : base(type)
        {
        }

        public TrueFalse(bool type) : base(type)
        {
        }
    }
}