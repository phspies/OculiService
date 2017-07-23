using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;

public class DaclUtil
{
    public static void SetDirDacl(string folder, string account)
    {
        try
        {
            DaclUtil.CleanDirSecurity(folder);
        }
        catch
        {
        }
        DirectoryInfo directoryInfo = new DirectoryInfo(folder);
        DirectorySecurity accessControl = directoryInfo.GetAccessControl();
        FileSystemAccessRule rule = new FileSystemAccessRule(account, FileSystemRights.FullControl, InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, PropagationFlags.None, AccessControlType.Allow);
        accessControl.SetAccessRule(rule);
        DirectorySecurity directorySecurity = accessControl;
        directoryInfo.SetAccessControl(directorySecurity);
    }

    public static void CleanDirSecurity(string dir)
    {
        DirectorySecurity accessControl = Directory.GetAccessControl(dir);
        AuthorizationRuleCollection accessRules = accessControl.GetAccessRules(true, false, typeof(NTAccount));
        List<FileSystemAccessRule> systemAccessRuleList = new List<FileSystemAccessRule>();
        foreach (FileSystemAccessRule systemAccessRule in (ReadOnlyCollectionBase)accessRules)
        {
            try
            {
                NTAccount ntAccount = (NTAccount)systemAccessRule.IdentityReference.Translate(typeof(NTAccount));
            }
            catch
            {
                systemAccessRuleList.Add(systemAccessRule);
            }
        }
        if (systemAccessRuleList.Count <= 0)
            return;
        foreach (FileSystemAccessRule rule in systemAccessRuleList)
            accessControl.RemoveAccessRule(rule);
        Directory.SetAccessControl(dir, accessControl);
    }

    public static void SetFileDacl(string file, string account)
    {
        try
        {
            DaclUtil.CleanFileSecurity(file);
        }
        catch
        {
        }
        FileInfo fileInfo = new FileInfo(file);
        FileSecurity accessControl = fileInfo.GetAccessControl();
        FileSystemAccessRule rule = new FileSystemAccessRule(account, FileSystemRights.FullControl, AccessControlType.Allow);
        accessControl.AddAccessRule(rule);
        FileSecurity fileSecurity = accessControl;
        fileInfo.SetAccessControl(fileSecurity);
    }

    public static void CleanFileSecurity(string file)
    {
        FileSecurity accessControl = File.GetAccessControl(file);
        AuthorizationRuleCollection accessRules = accessControl.GetAccessRules(true, false, typeof(NTAccount));
        List<FileSystemAccessRule> systemAccessRuleList = new List<FileSystemAccessRule>();
        foreach (FileSystemAccessRule systemAccessRule in (ReadOnlyCollectionBase)accessRules)
        {
            try
            {
                NTAccount ntAccount = (NTAccount)systemAccessRule.IdentityReference.Translate(typeof(NTAccount));
            }
            catch
            {
                systemAccessRuleList.Add(systemAccessRule);
            }
        }
        if (systemAccessRuleList.Count <= 0)
            return;
        foreach (FileSystemAccessRule rule in systemAccessRuleList)
            accessControl.RemoveAccessRule(rule);
        File.SetAccessControl(file, accessControl);
    }
}
