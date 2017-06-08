using System;using System.Collections.Generic;
using System.ComponentModel;

namespace OculiService.Common.ExceptionHandling
{
  public class ExceptionPolicy
  {
    private static readonly Dictionary<string, ExceptionPolicy> policies = new Dictionary<string, ExceptionPolicy>();
    private readonly Dictionary<Type, IExceptionHandler> handlers = new Dictionary<Type, IExceptionHandler>();

    private ExceptionPolicy()
    {
    }

    public static bool HandleException(Exception exception, string policyName)
    {
      if (exception == null)
        throw new ArgumentNullException("exception");
      if (string.IsNullOrEmpty("policyName"))
        throw new ArgumentException("policyName can not be null or empty");
      AggregateException aggregateException = exception as AggregateException;
      if (aggregateException != null)
      {
        try
        {
          aggregateException.Handle((Func<Exception, bool>) (e => !ExceptionPolicy.HandleException(e, policyName)));
        }
        catch (AggregateException ex)
        {
          return true;
        }
        return false;
      }
      lock (ExceptionPolicy.policies)
      {
        ExceptionPolicy local_5 = ExceptionPolicy.FindPolicy(policyName);
        if (local_5 == null)
          return true;
        return local_5.HandleException(exception);
      }
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void AddDefaultHandler<TException>(IExceptionHandler handler) where TException : Exception
    {
      ExceptionPolicy.AddHandlerInternal<TException>(string.Empty, handler);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void AddDefaultHandler<TException>(Func<Exception, bool> handler) where TException : Exception
    {
      ExceptionPolicy.AddHandlerInternal<TException>(string.Empty, (IExceptionHandler) new DelegateExceptionHandler(handler));
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void AddHandler<TException>(string policyName, IExceptionHandler handler) where TException : Exception
    {
      ExceptionPolicy.AddHandlerInternal<TException>(policyName, handler);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void AddHandler<TException>(string policyName, Func<Exception, bool> handler) where TException : Exception
    {
      ExceptionPolicy.AddHandlerInternal<TException>(policyName, (IExceptionHandler) new DelegateExceptionHandler(handler));
    }

    private static void AddHandlerInternal<TException>(string policyName, IExceptionHandler handler) where TException : Exception
    {
      lock (ExceptionPolicy.policies)
      {
        ExceptionPolicy local_2 = ExceptionPolicy.FindPolicy(policyName);
        if (local_2 == null)
        {
          local_2 = new ExceptionPolicy();
          ExceptionPolicy.policies.Add(policyName, local_2);
        }
        local_2.AddHandler(typeof (TException), handler);
      }
    }

    private static ExceptionPolicy FindPolicy(string policyName)
    {
      ExceptionPolicy exceptionPolicy;
      if (!ExceptionPolicy.policies.TryGetValue(policyName, out exceptionPolicy) && (string.IsNullOrEmpty(policyName) || !ExceptionPolicy.policies.TryGetValue(string.Empty, out exceptionPolicy)))
        return (ExceptionPolicy) null;
      return exceptionPolicy;
    }

    private bool HandleException(Exception exception)
    {
      IExceptionHandler handler = this.FindHandler(exception);
      if (handler != null)
        return handler.HandleException(exception);
      ExceptionPolicy policy = ExceptionPolicy.FindPolicy(string.Empty);
      if (policy == null || policy == this)
        return true;
      return policy.HandleException(exception);
    }

    private void AddHandler(Type exceptionType, IExceptionHandler handler)
    {
      this.handlers.Add(exceptionType, handler);
    }

    private IExceptionHandler FindHandler(Exception exception)
    {
      for (Type key = exception.GetType(); key != typeof (object); key = key.BaseType)
      {
        IExceptionHandler exceptionHandler;
        if (this.handlers.TryGetValue(key, out exceptionHandler))
          return exceptionHandler;
      }
      return (IExceptionHandler) null;
    }
  }
}
