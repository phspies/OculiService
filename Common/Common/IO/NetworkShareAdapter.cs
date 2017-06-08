using OculiService.Common.Diagnostics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;

namespace OculiService.Common.IO
{
    internal sealed class NetworkShareAdapter : NetworkShareBase
    {
        private static Tracer _tracer = Tracer.GetTracer(typeof(NetworkShareAdapter));
        internal const uint NERR_Success = 0;
        internal const uint MAX_PREFERRED_LENGTH = 4294967294;
        internal const uint ERROR_INSUFFICIENT_BUFFER = 122;
        internal const uint ERROR_ENVVAR_NOT_FOUND = 203;
        internal const uint SE_DACL_PRESENT = 4;

        public override IEnumerable<NetworkShareInfo> GetShares(bool includeSecurity = false)
        {
            if (includeSecurity)
                return this.GetSharesWithSecurity();
            return this.GetSharesLegacy();
        }

        private IEnumerable<NetworkShareInfo> GetSharesLegacy()
        {
            IList<NetworkShareInfo> networkShareInfoList = (IList<NetworkShareInfo>)new List<NetworkShareInfo>();
            using (ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher("SELECT * FROM Win32_Share"))
            {
                foreach (ManagementObject managementObject in managementObjectSearcher.Get())
                {
                    using (managementObject)
                        networkShareInfoList.Add((NetworkShareInfo)managementObject);
                }
            }
            return (IEnumerable<NetworkShareInfo>)networkShareInfoList;
        }

        public override void CreateFileShare(NetworkShareInfo share)
        {
            using (ManagementClass managementClass1 = new ManagementClass("Win32_Share"))
            {
                ManagementClass wmiClassTrustee = new ManagementClass("Win32_Trustee");
                try
                {
                    ManagementClass wmiClassAce = new ManagementClass("Win32_Ace");
                    try
                    {
                        using (ManagementClass managementClass2 = new ManagementClass("Win32_SecurityDescriptor"))
                        {
                            using (ManagementBaseObject methodParameters = managementClass1.GetMethodParameters("Create"))
                            {
                                ManagementObject[] managementObjectArray = (ManagementObject[])null;
                                try
                                {
                                    managementObjectArray = share.Aces == null ? new ManagementObject[0] : share.Aces.Select<Ace, ManagementObject>((Func<Ace, ManagementObject>)(a =>
                                   {
                                       if (a == null)
                                       {
                                           throw new NetworkShareAdapterException("Missing ACE information", 0);
                                       }
                                       NetworkShareAdapter._tracer.TraceInformation("Creating ACE for account {0}\\{1}", (object)a.domain, (object)(a.name ?? new SecurityIdentifier(a.sid, 0).ToString()));
                                       ManagementObject instance1 = wmiClassTrustee.CreateInstance();
                                       instance1["Domain"] = (object)a.domain;
                                       instance1["Name"] = (object)a.name;
                                       instance1["SID"] = (object)a.sid;
                                       ManagementObject instance2 = wmiClassAce.CreateInstance();
                                       string index1 = "AccessMask";

                                       instance2[index1] = (ValueType)a.accessMask;
                                       string index2 = "AceFlags";

                                       instance2[index2] = (Enum)a.aceFlags;
                                       string index3 = "AceType";

                                       instance2[index3] = (Enum)a.aceType;
                                       string index4 = "Trustee";
                                       ManagementObject managementObject = instance1;
                                       instance2[index4] = (object)managementObject;
                                       return instance2;
                                   })).ToArray<ManagementObject>();
                                    ManagementObject instance = managementClass2.CreateInstance();
                                    instance["ControlFlags"] = (object)4U;
                                    instance["DACL"] = (object)managementObjectArray;
                                    methodParameters["Path"] = (object)share.Path.Replace('/', '\\');
                                    methodParameters["Name"] = (object)share.Name;
                                    methodParameters["Type"] = (object)share.TypeMask;
                                    int? maxUsers = share.MaxUsers;
                                    if (maxUsers.HasValue)
                                    {
                                        ManagementBaseObject managementBaseObject = methodParameters;
                                        string index = "MaximumAllowed";
                                        maxUsers = share.MaxUsers;
                                        managementBaseObject[index] = (ValueType)(uint)maxUsers.Value;
                                    }
                                    methodParameters["Access"] = (object)instance;
                                    using (ManagementBaseObject managementBaseObject = managementClass1.InvokeMethod("Create", methodParameters, (InvokeMethodOptions)null))
                                    {
                                        uint num = (uint)managementBaseObject.Properties["ReturnValue"].Value;
                                        if ((int)num != 0)
                                            throw new NetworkShareAdapterException(string.Format("Error adding share {0}", (object)share.Name), (int)num);
                                    }
                                }
                                finally
                                {
                                    if (managementObjectArray != null)
                                    {
                                        foreach (ManagementObject managementObject in managementObjectArray)
                                            managementObject.Dispose();
                                    }
                                }
                            }
                        }
                    }
                    finally
                    {
                        if (wmiClassAce != null)
                            wmiClassAce.Dispose();
                    }
                }
                finally
                {
                    if (wmiClassTrustee != null)
                        wmiClassTrustee.Dispose();
                }
            }
        }

        public override void DeleteFileShare(string share)
        {
            using (ManagementObject managementObject = new ManagementObject(string.Format("Win32_Share.Name=\"{0}\"", (object)share)))
            {
                if ((int)(uint)managementObject.InvokeMethod("Delete", (object[])null) != 0)
                    throw new ManagementException(string.Format("Error deleting share: {0}", (object)share));
            }
        }

        internal IEnumerable<NetworkShareInfo> GetSharesWithSecurity()
        {
            IntPtr zero = IntPtr.Zero;
            IntPtr bufptr;
            uint entriesread;
            uint totalentries;
            uint num1 = NetworkShareAdapter.NetShareEnum((string)null, Environment.OSVersion.IsWindows2008OrLater() ? 503U : 502U, out bufptr, 4294967294U, out entriesread, out totalentries, ref zero);
            if ((int)num1 != 0)
                throw new Win32Exception((int)num1, string.Format("Error getting shares: {0}", (object)num1));
            IList<NetworkShareInfo> networkShareInfoList = (IList<NetworkShareInfo>)new List<NetworkShareInfo>();
            try
            {
                NetworkShareAdapter._tracer.TraceInformation("Found {0} shares", (object)entriesread);
                for (int index = 0; (long)entriesread > (long)index; ++index)
                {
                    NetworkShareAdapter.ShareInfoAdapter share = new NetworkShareAdapter.ShareInfoAdapter(bufptr, index, Environment.OSVersion);
                    NetworkShareAdapter._tracer.TraceInformation("Enumerating share {0}", (object)share.netname);
                    networkShareInfoList.Add(new NetworkShareInfo(share.netname, (int)share.type, share.path, this.GetAces(share), 0 > (int)share.max_uses ? new int?() : new int?((int)share.max_uses), share.servername));
                }
            }
            finally
            {
                int num2 = (int)NetworkShareAdapter.NetApiBufferFree(bufptr);
            }
            return (IEnumerable<NetworkShareInfo>)networkShareInfoList;
        }

        private IEnumerable<Ace> GetAces(NetworkShareAdapter.ShareInfoAdapter share)
        {
            IntPtr zero = IntPtr.Zero;
            try
            {
                if (!NetworkShareAdapter.IsValidSecurityDescriptor(share.security_descriptor))
                {
                    NetworkShareAdapter._tracer.TraceInformation("Invalid security descriptor for share {0}", (object)share.netname);
                    return (IEnumerable<Ace>)null;
                }
                bool lpbDaclPresent;
                bool lpbDaclDefaulted;
                if (!NetworkShareAdapter.GetSecurityDescriptorDacl(share.security_descriptor, out lpbDaclPresent, ref zero, out lpbDaclDefaulted))
                    throw new Win32Exception(Marshal.GetLastWin32Error(), string.Format("Error getting DACL for share {0}", (object)share.netname));
            }
            catch (Win32Exception ex)
            {
                NetworkShareAdapter._tracer.TraceInformation("Unable to obtain DACL for share {0}: {1}", (object)share.netname, (object)ex);
                return (IEnumerable<Ace>)null;
            }
            IList<Ace> aceList = (IList<Ace>)new List<Ace>();
            if (IntPtr.Zero == zero)
            {
                NetworkShareAdapter._tracer.TraceInformation("Security descriptor does not contain a DACL");
                return (IEnumerable<Ace>)aceList;
            }
            NetworkShareAdapter.ACL structure1 = (NetworkShareAdapter.ACL)Marshal.PtrToStructure(zero, typeof(NetworkShareAdapter.ACL));
            NetworkShareAdapter._tracer.TraceInformation("ACE count: {0}", (object)structure1.AceCount);
            for (uint dwAceIndex = 0; (uint)structure1.AceCount > dwAceIndex; ++dwAceIndex)
            {
                NetworkShareAdapter._tracer.TraceInformation("Iterating ACE: {0}", (object)dwAceIndex);
                try
                {
                    IntPtr pAce;
                    if (!NetworkShareAdapter.GetAce(zero, dwAceIndex, out pAce))
                        throw new Win32Exception(Marshal.GetLastWin32Error(), string.Format("Error getting ACE for share {0}", (object)share.netname));
                    NetworkShareAdapter.ACE structure2 = (NetworkShareAdapter.ACE)Marshal.PtrToStructure(pAce, typeof(NetworkShareAdapter.ACE));
                    IntPtr num = new IntPtr(pAce.ToInt64() + Marshal.OffsetOf(typeof(NetworkShareAdapter.ACE), "SidStart").ToInt64());
                    uint cchName = 0;
                    uint cchReferencedDomainName = 0;
                    StringBuilder lpName = new StringBuilder((int)cchName);
                    StringBuilder lpReferencedDomainName = new StringBuilder((int)cchReferencedDomainName);
                    NetworkShareAdapter.SID_NAME_USE peUse;
                    if (NetworkShareAdapter.LookupAccountSid((string)null, num, (StringBuilder)null, ref cchName, (StringBuilder)null, ref cchReferencedDomainName, out peUse) || 122L == (long)Marshal.GetLastWin32Error() || 203L == (long)Marshal.GetLastWin32Error())
                    {
                        lpName.EnsureCapacity((int)cchName);
                        lpReferencedDomainName.EnsureCapacity((int)cchReferencedDomainName);
                        if (!NetworkShareAdapter.LookupAccountSid((string)null, num, lpName, ref cchName, lpReferencedDomainName, ref cchReferencedDomainName, out peUse))
                            NetworkShareAdapter._tracer.TraceInformation("Unable to lookup account name for SID {0}: {1}", (object)new SecurityIdentifier(num), (object)Marshal.GetLastWin32Error());
                    }
                    else
                        NetworkShareAdapter._tracer.TraceInformation("Unable to determine account name capacity for SID {0}: {1}", (object)new SecurityIdentifier(num), (object)Marshal.GetLastWin32Error());
                    NetworkShareAdapter._tracer.TraceInformation("Share {0}: ACE type: {1}, flags: {2}, mask: {3}, user: {4}\\{5}, server: {6}", (object)share.netname, (object)structure2.Header.AceType, (object)structure2.Header.AceFlags, (object)structure2.Mask, (object)lpReferencedDomainName.ToString(), (object)(lpName.Length == 0 ? new SecurityIdentifier(num).ToString() : lpName.ToString()), (object)share.servername);
                    uint lengthSid = NetworkShareAdapter.GetLengthSid(num);
                    byte[] destination = new byte[(int)lengthSid];
                    Marshal.Copy(num, destination, 0, (int)lengthSid);
                    aceList.Add(new Ace()
                    {
                        accessMask = (int)structure2.Mask,
                        aceFlags = (AceFlags)structure2.Header.AceFlags,
                        aceType = (AceType)structure2.Header.AceType,
                        domain = lpReferencedDomainName.Length == 0 ? (string)null : lpReferencedDomainName.ToString(),
                        name = lpName.Length == 0 ? (string)null : lpName.ToString(),
                        sid = destination
                    });
                }
                catch (Win32Exception ex)
                {
                    NetworkShareAdapter._tracer.TraceInformation("Unable to obtain ACE for share {0}: {1}", (object)share.netname, (object)ex);
                    aceList.Add((Ace)null);
                }
            }
            return (IEnumerable<Ace>)aceList;
        }

        [DllImport("netapi32.dll", CharSet = CharSet.Unicode)]
        internal static extern uint NetShareEnum(string servername, uint level, out IntPtr bufptr, uint prefmaxlen, out uint entriesread, out uint totalentries, ref IntPtr resume_handle);

        [DllImport("advapi32.dll", SetLastError = true)]
        internal static extern bool GetSecurityDescriptorDacl(IntPtr pSecurityDescriptor, out bool lpbDaclPresent, ref IntPtr pDacl, out bool lpbDaclDefaulted);

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern bool LookupAccountSid(string lpSystemName, IntPtr lpSid, StringBuilder lpName, ref uint cchName, StringBuilder lpReferencedDomainName, ref uint cchReferencedDomainName, out NetworkShareAdapter.SID_NAME_USE peUse);

        [DllImport("advapi32.dll", SetLastError = true)]
        internal static extern bool GetAce(IntPtr pAcl, uint dwAceIndex, out IntPtr pAce);

        [DllImport("netapi32.dll")]
        internal static extern uint NetApiBufferFree(IntPtr Buffer);

        [DllImport("advapi32.dll", SetLastError = true)]
        internal static extern bool IsValidSecurityDescriptor(IntPtr pSecurityDescriptor);

        [DllImport("advapi32.dll")]
        internal static extern uint GetLengthSid(IntPtr pSid);

        private class ShareInfoAdapter
        {
            private NetworkShareAdapter.SHARE_INFO_502 _info502;
            private NetworkShareAdapter.SHARE_INFO_503 _info503;
            private bool _is2008OrLater;

            internal string netname
            {
                get
                {
                    if (this._is2008OrLater)
                        return this._info503.shi503_netname;
                    return this._info502.shi502_netname;
                }
            }

            internal uint type
            {
                get
                {
                    if (this._is2008OrLater)
                        return this._info503.shi503_type;
                    return this._info502.shi502_type;
                }
            }

            internal string remark
            {
                get
                {
                    if (this._is2008OrLater)
                        return this._info503.shi503_remark;
                    return this._info502.shi502_remark;
                }
            }

            internal uint permissions
            {
                get
                {
                    if (this._is2008OrLater)
                        return this._info503.shi503_permissions;
                    return this._info502.shi502_permissions;
                }
            }

            internal uint max_uses
            {
                get
                {
                    if (this._is2008OrLater)
                        return this._info503.shi503_max_uses;
                    return this._info502.shi502_max_uses;
                }
            }

            internal uint current_uses
            {
                get
                {
                    if (this._is2008OrLater)
                        return this._info503.shi503_current_uses;
                    return this._info502.shi502_current_uses;
                }
            }

            internal string path
            {
                get
                {
                    if (this._is2008OrLater)
                        return this._info503.shi503_path;
                    return this._info502.shi502_path;
                }
            }

            internal string passwd
            {
                get
                {
                    if (this._is2008OrLater)
                        return this._info503.shi503_passwd;
                    return this._info502.shi502_passwd;
                }
            }

            internal string servername
            {
                get
                {
                    if (this._is2008OrLater)
                        return this._info503.shi503_servername;
                    return "*";
                }
            }

            internal uint reserved
            {
                get
                {
                    if (this._is2008OrLater)
                        return this._info503.shi503_reserved;
                    return this._info502.shi502_reserved;
                }
            }

            internal IntPtr security_descriptor
            {
                get
                {
                    if (this._is2008OrLater)
                        return this._info503.shi503_security_descriptor;
                    return this._info502.shi502_security_descriptor;
                }
            }

            internal ShareInfoAdapter(IntPtr ptrInfo, int index, OperatingSystem os)
            {
                this._is2008OrLater = os.IsWindows2008OrLater();
                int num = Marshal.SizeOf(this._is2008OrLater ? typeof(NetworkShareAdapter.SHARE_INFO_503) : typeof(NetworkShareAdapter.SHARE_INFO_502));
                if (this._is2008OrLater)
                    this._info503 = (NetworkShareAdapter.SHARE_INFO_503)Marshal.PtrToStructure(new IntPtr(ptrInfo.ToInt64() + (long)(num * index)), typeof(NetworkShareAdapter.SHARE_INFO_503));
                else
                    this._info502 = (NetworkShareAdapter.SHARE_INFO_502)Marshal.PtrToStructure(new IntPtr(ptrInfo.ToInt64() + (long)(num * index)), typeof(NetworkShareAdapter.SHARE_INFO_502));
            }
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal struct SHARE_INFO_502
        {
            internal string shi502_netname;
            internal uint shi502_type;
            internal string shi502_remark;
            internal uint shi502_permissions;
            internal uint shi502_max_uses;
            internal uint shi502_current_uses;
            internal string shi502_path;
            internal string shi502_passwd;
            internal uint shi502_reserved;
            internal IntPtr shi502_security_descriptor;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal struct SHARE_INFO_503
        {
            internal string shi503_netname;
            internal uint shi503_type;
            internal string shi503_remark;
            internal uint shi503_permissions;
            internal uint shi503_max_uses;
            internal uint shi503_current_uses;
            internal string shi503_path;
            internal string shi503_passwd;
            internal string shi503_servername;
            internal uint shi503_reserved;
            internal IntPtr shi503_security_descriptor;
        }

        internal struct ACL
        {
            internal byte AclRevision;
            internal byte Sbz1;
            internal ushort AclSize;
            internal ushort AceCount;
            internal ushort Sbz2;
        }

        internal struct ACE_HEADER
        {
            internal byte AceType;
            internal byte AceFlags;
            internal ushort AceSize;
        }

        internal struct ACE
        {
            internal NetworkShareAdapter.ACE_HEADER Header;
            internal uint Mask;
            internal uint SidStart;
        }

        internal enum SID_NAME_USE
        {
            SidTypeUser = 1,
            SidTypeGroup = 2,
            SidTypeDomain = 3,
            SidTypeAlias = 4,
            SidTypeWellKnownGroup = 5,
            SidTypeDeletedAccount = 6,
            SidTypeInvalid = 7,
            SidTypeUnknown = 8,
            SidTypeComputer = 9,
        }
    }
}
