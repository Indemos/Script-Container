using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace ScriptContainer
{
  public partial class ScriptControl : IDisposable
  {
    [Inject] protected IJSRuntime _scripts { get; set; }

    /// <summary>
    /// Script reference
    /// </summary>
    protected Task<IJSObjectReference> _scriptModule = null;

    /// <summary>
    /// On size event
    /// </summary>
    public Action<ScriptMessage> OnSize { get; set; }

    /// <summary>
    /// On load event
    /// </summary>
    public Action OnLoad { get; set; }

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
    /// Load
    /// </summary>
    /// <param name="setup"></param>
    protected override async Task OnAfterRenderAsync(bool setup)
    {
      if (setup)
      {
        ScriptService.Instance.Observers.Add(this);

        _scriptModule = _scripts.InvokeAsync<IJSObjectReference>("import", "./_content/ScriptContainer/ScriptControl.razor.js").AsTask();

        var instance = DotNetObjectReference.Create(ScriptService.Instance);

        await _scriptModule;
        await _scripts.InvokeVoidAsync("adapterSetProcessorInstance", instance);

        if (OnLoad is not null)
        {
          OnLoad();
        }
      }

      await base.OnAfterRenderAsync(setup);
    }

    /// <summary>
    /// Dispose
    /// </summary>
    /// <param name="version"></param>
    protected void Dispose(int version)
    {
      _scriptModule?.ContinueWith(o => o.Dispose());

      ScriptService.Instance.Observers.Remove(this);
    }

    /// <summary>
    /// Dispose
    /// </summary>
    void IDisposable.Dispose()
    {
      Dispose(0);
    }
  }
}
