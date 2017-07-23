using System.Collections.Generic;
using VimApi;

namespace OculiService.CloudProviders.VMware
{
  internal class FolderOutsideDC : VCManagedItem, IVimFolderOutsideDC, IVimManagedItem
  {
    internal FolderOutsideDC(IVimService vimService, ManagedObjectReference managedObject)
      : base(vimService, managedObject)
    {
      this.ManagedObject = new ManagedObjectReference();
      this.ManagedObject = managedObject;
    }

    public override IVimManagedItem[] GetChildren()
    {
      List<IVimManagedItem> vimManagedItemList = new List<IVimManagedItem>();
      ManagedObjectReference managedObject1 = this.ManagedObject;
      string path1 = "childEntity";
      string childType1 = "Datacenter";
      string[] childProperties1 = new string[1]{ "name" };
      foreach (ManagedObjectAndProperties objectAndProperty in this.GetManagedObjectAndProperties(managedObject1, path1, childType1, childProperties1))
      {
        IVimManagedItem vimManagedItem = (IVimManagedItem) new Datacenter(this.VcService, objectAndProperty.ManagedObject);
        vimManagedItem.Name = (string) objectAndProperty.Properties["name"];
        vimManagedItemList.Add(vimManagedItem);
      }
      ManagedObjectReference managedObject2 = this.ManagedObject;
      string path2 = "childEntity";
      string childType2 = "Folder";
      string[] childProperties2 = new string[1]{ "name" };
      foreach (ManagedObjectAndProperties objectAndProperty in this.GetManagedObjectAndProperties(managedObject2, path2, childType2, childProperties2))
      {
        if (!(this.ManagedObject.Value == objectAndProperty.ManagedObject.Value))
        {
          IVimManagedItem vimManagedItem = (IVimManagedItem) new FolderOutsideDC(this.VcService, objectAndProperty.ManagedObject);
          vimManagedItem.Name = (string) objectAndProperty.Properties["name"];
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
