# Script Container

This RCL library contains .NET wrapper around JS `window.onresize` event and additional methods to get size of the document or specified HTML element. 

- `GetDocBounds()` - get document's width and height 
- `GetElementBounds(ElementReference)` - get element's width and height  

Can be used with any Blazor app, either Server side or Wasm.

# Nuget

```
Install-Package ScriptContainer -Version 1.0.1-prerelease
```

# Sample 

The code below is an excerpt. 
Complete sample can be found in the [Samples](https://github.com/Indemos/ScriptContainer/tree/main/Samples) folder.

```C#
@using ScriptContainer

<ScriptControl @ref="SomeControl"></ScriptControl>

@code
{
  public ScriptControl SomeControl { get; set; }

  protected override async Task OnAfterRenderAsync(bool setup)
  {
    if (setup)
    {
      SomeControl.OnSize = async message => await GetBounds(); // Subscribe to resize event
      SomeControl.OnLoad = async () => await GetBounds();      // Subscribe to load event
    }
  }

  protected async Task GetBounds()
  {
    var docBounds = await SomeControl.GetDocBounds();
    var itemBounds = await SomeControl.GetElementBounds(SomeElement);

  }
}
```
