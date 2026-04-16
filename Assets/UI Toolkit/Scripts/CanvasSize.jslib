mergeInto(LibraryManager.library, {
  GetCanvasHeight: function () {
    var canvas = document.querySelector("#unity-canvas");
    return canvas.getBoundingClientRect().height;
  }, 
  GetCanvasWidth: function () {
    var canvas = document.querySelector("#unity-canvas");
    return canvas.getBoundingClientRect().width;
  }
});