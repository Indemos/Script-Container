function ScriptModule(instance, options) {

  this.events = [];
  this.sizeObservers = [];
  this.serviceInstance = instance;

  /// <summary>
  /// Get window size
  /// </summary>
  this.getDocBounds = () => this.getElementBounds(document.body);

  /// <summary>
  /// Get element size
  /// </summary>
  /// <param name="element"></param>
  this.getElementBounds = element => {
    const bounds = element.getBoundingClientRect();
    return {
      X: element.clientWidth || element.scrollWidth || bounds.width,
      Y: element.clientHeight || element.scrollHeight || bounds.height
    };
  };

  /// <summary>
  /// Subscribe to custom event
  /// </summary>
  /// <param name="element"></param>
  /// <param name="event"></param>
  /// <param name="action"></param>
  this.subscribe = (element, event, action) => {
    let scheduler = null;
    let index = this.sizeObservers.length;
    let change = e => {
      clearTimeout(scheduler);
      scheduler = setTimeout(() => {
        this.serviceInstance && this
          .serviceInstance
          .invokeMethodAsync(action, e, index, event)
          .catch(o => this.unsubscribe());
      }, options.interval || 100);
    };
    element.addEventListener(event, change, false);
    this.events.push({ element, event, change });
    return this.events.length - 1;
  };

  /// <summary>
  /// Unsubscribe from custom event
  /// </summary>
  /// <param name="index"></param>
  this.unsubscribe = index => {
    this.events.forEach((o, i) => {
      if (this.events[index] || i === index) {
        o.element.removeEventListener(o.event, o.change);
        this.events[i] = null;
      }
    });
    this.events = this.events.filter(o => o);
  };

  /// <summary>
  /// Subscribe to element resize
  /// </summary>
  /// <param name="element"></param>
  /// <param name="action"></param>
  this.subscribeToSize = (element, action) => {
    let scheduler = null;
    let index = this.sizeObservers.length;
    let change = e => {
      clearTimeout(scheduler);
      scheduler = setTimeout(() => {
        this.serviceInstance && this
          .serviceInstance
          .invokeMethodAsync(action, e, index, "resize")
          .catch(o => this.unsubscribeFromSize());
      }, options.interval || 100);
    };
    this.sizeObservers.push(new ResizeObserver(change).observe(element));
    return this.sizeObservers.length - 1;
  };

  /// <summary>
  /// Unsubscribe from size observer
  /// </summary>
  /// <param name="index"></param>
  this.unsubscribeFromSize = index => {
    this.sizeObservers.forEach((o, i) => {
      if (this.sizeObservers[index] || i === index) {
        this.sizeObservers[i].disconnect();
        this.sizeObservers[i] = null;
      }
    });
    this.sizeObservers = this.sizeObservers.filter(o => o);
  };

  try {
    this.unsubscribeFromSize();
    this.subscribeToSize(document.body, "OnChangeAction");
  } catch (e) {
    this.unsubscribe();
    this.subscribe(window, "resize", "OnChangeAction");
  }
};

export function getScriptModule(instance, options) {
  return new ScriptModule(instance, options);
};
