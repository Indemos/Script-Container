using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ScriptContainer
{
  /// <summary>
  /// Singleton service
  /// </summary>
  public class ScriptService : IDisposable, IAsyncDisposable
  {
    /// <summary>
    /// Script runtime
    /// </summary>
    private IJSRuntime _scripts = null;

    /// <summary>
    /// Script reference
    /// </summary>
    private IJSObjectReference _scriptModule = null;

    /// <summary>
    /// Script instance 
    /// </summary>
    private IJSObjectReference _scriptInstance = null;

    /// <summary>
    /// Service instance 
    /// </summary>
    private DotNetObjectReference<ScriptService> _serviceInstance = null;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="scripts"></param>
    public ScriptService(IJSRuntime scripts)
    {
      _scripts = scripts;
    }

    /// <summary>
    /// On size event
    /// </summary>
    public Action<ScriptMessage> OnSize { get; set; } = o => { };

    /// <summary>
    /// Copy image to canvas
    /// </summary>
    /// <param name="canvas"></param>
    /// <param name="source"></param>
    /// <returns></returns>
    public async Task<ScriptMessage> SetCanvasImage(ElementReference canvas, string source)
    {
      if (_scriptInstance is not null)
      {
        return await _scriptInstance.InvokeAsync<ScriptMessage>("setCanvasImage", canvas, source);
      }

      return null;
    }

    /// <summary>
    /// Get document bounds
    /// </summary>
    /// <returns></returns>
    public async Task<ScriptMessage> GetDocBounds()
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
    public async Task<ScriptMessage> GetElementBounds(ElementReference element)
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
    public async Task<ScriptService> CreateModule(IDictionary<string, dynamic> options = null)
    {
      options ??= new Dictionary<string, dynamic>();

      if (options.TryGetValue("interval", out dynamic interval) is false)
      {
        options["interval"] = 100;
      }

      _serviceInstance = DotNetObjectReference.Create(this);
      _scriptModule = await _scripts.InvokeAsync<IJSObjectReference>("import", "./_content/ScriptContainer/ScriptControl.razor.js");
      _scriptInstance = await _scriptModule.InvokeAsync<IJSObjectReference>("getScriptModule", _serviceInstance, options);

      return this;
    }

    /// <summary>
    /// Script proxy
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    [JSInvokable]
    public Task<dynamic> OnScriptSize(ScriptMessage message)
    {
      if (OnSize is not null)
      {
        OnSize(message);
      }

      return Task.FromResult<dynamic>(0);
    }

    /// <summary>
    /// Dispose
    /// </summary>
    public void Dispose() => Task.Run(DisposeAsync).GetAwaiter().GetResult();

    /// <summary>
    /// Dispose
    /// </summary>
    /// <returns></returns>
    public async ValueTask DisposeAsync()
    {
      await _scriptInstance.InvokeVoidAsync("dispose");
      await _scriptInstance.DisposeAsync();
      await _scriptModule.DisposeAsync();

      _serviceInstance?.Dispose();
    }
  }
}
