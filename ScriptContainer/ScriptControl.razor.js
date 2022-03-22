function ScriptModule(instance, options) {

  this.events = [];
  this.sizeScheduler = null;
  this.serviceInstance = instance;

  this.getDocBounds = () => {
    return {
      Width: document.body.clientWidth,
      Height: document.body.clientHeight
    };
  };

  this.getElementBounds = (element) => {
    const bounds = element.getBoundingClientRect();
    return {
      Width: bounds.width,
      Height: bounds.height
    };
  };

  this.onSize = (e) => {
    clearTimeout(this.sizeScheduler);
    this.sizeScheduler = setTimeout(() => {
      this.serviceInstance.invokeMethodAsync('OnScriptSize', this.getDocBounds());
    }, options.interval);
  };

  this.subscribe = (element, e, done) => {
    element.addEventListener(e, done, false);
    this.events.push({ element, e, done });
  };

  this.dispose = () => {
    this.events.forEach(o => o.element.removeEventListener(o.e, o.done));
  };

  this.subscribe(window, 'resize', this.onSize);
};

export function getScriptModule(instance, options) {
  return new ScriptModule(instance, options);
};
