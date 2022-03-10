using Microsoft.JSInterop;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ScriptContainer
{
  /// <summary>
  /// Singleton service
  /// </summary>
  public sealed class ScriptService
  {
    static ScriptService() {}
    private ScriptService() {}
    public static ScriptService Instance { get; } = new ScriptService();
    public IList<ScriptControl> Observers = new List<ScriptControl>();

    [JSInvokable]
    public Task OnSize(ScriptMessage message)
    {
      foreach (var observer in Observers)
      {
        if (observer.OnSize is not null)
        {
          observer.OnSize(message);
        }
      }

      return Task.CompletedTask;
    }
  }
}
