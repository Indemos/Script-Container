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
  public class ScriptService : IDisposable
  {
    /// <summary>
    /// Script runtime
    /// </summary>
    private IJSRuntime _scripts = null;

    /// <summary>
    /// Script reference
    /// </summary>
    private Task<IJSObjectReference> _scriptModule = null;

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
    public Action<ScriptMessage> OnSize { get; set; }

    /// <summary>
    /// Get document bounds
    /// </summary>
    /// <returns></returns>
    public async Task<ScriptMessage> GetDocBounds()
    {
      if (_scriptModule is not null)
      {
        await _scriptModule;
        return await _scripts.InvokeAsync<ScriptMessage>("adapterGetDocBounds");
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
      if (_scriptModule is not null)
      {
        await _scriptModule;
        return await _scripts.InvokeAsync<ScriptMessage>("adapterGetElementBounds", element);
      }

      return null;
    }

    /// <summary>
    /// Setup script proxy under specified namespace
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public async Task<ScriptService> CreateModule(string name = null)
    {
      _scriptModule = _scripts.InvokeAsync<IJSObjectReference>("import", "./_content/ScriptContainer/ScriptControl.razor.js").AsTask();

      var instance = DotNetObjectReference.Create(this);

      await _scriptModule;
      await _scripts.InvokeVoidAsync("adapterSetProcessorInstance", name ?? Guid.NewGuid().ToString("N"), instance);

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
    /// <exception cref="NotImplementedException"></exception>
    public void Dispose()
    {
      _scriptModule?.ContinueWith(o => o.Dispose());
    }
  }
}
