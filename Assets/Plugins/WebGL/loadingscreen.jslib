mergeInto(LibraryManager.library, {
  HideLoadingAndThreeCanvas: function() {
    var loadingOverlay = document.getElementById('loading-overlay');
    var threeCanvas = document.getElementById('three-canvas');
    if (loadingOverlay) loadingOverlay.remove();
    if (threeCanvas) threeCanvas.remove();
  }
});