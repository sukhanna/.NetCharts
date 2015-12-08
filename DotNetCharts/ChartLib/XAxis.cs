using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Globalization;

namespace ChartLib
{
  public class XAxis<T> : Axis
    where T : AxisScale, new()
  {
    public XAxis()
    {
      AxisScale = new T();
      Initialize();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="g"></param>
    /// <param name="physicalBounds"></param>
    /// <param name="logicalBounds"></param>
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
    /// 
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

      // calculate major and minor ticks
      AxisScale.CalculateMajorTicks(physicalBounds.Width, logicalBounds.Left, logicalBounds.Right);
      AxisScale.CalculateMinorTicks(physicalBounds.Width, logicalBounds.Left, logicalBounds.Right);
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

      // set default offset
      //SizeF size = g.MeasureString(AxisLabel.LabelText, AxisLabel.LabelFont);
      //LabelOffset = size.Height;

      // draw major ticks divisions
      for (int i = 0; i < AxisScale.MajorTicksCollection.Count; ++i)
      {
        // transform from logical (data) to physical (screen) coordinates
        float physicalValue = AxisScale.TransformSource(new RangeFloat(logicalBounds.Left, logicalBounds.Right),
          new RangeFloat(physicalBounds.Left, physicalBounds.Right), AxisScale.MajorTicksCollection.ElementAt(i));

        PointF startPoint = new PointF(physicalValue, physicalBounds.Bottom);

        // Add to draw major grid lines
        // leave 15.0f for Major ticks
        PointF endPoint = new PointF(startPoint.X, physicalBounds.Top);
        SurfaceGrid.MajorGridCollection.Add(new GridPoints(new PointF(startPoint.X, startPoint.Y - 15.0f), endPoint));

        // Draw tick text
        if (!HideTickText)
        {
          string value = AxisScale.MajorTicksCollection.ElementAt(i).ToString(NumberFormat, CultureInfo.CurrentCulture);
          SizeF size = g.MeasureString(value, TickTextFont);
          PointF startFrom = new PointF(startPoint.X - size.Width / 2, startPoint.Y + size.Height / 2);
          if (startFrom.X + size.Width + 2 >= oldStartFrom.X && startFrom.Y == oldStartFrom.Y)
          {
            startFrom.Y += size.Height;
            //LabelOffset = Math.Abs(physicalBounds.Bottom - startFrom.Y) + size.Height + 2.0f;
          }

          using (Brush tickTextBrush = new SolidBrush(TickTextColor))
            g.DrawString(value, tickTextFont, tickTextBrush, startFrom);

          oldStartFrom.X = startFrom.X + size.Width + 5.0f;
          oldStartFrom.Y = startFrom.Y;
        }

        // Draw major tick marks
        endPoint.Y = physicalBounds.Bottom - 15.0f;
        DrawTickMark(g, Color, startPoint, endPoint);
      }

      for (int i = 0; i < AxisScale.MinorTicksCollection.Count; ++i)
      {
        // transform from logical (data) to physical (screen) coordinates
        float physicalValue = AxisScale.TransformSource(new RangeFloat(logicalBounds.Left, logicalBounds.Right),
          new RangeFloat(physicalBounds.Left, physicalBounds.Right), AxisScale.MinorTicksCollection.ElementAt(i));

        PointF startPoint = new PointF(physicalValue, physicalBounds.Bottom);

        // Add to draw minor grid lines
        // leave 5.0f for Minor ticks
        PointF endPoint = new PointF(startPoint.X, physicalBounds.Top);
        SurfaceGrid.MinorGridCollection.Add(new GridPoints(new PointF(startPoint.X, startPoint.Y - 5.0f), endPoint));

        // Draw minor ticks
        endPoint.Y = physicalBounds.Bottom - 5.0f;
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
      GridPoints[] boundaryPoints = new GridPoints[]
                                          {
                                             new GridPoints(new PointF(bounds.Left, bounds.Bottom - 15.0f),
                                                            new PointF(bounds.Left, bounds.Top)),
                                             new GridPoints(new PointF(bounds.Right, bounds.Bottom - 15.0f),
                                                            new PointF(bounds.Right, bounds.Top))
                                          };
      foreach (GridPoints boundaryPoint in boundaryPoints)
      {
        if (SurfaceGrid.MajorGridCollection.Contains(boundaryPoint))
          SurfaceGrid.MajorGridCollection.Remove(boundaryPoint);
      }

      boundaryPoints = new GridPoints[]
                             {
                                new GridPoints(new PointF(bounds.Left, bounds.Bottom - 5.0f),
                                               new PointF(bounds.Left, bounds.Top)),
                                new GridPoints(new PointF(bounds.Right, bounds.Bottom - 5.0f),
                                               new PointF(bounds.Right, bounds.Top))
                             };
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
    /// <param name="color"></param>
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
      if (size.Width >= physicalBounds.Width) return;
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

      float left = Math.Min(AxisScale.MajorTicksCollection.ElementAt(0), logicalBounds.Left);
      float right = Math.Max(AxisScale.MajorTicksCollection.ElementAt(AxisScale.MajorTicksCollection.Count - 1), logicalBounds.Right);

      return new RectangleF(left, logicalBounds.Top, right - left, logicalBounds.Height);
    }
  }
}
