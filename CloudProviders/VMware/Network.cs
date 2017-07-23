using System.Collections.Generic;
using VimApi;

namespace OculiService.CloudProviders.VMware
{
  public class Network : VCManagedItem, IVimNetwork, IVimManagedItem
  {
    public static string[] VCProperties = new string[1]{ "name" };
    public static string[] VCPortgroupProperties = new string[2]{ "name", "key" };
    private NetworkProperties _properties;

    public NetworkProperties Properties
    {
      get
      {
        return this._properties;
      }
      set
      {
        this._properties = value;
      }
    }

    public bool IsDistributed
    {
      get
      {
        return this._properties.IsDistributed;
      }
    }

    public string PortgroupKey
    {
      get
      {
        if (!this.IsDistributed)
          return string.Empty;
        if (string.IsNullOrEmpty(this._properties.PortgroupKey))
          this.GetCommonProperties(this.GetProperties(Network.VCPortgroupProperties));
        return this._properties.PortgroupKey;
      }
    }

    internal Network(IVimService vimService, ManagedObjectReference managedObject)
      : base(vimService, managedObject)
    {
      if (managedObject.type == "DistributedVirtualPortgroup")
        this._properties.IsDistributed = true;
      else
        this._properties.IsDistributed = false;
    }

    public NetworkProperties GetCommonProperties()
    {
      Dictionary<string, object> dictionary = !this.IsDistributed ? this.GetProperties(Network.VCProperties) : this.GetProperties(Network.VCPortgroupProperties);
      this._properties.Name = (string) dictionary["name"];
      if (dictionary.ContainsKey("key"))
        this._properties.PortgroupKey = (string) dictionary["key"];
      this.Name = this._properties.Name;
      return this._properties;
    }

    public void GetCommonProperties(Dictionary<string, object> properties)
    {
      this._properties.Name = (string) properties["name"];
      if (this.IsDistributed && !properties.ContainsKey("key"))
      {
        properties = this.GetProperties(Network.VCPortgroupProperties);
        if (properties.ContainsKey("key"))
          this._properties.PortgroupKey = (string) properties["key"];
      }
      this.Name = this._properties.Name;
    }

    public override string GetName()
    {
      if (string.IsNullOrEmpty(this._properties.Name))
        this.GetCommonProperties();
      return this.Name;
    }
  }
}
