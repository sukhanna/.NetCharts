using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChartLib
{
  public class BaseDispose : IDisposable
  {
    private bool m_disposed = false;

    #region IDisposable

    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
      if (!m_disposed)
      {
        if (disposing)
        {
          // dispose managed resources
        }

        // dispose unmanaged resources and set large objects to null

        m_disposed = true;
      }
    }

    protected void Disposed()
    {
      if (m_disposed)
      {
        throw new ObjectDisposedException(GetType().FullName);
      }
    }

    ~BaseDispose()
    {
      // if object is not disposed correctly then we will end up here, get the object information
      ObjectNotDisposed();
      Dispose(false);
    }


    private void ObjectNotDisposed()
    {
      Type type = GetType();
      while (type != null)
      {
        System.Diagnostics.Debug.WriteLine(string.Format("Finalizating disposable object of type {0}", type.FullName));
        type = type.BaseType;
      }
    }
    #endregion
  }
}
