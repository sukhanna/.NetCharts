using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Drawing.Drawing2D;
using System.Diagnostics;

namespace ChartLib
{
  /// <summary>
  /// Graphics utility static class. Contains the static methods to scale font,
  /// logarithmic color coding, interpolation of data (bi-linear algorithm),calculate
  /// the data bounds used in zoom selection and axis settings, transformation of the
  /// points from source to target and transformation of the points from target to source.
  /// </summary>
  public static class ChartUtility
  {
    /// <summary>
    /// Get a Font exactly the same as the passed in one, except for scale factor.
    /// </summary>
    /// <param name="initialFont">The font to scale.</param>
    /// <param name="scale">Scale by this factor.</param>
    /// <returns>The scaled font.</returns>
    public static Font ScaleFont(Font initialFont, float scale)
    {
      if (initialFont == null)
        return null;

      FontStyle fontStyle = initialFont.Style;
      GraphicsUnit graphicsUnit = initialFont.Unit;
      float fontSize = initialFont.Size;
      fontSize = fontSize * scale;
      if (fontSize < 6.5f)
        return new Font(initialFont.Name, 6.5f, fontStyle, graphicsUnit);
      else
        return new Font(initialFont.Name, fontSize, fontStyle, graphicsUnit);
    }

    public static Font AppropriateFont(Graphics g, float minFontSize, float maxFontSize,
       SizeF layoutSize, string text, Font font)
    {
      if (g == null)
        throw new ArgumentNullException("g");
      if (font == null)
        throw new ArgumentNullException("font");

      if (maxFontSize <= minFontSize)
        return font;

      SizeF extent = g.MeasureString(text, font);
      float hRatio = layoutSize.Height / extent.Height;
      float wRatio = layoutSize.Width / extent.Width;
      float ratio = Math.Min(hRatio, wRatio);
      float newSize = font.Size * ratio;

      if (newSize < minFontSize)
        newSize = minFontSize;
      else if (newSize > maxFontSize)
        newSize = maxFontSize;

      return new Font(font.FontFamily, newSize, font.Style);
    }

    // This would be required in future for setup tool font handling
    //public static Font GetAdjustedFont(Graphics GraphicRef, string GraphicString, Font OriginalFont, int ContainerWidth, int MaxFontSize, int MinFontSize, bool SmallestOnFail)
    //{
    //   // We utilize MeasureString which we get via a control instance           
    //   for (int AdjustedSize = MaxFontSize; AdjustedSize >= MinFontSize; AdjustedSize--)
    //   {
    //      Font TestFont = new Font(OriginalFont.Name, AdjustedSize, OriginalFont.Style);

    //      // Test the string with the new size
    //      SizeF AdjustedSizeNew = GraphicRef.MeasureString(GraphicString, TestFont);

    //      if (ContainerWidth > Convert.ToInt32(AdjustedSizeNew.Width))
    //      {
    //         // Good font, return it
    //         return TestFont;
    //      }
    //   }

    //   // If you get here there was no font size that worked
    //   // return MinimumSize or Original?
    //   if (SmallestOnFail)
    //   {
    //      return new Font(OriginalFont.Name, MinFontSize, OriginalFont.Style);
    //   }
    //   else
    //   {
    //      return OriginalFont;
    //   }
    //}

    /// <summary>
    /// shorten the controls text to fit within control size/ not to extend beyond the right edge of control
    /// AutoSize property of labels should be false while calling this method
    /// </summary>
    /// <param name="control"></param>
    /// <param name="availableWidth"></param>
    /// <param name="fullText"></param>
    public static string ShortenText(Graphics g, Font font, int availableWidth, string fullText)
    {
      if (g == null)
        throw new ArgumentNullException("g");
      if (font == null)
        throw new ArgumentNullException("font");
      if (fullText == null)
        throw new ArgumentNullException("fullText");

      const string ellipses = "...";
      SizeF ellipsesSize = g.MeasureString(ellipses, font);
      int ellipsesWidth = (int)ellipsesSize.Width;
      if (ellipsesWidth > availableWidth)
        return "";

      string starter = "";
      string ender = "";
      int completeLength = fullText.Length;
      int middle = (completeLength / 2) + (completeLength % 2);
      string prevTitle = fullText;
      string newStarter = null;
      for (int i = 0; i < middle; i++)
      {
        StringBuilder stringBuilder = new StringBuilder();
        newStarter = starter + fullText[i];
        string newEnder = ender;
        if ((completeLength - i) > middle)
        {
          stringBuilder.Append(fullText[completeLength - i - 1]);
          stringBuilder.Append(newEnder);
          newEnder = stringBuilder.ToString();
        }
        string newTitle = newStarter + ellipses + newEnder;
        SizeF newTitleSize = g.MeasureString(newTitle, font);
        if ((int)newTitleSize.Width > availableWidth)
          break;
        starter = newStarter;
        ender = newEnder;
        prevTitle = newTitle;
        continue;
      }
      return prevTitle;
    }


    /// <summary>
    /// Transform the source point into destination point.
    /// </summary>
    /// <param name="from">Source Rectangle</param>
    /// <param name="to">Destination Rectangle</param>
    /// <param name="ptSrc">Source Point</param>
    /// <returns>Transformed Destination Point</returns>
    public static PointF TransformSource(RectangleF from, RectangleF to, PointF ptSrc)
    {
      PointF ptDest = new PointF();
      ptDest.X = (((ptSrc.X - from.Left) / from.Width) * to.Width) + to.Left;
      ptDest.Y = (((ptSrc.Y - from.Top) / from.Height) * to.Height) + to.Top;
      return ptDest;
    }

    /// <summary>
    /// Currently used only for maintaining relative positions of markers w.r.t to data
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <param name="ptSrc"></param>
    /// <returns></returns>
    public static Point TransformSource(RectangleF from, RectangleF to, Point ptSrc)
    {
      Point ptDest = new Point();
      ptDest.X =
         (int)(Math.Round((((ptSrc.X - from.Left) / from.Width) * to.Width) + to.Left, MidpointRounding.AwayFromZero));
      ptDest.Y =
         (int)(Math.Round((((ptSrc.Y - from.Top) / from.Height) * to.Height) + to.Top, MidpointRounding.AwayFromZero));
      return ptDest;
    }

    /// <summary>
    /// Extract the X and Y point from the source and destination rectangle
    /// </summary>
    /// <param name="from">Source Rectangle</param>
    /// <param name="to">Destination Rectangle</param>
    /// <param name="srcX">source X point</param>
    /// <param name="srcY">source Y point</param>
    public static void TransformSource(RectangleF from, RectangleF to, ref float srcX, ref float srcY)
    {
      srcX = (((srcX - from.Left) / from.Width) * to.Width) + to.Left;
      srcY = (((srcY - from.Top) / from.Height) * to.Height) + to.Top;
    }

    /// <summary>
    /// Calculate the width and height of the destination.
    /// </summary>
    /// <param name="from">Source Rectangle</param>
    /// <param name="to">Destination Rectangle</param>
    /// <param name="szSrc">ordered pair of width and height of the source</param>
    /// <returns>ordered pair of height and width of destination</returns>
    public static SizeF TransformSource(RectangleF from, RectangleF to, SizeF szSrc)
    {
      SizeF szDest = new SizeF();
      szDest.Width = (szSrc.Width / from.Width) * to.Width;
      szDest.Height = (szSrc.Height / from.Height) * to.Height;
      return szDest;
    }

    /// <summary>
    /// Transform the source rectangle into destination rectangle.
    /// </summary>
    /// <param name="from">Source Rectangle</param>
    /// <param name="to">Destination Rectangle</param>
    /// <param name="ptSrc">Source rectangle</param>
    /// <returns>Transformed Destination rectangle</returns>
    public static RectangleF TransformSource(RectangleF from, RectangleF to, RectangleF rectSrc)
    {
      float left = (((rectSrc.Left - from.Left) / from.Width) * to.Width) + to.Left;
      float top = (((rectSrc.Top - from.Top) / from.Height) * to.Height) + to.Top;
      float right = (((rectSrc.Right - from.Left) / from.Width) * to.Width) + to.Left;
      float bottom = (((rectSrc.Bottom - from.Top) / from.Height) * to.Height) + to.Top;
      return new RectangleF(left, top, right - left, bottom - top);
    }

    public static float TransformSource(RangeFloat from, RangeFloat to, float source)
    {
      return (float)((((source - from.Min) / from.Delta) * to.Delta) + to.Min);
    }

    /// <summary>
    /// Convert Linear data rect (world points) to log
    /// </summary>
    /// <param name="dataRect">world bounds</param>
    /// <param name="bXLog">true if x axis is log</param>
    /// <param name="bYLog">true if y axis is log</param>
    /// <returns></returns>
    public static RectangleF ConvertDataRectToScale(RectangleF dataRect, bool bXLog, bool bYLog)
    {
      float left = dataRect.Left;
      float top = dataRect.Top;
      float right = dataRect.Right;
      float bottom = dataRect.Bottom;
      if (bYLog)
      {
        top = top <= 0.0 ? top : (float)Math.Log10(top);
        bottom = bottom <= 0.0 ? bottom : (float)Math.Log10(bottom);
      }
      if (bXLog)
      {
        left = left <= 0.0 ? left : (float)Math.Log10(left);
        right = right <= 0.0 ? right : (float)Math.Log10(right);
      }
      return new RectangleF(left, top, right - left, bottom - top);
    }

    /// <summary>
    /// Map from world value(data value) to physical point on screen
    /// </summary>
    /// <param name="dataRect">world bounds</param>
    /// <param name="boundingRect">physical bounds</param>
    /// <param name="point">point to transform</param>
    /// <param name="bXLog">true if x axis is log</param>
    /// <param name="bYLog">true if y axis is log</param>
    /// <returns></returns>
    public static PointF WorldToPhysical(RectangleF dataRect, RectangleF boundingRect, PointF point,
                                         bool bXLog, bool bYLog)
    {
      RectangleF convertedDataRect = ConvertDataRectToScale(dataRect, bXLog, bYLog);
      if (bXLog)
      {
        point.X = (point.X <= 0.0) ? convertedDataRect.Left : (float)Math.Log10(point.X);
      }
      if (bYLog)
      {
        point.Y = (point.Y <= 0.0) ? convertedDataRect.Top : (float)Math.Log10(point.Y);
      }
      return TransformSource(convertedDataRect, boundingRect, point);
    }

    /// <summary>
    /// Map from physical point on screen to world value(data value)
    /// </summary>
    /// <param name="dataRect">world bounds</param>
    /// <param name="boundingRect">physical bounds</param>
    /// <param name="point">point to transform</param>
    /// <param name="bXLog">true if x axis is log</param>
    /// <param name="bYLog">true if y axis is log</param>
    /// <returns></returns>
    public static PointF PhysicalToWorld(RectangleF boundingRect, RectangleF dataRect, PointF point,
                                         bool bXLog, bool bYLog)
    {
      RectangleF convertedDataRect = ConvertDataRectToScale(dataRect, bXLog, bYLog);
      point = TransformSource(boundingRect, convertedDataRect, point);
      if (bXLog)
      {
        point.X = (float)Math.Pow(10, point.X);
      }
      if (bYLog)
      {
        point.Y = (float)Math.Pow(10, point.Y);
      }
      return point;
    }

    /// <summary>
    /// convert from db to linear power
    /// </summary>
    /// <param name="dbValue"></param>
    /// <returns></returns>
    public static float DecibelToLinear(float dbValue)
    {
      return (float)Math.Pow(10, (dbValue * 0.1));
    }

    /// <summary>
    /// convert from linear to db power
    /// </summary>
    /// <param name="linearValue"></param>
    /// <param name="mindBValue"></param>
    /// <returns></returns>
    public static float LinearToDecibel(float linearValue, float mindBValue)
    {
      if (linearValue <= 0.0)
      {
        return mindBValue;
      }

      return (float)(10 * Math.Log10(linearValue));
    }

    /// <summary>
    /// Returns true if the absolute difference between numbers is less than Epsilon
    /// </summary>
    /// <param name="firstNumber">first number to compare</param>
    /// <param name="secondNumber">second number to compare</param>
    /// <returns>true if equal else false</returns>
    public static bool FloatEqual(float firstNumber, float secondNumber)
    {
      const float epsilon = float.Epsilon * 1000.0f;
      return Math.Abs(firstNumber - secondNumber) < epsilon;
    }

    /// <summary>
    /// Returns true if the absolute difference between numbers is less than Epsilon
    /// </summary>
    /// <param name="firstNumber">first number to compare</param>
    /// <param name="secondNumber">second number to compare</param>
    /// <returns>true if equal else false</returns>
    public static bool PointsEqual(PointF point1, PointF point2)
    {
      bool equals = false;
      const float epsilon = 1e-3f;
      if (System.Math.Abs(point1.X - point2.X) < epsilon &&
         System.Math.Abs(point1.Y - point2.Y) < epsilon)
      {
        equals = true;
      }

      return equals;
    }

    /// <summary>
    /// creates the best fit image
    /// </summary>
    /// <param name="image"></param>
    /// <param name="boundingRect"></param>
    /// <returns></returns>
    public static Bitmap BestFitImage(Image image, RectangleF boundingRect)
    {
      if (image == null)
        return null;

      float largestRatio = Math.Max(image.Width / boundingRect.Width, image.Height / boundingRect.Height);
      float posX = (boundingRect.Width * largestRatio / 2 - image.Width / 2);
      float posY = (boundingRect.Height * largestRatio / 2 - image.Height / 2);
      Bitmap offscreenBitmap = new Bitmap((int)boundingRect.Width, (int)boundingRect.Height);
      Matrix matrix = new Matrix(1.0f / largestRatio, 0, 0, 1.0f / largestRatio, 0, 0);
      matrix.Translate(posX, posY);
      using (Graphics offscreenGraphics = Graphics.FromImage(offscreenBitmap))
      {
        offscreenGraphics.Transform = matrix;
        offscreenGraphics.DrawImageUnscaled(image, 0, 0);
      }
      return offscreenBitmap;
    }

    /// <summary>
    /// Fit image in bounds
    /// </summary>
    /// <param name="image"></param>
    /// <param name="boundingRect"></param>
    /// <returns></returns>
    public static Bitmap FitToBounds(Image image, RectangleF boundingRect)
    {
      if (image == null)
        return null;

      float xRatio = image.Width / boundingRect.Width;
      float yRatio = image.Height / boundingRect.Height;
      float posX = (boundingRect.Width * xRatio / 2 - image.Width / 2);
      float posY = (boundingRect.Height * yRatio / 2 - image.Height / 2);
      Bitmap offscreenBitmap = new Bitmap((int)boundingRect.Width, (int)boundingRect.Height);
      Matrix matrix = new Matrix(1.0f / xRatio, 0, 0, 1.0f / yRatio, 0, 0);
      matrix.Translate(posX, posY);
      using (Graphics offscreenGraphics = Graphics.FromImage(offscreenBitmap))
      {
        offscreenGraphics.Transform = matrix;
        offscreenGraphics.DrawImageUnscaled(image, 0, 0);
      }
      return offscreenBitmap;
    }

    /// <summary>
    /// linear Interpolate X
    /// </summary>
    /// <param name="x1"></param>
    /// <param name="y1"></param>
    /// <param name="x2"></param>
    /// <param name="y2"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public static float InterpolateX(float x1, float y1, float x2, float y2, float y)
    {
      float aTmp = (x2 - x1) / (y2 - y1) * (y - y1);
      return x1 + aTmp;
    }

    /// <summary>
    /// linear Interpolate Y
    /// </summary>
    /// <param name="x1"></param>
    /// <param name="y1"></param>
    /// <param name="x2"></param>
    /// <param name="y2"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public static float InterpolateY(float x1, float y1, float x2, float y2, float x)
    {
      float aTmp = (y2 - y1) / (x2 - x1) * (x - x1);
      return y1 + aTmp;
    }

    /// <summary>
    /// check for negative bounds and clip to lie in range
    /// as we cant access pixels outside the range
    /// </summary>
    /// <param name="dataRect"></param>
    private static void ClipDataRectInRange(ref RectangleF dataRect, Rectangle sourceRect)
    {
      // dataRectleft and top can be less than sourceRect left and top so always take maximum
      float left = Math.Max(sourceRect.Left, Math.Max(0, dataRect.X));
      float top = Math.Max(sourceRect.Top, Math.Max(0, dataRect.Y));
      // dataRect right and bottom can be larger than sourceRect limits so always take minimum
      float right = Math.Min(sourceRect.Right + 1, Math.Max(0, dataRect.Right));
      float bottom = Math.Min(sourceRect.Bottom + 1, Math.Max(0, dataRect.Bottom));
      dataRect = new RectangleF(left, top, right - left, bottom - top);
    }

    /// <summary>
    /// Calculate the data bounds from the passed in data points
    /// </summary>
    /// <param name="dataPoints">passed in data points</param>
    /// <returns>Rectangle containing Data Bound for passed in data points</returns>
    public static RectangleF CalcDataBounds(PointF[] dataPoints)
    {
      if (dataPoints.Length == 0)
        return new RectangleF(0, 0, 0, 0);

      float minX, minY, maxX, maxY;

      minX = maxX = dataPoints[0].X;
      minY = maxY = dataPoints[0].Y;

      for (int i = 0; i < dataPoints.Length; i++)
      {
        if (dataPoints[i].X < minX)
          minX = dataPoints[i].X;
        else if (dataPoints[i].X > maxX)
          maxX = dataPoints[i].X;

        if (dataPoints[i].Y < minY)
          minY = dataPoints[i].Y;
        else if (dataPoints[i].Y > maxY)
          maxY = dataPoints[i].Y;
      }

      // at least 1 pixel height is required
      float height = maxY - minY;
      if (height == 0.0)
        height = 1f;

      // Data Rect containing bounds for dataPoints
      return new RectangleF(minX, minY, maxX - minX, height);
    }

    /// <summary>
    /// generate random numbers for these many samples using Box-Muller transformation
    /// </summary>
    /// <param name="samples">array which contains random samples result</param>
    /// <param name="rootMeanSquare"></param>
    /// <returns></returns>
    public static void GenerateRandomNumers(double[] samples, double rootMeanSquare)
    {
      // used to generate random numbers between 0 and 1 for NextDouble()
      Random generator = new Random();
      bool bFirstValue = false;
      double secondValue = 0;
      int numberOfSamples = samples.Length;

      // polar form of the Box-Muller transformation allows us to transform uniformly distributed random variables,
      // to a new set of random variables with a Gaussian (or Normal) distribution.
      for (int index = 0; index < numberOfSamples; index++)
      {
        if (bFirstValue)
        {
          bFirstValue = false;
          samples[index] = secondValue * rootMeanSquare;
        }
        else
        {
          bFirstValue = true;
          while (true)
          {
            double firstRandom = 2.0 * generator.NextDouble() - 1.0;
            double secondRandom = 2.0 * generator.NextDouble() - 1.0;
            double randomSum = firstRandom * firstRandom + secondRandom * secondRandom;

            if (randomSum > 1) continue;
            double y = Math.Sqrt(-2.0 * Math.Log(randomSum) / randomSum);
            samples[index] = firstRandom * y * rootMeanSquare;
            secondValue = secondRandom * y;
            break;
          }
        }
      }
    }

    /// <summary>
    /// Include boundary points also to check if mouse points is in drawing area
    /// </summary>
    /// <param name="bounds">point contained in this region</param>
    /// <param name="point">point</param>
    /// <returns>true if Bounds contains the this point</returns>
    public static bool Contains(RectangleF bounds, PointF point)
    {
      // outside drawing area, don't draw markers
      return ((point.X >= bounds.Left && point.X <= bounds.Right) && point.Y >= bounds.Top)
         && point.Y <= bounds.Bottom;
    }

    public static RectangleF GetRectFromPoint(PointF from, PointF to)
    {
      float minX, maxX, minY, maxY;
      minX = Math.Min(from.X, to.X);
      minY = Math.Min(to.Y, from.Y);
      maxX = Math.Max(from.X, to.X);
      maxY = Math.Max(to.Y, from.Y);
      return new RectangleF(minX, minY, maxX - minX, maxY - minY);
    }

    public static bool IsInfinitesimal(double value)
    {
      return (Math.Abs(value - 0.0) < 1.192092896e-07F);
    }

    public static Rectangle RoundOffRectangle(RectangleF rect)
    {
      int left = RoundToInt(rect.Left);
      int top = RoundToInt(rect.Top);
      int right = RoundToInt(rect.Right);
      int bottom = RoundToInt(rect.Bottom);
      return new Rectangle(left, top, right - left, bottom - top);
    }

    public static Rectangle RoundOffRectangle(float left, float top, float right, float bottom)
    {
      int l = RoundToInt(left);
      int t = RoundToInt(top);
      int r = RoundToInt(right);
      int b = RoundToInt(bottom);
      return new Rectangle(l, t, r - l, b - t);
    }

    public static Rectangle RoundOffRectangle(PointF topLeft, PointF bottomRight)
    {
      int left = RoundToInt(topLeft.X);
      int top = RoundToInt(topLeft.Y);
      int right = RoundToInt(bottomRight.X);
      int bottom = RoundToInt(bottomRight.Y);
      return new Rectangle(left, top, right - left, bottom - top);
    }

    public static int RoundToInt(double val)
    {
      // Note: floor intentionally removed, this is what the (int) cast does anyway
      // TBD: find out how to get rid of ceil
      return (val < 0.0 ? (int)Math.Ceiling(val - 0.5) : (int)(val + 0.5));
    }

    /// <summary>
    /// generate sin wave
    /// </summary>
    /// <param name="sourcePixels">gives back sin wave data in source pixel array</param>
    /// <param name="amplitude">amplitude</param>
    public static void GenerateSinWave(float[] sourcePixels, float amplitude)
    {
      if (sourcePixels == null)
        throw new ArgumentNullException("sourcePixels");

      for (int index = 1; index < sourcePixels.Length; index++)
      {
        // y = A * Sin(2*PI*t/T)
        sourcePixels[index] = amplitude * (float)Math.Sin(2 * Math.PI * index / sourcePixels.Length);
      }
    }

    public static void GenerateSinWave(float[] sourcePixels, float amplitude, float timePeriod)
    {
      if (sourcePixels == null)
        throw new ArgumentNullException("sourcePixels");

      System.Diagnostics.Debug.Assert(sourcePixels.Length % timePeriod == 0);

      for (int index = 0; index < sourcePixels.Length; index++)
      {
        // y = A * Sin(2*PI*t/T)
        sourcePixels[index] = amplitude * (float)Math.Sin(2 * Math.PI * (index % timePeriod) / timePeriod);
      }
    }

    /// <summary>
    /// generate square wave
    /// </summary>
    /// <param name="sourcePixels">gives back square wave data in source pixel array</param>
    /// <param name="amplitude">amplitude</param>
    public static void GenerateSquareWave(float[] sourcePixels, float amplitude)
    {
      if (sourcePixels == null)
        throw new ArgumentNullException("sourcePixels");

      for (int index = 0; index < sourcePixels.Length; index++)
      {
        int mod = index % (sourcePixels.Length - 1);
        if (mod <= sourcePixels.Length / 4 || mod >= sourcePixels.Length * 3 / 4)
        {
          sourcePixels[index] = amplitude;
        }
        else if (mod <= sourcePixels.Length * 3 / 4)
        {
          sourcePixels[index] = -amplitude;
        }
      }
    }

    /// <summary>
    /// generate triangle wave
    /// </summary>
    /// <param name="sourcePixels">gives back triangle wave data in source pixel array</param>
    /// <param name="amplitude">amplitude</param>
    public static void GenerateTriangleWave(float[] sourcePixels, float amplitude)
    {
      if (sourcePixels == null)
        throw new ArgumentNullException("sourcePixels");

      float slope = 4 / (float)sourcePixels.Length;
      for (int index = 0; index < sourcePixels.Length; index++)
      {
        if (index <= sourcePixels.Length / 2)
        {
          int mod = index % (sourcePixels.Length - 1);
          if (mod <= sourcePixels.Length / 4)
          {
            sourcePixels[index] = slope * index * amplitude;
          }
          else if (mod <= sourcePixels.Length * 3 / 4)
          {
            sourcePixels[index] = sourcePixels[sourcePixels.Length / 2 - index];
          }
        }
        else
        {
          sourcePixels[index] = -sourcePixels[index - sourcePixels.Length / 2];
        }
      }
    }
  }
}
