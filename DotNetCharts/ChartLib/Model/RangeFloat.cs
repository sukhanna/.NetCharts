using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChartLib
{
  public class RangeFloat : Range<float>
  {
    public RangeFloat()
    { }

    public RangeFloat(float min, float max): this()
    {
      Max = Math.Max(max, min);
      Min = Math.Min(max, min);
      Delta = Max - Min;
    }
  }
}
