using OculiService.Common.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading;
using VimApi;

namespace OculiService.CloudProviders.VMware
{
  public abstract class VCManagedItem : IVimManagedItem
  {
    private ManagedObjectReference _managedObject;
    private string _Name;
    protected IVimService VcService;

    public ManagedObjectReference ManagedObject
    {
      get
      {
        return this._managedObject;
      }
      set
      {
        this._managedObject = value;
      }
    }

    public string Name
    {
      get
      {
        return this._Name;
      }
      set
      {
        this._Name = value;
      }
    }

    public DateTime CurrentTime
    {
      get
      {
        return this.VcService.CurrentTime;
      }
    }

    internal VCManagedItem(IVimService service, ManagedObjectReference managedObject)
    {
      this.VcService = service;
      this._managedObject = managedObject;
    }

    public virtual IVimManagedItem[] GetChildren()
    {
      return (IVimManagedItem[]) null;
    }

    public Dictionary<string, object> GetProperties(string[] properties)
    {
      return this.VcService.GetProperties(new ManagedObjectReference[1]{ this._managedObject }, properties)[this._managedObject];
    }

    public object GetProperty(string property)
    {
      object obj = (object) null;
      for (int index = 0; index < 3; ++index)
      {
        Dictionary<ManagedObjectReference, Dictionary<string, object>> properties = this.VcService.GetProperties(new ManagedObjectReference[1]{ this._managedObject }, new string[1]{ property });
        if (properties[this._managedObject] != null && properties[this._managedObject].ContainsKey(property))
        {
          obj = properties[this._managedObject][property];
          break;
        }
        Thread.Sleep(2000);
      }
      return obj;
    }

    protected ManagedObjectReference[] GetManagedObjects(string[] properties)
    {
      return this.GetManagedObjects(this._managedObject, properties);
    }

    protected ManagedObjectReference[] GetManagedObjects(ManagedObjectReference mor, string[] properties)
    {
      ObjectContent[] objectContents = this.getObjectContents(mor, properties);
      List<ManagedObjectReference> managedObjectReferenceList = new List<ManagedObjectReference>();
      if (objectContents != null && objectContents.Length != 0)
      {
        for (int index1 = 0; index1 < objectContents.Length; ++index1)
        {
          ObjectContent objectContent = objectContents[index1];
          if (objectContent != null)
          {
            DynamicProperty[] propSet = objectContent.propSet;
            if (propSet != null)
            {
              for (int index2 = 0; index2 < propSet.Length; ++index2)
              {
                if (!propSet[index2].val.GetType().IsArray)
                {
                  ManagedObjectReference val = (ManagedObjectReference) propSet[index2].val;
                  managedObjectReferenceList.Add(val);
                }
                else
                {
                  Array val = (Array) propSet[index2].val;
                  for (int index3 = 0; index3 < val.Length; ++index3)
                  {
                    ManagedObjectReference managedObjectReference = (ManagedObjectReference) val.GetValue(index3);
                    managedObjectReferenceList.Add(managedObjectReference);
                  }
                }
              }
            }
          }
        }
      }
      return managedObjectReferenceList.ToArray();
    }

    protected ObjectContent[] getObjectContents(ManagedObjectReference managedObject, string[] properties)
    {
      return this.VcService.getObjectContents(new ManagedObjectReference[1]{ managedObject }, properties);
    }

    public virtual string GetName()
    {
      return (string) null;
    }

    public object[] WaitForValues(VimClientlContext rstate, string[] filterProps, string[] endWaitProps, object[][] expectedVals)
    {
      ILogger logger = ((VCService) this.VcService).Logger;
      if (logger != null)
        logger.Verbose("WaitForValues begin", "Task");
      string version = "";
      object[] vals1 = new object[endWaitProps.Length];
      object[] vals2 = new object[filterProps.Length];
      PropertyFilterSpec spec = new PropertyFilterSpec();
      spec.objectSet = new ObjectSpec[1]
      {
        new ObjectSpec()
      };
      spec.objectSet[0].obj = this._managedObject;
      spec.propSet = new PropertySpec[1]
      {
        new PropertySpec()
      };
      spec.propSet[0].pathSet = filterProps;
      spec.propSet[0].type = this._managedObject.type;
      spec.objectSet[0].selectSet = (SelectionSpec[]) null;
      spec.objectSet[0].skip = false;
      spec.objectSet[0].skipSpecified = true;
      ManagedObjectReference filter1 = this.VcService.Service.CreateFilter(this.VcService.PropertyCollector, spec, true);
      bool flag = false;
      ObjectUpdate objectUpdate1 = (ObjectUpdate) null;
      PropertyChange[] propertyChangeArray = (PropertyChange[]) null;
      if (logger != null)
        logger.Verbose("wait loop begin", "Task");
      Stopwatch stopwatch = new Stopwatch();
      stopwatch.Start();
      long elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
      int num = 0;
label_33:
      while (!flag)
      {
        if (rstate.IsVimClientStopping())
          throw new EsxAbortException();
        if (stopwatch.ElapsedMilliseconds > (long) (rstate.TimeoutSec * 1000))
          throw new EsxException("WaitForValues: Operation timed out", true);
        UpdateSet updateSet;
        try
        {
          updateSet = this.VcService.Service.WaitForUpdates(this.VcService.PropertyCollector, version);
        }
        catch (WebException ex)
        {
          if (ex.Status != WebExceptionStatus.Timeout)
            throw;
          else
            continue;
        }
        version = updateSet.version;
        if (updateSet != null && updateSet.filterSet != null)
        {
          foreach (PropertyFilterUpdate filter2 in updateSet.filterSet)
          {
            ObjectUpdate[] objectSet = filter2.objectSet;
            if (objectSet != null)
            {
              objectUpdate1 = (ObjectUpdate) null;
              propertyChangeArray = (PropertyChange[]) null;
              for (int index = 0; index < objectSet.Length; ++index)
              {
                ObjectUpdate objectUpdate2 = objectSet[index];
                if (objectUpdate2.kind == ObjectUpdateKind.modify || objectUpdate2.kind == ObjectUpdateKind.enter || objectUpdate2.kind == ObjectUpdateKind.leave)
                {
                  foreach (PropertyChange change in objectUpdate2.changeSet)
                  {
                    this.UpdateValues(endWaitProps, vals1, change);
                    this.UpdateValues(filterProps, vals2, change);
                    if (change.name.Equals("info.progress") && change.val != null && (logger != null && stopwatch.ElapsedMilliseconds - elapsedMilliseconds > 60000L))
                    {
                      string s = change.val.ToString();
                      int result = 0;
                      if (int.TryParse(s, out result) && result > num)
                      {
                        logger.Verbose(string.Format("Task is {0}% complete", (object) s, (object) "Task"));
                        elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
                        num = result;
                      }
                    }
                  }
                }
              }
            }
          }
          int index1 = 0;
          while (true)
          {
            if (index1 < vals1.Length && !flag)
            {
              for (int index2 = 0; index2 < expectedVals[index1].Length && !flag; ++index2)
                flag = expectedVals[index1][index2].Equals(vals1[index1]) | flag;
              ++index1;
            }
            else
              goto label_33;
          }
        }
      }
      if (logger != null)
        logger.Verbose("wait end", "Task");
      for (int index = 0; index < 2; ++index)
      {
        try
        {
          this.VcService.Service.DestroyPropertyFilter(filter1);
          break;
        }
        catch (Exception ex)
        {
        }
      }
      if (logger != null)
        logger.Verbose("wait loop end", "Task");
      return vals2;
    }

    protected void UpdateValues(string[] props, object[] vals, PropertyChange propchg)
    {
      for (int index = 0; index < props.Length; ++index)
      {
        if (propchg.name.LastIndexOf(props[index]) >= 0)
          vals[index] = propchg.op != PropertyChangeOp.remove ? propchg.val : (object) "";
      }
    }

    protected ManagedObjectAndProperties[] GetManagedObjectAndProperties(ManagedObjectReference managedObject, string path, string childType, string[] childProperties)
    {
      Dictionary<string, object> propertyValues;
      return this.GetManagedObjectAndProperties(managedObject, path, childType, childProperties, (string[]) null, out propertyValues);
    }

    protected ManagedObjectAndProperties[] GetManagedObjectAndProperties(ManagedObjectReference managedObject, string path, string childType, string[] childProperties, string[] properties, out Dictionary<string, object> propertyValues)
    {
      PropertySpec propertySpec1 = new PropertySpec();
      propertySpec1.type = this._managedObject.type;
      propertySpec1.all = false;
      propertySpec1.pathSet = properties;
      PropertySpec propertySpec2 = new PropertySpec();
      propertySpec2.type = childType;
      propertySpec2.all = false;
      propertySpec2.pathSet = childProperties;
      ObjectSpec objectSpec = new ObjectSpec();
      objectSpec.obj = managedObject;
      objectSpec.skip = false;
      objectSpec.selectSet = new SelectionSpec[1]
      {
        (SelectionSpec) new TraversalSpec()
        {
          type = managedObject.type,
          path = path
        }
      };
      PropertyFilterSpec propertyFilterSpec = new PropertyFilterSpec();
      if (properties != null)
        propertyFilterSpec.propSet = new PropertySpec[2]
        {
          propertySpec1,
          propertySpec2
        };
      else
        propertyFilterSpec.propSet = new PropertySpec[1]
        {
          propertySpec2
        };
      propertyFilterSpec.objectSet = new ObjectSpec[1]
      {
        objectSpec
      };
      List<ManagedObjectAndProperties> objectAndPropertiesList = new List<ManagedObjectAndProperties>();
      ObjectContent[] objectContentArray = this.VcService.RetrieveProperties(new PropertyFilterSpec[1]{ propertyFilterSpec });
      propertyValues = (Dictionary<string, object>) null;
      if (objectContentArray != null)
      {
        foreach (ObjectContent objectContent in objectContentArray)
        {
          if (objectContent != null)
          {
            if (objectContent.obj.Value == this._managedObject.Value)
              propertyValues = this.VcService.PropSetToDictionary(objectContent.propSet);
            else
              objectAndPropertiesList.Add(new ManagedObjectAndProperties()
              {
                ManagedObject = objectContent.obj,
                Properties = this.VcService.PropSetToDictionary(objectContent.propSet)
              });
          }
        }
      }
      return objectAndPropertiesList.ToArray();
    }

    public void InitializeManagedObject()
    {
      if (this._managedObject != null)
        return;
      IVimHost host = this.VcService.GetHost(this.Name, false);
      if (host == null)
        return;
      this._managedObject = host.ManagedObject;
    }

    protected string GetVolumeName(string volName)
    {
      return string.IsNullOrEmpty(volName) ? "[Local]" : "[" + volName + "]";
    }
  }
}
