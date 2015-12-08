using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Diagnostics;

namespace ChartLib
{
  public class LineChart : BaseChart
  {
    public override void Draw(Graphics g, RectangleF physicalBounds)
    {
      base.Draw(g, physicalBounds);

      // draw charts within axis
      RectangleF clipBounds = GetClippedPhysicalBounds(g, physicalBounds);
      clipBounds.Inflate(-1, -1);

      float[] data = OneDimFloatData.GetData();

      // minimum 2 points to draw the line
      Debug.Assert(data.GetLength(0) >= 2);

      float min = (float)OneDimFloatData.DataRange.XRange.Min;

      RectangleF logicalBounds = OneDimFloatData.DataRange.Bounds;

      // Invert the y-axis
      float yOffset = physicalBounds.Bottom + physicalBounds.Top;

      PointF from = ChartUtility.TransformSource(logicalBounds, clipBounds, new PointF(min, data[0]));
      PointF to = new PointF(0, 0);

      using (Pen pen = new Pen(Color))
      {
        // daw line graphics
        for (int i = 1; i < data.GetLength(0); ++i)
        {
          min += 1;
          to = ChartUtility.TransformSource(logicalBounds, clipBounds, new PointF(min, data[i]));
          g.DrawLine(pen, from.X, yOffset - from.Y, to.X, yOffset - to.Y);
          from = to;
        }
      }
    }
  }
}
