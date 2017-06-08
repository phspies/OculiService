using Microsoft.Win32.SafeHandles;
using System.IO;

namespace OculiService.Common.IO
{
  public interface ITransactions
  {
    bool TransactionsSupported { get; }

    SafeFileHandle CreateTransaction();

    Stream Open(SafeFileHandle transaction, string path);

    void Commit(SafeFileHandle transaction);

    void Rollback(SafeFileHandle transaction);
  }
}
