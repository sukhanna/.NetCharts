using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ChartLib
{
  /// <summary>
  /// Derieve from IDrawable interface to draw graphics on <see cref="RenderView"/>
  /// </summary>
  public interface IDrawable
  {
    void Draw(Graphics g, RectangleF physicalBounds);
  }
}
