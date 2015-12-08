using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Globalization;

namespace ChartLib
{
  public class YAxis<T> : Axis
    where T : AxisScale, new()
  {
    public YAxis()
    {
      AxisScale = new T();
      Initialize();
    }

    internal override void Draw(Graphics g, RectangleF physicalBounds, RectangleF logicalBounds)
    {
      base.Draw(g, physicalBounds, logicalBounds);

      // draw tick marks (subclass responsibility).
      DrawTicks(g, physicalBounds, logicalBounds);

      // draw the axis label
      DrawLabel(g, physicalBounds, logicalBounds);

      // draw grid lines
      SurfaceGrid.Draw(g);
    }

    /// <summary>
    /// Override to draw the tick marks on x axis.
    /// </summary>
    /// <param name="g"></param>
    /// <param name="physicalBounds"></param>
    /// <param name="logicalBounds"></param>
    private void DrawTicks(Graphics g, RectangleF physicalBounds, RectangleF logicalBounds)
    {
      SurfaceGrid.Clear();

      // draw horizontal axis
      using (Pen penAxis = new Pen(Color))
      {
        g.DrawRectangle(penAxis, new Rectangle((int)physicalBounds.Left, (int)physicalBounds.Top,
          (int)physicalBounds.Width, (int)physicalBounds.Height));
      }

      //if (AxisScale.IsLogScale)
      //{
      //  float clipLogPoint = logicalBounds.Top <= 1e-10f ? 1e-10f : logicalBounds.Top;
      //  logicalBounds = new RectangleF(logicalBounds.Left, clipLogPoint, logicalBounds.Width, logicalBounds.Height);
      //}

      // calculate major and minor ticks
      AxisScale.CalculateMajorTicks(physicalBounds.Height, logicalBounds.Top, logicalBounds.Bottom);
      AxisScale.CalculateMinorTicks(physicalBounds.Height, logicalBounds.Top, logicalBounds.Bottom);
      logicalBounds = AdjustChangedScaleRange(logicalBounds);
      DrawTickDivisions(g, physicalBounds, logicalBounds);
    }

    /// <summary>
    /// draw the tick marks on x axis.
    /// </summary>
    /// <param name="g"></param>
    /// <param name="physicalBounds"></param>
    /// <param name="logicalBounds"></param>
    private void DrawTickDivisions(Graphics g, RectangleF physicalBounds, RectangleF logicalBounds)
    {
      Font tickTextFont = TickTextFont;
      PointF oldStartFrom = new PointF(0, 0);
      float largestTextWidth = float.MinValue;
      float yOffset = physicalBounds.Bottom + physicalBounds.Top;

      // set default offset
      //SizeF size = g.MeasureString(AxisLabel.LabelText, AxisLabel.LabelFont);
      //LabelOffset = 2 * size.Height;

      // draw major ticks divisions
      for (int i = 0; i < AxisScale.MajorTicksCollection.Count; ++i)
      {
        // transform from logical (data) to physical (screen) coordinates
        float physicalValue = AxisScale.TransformSource(new RangeFloat(logicalBounds.Top, logicalBounds.Bottom),
          new RangeFloat(physicalBounds.Top, physicalBounds.Bottom), AxisScale.MajorTicksCollection.ElementAt(i));

        PointF startPoint = new PointF(physicalBounds.Left, physicalValue);

        // Add to draw major grid lines
        // leave 15.0f for Major ticks
        startPoint.Y = yOffset - startPoint.Y;
        PointF endPoint = new PointF(startPoint.X + physicalBounds.Width, startPoint.Y);
        SurfaceGrid.MajorGridCollection.Add(new GridPoints(new PointF(startPoint.X + 15.0f, startPoint.Y), endPoint));

        // Draw tick text
        if (!HideTickText)
        {
          string value = AxisScale.MajorTicksCollection.ElementAt(i).ToString(NumberFormat, CultureInfo.CurrentCulture);
          SizeF size = g.MeasureString(value, TickTextFont);
          PointF startFrom = new PointF(startPoint.X - size.Width - 5, startPoint.Y - size.Height / 2);
          if (largestTextWidth < size.Width)
          {
            largestTextWidth = size.Width;
            //LabelOffset = Math.Abs(startFrom.X - bounds.Left) + size.Height;
          }

          using (Brush tickTextBrush = new SolidBrush(TickTextColor))
            g.DrawString(value, tickTextFont, tickTextBrush, startFrom);
        }

        // Draw major tick marks
        endPoint.X = startPoint.X + 15.0f;
        DrawTickMark(g, Color, startPoint, endPoint);
      }

      for (int i = 0; i < AxisScale.MinorTicksCollection.Count; ++i)
      {
        // transform from logical (data) to physical (screen) coordinates
        float physicalValue = AxisScale.TransformSource(new RangeFloat(logicalBounds.Top, logicalBounds.Bottom),
          new RangeFloat(physicalBounds.Top, physicalBounds.Bottom), AxisScale.MinorTicksCollection.ElementAt(i));

        PointF startPoint = new PointF(physicalBounds.Left, physicalValue);

        // Add to draw minor grid lines
        // leave 5.0f for Major ticks
        startPoint.Y = yOffset - startPoint.Y;
        PointF endPoint = new PointF(startPoint.X + physicalBounds.Width, startPoint.Y);
        SurfaceGrid.MinorGridCollection.Add(new GridPoints(new PointF(startPoint.X + 5.0f, startPoint.Y), endPoint));

        // Draw minor ticks
        endPoint.X = startPoint.X + 5.0f;
        DrawTickMark(g, Color, startPoint, endPoint);
      }

      ExcludeGridBoundaries(physicalBounds);
    }

    /// <summary>
    /// Exclude grid boundaries to see axis
    /// </summary>
    /// <param name="bounds"></param>
    private void ExcludeGridBoundaries(RectangleF bounds)
    {
      GridPoints[] boundaryPoints = new GridPoints[] {
        new GridPoints(new PointF(bounds.Left + 15.0f, bounds.Bottom),
        new PointF(bounds.Right, bounds.Bottom)),
       new GridPoints(new PointF(bounds.Left + 15.0f, bounds.Top),
       new PointF(bounds.Right, bounds.Top))
      };

      foreach (GridPoints boundaryPoint in boundaryPoints)
      {
        if (SurfaceGrid.MajorGridCollection.Contains(boundaryPoint))
          SurfaceGrid.MajorGridCollection.Remove(boundaryPoint);
      }

      boundaryPoints = new GridPoints[] {
        new GridPoints(new PointF(bounds.Left + 5.0f, bounds.Bottom),
          new PointF(bounds.Right, bounds.Bottom)),
          new GridPoints(new PointF(bounds.Left + 5.0f, bounds.Top),
            new PointF(bounds.Right, bounds.Top))};

      foreach (GridPoints boundaryPoint in boundaryPoints)
      {
        if (SurfaceGrid.MinorGridCollection.Contains(boundaryPoint))
          SurfaceGrid.MinorGridCollection.Remove(boundaryPoint);
      }
    }

   /// <summary>
   /// 
   /// </summary>
   /// <param name="g"></param>
   /// <param name="physicalBounds"></param>
   /// <param name="logicalBounds"></param>
    private void DrawLabel(Graphics g, RectangleF physicalBounds, RectangleF logicalBounds)
    {
      if (g == null)
        throw new ArgumentNullException("g");

      if (Label == null)
        return;

      SizeF size = g.MeasureString(Label.LabelText, Label.LabelFont);
      // Only draw if the label would fit on the screen
      if (size.Width >= physicalBounds.Height) return;
      //AxisLabel.LabelOffset = new PointF(LabelOffset, 0);
      Label.Draw(g, physicalBounds);
    }

    /// <summary>
    /// adjust data range if scale has changed internally due to discrete axis or externally from GUI
    /// </summary>
    /// <param name="dataBounds"></param>
    /// <returns></returns>
    private RectangleF AdjustChangedScaleRange(RectangleF logicalBounds)
    {
      if (AxisScale.MajorTicksCollection.Count == 0)
        return logicalBounds;

      float top = Math.Min(AxisScale.MajorTicksCollection.ElementAt(0), logicalBounds.Top);
      float bottom = Math.Max(AxisScale.MajorTicksCollection.ElementAt(AxisScale.MajorTicksCollection.Count - 1), logicalBounds.Bottom);

      return new RectangleF(logicalBounds.Left, top, logicalBounds.Width, bottom - top);
    }
  }
}
