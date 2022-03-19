window.adapterProcessorInstances = window.adapterProcessorInstances || {};

window.adapterSetProcessorInstance =
  window.adapterSetProcessorInstance ||
  ((name, instance) => window.adapterProcessorInstances[name] = instance);

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
  ((...args) => {
  for (let name in window.adapterProcessorInstances) {
    console.log(name, window.adapterProcessorInstances[name])
      window.adapterProcessorInstances[name] && window.adapterProcessorInstances[name].invokeMethodAsync('OnScriptSize', window.adapterGetDocBounds());
    }
  });

if (window.onresize !== window.adapterOnSize) {

  const scope = (...cbs) => (...inputs) => cbs.forEach(o => o && o(inputs));

  window.adapterOnSize
    = window.onresize
    = scope(window.onresize, window.adapterOnSize);
}
