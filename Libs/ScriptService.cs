using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace ScriptContainer
{
  /// <summary>
  /// Singleton service
  /// </summary>
  public class ScriptService : IAsyncDisposable
  {
    /// <summary>
    /// Script runtime
    /// </summary>
    private IJSRuntime _runtime;

    /// <summary>
    /// Script reference
    /// </summary>
    private IJSObjectReference _scriptModule;

    /// <summary>
    /// Script instance 
    /// </summary>
    private IJSObjectReference _scriptInstance;

    /// <summary>
    /// Service instance 
    /// </summary>
    private DotNetObjectReference<ScriptService> _serviceInstance;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="runtime"></param>
    public ScriptService(IJSRuntime runtime) => _runtime = runtime;

    /// <summary>
    /// On size event
    /// </summary>
    public Action<dynamic, int, string> OnChange { get; set; } = (o, i, action) => { };

    /// <summary>
    /// Get document bounds
    /// </summary>
    /// <returns></returns>
    public async Task<ScriptMessage?> GetDocBounds()
    {
      if (_scriptInstance is not null)
      {
        return await _scriptInstance.InvokeAsync<ScriptMessage>("getDocBounds");
      }

      return null;
    }

    /// <summary>
    /// Get element bounds
    /// </summary>
    /// <param name="element"></param>
    /// <returns></returns>
    public async Task<ScriptMessage?> GetElementBounds(ElementReference element)
    {
      if (_scriptInstance is not null)
      {
        return await _scriptInstance.InvokeAsync<ScriptMessage>("getElementBounds", element);
      }

      return null;
    }

    /// <summary>
    /// Setup script proxy under specified namespace
    /// </summary>
    /// <returns></returns>
    public async Task<ScriptService> CreateModule(IDictionary<string, object> options = null)
    {
      await DisposeAsync();

      options ??= new Dictionary<string, object>();

      _serviceInstance = DotNetObjectReference.Create(this);
      _scriptModule = await _runtime.InvokeAsync<IJSObjectReference>("import", "./_content/ScriptContainer/ScriptControl.razor.js");
      _scriptInstance = await _scriptModule.InvokeAsync<IJSObjectReference>("getScriptModule", _serviceInstance, options);

      return this;
    }

    /// <summary>
    /// Script proxy
    /// </summary>
    /// <param name="message"></param>
    /// <param name="index"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    [JSInvokable]
    public void OnChangeAction(dynamic message, int index, string action) => OnChange(message, index, action);

    /// <summary>
    /// Dispose
    /// </summary>
    /// <returns></returns>
    public async ValueTask DisposeAsync()
    {
      OnChange = (o, i, action) => { };

      if (_scriptInstance is not null)
      {
        await _scriptInstance.DisposeAsync();
      }

      if (_scriptModule is not null)
      {
        await _scriptModule.DisposeAsync();
      }

      _serviceInstance?.Dispose();

      _scriptModule = null;
      _scriptInstance = null;
      _serviceInstance = null;
    }
  }
}
