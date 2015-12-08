using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ChartLib
{
  public class DataRangeFloat : DataRange<float>
  {
    public override System.Drawing.RectangleF Bounds
    {
      get { return new RectangleF(XRange.Min, YRange.Min, XRange.Max - XRange.Min, YRange.Max - YRange.Min); }
    }
  }
}
