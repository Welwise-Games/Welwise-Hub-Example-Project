mergeInto(LibraryManager.library, {
    IsMobileDevice: function () {
        const userAgent = navigator.userAgent;
        return /Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(userAgent) ? 1 : 0;
    }
});