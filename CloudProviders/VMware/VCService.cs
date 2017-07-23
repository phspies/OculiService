using Microsoft.Win32;
using OculiService.Common.Diagnostics;
using OculiService.Common.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.Threading;
using System.Web.Services.Protocols;
using System.Xml;
using VimApi;

namespace OculiService.CloudProviders.VMware
{
  public class VCService : IVimService
  {
    private static object _lockPolicyCert = new object();
    private static readonly Tracer tracer = Tracer.GetTracer(typeof (VCService));
    private AutoResetEvent _shutdownEvent = new AutoResetEvent(false);
    private int _nRetries = 1;
    private VimService _Service;
    private ServiceContent _ServiceContent;
    private ManagedObjectReference _SvcRef;
    private ManagedObjectReference _PropCol;
    private IVimFolderOutsideDC _RootFolder;
    private Thread _keepAliveThread;
    private bool _loggedOn;
    private bool _shutdown;
    private bool _logginOff;
    private ILogger _logger;
    private string _hostName;
    private string _userName;
    private string _password;
    private ICredential _credential;
    private const VMwareCertificatePolicy DEFAULT_CERT_POLICY = VMwareCertificatePolicy.AllowValid;

    public ManagedObjectReference PropertyCollector
    {
      get
      {
        return this._PropCol;
      }
    }

    public IVimFolderOutsideDC RootFolder
    {
      get
      {
        return this._RootFolder;
      }
    }

    public VimService Service
    {
      get
      {
        return this._Service;
      }
    }

    internal ManagedObjectReference Root
    {
      get
      {
        return this._ServiceContent.rootFolder;
      }
    }

    public string FullName
    {
      get
      {
        return this._ServiceContent.about.fullName;
      }
    }

    public bool IsVirtualCenter
    {
      get
      {
        return this._ServiceContent.about.productLineId.Equals("vpx", StringComparison.OrdinalIgnoreCase);
      }
    }

    public string ApiVersion
    {
      get
      {
        return this._ServiceContent.about.apiVersion;
      }
    }

    public ManagedObjectReference FileManager
    {
      get
      {
        return this._ServiceContent.fileManager;
      }
    }

    public ManagedObjectReference VirtualDiskManager
    {
      get
      {
        return this._ServiceContent.virtualDiskManager;
      }
    }

    public DateTime CurrentTime
    {
      get
      {
        return this._Service.CurrentTime(this._SvcRef);
      }
    }

    internal ManagedObjectReference DVSManager
    {
      get
      {
        return this._ServiceContent.dvSwitchManager;
      }
    }

    private ManagedObjectReference CustomizationSpecManager
    {
      get
      {
        return this._ServiceContent.customizationSpecManager;
      }
    }

    public ILogger Logger
    {
      get
      {
        return this._logger;
      }
    }

    internal VCService(ILogger tlLogger, string hostName, int port, string userName, string password, int nRetries)
    {
      this._init(tlLogger, hostName, port, userName, password, (ICredential) null, nRetries);
    }

    internal VCService(ILogger logger, string hostName, int port, string userName, string password)
    {
      this._init(logger, hostName, port, userName, password, (ICredential) null, 1);
    }

    internal VCService(ILogger logger, string hostName, int port, ICredential cred, int nRetries)
    {
      this._init(logger, hostName, port, (string) null, (string) null, cred, nRetries);
    }

    internal VCService(ILogger tlLogger, string hostName, int port, NetworkCredential cred)
    {
      this._init(tlLogger, hostName, port, CUtils.GetFullUserName(cred), cred.Password, (ICredential) null, 1);
    }

    private void _init(ILogger logger, string hostName, int port, string userName, string password, ICredential cred, int nRetries)
    {
      this._logger = logger;
      if ((string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password)) && cred == null)
        throw new Exception("Provide credentials for the selected vCenter.");
      string str = "https://" + (port > 0 ? CUtils.CombinServerNameAndPort(hostName, port) : hostName) + "/sdk";
      if (this._logger != null)
        this._logger.Verbose(string.Format("Initialized VimService URL as {0}.", (object) str));
      else
        VCService.tracer.TraceInformation("Initialized VimService URL as {0}.", (object) str);
      this._Service = new VimService();
      this._Service.Url = str;
      this._Service.CookieContainer = new CookieContainer();
      this._SvcRef = new ManagedObjectReference();
      this._SvcRef.type = "ServiceInstance";
      this._SvcRef.Value = "ServiceInstance";
      VMwareCertificatePolicy overallCertPolicy = VCService.GetOverallCertPolicy(logger, hostName);
      logger.LogInformation(string.Format("Using VMware certificate policy {0}.", (object) overallCertPolicy));
      lock (VCService._lockPolicyCert)
      {
        using (CertificatePoilcyOverride resource_0 = new CertificatePoilcyOverride(overallCertPolicy, (Func<object, bool>) (s => this._Service.RequestSoapContext.WebRequest == s), logger))
        {
          try
          {
            this._ServiceContent = this._Service.RetrieveServiceContent(this._SvcRef);
          }
          catch (Exception exception_0)
          {
            if (this.HandleRetrieveServiceContentException(exception_0, resource_0))
              throw;
          }
        }
      }
      this._PropCol = this._ServiceContent.propertyCollector;
      this._hostName = hostName;
      this._userName = string.IsNullOrEmpty(userName) ? cred.Username : userName;
      if (this._userName.Contains("@"))
      {
        string username;
        string domain;
        CUtils.SplitUsernameAndDomain(userName, out username, out domain);
        this._userName = domain + "\\" + username;
      }
      this._password = string.IsNullOrEmpty(password) ? cred.Password : password;
      this._credential = cred;
      this._nRetries = nRetries;
    }

    private bool HandleRetrieveServiceContentException(Exception e, CertificatePoilcyOverride overridePolicy)
    {
      if (e is WebException && WebExceptionStatus.TrustFailure == ((WebException) e).Status)
        throw new VMwareCertificateException(new X509Certificate2((X509Certificate) overridePolicy.FailedCertificate), e);
      return true;
    }

    public void Heartbeat()
    {
      this._Service.CurrentTime(this._SvcRef);
    }

    private void ThreadFunc()
    {
      while (!this._logginOff)
      {
        this._shutdownEvent.WaitOne(600000, false);
        if (!this._logginOff)
        {
          try
          {
            this._Service.CurrentTime(this._SvcRef);
          }
          catch (SoapException ex1)
          {
            if (ex1.Detail.InnerXml.Contains("xsi:type=\"NoPermission\""))
            {
              bool flag = false;
              while (!this._logginOff && !flag)
                flag = Monitor.TryEnter((object) this, 1000);
              if (!this._logginOff)
              {
                try
                {
                  this._LogOff();
                }
                catch (Exception ex2)
                {
                }
                try
                {
                  this._LogOn();
                }
                catch (Exception ex2)
                {
                }
              }
              if (flag)
                Monitor.Exit((object) this);
            }
          }
          catch (Exception ex)
          {
          }
        }
        else
          break;
      }
      if (this._logger == null)
        return;
      this._logger.Information("KeepAlive thread: terminating", "Vim");
    }

    public void Logon()
    {
      lock (this)
      {
        this._LogOn();
        this._shutdownEvent.Reset();
        this._keepAliveThread = new Thread(new ThreadStart(this.ThreadFunc));
        this._keepAliveThread.IsBackground = true;
        this._keepAliveThread.Start();
      }
    }

    private void _LogOn()
    {
      if (!this._loggedOn && !this._shutdown)
      {
        if (this._logger != null)
          this._logger.Verbose("Logging on to vim service at " + this._Service.Url, "Vim");
        else
          VCService.tracer.TraceInformation("Logging on to vim service at " + this._Service.Url);
        string userName;
        string password;
        if (this._credential != null)
        {
          userName = this._credential.Username;
          password = this._credential.Password;
        }
        else
        {
          userName = this._userName;
          password = this._password;
        }
        if (userName == null || password == null)
          throw new EsxException("VC Logon: user name or password is not set", false);
        this._Service.Login(this._ServiceContent.sessionManager, userName, password, (string) null);
        this._RootFolder = (IVimFolderOutsideDC) new FolderOutsideDC((IVimService) this, this._ServiceContent.rootFolder);
        this._RootFolder.Name = (string) this._RootFolder.GetProperties(new string[1]{ "name" })["name"];
        if (this._logger != null)
          this._logger.Verbose("Vim Logon: logged on successfully", "Vim");
        else
          VCService.tracer.TraceInformation("Vim Logon: logged on successfully");
      }
      this._loggedOn = true;
      this._logginOff = false;
      this._shutdown = false;
    }

    public void LogOff()
    {
      lock (this)
      {
        if (!this._loggedOn)
          return;
        if (this._shutdown)
          return;
        try
        {
          if (this._logger != null)
            this._logger.Information("Vim Logoff: joining KeepAlive thread", "Vim");
          this._logginOff = true;
          this._shutdownEvent.Set();
          this._keepAliveThread.Join();
          this._LogOff();
        }
        catch (Exception exception_0)
        {
        }
      }
    }

    private void _LogOff()
    {
      if (!this._loggedOn)
        return;
      if (this._logger != null)
        this._logger.Information("Vim Logoff: begin", "Vim");
      this._loggedOn = false;
      this._logginOff = false;
      try
      {
        this._Service.Logout(this._ServiceContent.sessionManager);
      }
      catch (Exception ex)
      {
      }
      if (this._logger == null)
        return;
      this._logger.Information("Vim Logoff: logged off successfully", "Vim");
    }

    public void Shutdown()
    {
      lock (this)
      {
        if (this._loggedOn)
          this.LogOff();
        this._Service.Dispose();
        this._Service = (VimService) null;
        this._ServiceContent = (ServiceContent) null;
        this._shutdown = true;
        this._loggedOn = false;
        this._logginOff = false;
      }
    }

    public IVimHost SearchHostByIP(string ip, bool retrieveCommonProperties)
    {
      IVimHost vimHost = (IVimHost) null;
      IPAddress address = (IPAddress) null;
      if (!IPAddress.TryParse(ip, out address))
        return vimHost;
      ManagedObjectReference byIp = this._Service.FindByIp(this._ServiceContent.searchIndex, (ManagedObjectReference) null, ip, false);
      if (byIp != null)
      {
        vimHost = (IVimHost) new Host((IVimService) this, byIp);
        if (retrieveCommonProperties)
        {
          ServerProperties commonProperties = vimHost.GetCommonProperties();
          vimHost.Name = commonProperties.Name;
        }
      }
      return vimHost;
    }

    public IVimHost SearchHostByDnsName(string dnsName, bool retrieveCommonProperties)
    {
      ManagedObjectReference byDnsName = this._Service.FindByDnsName(this._ServiceContent.searchIndex, (ManagedObjectReference) null, dnsName, false);
      IVimHost vimHost = (IVimHost) null;
      if (byDnsName != null)
      {
        vimHost = (IVimHost) new Host((IVimService) this, byDnsName);
        if (retrieveCommonProperties)
        {
          ServerProperties commonProperties = vimHost.GetCommonProperties();
          vimHost.Name = commonProperties.Name;
        }
      }
      return vimHost;
    }

    public IVimHost SearchHostByUuid(string uuid, bool retrieveCommonProperties)
    {
      ManagedObjectReference byUuid = this._Service.FindByUuid(this._ServiceContent.searchIndex, (ManagedObjectReference) null, uuid, false, false, false);
      IVimHost vimHost = (IVimHost) null;
      if (byUuid != null)
      {
        vimHost = (IVimHost) new Host((IVimService) this, byUuid);
        if (retrieveCommonProperties)
        {
          ServerProperties commonProperties = vimHost.GetCommonProperties();
          vimHost.Name = commonProperties.Name;
        }
      }
      return vimHost;
    }

    public IVimVm SearchVmByUuid(string uuid)
    {
      return this.SearchVmByUuid(uuid, true);
    }

    public IVimVm SearchVmByUuid(string uuid, bool retrieveCommonProperties)
    {
      ManagedObjectReference byUuid = this._Service.FindByUuid(this._ServiceContent.searchIndex, (ManagedObjectReference) null, uuid, true, false, false);
      IVimVm vimVm = (IVimVm) null;
      if (byUuid != null)
      {
        vimVm = (IVimVm) new Vm((IVimService) this, byUuid);
        if (retrieveCommonProperties)
        {
          VmProperties commonProperties = vimVm.GetCommonProperties();
          vimVm.Name = commonProperties.Name;
        }
      }
      if (vimVm == null)
        throw new NoSuchVmException("No such virtual machine: " + uuid);
      return vimVm;
    }

    public IVimVm SearchVmByDnsName(string dnsName)
    {
      return this.SearchVmByDnsName(dnsName, true);
    }

    public IVimVm SearchVmByDnsName(string dnsName, bool retrieveCommonProperties)
    {
      ManagedObjectReference byDnsName = this._Service.FindByDnsName(this._ServiceContent.searchIndex, (ManagedObjectReference) null, dnsName, true);
      IVimVm vimVm = (IVimVm) null;
      if (byDnsName != null)
      {
        vimVm = (IVimVm) new Vm((IVimService) this, byDnsName);
        if (retrieveCommonProperties)
        {
          VmProperties commonProperties = vimVm.GetCommonProperties();
          vimVm.Name = commonProperties.Name;
        }
      }
      return vimVm;
    }

    public IVimDatacenter[] GetDatacenters()
    {
      List<IVimDatacenter> datacenters = new List<IVimDatacenter>();
      this.getChildren(this.RootFolder, datacenters);
      return datacenters.ToArray();
    }

    public List<IVimDatastore> GetAllDatastores()
    {
      ObjectContent[] datastoresObjectContents = this.getAllDatastoresObjectContents();
      List<IVimDatastore> vimDatastoreList = new List<IVimDatastore>();
      if (datastoresObjectContents != null)
      {
        foreach (ObjectContent objectContent in datastoresObjectContents)
        {
          Dictionary<string, object> dictionary = this.PropSetToDictionary(objectContent.propSet);
          IVimDatastore vimDatastore = (IVimDatastore) new Datastore((IVimService) this, objectContent.obj);
          vimDatastore.GetCommonProperties(dictionary);
          vimDatastoreList.Add(vimDatastore);
        }
      }
      return vimDatastoreList;
    }

    public IVimDatastore GetDatastoreByUrl(string url)
    {
      IVimDatastore vimDatastore1 = (IVimDatastore) null;
      ObjectContent[] datastoresObjectContents = this.getAllDatastoresObjectContents();
      List<IVimDatastore> vimDatastoreList = new List<IVimDatastore>();
      if (datastoresObjectContents != null)
      {
        foreach (ObjectContent objectContent in datastoresObjectContents)
        {
          Dictionary<string, object> dictionary = this.PropSetToDictionary(objectContent.propSet);
          IVimDatastore vimDatastore2 = (IVimDatastore) new Datastore((IVimService) this, objectContent.obj);
          vimDatastore2.GetCommonProperties(dictionary);
          if (string.Compare(url, vimDatastore2.DsProperties.Url, true) == 0)
          {
            vimDatastore1 = vimDatastore2;
            break;
          }
        }
      }
      return vimDatastore1;
    }

    public IVimDatastore GetDatastoreByName(string name)
    {
      IVimDatastore vimDatastore1 = (IVimDatastore) null;
      ObjectContent[] datastoresObjectContents = this.getAllDatastoresObjectContents();
      List<IVimDatastore> vimDatastoreList = new List<IVimDatastore>();
      if (datastoresObjectContents != null)
      {
        foreach (ObjectContent objectContent in datastoresObjectContents)
        {
          Dictionary<string, object> dictionary = this.PropSetToDictionary(objectContent.propSet);
          IVimDatastore vimDatastore2 = (IVimDatastore) new Datastore((IVimService) this, objectContent.obj);
          vimDatastore2.GetCommonProperties(dictionary);
          if (string.Compare(name, vimDatastore2.Name, true) == 0)
          {
            vimDatastore1 = vimDatastore2;
            break;
          }
        }
      }
      return vimDatastore1;
    }

    private ObjectContent[] getAllDatastoresObjectContents()
    {
      PropertySpec propertySpec = new PropertySpec();
      propertySpec.type = "Datastore";
      propertySpec.all = false;
      propertySpec.pathSet = Datastore.VCProperties;
      TraversalSpec traversalSpec1 = new TraversalSpec();
      traversalSpec1.name = "dsFolderSpec";
      traversalSpec1.type = "Folder";
      traversalSpec1.path = "childEntity";
      traversalSpec1.skip = true;
      traversalSpec1.selectSet = new SelectionSpec[1]
      {
        new SelectionSpec()
      };
      traversalSpec1.selectSet[0].name = "dsFolderSpec";
      TraversalSpec traversalSpec2 = new TraversalSpec();
      traversalSpec2.name = "datacenterSpec";
      traversalSpec2.type = "Datacenter";
      traversalSpec2.path = "datastoreFolder";
      traversalSpec2.skip = false;
      traversalSpec2.selectSet = new SelectionSpec[2]
      {
        new SelectionSpec(),
        (SelectionSpec) traversalSpec1
      };
      traversalSpec2.selectSet[0].name = "datacenterSpec";
      TraversalSpec traversalSpec3 = new TraversalSpec();
      traversalSpec3.name = "tFolderSpec";
      traversalSpec3.type = "Folder";
      traversalSpec3.path = "childEntity";
      traversalSpec3.skip = true;
      traversalSpec3.selectSet = new SelectionSpec[2]
      {
        new SelectionSpec(),
        (SelectionSpec) traversalSpec2
      };
      traversalSpec3.selectSet[0].name = "tFolderSpec";
      return this.RetrieveProperties(new PropertyFilterSpec[1]{ new PropertyFilterSpec() { propSet = new PropertySpec[1]{ propertySpec }, objectSet = new ObjectSpec[1]{ new ObjectSpec() { obj = this.RootFolder.ManagedObject, skip = true, selectSet = new SelectionSpec[1]{ (SelectionSpec) traversalSpec3 } } } } });
    }

    public IVimHost[] GetHosts(IVimDatastore[] datastores)
    {
      Dictionary<ManagedObjectReference, Dictionary<string, object>> properties1 = this.GetProperties((IVimManagedItem[]) datastores, new string[1]{ "host" });
      Dictionary<string, ManagedObjectReference> dictionary1 = new Dictionary<string, ManagedObjectReference>((IEqualityComparer<string>) StringComparer.CurrentCultureIgnoreCase);
      foreach (Dictionary<string, object> dictionary2 in properties1.Values)
      {
        foreach (DatastoreHostMount datastoreHostMount in (DatastoreHostMount[]) dictionary2["host"])
        {
          if (!dictionary1.ContainsKey(datastoreHostMount.key.Value))
            dictionary1.Add(datastoreHostMount.key.Value, datastoreHostMount.key);
        }
      }
      Dictionary<ManagedObjectReference, Dictionary<string, object>> properties2 = this.GetProperties(CUtils.CollectionToArray<ManagedObjectReference>((ICollection<ManagedObjectReference>) dictionary1.Values), Host.VCProperties);
      List<IVimHost> vimHostList = new List<IVimHost>();
      foreach (ManagedObjectReference key in properties2.Keys)
      {
        try
        {
          Dictionary<string, object> hostProperties = properties2[key];
          IVimHost vimHost = (IVimHost) new Host((IVimService) this, key);
          vimHost.GetCommonProperties(hostProperties);
          vimHostList.Add(vimHost);
        }
        catch (Exception ex)
        {
        }
      }
      return vimHostList.ToArray();
    }

    public ObjectContent[] getObjectContents(ManagedObjectReference[] managedObjects, string[] properties)
    {
      if (managedObjects == null)
        return (ObjectContent[]) null;
      PropertyFilterSpec propertyFilterSpec = new PropertyFilterSpec();
      propertyFilterSpec.propSet = new PropertySpec[1]
      {
        new PropertySpec()
      };
      propertyFilterSpec.propSet[0].all = false;
      propertyFilterSpec.propSet[0].type = managedObjects[0].type;
      propertyFilterSpec.propSet[0].pathSet = properties;
      propertyFilterSpec.objectSet = new ObjectSpec[managedObjects.Length];
      for (int index = 0; index < managedObjects.Length; ++index)
      {
        propertyFilterSpec.objectSet[index] = new ObjectSpec();
        propertyFilterSpec.objectSet[index].skip = false;
        propertyFilterSpec.objectSet[index].obj = managedObjects[index];
      }
      return this.RetrieveProperties(new PropertyFilterSpec[1]{ propertyFilterSpec });
    }

    public Dictionary<ManagedObjectReference, Dictionary<string, object>> GetProperties(ManagedObjectReference[] managedObjects, string[] properties)
    {
      ObjectContent[] objectContents = this.getObjectContents(managedObjects, properties);
      Dictionary<ManagedObjectReference, Dictionary<string, object>> dictionary = new Dictionary<ManagedObjectReference, Dictionary<string, object>>();
      if (objectContents != null)
      {
        foreach (ObjectContent objectContent in objectContents)
        {
          DynamicProperty[] propSet = objectContent.propSet;
          ManagedObjectReference equivelentMor = this.getEquivelentMor(objectContent.obj.Value, managedObjects);
          dictionary.Add(equivelentMor, new Dictionary<string, object>((IEqualityComparer<string>) StringComparer.CurrentCultureIgnoreCase));
          if (propSet != null)
          {
            for (int index = 0; index < propSet.Length; ++index)
            {
              DynamicProperty dynamicProperty = propSet[index];
              if (dynamicProperty != null)
                dictionary[equivelentMor].Add(dynamicProperty.name, dynamicProperty.val);
            }
          }
        }
      }
      return dictionary;
    }

    private ManagedObjectReference getEquivelentMor(string morValue, ManagedObjectReference[] managedObjects)
    {
      ManagedObjectReference managedObjectReference = (ManagedObjectReference) null;
      foreach (ManagedObjectReference managedObject in managedObjects)
      {
        if (morValue == managedObject.Value)
        {
          managedObjectReference = managedObject;
          break;
        }
      }
      return managedObjectReference;
    }

    public Dictionary<ManagedObjectReference, Dictionary<string, object>> GetProperties(IVimManagedItem[] items, string[] properties)
    {
      if (items == null)
        return (Dictionary<ManagedObjectReference, Dictionary<string, object>>) null;
      return this.GetProperties(this.VCManagedItemsToMors(items), properties);
    }

    public ManagedObjectReference[] VCManagedItemsToMors(IVimManagedItem[] items)
    {
      if (items == null || items.Length == 0)
        return (ManagedObjectReference[]) null;
      ManagedObjectReference[] managedObjectReferenceArray = new ManagedObjectReference[items.Length];
      int index = 0;
      foreach (IVimManagedItem vimManagedItem in items)
      {
        managedObjectReferenceArray[index] = vimManagedItem.ManagedObject;
        ++index;
      }
      return managedObjectReferenceArray;
    }

    public void UnregisterVm(IVimVm vm)
    {
      this._Service.UnregisterVM(vm.ManagedObject);
    }

    public void UnregisterAndDestroyVm(IVimVm vm, VimClientlContext ctx)
    {
      new Task((IVimService) this, this.Service.Destroy_Task(vm.ManagedObject)).WaitForResult("DeleteVm", ctx);
    }

    public ObjectContent[] RetrieveProperties(PropertyFilterSpec[] pfSpec)
    {
      return CUtils.RetryIf<ObjectContent[]>(6, 5000, (Func<ObjectContent[]>) (() => this.Service.RetrieveProperties(this.PropertyCollector, pfSpec)), (Func<Exception, bool>) (ex =>
      {
        this._logger.Information(ex, "Exception retrieving properties: ", "Vim");
        return ex is WebException;
      }));
    }

    public IVimHost GetHost(string name)
    {
      return this.GetHost(name, true);
    }

    public IVimHost GetHost(string name, bool retrieveCommonProperties)
    {
      IVimHost vimHost1 = this.SearchHostByIP(name, retrieveCommonProperties);
      if (vimHost1 != null)
        return vimHost1;
      IVimHost vimHost2 = this.SearchHostByDnsName(name, retrieveCommonProperties);
      if (vimHost2 != null)
        return vimHost2;
      this.logInformationIfLoggerNotNullGetHost(string.Format("The ESX host: {0} could not be found by IP or DNS search - attempting case insensitive comparison.", (object) name));
      Dictionary<string, IVimHost> allHostsDict = this.GetAllHostsDict();
      if (allHostsDict.Count == 0)
      {
        this.logInformationIfLoggerNotNullGetHost("No hosts were found.");
        return (IVimHost) null;
      }
      if (allHostsDict.TryGetValue(name, out vimHost2))
        return vimHost2;
      this.logInformationIfLoggerNotNullGetHost(string.Format("We still couldn't find the host.  We did find these hosts though: {0}", (object) string.Join(", ", allHostsDict.Keys.ToArray<string>())));
      return (IVimHost) null;
    }

    private void logInformationIfLoggerNotNullGetHost(string message)
    {
      if (this._logger == null)
        return;
      this._logger.Information(message, "GetHost");
    }

    public IVimHost GetHostWithoutCaseInsensetiveComparison(string name)
    {
      IPAddress address;
      return !IPAddress.TryParse(name, out address) ? this.SearchHostByDnsName(name, false) : this.SearchHostByIP(name, false);
    }

    public Dictionary<string, IVimHost> GetAllHostsDict()
    {
      ObjectContent[] hostsObjectContents = this.GetAllHostsObjectContents();
      Dictionary<string, IVimHost> dictionary1 = new Dictionary<string, IVimHost>((IEqualityComparer<string>) StringComparer.CurrentCultureIgnoreCase);
      if (hostsObjectContents != null)
      {
        foreach (ObjectContent objectContent in hostsObjectContents)
        {
          try
          {
            Dictionary<string, object> dictionary2 = this.PropSetToDictionary(objectContent.propSet);
            Host host = new Host((IVimService) this, objectContent.obj);
            host.GetCommonProperties(dictionary2);
            if (!string.IsNullOrEmpty(host.Name))
            {
              if (!dictionary1.ContainsKey(host.Name))
                dictionary1.Add(host.Name, (IVimHost) host);
            }
          }
          catch (Exception ex)
          {
            if (this.Logger != null)
            {
              this.Logger.Error(string.Format("The ESX Managed Object: {0} does not have all the properties initialized", (object) objectContent.obj.Value), "GetAllHostsDict");
              this.Logger.Error(ex);
            }
          }
        }
      }
      return dictionary1;
    }

    public ObjectContent[] GetAllHostsObjectContents()
    {
      TraversalSpec traversalSpec1 = new TraversalSpec();
      traversalSpec1.name = "computeResourceHostTraversalSpec";
      traversalSpec1.type = "ComputeResource";
      traversalSpec1.path = "host";
      traversalSpec1.skip = false;
      TraversalSpec traversalSpec2 = new TraversalSpec();
      traversalSpec2.name = "datacenterHostTraversalSpec";
      traversalSpec2.type = "Datacenter";
      traversalSpec2.path = "hostFolder";
      traversalSpec2.skip = false;
      traversalSpec2.selectSet = new SelectionSpec[1]
      {
        new SelectionSpec()
      };
      traversalSpec2.selectSet[0].name = "folderTraversalSpec";
      TraversalSpec traversalSpec3 = new TraversalSpec();
      traversalSpec3.name = "folderTraversalSpec";
      traversalSpec3.type = "Folder";
      traversalSpec3.path = "childEntity";
      traversalSpec3.skip = false;
      traversalSpec3.selectSet = new SelectionSpec[3]
      {
        new SelectionSpec(),
        (SelectionSpec) traversalSpec2,
        (SelectionSpec) traversalSpec1
      };
      traversalSpec3.selectSet[0].name = "folderTraversalSpec";
      PropertySpec[] propertySpecArray = new PropertySpec[1]{ new PropertySpec() };
      propertySpecArray[0].all = false;
      propertySpecArray[0].pathSet = Host.VCProperties;
      propertySpecArray[0].type = "HostSystem";
      PropertyFilterSpec propertyFilterSpec = new PropertyFilterSpec();
      propertyFilterSpec.propSet = propertySpecArray;
      propertyFilterSpec.objectSet = new ObjectSpec[1]
      {
        new ObjectSpec()
      };
      propertyFilterSpec.objectSet[0].obj = this.RootFolder.ManagedObject;
      propertyFilterSpec.objectSet[0].skip = false;
      propertyFilterSpec.objectSet[0].selectSet = new SelectionSpec[1]
      {
        (SelectionSpec) traversalSpec3
      };
      return this.Service.RetrieveProperties(this.PropertyCollector, new PropertyFilterSpec[1]{ propertyFilterSpec });
    }

    private void getChildren(IVimFolderOutsideDC folder, List<IVimDatacenter> datacenters)
    {
      foreach (IVimManagedItem child in folder.GetChildren())
      {
        if (child.ManagedObject.type == "Datacenter")
        {
          Datacenter datacenter = new Datacenter((IVimService) this, child.ManagedObject);
          datacenters.Add((IVimDatacenter) child);
        }
        else
          this.getChildren((IVimFolderOutsideDC) child, datacenters);
      }
    }

    public Dictionary<string, object> PropSetToDictionary(DynamicProperty[] dynamicProperties)
    {
      Dictionary<string, object> dictionary = new Dictionary<string, object>((IEqualityComparer<string>) StringComparer.CurrentCultureIgnoreCase);
      if (dynamicProperties != null)
      {
        for (int index = 0; index < dynamicProperties.Length; ++index)
        {
          DynamicProperty dynamicProperty = dynamicProperties[index];
          if (dynamicProperty != null)
            dictionary.Add(dynamicProperty.name, dynamicProperty.val);
        }
      }
      return dictionary;
    }

    public static string GetRemoteVMToolsDir(string server, string username, string password)
    {
      return WMIUtils.GetRemoteRegistryValueString(WMIUtils.ConnectToServerDefaultPath(server, username, password), "SOFTWARE\\VMware, Inc.\\VMware Tools", "InstallPath");
    }

    public static string GetVMToolsDir()
    {
      return (string) Registry.LocalMachine.OpenSubKey("SOFTWARE\\VMware, Inc.\\VMware Tools").GetValue("InstallPath");
    }

    public int VC_CPU_Load()
    {
      string username;
      string password;
      if (this._credential != null)
      {
        username = this._credential.Username;
        password = this._credential.Password;
      }
      else
      {
        username = this._userName;
        password = this._password;
      }
      return (int) WMIUtils.GetProcessInfo(this._hostName, username, password, "vpxd").PercentCPU.Value;
    }

    public List<string> SearchDatastoreSubFolder(string esxHost, string folderName, VimClientlContext ctx)
    {
      return this.GetHost(esxHost).SearchDatastoreSubFolder(folderName, ctx);
    }

    public Dictionary<string, IVimVm> GetAllVMsDictWithUuid()
    {
      ObjectContent[] vmsObjectContents = this.GetAllVMsObjectContents();
      Dictionary<string, IVimVm> dictionary1 = new Dictionary<string, IVimVm>((IEqualityComparer<string>) StringComparer.CurrentCultureIgnoreCase);
      if (vmsObjectContents != null)
      {
        foreach (ObjectContent objectContent in vmsObjectContents)
        {
          try
          {
            Dictionary<string, object> dictionary2 = this.PropSetToDictionary(objectContent.propSet);
            IVimVm vimVm = (IVimVm) new Vm((IVimService) this, objectContent.obj);
            vimVm.GetCommonProperties(dictionary2);
            if (!string.IsNullOrEmpty(vimVm.Uuid))
            {
              if (!vimVm.VMProperties.IsTemplate)
              {
                if (!dictionary1.ContainsKey(vimVm.Uuid))
                  dictionary1.Add(vimVm.Uuid, vimVm);
              }
            }
          }
          catch (Exception ex)
          {
          }
        }
      }
      return dictionary1;
    }

    public Dictionary<string, IVimVm> GetAllVMsDictWithName()
    {
      VCService.tracer.TraceInformation("Retrieving all VMs from the host.");
      ObjectContent[] vmsObjectContents = this.GetAllVMsObjectContents();
      Dictionary<string, IVimVm> dictionary1 = new Dictionary<string, IVimVm>((IEqualityComparer<string>) StringComparer.CurrentCultureIgnoreCase);
      if (vmsObjectContents != null)
      {
        foreach (ObjectContent objectContent in vmsObjectContents)
        {
          try
          {
            Dictionary<string, object> dictionary2 = this.PropSetToDictionary(objectContent.propSet);
            IVimVm vimVm = (IVimVm) new Vm((IVimService) this, objectContent.obj);
            vimVm.GetCommonProperties(dictionary2);
            if (!string.IsNullOrEmpty(vimVm.Name))
            {
              if (!vimVm.VMProperties.IsTemplate)
              {
                if (!dictionary1.ContainsKey(vimVm.Name))
                  dictionary1.Add(vimVm.Name, vimVm);
              }
            }
          }
          catch (Exception ex)
          {
          }
        }
      }
      VCService.tracer.TraceInformation("Finished retrieving VMs.");
      return dictionary1;
    }

    private ObjectContent[] GetAllVMsObjectContents()
    {
      PropertySpec propertySpec = new PropertySpec();
      propertySpec.type = "VirtualMachine";
      propertySpec.all = false;
      propertySpec.pathSet = Vm.VCProperties;
      TraversalSpec traversalSpec1 = new TraversalSpec();
      traversalSpec1.name = "resourcePoolSpec";
      traversalSpec1.type = "VirtualApp";
      traversalSpec1.path = "vm";
      traversalSpec1.skip = true;
      traversalSpec1.selectSet = new SelectionSpec[1]
      {
        new SelectionSpec()
      };
      traversalSpec1.selectSet[0].name = "resourcePoolSpec";
      TraversalSpec traversalSpec2 = new TraversalSpec();
      traversalSpec2.name = "vAppSpec";
      traversalSpec2.type = "VirtualApp";
      traversalSpec2.path = "vm";
      traversalSpec2.skip = true;
      traversalSpec2.selectSet = new SelectionSpec[2]
      {
        new SelectionSpec(),
        (SelectionSpec) traversalSpec1
      };
      traversalSpec2.selectSet[0].name = "vAppSpec";
      TraversalSpec traversalSpec3 = new TraversalSpec();
      traversalSpec3.name = "vmFolderSpec";
      traversalSpec3.type = "Folder";
      traversalSpec3.path = "childEntity";
      traversalSpec3.skip = true;
      traversalSpec3.selectSet = new SelectionSpec[2]
      {
        new SelectionSpec(),
        (SelectionSpec) traversalSpec2
      };
      traversalSpec3.selectSet[0].name = "vmFolderSpec";
      TraversalSpec traversalSpec4 = new TraversalSpec();
      traversalSpec4.name = "datacenterSpec";
      traversalSpec4.type = "Datacenter";
      traversalSpec4.path = "vmFolder";
      traversalSpec4.skip = false;
      traversalSpec4.selectSet = new SelectionSpec[2]
      {
        new SelectionSpec(),
        (SelectionSpec) traversalSpec3
      };
      traversalSpec4.selectSet[0].name = "datacenterSpec";
      TraversalSpec traversalSpec5 = new TraversalSpec();
      traversalSpec5.name = "tFolderSpec";
      traversalSpec5.type = "Folder";
      traversalSpec5.path = "childEntity";
      traversalSpec5.skip = true;
      traversalSpec5.selectSet = new SelectionSpec[2]
      {
        new SelectionSpec(),
        (SelectionSpec) traversalSpec4
      };
      traversalSpec5.selectSet[0].name = "tFolderSpec";
      return this.RetrieveProperties(new PropertyFilterSpec[1]{ new PropertyFilterSpec() { propSet = new PropertySpec[1]{ propertySpec }, objectSet = new ObjectSpec[1]{ new ObjectSpec() { obj = this.RootFolder.ManagedObject, skip = true, selectSet = new SelectionSpec[1]{ (SelectionSpec) traversalSpec5 } } } } });
    }

    public IVimVm GetVmOrVmTemplate(string name)
    {
      IVimVm vimVm1 = (IVimVm) null;
      ObjectContent[] vmsObjectContents = this.GetAllVMsObjectContents();
      Dictionary<string, IVimVm> dictionary1 = new Dictionary<string, IVimVm>((IEqualityComparer<string>) StringComparer.CurrentCultureIgnoreCase);
      if (vmsObjectContents != null)
      {
        foreach (ObjectContent objectContent in vmsObjectContents)
        {
          try
          {
            Dictionary<string, object> dictionary2 = this.PropSetToDictionary(objectContent.propSet);
            IVimVm vimVm2 = (IVimVm) new Vm((IVimService) this, objectContent.obj);
            vimVm2.GetCommonProperties(dictionary2);
            if (string.Compare(vimVm2.Name, name, true) == 0)
            {
              vimVm1 = vimVm2;
              break;
            }
          }
          catch (Exception ex)
          {
          }
        }
      }
      return vimVm1;
    }

    public Dictionary<string, InventoryNode> GetVmInventory()
    {
      return new BrowseForVmInventory((IVimService) this).LoadInventory();
    }

    public Dictionary<string, InventoryNode> GetHostInventory()
    {
      return new BrowseForESXInventory((IVimService) this).LoadInventory();
    }

    public InventoryNode GetRootFolderOfInventory(Dictionary<string, InventoryNode> inventory)
    {
      foreach (InventoryNode inventoryNode in inventory.Values)
      {
        if (this.RootFolder.ManagedObject.Value == inventoryNode.ManagedObject.Value)
          return inventoryNode;
      }
      return (InventoryNode) null;
    }

    public IVimFolderOutsideDC GetFolderOutsideDC(ManagedObjectReference managedObject)
    {
      return (IVimFolderOutsideDC) new FolderOutsideDC((IVimService) this, managedObject);
    }

    public IVimFolderInsideDC GetFolderInsideDC(ManagedObjectReference managedObject)
    {
      return (IVimFolderInsideDC) new FolderInsideDC((IVimService) this, managedObject);
    }

    public IVimVm GetVm(ManagedObjectReference managedObject)
    {
      return (IVimVm) new Vm((IVimService) this, managedObject);
    }

    public IVimHost GetHostManagedItem(ManagedObjectReference managedObject)
    {
      return (IVimHost) new Host((IVimService) this, managedObject);
    }

    public IVimDatacenter GetDatacenter(ManagedObjectReference managedObject)
    {
      return (IVimDatacenter) new Datacenter((IVimService) this, managedObject);
    }

    public static string BuildDiskName(string serverName, string diskName)
    {
      return serverName + "_" + diskName + ".vmdk";
    }

    public static string GetVolumeName(string volName)
    {
      return string.IsNullOrEmpty(volName) ? "[Local] " : "[" + volName + "] ";
    }

    public CustomizationSpecItem GetCustomizationSpec(string name)
    {
      return this._Service.GetCustomizationSpec(this.CustomizationSpecManager, name);
    }

    public static string GetCertPolicySettingName(string server = null)
    {
      if (!string.IsNullOrEmpty(server))
        return string.Format("vmwareServerCertificatePolicy-{0}", (object) server);
      return "vmwareServerCertificatePolicy";
    }

    public static VMwareCertificatePolicy GetOverallCertPolicy(ILogger logger, string server)
    {
      VMwareCertificatePolicy? nullable1 = new VMwareCertificatePolicy?();
      try
      {
        VMwareCertificatePolicy? nullable2 = new VMwareCertificatePolicy?();
        VMwareCertificatePolicy? userCertPolicy;
        if (!(userCertPolicy = VCService.GetUserCertPolicy(logger, server)).HasValue && !(userCertPolicy = VCService.GetUserCertPolicy(logger, (string) null)).HasValue)
          VCService.tracer.TraceInformation(string.Format("User setting {0} not found.", (object) VCService.GetCertPolicySettingName((string) null)));
        else
          nullable1 = new VMwareCertificatePolicy?(userCertPolicy.Value);
      }
      catch (Exception ex)
      {
        logger.LogWarning(ex, string.Format("Error obtaining user setting {0}.", (object) VCService.GetCertPolicySettingName((string) null)), new object[0]);
      }
      if (!nullable1.HasValue)
      {
        VMwareCertificatePolicy result;
        if (!System.Enum.TryParse<VMwareCertificatePolicy>(ConfigurationManager.AppSettings[VCService.GetCertPolicySettingName(server)], out result) && !System.Enum.TryParse<VMwareCertificatePolicy>(ConfigurationManager.AppSettings[VCService.GetCertPolicySettingName((string) null)], out result))
          VCService.tracer.TraceInformation(string.Format("Application setting {0} not found.", (object) VCService.GetCertPolicySettingName((string) null)));
        else
          nullable1 = new VMwareCertificatePolicy?(result);
      }
      if (nullable1.HasValue)
        return nullable1.Value;
      logger.FormatInformation(string.Format("{0} setting invalid or not found.", (object) VCService.GetCertPolicySettingName((string) null)));
      return VMwareCertificatePolicy.AllowValid;
    }

    public static VMwareCertificatePolicy? GetUserCertPolicy(ILogger logger, string server = null)
    {
      ConfigurationSectionGroup sectionGroup = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal).GetSectionGroup("userSettings");
      ClientSettingsSection clientSettingsSection = (sectionGroup != null ? sectionGroup.Sections.Get("OculiService.Console.Properties.Settings") : (ConfigurationSection) null) as ClientSettingsSection;
      if (clientSettingsSection == null)
      {
        VCService.tracer.TraceInformation("Missing group or section.");
        return new VMwareCertificatePolicy?();
      }
      string policySettingName = VCService.GetCertPolicySettingName(server);
      SettingElement settingElement = clientSettingsSection.Settings.Get(policySettingName);
      if (settingElement == null)
      {
        VCService.tracer.TraceInformation(string.Format("{0} setting not found.", (object) policySettingName));
        return new VMwareCertificatePolicy?();
      }
      VMwareCertificatePolicy result;
      if (!System.Enum.TryParse<VMwareCertificatePolicy>(settingElement.Value.ValueXml.InnerText, out result))
      {
        logger.LogWarning(string.Format("{0} setting invalid.", (object) policySettingName));
        return new VMwareCertificatePolicy?();
      }
      if (server == null)
        VCService.tracer.TraceInformation(string.Format("Found user VMware certificate policy {0}.", (object) result));
      else
        VCService.tracer.TraceInformation(string.Format("Found user VMware certificate policy {0} for {1}.", (object) result, (object) server));
      return new VMwareCertificatePolicy?(result);
    }

    public static void SetUserCertPolicy(string server, VMwareCertificatePolicy? policy, ILogger logger)
    {
      logger.LogInformation("Setting user VMware certificate policy for {0} to {1}.", new object[2]
      {
        (object) server,
        (object) (!policy.HasValue ? "default" : policy.Value.ToString())
      });
      System.Configuration.Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal);
      ConfigurationSectionGroup sectionGroup = configuration.GetSectionGroup("userSettings");
      ClientSettingsSection clientSettingsSection = (sectionGroup != null ? sectionGroup.Sections.Get("OculiService.Console.Properties.Settings") : (ConfigurationSection) null) as ClientSettingsSection;
      if (clientSettingsSection == null)
        throw new InvalidOperationException("Missing group or section");
      string policySettingName = VCService.GetCertPolicySettingName(server);
      SettingElement element = clientSettingsSection.Settings.Get(policySettingName);
      if (policy.HasValue)
      {
        if (element == null)
        {
          VCService.tracer.TraceInformation(string.Format("{0} setting not found, adding.", (object) policySettingName));
          element = new SettingElement(policySettingName, SettingsSerializeAs.String);
          element.Value = new SettingValueElement()
          {
            ValueXml = (XmlNode) new XmlDocument().CreateElement("value")
          };
          clientSettingsSection.Settings.Add(element);
        }
        VCService.tracer.TraceInformation(string.Format("Setting {0}.", (object) policySettingName));
        element.Value.ValueXml.InnerText = ((int) policy.Value).ToString();
      }
      else
      {
        if (element == null)
          return;
        VCService.tracer.TraceInformation(string.Format("Removing {0}.", (object) policySettingName));
        clientSettingsSection.Settings.Remove(element);
      }
      configuration.Save();
      ConfigurationManager.RefreshSection(clientSettingsSection.SectionInformation.SectionName);
    }

    public static void InstallCertificate(X509Certificate2 certificate)
    {
      X509Store certificateStore = VCService.GetCertificateStore();
      try
      {
        certificateStore.Open(OpenFlags.ReadWrite);
        certificateStore.Add(certificate);
      }
      finally
      {
        certificateStore.Close();
      }
    }

    public static bool IsCertificateNeeded(string server, ILogger logger, out byte[] certificate, out string error)
    {
      VMwareCertificatePolicy overallCertPolicy = VCService.GetOverallCertPolicy(logger, server);
      logger.LogInformation(string.Format("Using certificate policy {0}.", (object) overallCertPolicy));
      HttpWebRequest request = (HttpWebRequest) null;
      using (CertificatePoilcyOverride certificatePoilcyOverride = new CertificatePoilcyOverride(overallCertPolicy, (Func<object, bool>) (s => request == s), logger))
      {
        try
        {
          request = WebRequest.CreateHttp(string.Format("https://{0}", (object) server));
          using (request.GetResponse())
            ;
        }
        catch (Exception ex)
        {
          if (ex is WebException && WebExceptionStatus.TrustFailure == ((WebException) ex).Status)
          {
            logger.LogWarning(ex, "Certificate issue", new object[0]);
            certificate = certificatePoilcyOverride.FailedCertificate.GetRawCertData();
            error = ex.Message;
            return true;
          }
          logger.LogWarning(ex, "Other issue", new object[0]);
        }
        certificate = (byte[]) null;
        error = (string) null;
        return false;
      }
    }

    internal static X509Store GetCertificateStore()
    {
      return new X509Store(StoreName.My, WindowsIdentity.GetCurrent().IsSystem ? StoreLocation.LocalMachine : StoreLocation.CurrentUser);
    }
  }
}
