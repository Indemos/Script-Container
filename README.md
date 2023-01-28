# Script Container

This RCL library contains .NET wrapper around JS `window.onresize` event and additional methods to get size of the document or specified HTML element. 

- `GetDocBounds()` - get document's width and height 
- `GetElementBounds(ElementReference)` - get element's width and height  

Can be used with any Blazor app, either Server side or Wasm.

# Status

```
Install-Package ScriptContainer
```

![GitHub Workflow Status (with event)](https://img.shields.io/github/actions/workflow/status/Indemos/Script-Container/dotnet.yml?event=push)
![GitHub](https://img.shields.io/github/license/Indemos/Script-Container)
![GitHub](https://img.shields.io/badge/system-Windows%20%7C%20Linux%20%7C%20Mac-blue)

# Sample 

The code below is an excerpt. 
Complete sample can be found in the [Samples](https://github.com/Indemos/ScriptContainer/tree/main/Samples) folder.

```C#
@using ScriptContainer
@inject IJSRuntime scriptService

<div @ref="SomeElement">Demo</div>

@code
{
  public ScriptService ScaleService { get; set; }
  public ElementReference SomeElement { get; set; }

  protected override async Task OnAfterRenderAsync(bool setup)
  {
    if (setup)
    {
      ScaleService = new ScriptService(scriptService);

      await ScaleService.CreateModule();
      await GetBounds();

      ScaleService.OnSize = async message => await GetBounds();
    }
  }

  protected async Task GetBounds()
  {
    var docBounds = await ScaleService.GetDocBounds();
    var itemBounds = await ScaleService.GetElementBounds(SomeElement);
  }
}
```
