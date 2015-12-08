using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChartLib
{
  public class Range<T>
  {
    public Range()
    {
    }

    public Range(T min, T max) : 
      this()
    {
      m_min = min;
      m_max = max;
    }

    private T m_min;
    public T Min
    {
      get { return m_min; }
      set { m_min = value; }
    }

    private T m_max;
    public T Max
    {
      get { return m_max; }
      set { m_max = value; }
    }

    private T m_delta;
    public T Delta
    {
      get { return m_delta; }
      set { m_delta = value; }
    }
  }
}
