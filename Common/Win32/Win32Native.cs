using OculiService.Win32.SafeHandles;
using Microsoft.Win32;
using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace OculiService.Win32
{
  [SuppressUnmanagedCodeSecurity]
  [SecurityCritical]
  public static class Win32Native
  {
    internal static readonly IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);
    internal const int KEY_QUERY_VALUE = 1;
    internal const int KEY_SET_VALUE = 2;
    internal const int KEY_CREATE_SUB_KEY = 4;
    internal const int KEY_ENUMERATE_SUB_KEYS = 8;
    internal const int KEY_NOTIFY = 16;
    internal const int KEY_CREATE_LINK = 32;
    internal const int KEY_READ = 131097;
    internal const int KEY_WRITE = 131078;
    internal const int KEY_WOW64_64KEY = 256;
    internal const int KEY_WOW64_32KEY = 512;
    internal const int REG_OPTION_NON_VOLATILE = 0;
    internal const int REG_OPTION_VOLATILE = 1;
    internal const int REG_OPTION_CREATE_LINK = 2;
    internal const int REG_OPTION_BACKUP_RESTORE = 4;
    internal const int REG_NONE = 0;
    internal const int REG_SZ = 1;
    internal const int REG_EXPAND_SZ = 2;
    internal const int REG_BINARY = 3;
    internal const int REG_DWORD = 4;
    internal const int REG_DWORD_LITTLE_ENDIAN = 4;
    internal const int REG_DWORD_BIG_ENDIAN = 5;
    internal const int REG_LINK = 6;
    internal const int REG_MULTI_SZ = 7;
    internal const int REG_RESOURCE_LIST = 8;
    internal const int REG_FULL_RESOURCE_DESCRIPTOR = 9;
    internal const int REG_RESOURCE_REQUIREMENTS_LIST = 10;
    internal const int REG_QWORD = 11;
    public const int ERROR_SUCCESS = 0;
    internal const int ERROR_INVALID_FUNCTION = 1;
    internal const int ERROR_FILE_NOT_FOUND = 2;
    internal const int ERROR_PATH_NOT_FOUND = 3;
    internal const int ERROR_ACCESS_DENIED = 5;
    internal const int ERROR_INVALID_HANDLE = 6;
    internal const int ERROR_NOT_ENOUGH_MEMORY = 8;
    internal const int ERROR_INVALID_DATA = 13;
    internal const int ERROR_INVALID_DRIVE = 15;
    internal const int ERROR_NO_MORE_FILES = 18;
    internal const int ERROR_NOT_READY = 21;
    internal const int ERROR_BAD_LENGTH = 24;
    internal const int ERROR_SHARING_VIOLATION = 32;
    internal const int ERROR_NOT_SUPPORTED = 50;
    internal const int ERROR_FILE_EXISTS = 80;
    internal const int ERROR_INVALID_PARAMETER = 87;
    internal const int ERROR_CALL_NOT_IMPLEMENTED = 120;
    internal const int ERROR_INSUFFICIENT_BUFFER = 122;
    internal const int ERROR_INVALID_NAME = 123;
    internal const int ERROR_BAD_PATHNAME = 161;
    internal const int ERROR_ALREADY_EXISTS = 183;
    internal const int ERROR_ENVVAR_NOT_FOUND = 203;
    internal const int ERROR_FILENAME_EXCED_RANGE = 206;
    internal const int ERROR_NO_DATA = 232;
    internal const int ERROR_PIPE_NOT_CONNECTED = 233;
    internal const int ERROR_MORE_DATA = 234;
    internal const int ERROR_DIRECTORY = 267;
    internal const int ERROR_OPERATION_ABORTED = 995;
    internal const int ERROR_NO_TOKEN = 1008;
    internal const int ERROR_DLL_INIT_FAILED = 1114;
    internal const int ERROR_NON_ACCOUNT_SID = 1257;
    internal const int ERROR_NOT_ALL_ASSIGNED = 1300;
    internal const int ERROR_UNKNOWN_REVISION = 1305;
    internal const int ERROR_INVALID_OWNER = 1307;
    internal const int ERROR_INVALID_PRIMARY_GROUP = 1308;
    internal const int ERROR_NO_SUCH_PRIVILEGE = 1313;
    internal const int ERROR_PRIVILEGE_NOT_HELD = 1314;
    internal const int ERROR_NONE_MAPPED = 1332;
    internal const int ERROR_INVALID_ACL = 1336;
    internal const int ERROR_INVALID_SID = 1337;
    internal const int ERROR_INVALID_SECURITY_DESCR = 1338;
    internal const int ERROR_BAD_IMPERSONATION_LEVEL = 1346;
    internal const int ERROR_CANT_OPEN_ANONYMOUS = 1347;
    internal const int ERROR_NO_SECURITY_ON_OBJECT = 1350;
    internal const int ERROR_TRUSTED_RELATIONSHIP_FAILURE = 1789;

    [DllImport("advapi32.dll", CharSet = CharSet.Auto, BestFitMapping = false)]
    internal static extern int RegConnectRegistry(string machineName, SafeRegistryHandle key, out SafeRegistryHandle result);

    [DllImport("advapi32.dll", CharSet = CharSet.Auto, BestFitMapping = false)]
    internal static extern int RegCreateKeyEx(SafeRegistryHandle hKey, string lpSubKey, int Reserved, string lpClass, int dwOptions, int samDesired, Win32Native.SECURITY_ATTRIBUTES lpSecurityAttributes, out SafeRegistryHandle hkResult, out int lpdwDisposition);

    [DllImport("advapi32.dll", CharSet = CharSet.Auto, BestFitMapping = false)]
    internal static extern int RegDeleteKey(SafeRegistryHandle hKey, string lpSubKey);

    [DllImport("advapi32.dll", CharSet = CharSet.Auto, BestFitMapping = false)]
    internal static extern int RegDeleteKeyEx(SafeRegistryHandle hKey, string lpSubKey, int samDesired, int Reserved);

    [DllImport("advapi32.dll", CharSet = CharSet.Auto, BestFitMapping = false)]
    internal static extern int RegDeleteValue(SafeRegistryHandle hKey, string lpValueName);

    [DllImport("advapi32.dll", CharSet = CharSet.Auto, BestFitMapping = false)]
    internal static extern int RegEnumKeyEx(SafeRegistryHandle hKey, int dwIndex, StringBuilder lpName, ref int lpcbName, int[] lpReserved, [Out] StringBuilder lpClass, int[] lpcbClass, long[] lpftLastWriteTime);

    [DllImport("advapi32.dll", CharSet = CharSet.Auto, BestFitMapping = false)]
    internal static extern int RegEnumValue(SafeRegistryHandle hKey, int dwIndex, StringBuilder lpValueName, ref int lpcbValueName, IntPtr lpReserved_MustBeZero, int[] lpType, byte[] lpData, int[] lpcbData);

    [DllImport("advapi32.dll")]
    internal static extern int RegFlushKey(SafeRegistryHandle hKey);

    [DllImport("advapi32.dll", CharSet = CharSet.Auto, BestFitMapping = false)]
    internal static extern int RegOpenKeyEx(SafeRegistryHandle hKey, string lpSubKey, int ulOptions, int samDesired, out SafeRegistryHandle hkResult);

    [DllImport("advapi32.dll", CharSet = CharSet.Auto, BestFitMapping = false)]
    internal static extern int RegOpenKeyEx(IntPtr hKey, string lpSubKey, int ulOptions, int samDesired, out SafeRegistryHandle hkResult);

    [DllImport("advapi32.dll", CharSet = CharSet.Auto, BestFitMapping = false)]
    internal static extern int RegQueryInfoKey(SafeRegistryHandle hKey, [Out] StringBuilder lpClass, int[] lpcbClass, IntPtr lpReserved_MustBeZero, ref int lpcSubKeys, int[] lpcbMaxSubKeyLen, int[] lpcbMaxClassLen, ref int lpcValues, int[] lpcbMaxValueNameLen, int[] lpcbMaxValueLen, int[] lpcbSecurityDescriptor, int[] lpftLastWriteTime);

    [DllImport("advapi32.dll")]
    internal static extern int RegQueryValueEx(SafeRegistryHandle hKey, string lpValueName, int lpReserved, ref int lpType, StringBuilder lpData, ref int lpcbData);

    [DllImport("advapi32.dll", CharSet = CharSet.Auto, BestFitMapping = false)]
    internal static extern int RegSetValueEx(SafeRegistryHandle hKey, string lpValueName, int Reserved, RegistryValueKind dwType, byte[] lpData, int cbData);

    [DllImport("advapi32.dll", CharSet = CharSet.Auto, BestFitMapping = false)]
    internal static extern int RegSetValueEx(SafeRegistryHandle hKey, string lpValueName, int Reserved, RegistryValueKind dwType, ref int lpData, int cbData);

    [DllImport("advapi32.dll", CharSet = CharSet.Auto, BestFitMapping = false)]
    internal static extern int RegSetValueEx(SafeRegistryHandle hKey, string lpValueName, int Reserved, RegistryValueKind dwType, ref long lpData, int cbData);

    [DllImport("advapi32.dll", CharSet = CharSet.Auto, BestFitMapping = false)]
    internal static extern int RegSetValueEx(SafeRegistryHandle hKey, string lpValueName, int Reserved, RegistryValueKind dwType, string lpData, int cbData);

    [StructLayout(LayoutKind.Sequential)]
    internal class SECURITY_ATTRIBUTES
    {
      internal int nLength;
      internal IntPtr lpSecurityDescriptor;
      internal int bInheritHandle;
    }
  }
}
