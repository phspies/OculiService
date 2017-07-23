using System;
using System.Runtime.Serialization;

[DataContract]
[Serializable]
public class ServerNicInfo
{
    [DataMember]
    public int Index = 9999;
    [DataMember]
    public string NicGuid = Guid.NewGuid().ToString("B");
    [DataMember]
    public int BusNumber;
    [DataMember]
    public int DeviceNumber;
    [DataMember]
    public string PNPInstanceId;
    [DataMember]
    public string TcpIpServiceUuid;
    [DataMember]
    public string VirtualNetwork;
    [DataMember]
    public string FriendlyName;
    [DataMember]
    public string[] IPAddresses;
    [DataMember]
    public string[] IPMasks;
    [DataMember]
    public string[] IPGateways;
    [DataMember]
    public string[] DNSAddrs;
    [DataMember]
    public bool DHCPEnabled;
    [DataMember]
    public VirtualNicType NicType;
    [DataMember]
    public string DNSDomain;

    public NicInfo NicInfo
    {
        get
        {
            return new NicInfo() { IPs = this.IPAddresses, PNPInstanceId = this.PNPInstanceId, TcpIpServiceUuid = this.TcpIpServiceUuid, VirtualNetwork = this.VirtualNetwork, FriendlyName = this.FriendlyName, BusNumber = this.BusNumber, DeviceNumber = this.DeviceNumber, Index = this.Index };
        }
    }

    public ServerNicInfo()
    {
    }

    public ServerNicInfo(NicInfo nic)
    {
        this.BusNumber = nic.BusNumber;
        this.DeviceNumber = nic.DeviceNumber;
        this.FriendlyName = nic.FriendlyName;
        this.Index = nic.Index;
        this.IPAddresses = nic.IPs;
        this.PNPInstanceId = nic.PNPInstanceId;
        this.TcpIpServiceUuid = nic.TcpIpServiceUuid;
        this.VirtualNetwork = nic.VirtualNetwork;
    }

    public ServerNicInfo(ServerNicInfo nic)
    {
        this.IPAddresses = (string[])nic.IPAddresses.Clone();
        this.IPMasks = (string[])nic.IPMasks.Clone();
        this.IPGateways = nic.IPGateways != null ? (string[])nic.IPGateways.Clone() : (string[])null;
        this.DNSAddrs = nic.DNSAddrs != null ? (string[])nic.DNSAddrs.Clone() : (string[])null;
        this.BusNumber = nic.BusNumber;
        this.DeviceNumber = nic.DeviceNumber;
        this.FriendlyName = nic.FriendlyName;
        this.Index = nic.Index;
        this.NicGuid = nic.NicGuid;
        this.NicType = nic.NicType;
        this.PNPInstanceId = nic.PNPInstanceId;
        this.TcpIpServiceUuid = nic.TcpIpServiceUuid;
        this.VirtualNetwork = nic.VirtualNetwork;
    }
}
