using OculiService.Core.Contract;

public static class NetShUtils
{
    public const string WANFAILOVERLOGFILE = "WAN-Failover-Results.log";

    public static string CreateNetShScript(ServerNicInfo[] Nics, OperatingSystemInfo OsInfo, string fileName)
    {
        string str = string.Empty;
        foreach (ServerNicInfo replicaNic in Nics)
        {
            if (NetShUtils.IsNicMapped(replicaNic))
            {
                str = str + "net start >> WAN-Failover-Results.log\r\n" + "ping 127.0.0.1 -n 20 >> WAN-Failover-Results.log\r\n" + "net start >> WAN-Failover-Results.log\r\n" + "netsh interface ip set address \"" + replicaNic.FriendlyName + "\" dhcp >> WAN-Failover-Results.log\r\n" + "netsh interface ip set dns \"" + replicaNic.FriendlyName + "\" dhcp >> WAN-Failover-Results.log\r\n";
                if (replicaNic.IPAddresses != null && replicaNic.IPAddresses.Length != 0)
                {
                    for (int index = 0; index < replicaNic.IPAddresses.Length; ++index)
                        str += NetShUtils.AddressNetShCmd(index == 0, replicaNic.FriendlyName, replicaNic.IPAddresses[index], replicaNic.IPMasks[index], OsInfo);
                }
                if (replicaNic.IPGateways != null && replicaNic.IPGateways.Length != 0)
                {
                    for (int index = 0; index < replicaNic.IPGateways.Length; ++index)
                        str += NetShUtils.GatewayNetShCmd(index == 0, replicaNic.FriendlyName, replicaNic.IPGateways[index], "0", OsInfo);
                }
                if (replicaNic.DNSAddrs != null && replicaNic.DNSAddrs.Length != 0)
                {
                    for (int index = 0; index < replicaNic.DNSAddrs.Length; ++index)
                        str += NetShUtils.DnsNetShCmd(index == 0, replicaNic.FriendlyName, replicaNic.DNSAddrs[index], OsInfo);
                }
            }
        }
        return str + "ipconfig /registerdns >> WAN-Failover-Results.log\r\n";
    }

    public static bool IsNicMapped(ServerNicInfo nic)
    {
        if (nic.VirtualNetwork != "---Discard---" && nic.IPAddresses != null)
            return (uint)nic.IPAddresses.Length > 0U;
        return false;
    }

    private static string IPCmd(string addr)
    {
        return IPHelper.IsIPv6(addr) ? "ipv6" : "ip";
    }

    private static string AddressNetShCmd(bool first, string name, string addr, string mask, OperatingSystemInfo OsInfo)
    {
        return "netsh interface " + NetShUtils.IPCmd(addr) + (first ? " set address " : " add address ") + "\"" + name + "\" " + (first ? "static " : "") + addr + " " + mask + " >> WAN-Failover-Results.log\r\n";
    }

    private static string GatewayNetShCmd(bool first, string name, string addr, string gwMetric, OperatingSystemInfo OsInfo)
    {
        string str = "netsh interface " + NetShUtils.IPCmd(addr) + " add address " + "\"" + name + "\" " + "gateway=" + addr + " ";
        return (string.IsNullOrEmpty(gwMetric) ? str + "gwmetric=" + gwMetric : str + "gwmetric=0") + " >> WAN-Failover-Results.log\r\n";
    }

    private static string DnsNetShCmd(bool first, string name, string addr, OperatingSystemInfo OsInfo)
    {
        return "netsh interface " + NetShUtils.IPCmd(addr) + (first ? " set dns " : " add dns ") + "\"" + name + "\" " + (first ? "static " : "") + addr + " >> WAN-Failover-Results.log\r\n";
    }
}
