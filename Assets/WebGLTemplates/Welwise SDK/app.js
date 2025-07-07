// Конфигурация приложения
const AppConfig = {
    aspectRatioMode: 'default',
    backgroundImageUrl: 'background.jpg'
};

// Кэширование DOM элементов
const elements = {
    canvas: document.getElementById('canvas'),
    loadingOverlay: document.getElementById('loading-overlay'),
    threeCanvas: document.getElementById('three-canvas'),
    planetPrerender: document.getElementById('planet-prerender'),
    progressBarFill: document.getElementById('progress-bar-fill'),
    body: document.body
};

// Состояние приложения
const state = {
    unityInstance: null,
    isFullscreen: false,
    resizeTimeout: null
};

// Проверка типа устройства
const isDesktop = () => !/iPhone|iPad|iPod|Android/i.test(navigator.userAgent);

// Проверка доступности изображения
const checkImage = url => new Promise(resolve => {
    const img = new Image();
    img.onload = () => resolve(true);
    img.onerror = () => resolve(false);
    img.src = url;
});

// Инициализация фона
const initBackground = async () => {
    if (AppConfig.aspectRatioMode === 'default') return;
    
    const available = await checkImage(AppConfig.backgroundImageUrl);
    if (!available) {
        console.warn('Background image not found, using default');
        AppConfig.backgroundImageUrl = 'LoadingBackground.jpg';
    }
};

// Обновление соотношения сторон
const updateAspectRatio = () => {
    if (AppConfig.aspectRatioMode === 'default' || !isDesktop()) {
        resetToFullscreen();
        return;
    }

    const targetRatio = AppConfig.aspectRatioMode === '16_9' ? 16/9 : 9/16;
    const { innerWidth: w, innerHeight: h } = window;
    
    let width, height;
    if (w / h > targetRatio) {
        height = h;
        width = h * targetRatio;
    } else {
        width = w;
        height = w / targetRatio;
    }

    applyCanvasSize(width, height, true);
    applyLoadingOverlaySize(width, height, true);
    applyBodyBackground();
    
    if (state.unityInstance && !state.isFullscreen) {
        scheduleUnityRepaint();
    }
};

// Сброс в полноэкранный режим
const resetToFullscreen = () => {
    applyCanvasSize('100%', '100%', false);
    applyLoadingOverlaySize('100%', '100%', false);
    clearBodyBackground();
    
    if (state.unityInstance && !state.isFullscreen) {
        scheduleUnityRepaint();
    }
};

// Применение размеров canvas
const applyCanvasSize = (width, height, centered) => {
    elements.canvas.style.width = `${width}${typeof width === 'number' ? 'px' : ''}`;
    elements.canvas.style.height = `${height}${typeof height === 'number' ? 'px' : ''}`;
    elements.canvas.style.position = 'absolute';
    
    if (centered) {
        elements.canvas.style.top = '50%';
        elements.canvas.style.left = '50%';
        elements.canvas.style.transform = 'translate(-50%, -50%)';
    } else {
        elements.canvas.style.top = '0';
        elements.canvas.style.left = '0';
        elements.canvas.style.transform = 'none';
    }
    
    // Обновление физических размеров
    const dpr = window.devicePixelRatio || 1;
    elements.canvas.width = (typeof width === 'number' ? width : window.innerWidth) * dpr;
    elements.canvas.height = (typeof height === 'number' ? height : window.innerHeight) * dpr;
};

// Применение размеров загрузочного экрана
const applyLoadingOverlaySize = (width, height, centered) => {
    if (!elements.loadingOverlay) return;
    
    elements.loadingOverlay.style.width = `${width}${typeof width === 'number' ? 'px' : ''}`;
    elements.loadingOverlay.style.height = `${height}${typeof height === 'number' ? 'px' : ''}`;
    
    if (centered) {
        elements.loadingOverlay.style.top = '50%';
        elements.loadingOverlay.style.left = '50%';
        elements.loadingOverlay.style.transform = 'translate(-50%, -50%)';
    } else {
        elements.loadingOverlay.style.top = '0';
        elements.loadingOverlay.style.left = '0';
        elements.loadingOverlay.style.transform = 'none';
    }
};

// Установка фона
const applyBodyBackground = () => {
    elements.body.style.backgroundImage = `url('${AppConfig.backgroundImageUrl}')`;
    elements.body.style.backgroundSize = 'cover';
    elements.body.style.backgroundRepeat = 'no-repeat';
    elements.body.style.backgroundColor = '#000';
};

// Очистка фона
const clearBodyBackground = () => {
    elements.body.style.backgroundImage = '';
    elements.body.style.backgroundSize = '';
    elements.body.style.backgroundRepeat = '';
    elements.body.style.backgroundColor = '';
};

// Планирование перерисовки Unity
const scheduleUnityRepaint = () => {
    if (!state.unityInstance) return;
    
    cancelAnimationFrame(state.repaintId);
    state.repaintId = requestAnimationFrame(() => {
        const { width, height } = elements.canvas;
        
        // Временное изменение размеров
        elements.canvas.width = width + 1;
        
        // Возврат размеров и перерисовка
        requestAnimationFrame(() => {
            elements.canvas.width = width;
            elements.canvas.height = height;
            
            if (state.unityInstance.Module?.Invalidate) {
                state.unityInstance.Module.Invalidate();
            }
            
            elements.canvas.focus();
        });
    });
};

// Обработчик прогресса загрузки
const onProgress = progress => {
    if (elements.progressBarFill) {
        elements.progressBarFill.style.width = `${Math.min(100, progress * 100)}%`;
    }
};

// Обработчик полноэкранного режима
const handleFullscreenChange = () => {
    state.isFullscreen = !!(
        document.fullscreenElement ||
        document.webkitFullscreenElement ||
        document.mozFullScreenElement ||
        document.msFullscreenElement
    );
    
    if (!state.isFullscreen) {
        setTimeout(() => {
            updateAspectRatio();
            scheduleUnityRepaint();
            elements.canvas.focus();
        }, 100);
    }
};

// Инициализация приложения
const initializeApp = async () => {
    await initBackground();
    updateAspectRatio();
    
    // Обработчики событий
    window.addEventListener('resize', () => {
        clearTimeout(state.resizeTimeout);
        state.resizeTimeout = setTimeout(updateAspectRatio, 200);
    });
    
    const fullscreenEvents = [
        'fullscreenchange',
        'webkitfullscreenchange',
        'mozfullscreenchange',
        'MSFullscreenChange'
    ];
    
    fullscreenEvents.forEach(event => {
        document.addEventListener(event, handleFullscreenChange);
    });
    
    // Загрузка Unity
    const loadUnity = () => {
        const loaderScript = document.createElement('script');
        loaderScript.src = `Build/{{{LOADER_FILENAME}}}`;
        loaderScript.onload = () => {
            createUnityInstance(elements.canvas, {
                dataUrl: `Build/{{{DATA_FILENAME}}}`,
                frameworkUrl: `Build/{{{FRAMEWORK_FILENAME}}}`,
                codeUrl: `Build/{{{CODE_FILENAME}}}`,
                streamingAssetsUrl: 'StreamingAssets',
                companyName: '{{{COMPANY_NAME}}}',
                productName: '{{{PRODUCT_NAME}}}',
                productVersion: '{{{PRODUCT_VERSION}}}'
            }, onProgress).then(instance => {
                state.unityInstance = instance;
                window.unityInstance = instance; // Глобальная переменная для SDK
                
                // Удаление загрузочного экрана
                if (elements.loadingOverlay.parentNode) {
                    elements.loadingOverlay.parentNode.removeChild(elements.loadingOverlay);
                }
                
                // Очистка Three.js сцены
                if (window.disposeThreeScene) {
                    window.disposeThreeScene();
                }
                
                updateAspectRatio();
                
                // Обработчик клика для перерисовки
                elements.canvas.addEventListener('click', () => {
                    if (!state.isFullscreen) scheduleUnityRepaint();
                });
            }).catch(error => {
                console.error('Unity initialization failed:', error);
            });
        };
        document.body.appendChild(loaderScript);
    };
    
    // Проверка готовности Three.js
    if (typeof THREE === 'undefined') {
        const threeScript = document.createElement('script');
        threeScript.src = 'lib/threeImport.js';
        threeScript.onload = loadUnity;
        document.body.appendChild(threeScript);
    } else {
        loadUnity();
    }
};

// Глобальные функции для взаимодействия
window.onThreeJsLoaded = () => {
    if (elements.planetPrerender) {
        elements.planetPrerender.style.opacity = '0';
        setTimeout(() => {
            if (elements.planetPrerender) {
                elements.planetPrerender.style.display = 'none';
            }
        }, 300);
    }
};

// Запуск приложения
document.addEventListener('DOMContentLoaded', initializeApp);