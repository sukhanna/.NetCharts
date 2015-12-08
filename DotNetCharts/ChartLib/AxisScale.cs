using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Drawing;

namespace ChartLib
{
  public abstract class AxisScale
  {
    #region properties

    public abstract void CalculateMajorTicks(float physicalLength, float min, float max);

    public abstract void CalculateMinorTicks(float physicalLength, float min, float max);

    protected Collection<float> m_majorTicksCollection = new Collection<float>();

    protected Collection<float> m_minorTicksCollection = new Collection<float>();

    /// <summary>
    /// Collection of Major ticks
    /// </summary>
    public IReadOnlyCollection<float> MajorTicksCollection
    {
      get { return m_majorTicksCollection; }
    }

    /// <summary>
    /// Collection of Minor ticks
    /// </summary>
    public IReadOnlyCollection<float> MinorTicksCollection
    {
      get { return m_minorTicksCollection; }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="logicalRange"></param>
    /// <param name="physicalRange"></param>
    /// <param name="source"></param>
    /// <returns></returns>
    public abstract float TransformSource(RangeFloat logicalRange, RangeFloat physicalRange, float source);
    #endregion
  }
}
