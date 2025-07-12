mergeInto(LibraryManager.library, {
  HideLoadingAndThreeCanvas: function() {
                // Удаление загрузочного экрана
                if (elements.loadingOverlay.parentNode) {
                    elements.loadingOverlay.parentNode.removeChild(elements.loadingOverlay);
                }
                
                // Очистка Three.js сцены
                if (window.disposeThreeScene) {
                    window.disposeThreeScene();
                }
  }
});