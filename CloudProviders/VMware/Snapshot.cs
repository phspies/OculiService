using VimApi;

namespace OculiService.CloudProviders.VMware
{
  internal class Snapshot : VCManagedItem, IVimSnapshot, IVimManagedItem
  {
    internal Snapshot(IVimService service, ManagedObjectReference managedObject)
      : base(service, managedObject)
    {
    }

    public override IVimManagedItem[] GetChildren()
    {
      return (IVimManagedItem[]) null;
    }
  }
}
