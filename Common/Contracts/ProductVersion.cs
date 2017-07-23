using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace OculiService.Core.Contract
{
  [DataContract]
  public class ProductVersion : IExtensibleDataObject
  {
    [DataMember]
    public int Major { get; set; }

    [DataMember]
    public int Minor { get; set; }

    [DataMember]
    public int ServicePack { get; set; }

    [DataMember]
    public int Build { get; set; }

    [DataMember]
    public int Hotfix { get; set; }

    public ExtensionDataObject ExtensionData { get; set; }

    public override string ToString()
    {
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.{1}.{2}.{3}.{4}", (object) this.Major, (object) this.Minor, (object) this.ServicePack, (object) this.Build, (object) this.Hotfix);
    }
  }
}
