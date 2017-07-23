using System.Collections.Generic;
using VimApi;

namespace OculiService.CloudProviders.VMware
{
  internal class FolderInsideDC : VCManagedItem, IVimFolderInsideDC, IVimManagedItem
  {
    internal FolderInsideDC(IVimService vimService, ManagedObjectReference managedObject)
      : base(vimService, managedObject)
    {
    }

    public override IVimManagedItem[] GetChildren()
    {
      List<IVimManagedItem> vimManagedItemList = new List<IVimManagedItem>();
      string[] properties1 = new string[1]{ "childEntity" };
      foreach (ManagedObjectReference managedObject in this.GetManagedObjects(properties1))
      {
        if (managedObject.type == "ComputeResource" || managedObject.type == "ClusterComputeResource")
        {
          foreach (ManagedObjectAndProperties objectAndProperty in new FolderInsideDC(this.VcService, managedObject).GetManagedObjectAndProperties(managedObject, "host", "HostSystem", Host.VCProperties))
          {
            IVimManagedItem vimManagedItem = (IVimManagedItem) new Host(this.VcService, objectAndProperty.ManagedObject);
            ((IVimHost) vimManagedItem).GetCommonProperties(objectAndProperty.Properties);
            vimManagedItemList.Add(vimManagedItem);
          }
        }
        else
        {
          IVimManagedItem vimManagedItem = (IVimManagedItem) new FolderInsideDC(this.VcService, managedObject);
          Dictionary<string, object> properties2 = vimManagedItem.GetProperties(new string[1]{ "name" });
          vimManagedItem.Name = (string) properties2["name"];
          vimManagedItemList.Add(vimManagedItem);
        }
      }
      return vimManagedItemList.ToArray();
    }

    public override string GetName()
    {
      this.Name = (string) this.GetProperties(new string[1]{ "name" })["name"];
      return this.Name;
    }
  }
}
