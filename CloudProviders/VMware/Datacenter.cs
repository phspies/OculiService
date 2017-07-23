using System.Collections.Generic;
using VimApi;

namespace OculiService.CloudProviders.VMware
{
  internal class Datacenter : VCManagedItem, IVimDatacenter, IVimManagedItem
  {
    public static string[] VCProperties = new string[3]{ "name", "effectiveRole", "vmFolder" };
    private DatacenterProperties _dcProperties;

    public DatacenterProperties DatacenterProperties
    {
      get
      {
        return this._dcProperties;
      }
      set
      {
        this._dcProperties = value;
      }
    }

    internal Datacenter(IVimService vimService, ManagedObjectReference managedObject)
      : base(vimService, managedObject)
    {
    }

    public void GetCommonProperties()
    {
      if (this._dcProperties.Name != null)
        return;
      this.GetCommonProperties(this.GetProperties(Datacenter.VCProperties));
    }

    public void GetCommonProperties(Dictionary<string, object> properties)
    {
      this._dcProperties.Name = (string) properties["name"];
      this._dcProperties.EffectiveRoles = (int[]) properties["effectiveRole"];
      this._dcProperties.VmFolder = (ManagedObjectReference) properties["vmFolder"];
      this.Name = this._dcProperties.Name;
    }

    public override IVimManagedItem[] GetChildren()
    {
      ManagedObjectAndProperties[] objectAndProperties1 = this.GetManagedObjectAndProperties(this.ManagedObject, "hostFolder", "Folder", new string[1]{ "name" });
      IVimManagedItem[] vimManagedItemArray = (IVimManagedItem[]) null;
      foreach (ManagedObjectAndProperties objectAndProperties2 in objectAndProperties1)
      {
        IVimFolderInsideDC vimFolderInsideDc = (IVimFolderInsideDC) new FolderInsideDC(this.VcService, objectAndProperties2.ManagedObject);
        vimFolderInsideDc.Name = (string) objectAndProperties2.Properties["name"];
        if (vimFolderInsideDc.Name == "host")
          vimManagedItemArray = vimFolderInsideDc.GetChildren();
      }
      return vimManagedItemArray;
    }

    public override string GetName()
    {
      if (this._dcProperties.Name == null)
        this.GetCommonProperties();
      return this._dcProperties.Name;
    }

    public ManagedObjectReference GetVmFolder()
    {
      if (this._dcProperties.VmFolder == null)
        this.GetCommonProperties();
      return this._dcProperties.VmFolder;
    }
  }
}
