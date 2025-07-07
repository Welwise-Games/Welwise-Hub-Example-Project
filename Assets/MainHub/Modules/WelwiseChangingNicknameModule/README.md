Модуля изменения ника. <br>Здесь присутствует авторитарный сервер.

<b>Установка</b>
1. Установите модули: https://github.com/Welwise-Games/NicknameShared_Module (добавьте define symbols SERVER и WELWISE_SHARED_MODULE_CLIENT_NETWORK для shared модуля), https://github.com/Welwise-Games/WelwiseGamesSDK и https://github.com/FirstGearGames/FishNet
2. Установите пакет по пути Package Manager -> Add package from git URL -> https://github.com/Welwise-Games/ChangingNickname_Module.git

<b>Код</b><br>
Если класс пишется с маленькой буквы, значит имеется ввиду название локальной переменной. Действия выполняются в хронологическом порядке.

Клиент:<br>

EntryPoint:<br>
1. Имплементировать IClientsNicknamesDataProvider и создать инстанс.
2. Вызвать NicknameChangingTools.Initialize, в параметрах который sharedClientsNicknamesConfig можно получить вызвав sharedNicknamesConfigsProviderService.GetSharedClientsNicknamesConfigAsync(), IPlayerData из сдк,
   а последний (nicknamesSharedEntryPointData) предоставляет данные, которые могут пригодиться вам.

При создании игрока:<br>
Вызвать NicknameChangingTools.InitializePlayer, nicknamesSharedEntryPointData содержит clientsNicknamesProviderService, а про playerTextController можно почитать в общем модуле.

При изменении никнейма в игре (ваша реализация)<br>
Вызвать clientManager.Broadcast(new SettingNicknameBroadcastForServer(nickname));

Сервер:<br>

EntryPoint:<br> 
Данные берутся оттуда же, откуда и у клиента.

1. Имплементировать IClientsNicknamesDataProvider (для сервера и клиента он может быть один) и создать инстанс.
2. По желанию имплементировать ICanSetNicknameConditionProvider.
3. Имлементировать IVisibleClientsProviderService.
4. Вызвать NicknamesSharedEntryPointTools.Initialize, если 1 пункт выполнен, передайте имплементатора последним аргументом. 
