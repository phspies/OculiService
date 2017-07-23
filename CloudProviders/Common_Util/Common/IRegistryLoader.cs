using System;

namespace Common_Util
{
  public interface IRegistryLoader : IDisposable
  {
    bool Disposed { get; }

    string GetCurrentControlSet();

    string GetRootKey();
  }
}
