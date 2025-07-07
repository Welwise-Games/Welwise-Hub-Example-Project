Общий модуль для всех модулей WelwiseGames

<b>Установка</b><br>
Установите пакеты по пути Package Manager -> Add package from git URL -> Вставить по очереди ссылки ниже:

1. https://github.com/Cysharp/UniTask.git?path=src/UniTask/Assets/Plugins/UniTask
2. https://github.com/Welwise-Games/Shared_Module.git

<b>Опционально</b><br>
Чтобы включить соответствующие части модуля, добавьте scripting define symbols в Project Settings -> Player -> Other Settings -> Script Compilation:
SERVER<br>WELWISE_SHARED_MODULE_LOCALIZATION (требуется пакет Package Manager -> Add package by name -> Вставьте com.unity.localization)<br>WELWISE_SHARED_MODULE_UI (требуется установить пакет assetstore.unity.com/packages/tools/animation/dotween-hotween-v2-27676, перезапустить юнити, появится панель - нужно нажать Open Panel и Add amsdef) и 
Package Manager -> Add package by name -> Вставьте com.unity.textmeshpro)<br>WELWISE_SHARED_MODULE_CLIENT_NETWORK (требуется установить пакет https://github.com/FirstGearGames/FishNet.git?path=Assets/FishNet)
