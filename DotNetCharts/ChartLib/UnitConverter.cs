
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChartLib
{
  public static class UnitConverter
  {
    /// <summary>
    /// convert a object, that already has a numeric type (or is null)
    /// to a string in engineering notation and unit
    /// </summary>
    /// <param name="valueType"></param>
    /// <param name="culture"></param>
    /// <param name="value"></param>
    /// <param name="format"></param>
    /// <returns></returns>
    public static string ConvertToString(
       GlobalTypes.EUnitTypes valueType,
       IFormatProvider culture,
       object value,
       string format)
    {
      string e = "";
      string sign = "";
      string v = "";
      string siUnit = GetSiUnit(valueType);
      if (value == null)
        v = "?";
      else
      {
        Debug.Assert(
           (value.GetType() == typeof(double))
           || (value.GetType() == typeof(float))
           || (value.GetType() == typeof(int))
           || (value.GetType() == typeof(uint))
           || (value.GetType() == typeof(Int64)));
        double d = Convert.ToDouble(value, culture);

        if (d < 0.0)
        {
          sign = "-";
          d = -d;
        }
        if (siUnit.Equals("%") || siUnit.Equals("dB") || string.IsNullOrEmpty(siUnit))
        {
          // % and dB does not participate in engineering notation
          // and if the value type does not have SI unit no engineering 
          // notation
        }
        else if (d >= 1e9)
        {
          d /= 1e9;
          e = "G";
        }
        else if (d >= 1e6)
        {
          d /= 1e6;
          e = "M";
        }
        else if (d >= 1e3)
        {
          d /= 1e3;
          e = "k";
        }
        else if ((d >= 1e0) || (d == 0))
        {
        }
        else if (d >= 1e-3)
        {
          d *= 1e3;
          e = "m";
        }
        else if (d >= 1e-6)
        {
          d *= 1e6;
          e = "u";
        }
        else if (d >= 1e-9)
        {
          d *= 1e9;
          e = "n";
        }
        else if (d >= 1e-12)
        {
          d *= 1e12;
          e = "p";
        }
        else
        {
          d *= 1e15;
          e = "f";
        }

        v = d.ToString(format, culture);
      }
      return sign + v + e + siUnit;
    }

    /// <summary>
    /// convert a string including unit to a double
    /// </summary>
    /// <param name="valueType"></param>
    /// <param name="culture"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static double ConvertFromString(
       GlobalTypes.EUnitTypes valueType,
       CultureInfo culture,
       string value)
    {
      if (value == null)
        throw new ArgumentNullException("value");

      string s = value;
      string siUnit = GetSiUnit(valueType);
      if (s.EndsWith(siUnit, true, culture))
        s = s.Substring(0, s.Length - siUnit.Length);
      double e = 1;
      if (s.EndsWith("G", false, culture))
        e = 1e9;
      else if (s.EndsWith("M", false, culture))
        e = 1e6;
      else if (s.EndsWith("k", false, culture))
        e = 1e3;
      else if (s.EndsWith("m", false, culture))
        e = 1e-3;
      else if (s.EndsWith("u", false, culture))
        e = 1e-6;
      else if (s.EndsWith("n", false, culture))
        e = 1e-9;
      else if (s.EndsWith("p", false, culture))
        e = 1e-12;
      else if (s.EndsWith("f", false, culture))
        e = 1e-15;
      if (e != 1)
        s = s.Substring(0, s.Length - 1);

      return e * double.Parse(s, culture);
    }

    private static string GetSiUnit(GlobalTypes.EUnitTypes valueType)
    {
      string type = "";
      switch (valueType)
      {
        case GlobalTypes.EUnitTypes.UnitTypeCapacitance:
          type = "F";
          break;
        case GlobalTypes.EUnitTypes.UnitTypeCurrent:
          type = "A";
          break;
        case GlobalTypes.EUnitTypes.UnitTypeDataRate:
          type = "Bit/s";
          break;
        case GlobalTypes.EUnitTypes.UnitTypeFloat:
        case GlobalTypes.EUnitTypes.UnitTypeInt64:
        case GlobalTypes.EUnitTypes.UnitTypeInt:
        case GlobalTypes.EUnitTypes.UnitTypeUint:
          type = "";
          break;
        case GlobalTypes.EUnitTypes.UnitTypeFrequency:
          type = "Hz";
          break;
        case GlobalTypes.EUnitTypes.UnitTypeTime:
          // Advantest uses "S", so Intel wants us to be consistent :-( :-(
          // need to find some way that our future instrument can
          // correctly use "s".
          //type = "s";
          type = "S";
          break;
        case GlobalTypes.EUnitTypes.UnitTypeVoltage:
          type = "V";
          break;
        case GlobalTypes.EUnitTypes.UnitTypeUI:
          type = "UI";
          break;
        case GlobalTypes.EUnitTypes.UnitTypeDbr:
          type = "dBr";
          break;
        case GlobalTypes.EUnitTypes.UnitTypePercent:
          type = "%";
          break;
        case GlobalTypes.EUnitTypes.UnitTypeDecibel:
          type = "dB";
          break;
        case GlobalTypes.EUnitTypes.UnitTypeUnknown:
          type = "";
          break;
        case GlobalTypes.EUnitTypes.UnitTypeBool:
        case GlobalTypes.EUnitTypes.UnitTypeError:
        case GlobalTypes.EUnitTypes.UnitTypeHex:
        case GlobalTypes.EUnitTypes.UnitTypeString:
        case GlobalTypes.EUnitTypes.UnitTypeStruct:
        default:
          throw new ArgumentOutOfRangeException("valueType");
      }
      return type;
    }
  }
}
