function ScriptModule(instance, options) {

  this.events = [];
  this.sizeScheduler = null;
  this.serviceInstance = instance;

  this.getDocBounds = () => {
    return {
      X: document.body.clientWidth,
      Y: document.body.clientHeight
    };
  };

  this.getElementBounds = (element) => {
    const bounds = element.getBoundingClientRect();
    return {
      X: bounds.width,
      Y: bounds.height
    };
  };

  this.onSize = () => {
    clearTimeout(this.sizeScheduler);
    this.sizeScheduler = setTimeout(() => {
      this
        .serviceInstance
        .invokeMethodAsync("OnSizeChange", this.getDocBounds())
        .catch(o => this.unsubscribe());
    }, options.interval);
  };

  this.subscribe = (element, e, done) => {
    element.addEventListener(e, done, false);
    this.events.push({ element, e, done });
  };

  this.unsubscribe = () => {
    this.events.forEach(o => o.element.removeEventListener(o.e, o.done));
  };

  this.subscribe(window, "resize", this.onSize);
};

export function getScriptModule(instance, options) {
  return new ScriptModule(instance, options);
};
