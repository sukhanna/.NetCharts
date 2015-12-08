using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace ChartLib
{
  public class DataRange<T>
  {
    private Range<T> m_xRange;
    public Range<T> XRange
    {
      get { return m_xRange; }
      set { m_xRange = value; }
    }

    private Range<T> m_yRange;
    public Range<T> YRange
    {
      get { return m_yRange; }
      set { m_yRange = value; }
    }

    public virtual RectangleF Bounds { get { throw new NotImplementedException(); } }
  }
}
