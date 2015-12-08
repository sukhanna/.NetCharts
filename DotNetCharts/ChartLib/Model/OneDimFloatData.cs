using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Runtime;

namespace ChartLib
{
  public class OneDimFloatData : OneDimData<float>
  {
    protected override void CalculateRange()
    {
      float[] data = GetData();

      if (data == null)
        return;

      Debug.Assert(data.Length > 0);

      int length = data.Length;
      float[] dataCopy = new float[length];
      data.CopyTo(dataCopy, 0);

      // Heap sort
      Array.Sort(dataCopy);
      DataRange = new DataRangeFloat();
      DataRange.XRange = new RangeFloat(0, length);
      DataRange.YRange = new RangeFloat(dataCopy[0], dataCopy[length - 1]);

      Debug.WriteLine("Total memory before GC.Collect() " + GC.GetTotalMemory(false).ToString());
      GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
      GC.Collect();
      Debug.WriteLine("Total memory after GC.Collect() " + GC.GetTotalMemory(true).ToString());
    }
  }
}
