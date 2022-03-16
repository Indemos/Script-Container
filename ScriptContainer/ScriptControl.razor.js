window.adapterSetProcessorInstance =
  window.adapterSetProcessorInstance ||
  (instance => window.adapterProcessorInstance = window.adapterProcessorInstance || instance);

window.adapterGetDocBounds =
  window.adapterGetDocBounds ||
  (() => {
    return {
      Width: document.body.clientWidth,
      Height: document.body.clientHeight
    };
  });

window.adapterGetElementBounds =
  window.adapterGetElementBounds ||
  (element => {
    const bounds = element.getBoundingClientRect();
    return {
      Width: bounds.width,
      Height: bounds.height
    };
  });

window.adapterOnSize =
  window.adapterOnSize ||
((...args) => window.adapterProcessorInstance && window.adapterProcessorInstance.invokeMethodAsync('OnScriptSize', window.adapterGetDocBounds()));

if (window.onresize !== window.adapterOnSize) {

  const scope = (...cbs) => (...inputs) => cbs.forEach(o => o && o(inputs));

  window.adapterOnSize
    = window.onresize
    = scope(window.onresize, window.adapterOnSize);
}
