using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace ChartLib
{
  public class BaseChart : IDrawable
  {
    public virtual void Draw(Graphics g, RectangleF physicalBounds)
    {
      if (g == null)
        throw new ArgumentNullException("g");

      if (OneDimFloatData == null)
        throw new ArgumentNullException("OneDimFloatData");

      g.ResetClip();

      RectangleF clipBounds = GetClippedPhysicalBounds(g, physicalBounds);

      g.SmoothingMode = SmoothingMode.AntiAlias;

      RectangleF logicalBounds = OneDimFloatData.DataRange.Bounds;

      // draw Legends

      // draw Axis
      if(XAxis != null)
        ((Axis)XAxis).Draw(g, clipBounds, logicalBounds);

      if(YAxis != null)
        ((Axis)YAxis).Draw(g, clipBounds, logicalBounds);
    }

    public OneDimFloatData OneDimFloatData { get; set; }

    public Axis XAxis { get; set; }

    public Axis YAxis { get; set; }

    public Color Color { get; set; }

    /// <summary>
    /// Accomodate Axis labels and other drawable objects 
    /// </summary>
    /// <param name="physicalBounds"></param>
    /// <returns></returns>
    protected virtual RectangleF GetClippedPhysicalBounds(Graphics g, RectangleF physicalBounds)
    {
      float x = 70f;
      float y = 70f;

      //if (XAxis != null)
      //{
      //  if(XAxis.AxisLabel != null)
      //  {
      //    y = g.MeasureString(XAxis.AxisLabel.LabelText, XAxis.AxisLabel.LabelFont).Width;
      //  }
      //}

      //if(YAxis != null)
      //{
      //  if (YAxis.AxisLabel != null)
      //  {
      //    x = g.MeasureString(YAxis.AxisLabel.LabelText, YAxis.AxisLabel.LabelFont).Width;
      //  }
      //}

      physicalBounds.Inflate(-x, -y);
      return physicalBounds;
    }
  }
}
