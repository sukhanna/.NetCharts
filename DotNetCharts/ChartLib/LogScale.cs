using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Drawing;

namespace ChartLib
{
  public class LogScale : AxisScale
  {
    #region overridden methods

    /// <summary>
    /// Determines the positions of Large(Major) ticks in world cordinates
    /// </summary>
    /// <param name="physicalLength">length of bounding rectangle.Width for Xaxis and Height for Y axis</param>
    /// <param name="min">minimum value in world</param>
    /// <param name="max">maximum value in world</param>
    public override void CalculateMajorTicks(float physicalLength, float min, float max)
    {
      m_majorTicksCollection.Clear();
      double roundTickDist = CalculateTickSpacing(min, max);

      // now determine first tick position.
      double first = 0.0f;


      if (min > 0.0)
      {
        double nToFirst = Math.Floor(Math.Log10(min) / roundTickDist) + 1.0f;
        first = nToFirst * roundTickDist;
      }
      // could miss one, if first is just below zero.
      if (first - roundTickDist >= Math.Log10(min))
      {
        first -= roundTickDist;
      }

      double mark = first;
      while (mark <= Math.Log10(max))
      {
        // up to here only logs are dealt with, but I want to return
        // a real value in the array list
        float val = (float)Math.Pow(10.0, mark);
        m_majorTicksCollection.Add(val);
        mark += roundTickDist;
      }
    }

    /// <summary>
    /// Determines the positions of Small(Minor) ticks in world coordinates
    /// </summary>
    /// <param name="physicalLength">length of bounding rectangle.Width for Xaxis and Height for Y axis</param>
    /// <param name="min">minimum value in world</param>
    /// <param name="max">maximum value in world</param>
    public override void CalculateMinorTicks(float physicalLength, float min, float max)
    {
      m_minorTicksCollection.Clear();

      // retrieve the spacing of the big ticks. Remember this is decades!
      float bigTickSpacing = CalculateTickSpacing(min, max);
      int nSmall = CalculateNumberOfSmallTicks(bigTickSpacing);

      // now we have to set the ticks
      // let us start with the easy case where the major tick distance
      // is larger than a decade
      if (bigTickSpacing > 1.0f)
      {
        if (m_majorTicksCollection.Count > 0)
        {
          // deal with the small ticks preceding the
          // first big tick
          float pos1 = m_majorTicksCollection[0];
          int safetyCount = 0;
          while (pos1 > min && (++safetyCount < 5000))
          {
            pos1 = pos1 / 10.0f;
            m_minorTicksCollection.Add(pos1);
          }
          // now go on for all other Major ticks
          for (int i = 0; i < m_majorTicksCollection.Count; ++i)
          {
            float pos = m_majorTicksCollection[i];
            for (int j = 1; j <= nSmall; ++j)
            {
              pos = pos * 10.0F;
              // check to see if we are still in the range
              if (pos < max)
              {
                m_minorTicksCollection.Add(pos);
              }
            }
          }
        }
      }
      else
      {
        // guess what...
        float[] m = { 2.0f, 3.0f, 4.0f, 5.0f, 6.0f, 7.0f, 8.0f, 9.0f };
        // Then we deal with the other ticks
        if (m_majorTicksCollection.Count > 0)
        {
          // first deal with the small ticks preceding the first big tick
          // positioning before the first tick
          float pos1 = m_majorTicksCollection[0] / 10.0f;
          for (int i = 0; i < m.Length; i++)
          {
            float pos = pos1 * m[i];
            if (pos > min)
            {
              m_minorTicksCollection.Add(pos);
            }
          }
          // now go on for all other Major ticks
          for (int i = 0; i < m_majorTicksCollection.Count; ++i)
          {
            pos1 = m_majorTicksCollection[i];
            for (int j = 0; j < m.Length; ++j)
            {
              float pos = pos1 * m[j];
              // check to see if we are still in the range
              if (pos < max)
              {
                m_minorTicksCollection.Add(pos);
              }
            }
          }
        }
        else
        {
          // probably a minor tick would anyway fall in the range
          // find the decade preceding the minimum
          double dec = Math.Floor(Math.Log10(min));
          float pos1 = (float)Math.Pow(10.0, dec);
          for (int i = 0; i < m.Length; i++)
          {
            float pos = pos1 * m[i];
            if (pos > min && pos < max)
            {
              m_minorTicksCollection.Add(pos);
            }
          }
        }
      }
    }

    public override float TransformSource(RangeFloat logicalRange, RangeFloat physicalRange, float source)
    {
      float min = (float)logicalRange.Min;
      float max = (float)logicalRange.Max;

      logicalRange.Min = min <= 0.0 ? min : (float)Math.Log10(min);
      logicalRange.Max = max <= 0.0 ? max : (float)Math.Log10(max);

      // convert point
      return ChartUtility.TransformSource(logicalRange, physicalRange, (float)Math.Pow(10, source));
    }
    #endregion

    #region private method

    /// <summary>
    /// Determines the tick spacing.
    /// </summary>
    /// <param name="min">minimum value in world</param>
    /// <param name="max">maximum value in world</param>
    /// <returns>The tick spacing (in decades)</returns>
    private float CalculateTickSpacing(float min, float max)
    {
      double magRange = (Math.Floor(Math.Log10(max)) - Math.Floor(Math.Log10(min)) + 1.0);

      if (magRange > 0.0)
      {
        // start with a major tick every order of magnitude, and
        // increment if in order not to have more than 10 ticks in
        // the graph.
        float roundTickDist = 1.0f;
        int nticks = (int)(magRange / roundTickDist);
        while (nticks > 10)
        {
          roundTickDist++;
          nticks = (int)(magRange / roundTickDist);
        }
        return roundTickDist;
      }
      else
      {
        return 0.0f;
      }
    }

    /// <summary>
    /// Determines the number of small ticks between two large ticks.
    /// </summary>
    /// <param name="bigTickDist">The distance between two large ticks.</param>
    /// <returns>The number of small ticks.</returns>
    private int CalculateNumberOfSmallTicks(float bigTickDist)
    {
      // if we are plotting every decade, we have to
      // put the log ticks. As a start, we can put every
      // small tick (.2,.3,.4,.5,.6,.7,.8,.9)
      return bigTickDist == 1.0f ? 8 : (int)bigTickDist - 1;
    }
    #endregion
  }
}
