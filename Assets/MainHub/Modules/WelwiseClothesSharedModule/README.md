Общий модуль для модулей одежды.

<b>Установка</b>
1. Установите модуль: https://github.com/Welwise-Games/Shared_Module
2. Установите пакет по пути Window -> Package Manager -> Add package from git URL -> https://github.com/Welwise-Games/ClothesShared_Module.git
3. После установки перейдите Window -> Asset Management -> Addressables -> Groups. Если настройки не созданы, создайте (большая кнопка в середине окна). 
4. Переместите папки вместе с их мета-файлами Addressables и Loadable из Runtime/Client и Runtime/Shared в Assets папку (в любую, кроме папки Editor). Важно это делать не в юнити, а через файловую систему, иначе файлы скопируются, а не переместяться (возможны конфликты).
5. Переместите файлы Clothes Shared Client Module Group и Clothes Shared Shared Module Group (вы можете найти их в поиске в папках Addressables, которые переместили пунктом ранее) в окно Groups (пункт 3) через drag and drop (если они там ещё не находятся).

<b>Код</b><br>
Если класс пишется с маленькой буквы, значит имеется ввиду название инстанса. 

EntryPoint:<br>
Создайте и сохраните инстанс itemsConfigsProviderService и clothesFactory.

При создании игрока:<br>
Вызовите ClothesSharedTools.GetPlayerColorableClothesViewController(itemsConfig, clothesFactory, serializableComponents, equippedItemsData), где itemsConfigs получается вызовом itemsConfigsProviderService.GetItemsConfigAsync(),
serializableComponents - получив компонент у игрока (пример заполнения можно посмотреть в папке Runtime/Client/Example/Example.prefab), а equippedItemsData например из сдк.
