using System;

namespace Oculi.Jobs.Context
{
  public interface IContextCleanup : IDisposable
  {
    void Add(string role, Action action);

    bool RoleAdded(string role);

    void Remove(string role);
  }
}
