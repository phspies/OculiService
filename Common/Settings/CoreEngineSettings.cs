using Microsoft.Win32;
using OculiService.Common.Interfaces;
using System;

namespace OculiService.Common
{
    public class CoreEngineSettings : ICoreEngineSettings
    {
        public const string RegistrySubkeyPath = @"SOFTWARE\OculiService";
        private string engine_id;
        private string organization_id;
        private string access_key;
        private string secret_key;
        private int? httpsPort;
        private int? apiTimeout;
        private bool? useWebProxy;
        private string webProxyUsername;
        private string webProxyPassword;
        private bool? bypassProxyForLocal;

        public bool BypassProxyForLocal
        {
            get
            {
                return GetGenericSetting<bool>(ref this.bypassProxyForLocal, "BypassProxyForLocal", true);
            }
        }
        public int HttpsPort
        {
            get
            {
                return GetGenericSetting<int>(ref this.httpsPort, "HttpsPort", 443);
            }
        }

        public string CoreEngineID
        {
            get
            {
                return GetSettingValue<string>("EngineID", string.Empty);
            }
            set
            {
                SetValue<string>("EngineID", value);
            }
        }
        public string OrganizationID
        {
            get
            {
                return GetSettingValue<string>("OrganizationID", string.Empty);
            }
            set
            {
                SetValue<string>("OrganizationID", value);
            }
        }
        public string AccessKey
        {
            get
            {
                return GetSettingValue<string>("AccessKey", string.Empty);
            }
            set
            {
                SetValue<string>("AccessKey", value);
            }
        }
        public string SecretKey
        {
            get
            {
                return GetSettingValue<string>("SecretKey", string.Empty);
            }
            set
            {
                SetValue<string>("SecretKey", value);
            }
        }
        public string TokenAccessCode
        {
            get
            {
                return GetSettingValue<string>("TokenAccessCode", string.Empty);
            }
            set
            {
                SetValue<string>("TokenAccessCode", value);
            }
        }
        public long TokenExpiry
        {
            get
            {
                return GetSettingValue<long>("TokenExpiry", 0);
            }
            set
            {
                SetValue<long>("TokenExpiry", value);
            }
        }
        public int ApiTimeout
        {
            get
            {
                return GetGenericSetting<int>(ref this.apiTimeout, "ApiTimeout", 30);
            }
        }
        public bool UseWebProxy
        {
            get
            {
                return GetGenericSetting<bool>(ref this.useWebProxy, "UseWebProxy", false);
            }
        }
        public string WebProxyUsername
        {
            get
            {
                return GetSettingValue<string>("WebProxyUsername", String.Empty);
            }
        }
        public string WebProxyPassword
        {
            get
            {
                return GetSettingValue<string>("WebProxyPassword", String.Empty);
            }
        }

        public static T GetGenericSetting<T>(ref T? setting, string name, T defaultValue) where T : struct
        {
            if (!setting.HasValue)
            {
                setting = new T?(GetSettingValue<T>(name, defaultValue));
            }
            return setting.Value;
        }

        public static void SetValue<T>(string name, T value)
        {
            RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(RegistrySubkeyPath, true);

            if (registryKey == null)
            {
                registryKey = Registry.LocalMachine.CreateSubKey(RegistrySubkeyPath);
            }
            if (registryKey != null && value != null)
            {
                registryKey.SetValue(name, value);
            }
        }
        public static T GetSettingValue<T>(string name, T defaultValue)
        {
            object obj = (object)null;
            RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(RegistrySubkeyPath, true);

            if (registryKey == null)
            {
                registryKey = Registry.LocalMachine.CreateSubKey(RegistrySubkeyPath);
            }
            if (registryKey != null)
            {
                obj = registryKey.GetValue(name);
                if (obj == null)
                {
                    registryKey.SetValue(name, defaultValue);
                    obj = registryKey.GetValue(name);
                }
                string str = obj as string;
                if (str != null)
                {
                    obj = ParseSetting(typeof(T), str);
                }
            }

            return (T)(obj ?? (object)defaultValue);
        }

        public static object ParseSetting(Type type, string value)
        {
            if (value != null)
            {
                if (type == typeof(string))
                    return (object)value;
                if (type == typeof(short))
                {
                    short result;
                    if (short.TryParse(value, out result))
                        return (object)result;
                }
                else if (type == typeof(int))
                {
                    int result;
                    if (int.TryParse(value, out result))
                        return (object)result;
                }
                else if (type == typeof(long))
                {
                    long result;
                    if (long.TryParse(value, out result))
                        return (object)result;
                }
                else if (type == typeof(bool))
                {
                    bool result;
                    if (bool.TryParse(value, out result))
                        return (object)result;
                }
                else
                {
                    TimeSpan result;
                    if (type == typeof(TimeSpan) && TimeSpan.TryParse(value, out result))
                        return (object)result;
                }
            }
            return (object)null;
        }
    }
}
