using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace ChartLib
{
  public class LinearScale : AxisScale
  {
    #region private variables

    /// <summary>
    /// If LargeTickStep is not specified, then a suitable value is
    /// calculated automatically. The value will be of the form
    /// Mantissa*10^e for some Mantissa in this set.
    /// </summary>
    private float[] m_mantissas = { 1.0f, 2.0f, 5.0f };

    /// <summary>
    /// If NumberOfSmallTicks is not specified.
    /// If specified LargeTickStep manually, then no small ticks unless
    /// NumberOfSmallTicks specified.
    /// </summary>
    private int[] m_smallTickCounts = { 4, 1, 4 };

    private int m_minPhysicalLargeTickStep = 30;

    #endregion

    #region AxisScale

    /// <summary>
    /// Determines the positions of Large(Major) ticks in world coordinates
    /// </summary>
    /// <param name="physicalLength">length of bounding rectangle.Width for Xaxis and Height for Y axis</param>
    /// <param name="min">minimum value in world</param>
    /// <param name="max">maximum value in world</param>
    public override void CalculateMajorTicks(float physicalLength, float min, float max)
    {
      m_majorTicksCollection.Clear();
      m_minorTicksCollection.Clear();

      //If suppose the Min == Max then we return it from here to get Rid of crash.
      if (min == max)
        return;

      //if spacing of large ticks too small in order to ensure that there are at least two of 
      // them. Then not more than two large ticks should be drawn
      bool bCullMiddle;

      // calculate distance between large ticks.
      float tickDist = CalculateLargeTickStep(physicalLength, min, max, out bCullMiddle);

      // calculate start position.
      float first = 0.0f;

      if (min > 0.0f)
      {
        float nToFirst = (float)Math.Floor(min / tickDist) + 1.0f;
        first = nToFirst * tickDist;
      }
      else
      {
        float nToFirst = (float)Math.Floor(-min / tickDist) - 1.0f;
        first = -nToFirst * tickDist;
      }

      // could miss one, if first is just below zero.
      if ((first - tickDist) >= min)
      {
        first -= tickDist;
      }

      // form collection of major(large) tick positions.
      float position = first;
      int safetyCount = 0;
      while ((position <= max) && (++safetyCount < 5000))
      {
        m_majorTicksCollection.Add(position);
        position += tickDist;
        if (ChartUtility.IsInfinitesimal(position))
          position = 0.0f;
      }

      // if the physical extent is too small, and the middle 
      // ticks should be turned into small ticks, then do this now.
      if (!bCullMiddle) return;
      if (m_majorTicksCollection.Count > 2)
      {
        for (int i = 1; i < m_majorTicksCollection.Count - 1; ++i)
        {
          m_minorTicksCollection.Add(m_majorTicksCollection[i]);
        }
      }
      Collection<float> culledPositions = new Collection<float>();
      culledPositions.Add(m_majorTicksCollection[0]);
      culledPositions.Add(m_majorTicksCollection[m_majorTicksCollection.Count - 1]);

      m_majorTicksCollection.Clear();
      foreach (float culledPos in culledPositions)
        m_majorTicksCollection.Add(culledPos);
    }

    /// <summary>
    /// Determines the positions of Small(Minor) ticks in world coordinates
    /// </summary>
    /// <param name="physicalLength">length of bounding rectangle.Width for Xaxis and Height for Y axis</param>
    /// <param name="min">minimum value in world</param>
    /// <param name="max">maximum value in world</param>
    public override void CalculateMinorTicks(float physicalLength, float min, float max)
    {
      // return if already generated.
      if (m_minorTicksCollection.Count > 0)
        return;

      bool bCullMiddle;
      float bigTickSpacing = CalculateLargeTickStep(physicalLength, min, max, out bCullMiddle);

      int nSmall = CalculateNumberOfSmallTicks(bigTickSpacing);
      float smallTickSpacing = bigTickSpacing / nSmall;

      // if there is at least one Major(large) tick
      if (m_majorTicksCollection.Count > 0)
      {
        float pos1 = m_majorTicksCollection[0] - smallTickSpacing;
        int safetyCount = 0;
        while (pos1 > min && (++safetyCount < 5000))
        {
          m_minorTicksCollection.Add(pos1);
          pos1 -= smallTickSpacing;
        }
      }

      for (int i = 0; i < m_majorTicksCollection.Count; ++i)
      {
        for (int j = 1; j < nSmall; ++j)
        {
          float pos = m_majorTicksCollection[i] + (j) * smallTickSpacing;
          if (pos <= max)
          {
            m_minorTicksCollection.Add(pos);
          }
        }
      }
    }

    public override float TransformSource(RangeFloat logicalRange, RangeFloat physicalRange, float source)
    {
      return ChartUtility.TransformSource(logicalRange, physicalRange, source);
    }

    #endregion

    #region private methods

    /// <summary>
    /// Determines the number of small ticks between two large ticks.
    /// </summary>
    /// <param name="bigTickDist">The distance between two large ticks.</param>
    /// <returns>The number of small ticks.</returns>
    private int CalculateNumberOfSmallTicks(float bigTickDist)
    {
      //if (NumberOfSmallTicks > 0)
      //{
      //  return (int)NumberOfSmallTicks + 1;
      //}

      if (bigTickDist > 0.0f)
      {
        double exponent = Math.Floor(Math.Log10(bigTickDist));
        double mantissa = Math.Pow(10.0, Math.Log10(bigTickDist) - exponent);
        for (int i = 0; i < m_mantissas.Length; ++i)
        {
          if (Math.Abs(mantissa - m_mantissas[i]) < 0.001)
          {
            return m_smallTickCounts[i] + 1;
          }
        }
      }
      return 0;
    }

    /// <summary>
    /// Calculates the world spacing between large ticks, based on the bounds(physical)
    /// length, world length, Mantissa values and m_minPhysicalLargeTickStep.
    /// </summary>
    /// <param name="physicalLength">length of bounding rectangle.Width for Xaxis and Height for Y axis</param>
    /// <param name="min">min data value or world min</param>
    /// <param name="max">max data value or world max</param>
    /// <param name="shouldCullMiddle">true if spacing of large ticks too small in order to ensure that there are at least two of 
    /// them. Then not more than two large ticks should be drawn</param>
    /// <returns>Large tick spacing</returns>
    private float CalculateLargeTickStep(float physicalLength, float min, float max, out bool shouldCullMiddle)
    {
      shouldCullMiddle = false;

      // Calculate large tick step.
      float range = max - min;
      // if axis has zero world length, then return arbitrary number.
      if (ChartUtility.FloatEqual(max, min))
      {
        return 1.0f;
      }

      float approxTickStep = (m_minPhysicalLargeTickStep / physicalLength) * range;


      double exponent = Math.Floor(Math.Log10(approxTickStep));
      double mantissa = Math.Pow(10.0, Math.Log10(approxTickStep) - exponent);

      // determine next whole mantissa below the approx one.
      int mantissaIndex = m_mantissas.Length - 1;
      for (int i = 1; i < m_mantissas.Length; ++i)
      {
        if (mantissa >= m_mantissas[i]) continue;
        mantissaIndex = i - 1;
        break;
      }

      // then choose next largest spacing. 
      mantissaIndex += 1;
      if (mantissaIndex == m_mantissas.Length)
      {
        mantissaIndex = 0;
        exponent += 1.0;
      }

      // now make sure that the returned value is such that at least two 
      // large tick marks will be displayed.
      double tickStep = Math.Pow(10.0, exponent) * m_mantissas[mantissaIndex];
      float physicalStep = (float)((tickStep / range) * physicalLength);

      while (physicalStep > physicalLength / 2)
      {
        shouldCullMiddle = true;
        mantissaIndex -= 1;
        if (mantissaIndex == -1)
        {
          mantissaIndex = m_mantissas.Length - 1;
          exponent -= 1.0;
        }
        tickStep = Math.Pow(10.0, exponent) * m_mantissas[mantissaIndex];
        physicalStep = (float)((tickStep / range) * physicalLength);
      }
      return (float)Math.Pow(10.0, exponent) * m_mantissas[mantissaIndex];
    }

    #endregion
  }
}
