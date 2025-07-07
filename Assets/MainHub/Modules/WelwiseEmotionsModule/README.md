Модуль эмоций позволяет выбирать и сохранять выбранные эмоции, проигрывать анимации в онлайне.
Здесь присутствует авторитарный сервер. Обратите внимание, что методы для сервера и клиента могут называться одинаково, но иметь разные неймспейсы (у клиента - Client, у сервера - Server).
Каждый содержится в своей сборке.

Если класс пишется с маленькой буквы, значит имеется ввиду инстанс.
Действия выполняются в хронологическом порядке:

Гайд по внедрению в проект:

Прежде всего заполните EmotionsAnimationsConfig.

Клиент:

Юнити:
Повесить на игрока компонент PlayerEmotionsSerializableComponents. На геймобжекте с аниматором: Animator, AnimatorStateObserver ParticleEventController. Указать на них ссылки в
PlayerEmotionsSerializableComponents.

Заполните EmotionsViewConfig

При создании игрока:
1. Создать PlayerEmotionsComponents, emotionsViewConfig получается через emotionsConfigsProviderService.GetEmotionsViewConfigAsync();
2. Только для овнера: Вызвать playerEmotionsComponents.SubscribeAnimatorController(ownerEmotionsPlayingSynchronizerService)


EntryPoint:
1. Создать EmotionsConfigsProviderService через конструктор.
2. Имплементировать INotOwnerPlayersComponentsProviderService (сервис, который возвращает компоненты неовнеров) и создать экземпляр.
3. Загрузить EmotionsAnimationsConfig через emotionsConfigsProviderService.GetEmotionsAnimationsConfig().
4. Вызвать EntryPointTools.RegisterServicesAndSubscribe, последним аргументом он возвращает EmotionsEntryPointData, в котором хранятся классы, которые могут вам пригодиться. Можете добавить в ссылки другие классы, если потребуется.

Сервер:

Entry Point:
Имплементировать IVisibleClientsProviderService (отдаёт всех видимых игроков по его Network Connection), создать экземпляр.

При регистрации клиента:
Создайте класс данных (ClientSelectedEmotionsData), передав emotionsAnimationsConfig через _emotionsConfigsProviderService.GetEmotionsAnimationsConfig();

При создании игрока:
Вызывать clientsSelectedEmotionsDataProviderService.TryAddingClientSelectedEmotionsData(networkConnection, clientData.SelectedEmotionsData). ClientData - данные конкретного игрока;

При уничтожении игрока:
Вызывать clientsSelectedEmotionsDataProviderService.TryRemovingClientSelectedEmotionsData;

При обновлении данных:
Подписывать clientsSelectedEmotionsDataProviderService.UpdatedData += SaveData. Имеется ввиду сохранение данных.
