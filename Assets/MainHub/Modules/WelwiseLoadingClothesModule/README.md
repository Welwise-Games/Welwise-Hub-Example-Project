Модуль загрузки одежды из сдк.

<b>Установка</b>
1. Установите модули: https://github.com/Welwise-Games/ClothesShared_Module и https://github.com/Welwise-Games/WelwiseGamesSDK
2. Установите пакет по пути Package Manager -> Add package from git URL -> https://github.com/Welwise-Games/LoadingClothes_Module.git

<b>Пример</b><br>
По пути Runtime/Client/Example/ExampleScene (внутри сцены) есть персонаж, на котором есть компонент ClothesLoader - он загружает одежду из сдк при запуске сцены. Для открытия сцены скопируйте её в папку Assets и откройте эту новую сцену.
Также на нём есть компонент PlayerColorableClothesViewSerializableComponents, где в полях компоненты MainSkinnedMeshRenderer - это любая часть персонажа с компонентом Skinned Mesh Renderer,
а DefaultClothesInstances - дефолтные части одежды, которые будут выключены, если на персонаже есть другая одежда той же категории (у персонажа в примере также всё указано). 

<b>Код</b><br>
Если класс пишется с маленькой буквы, значит имеется ввиду название инстанса. 

Создайте контроллер смены одежды (в readme clothes shared модуля написано как), передав данные через вызов sdk.PlayerData.GetEquippedItemsDataFromMetaverse();
