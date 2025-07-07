Модуль позволяет внедрить чат, который будет использоваться игроками для коммуникации.
Здесь присутствует авторитарный сервер и список запрещённых слов. Обратите внимание, что методы для сервера и клиента могут называться одинаково, но иметь разные неймспейсы (у клиента - Client, у сервера - Server).
Каждый содержится в своей сборке.

Если класс пишется с маленькой буквы, значит имеется ввиду инстанс. 
Действия выполняются в хронологическом порядке:

Гайд по внедрению в проект:

Список запрещённых слов:
Хранится в юнити по пути: Assets\StreamingAssets\forbidden_words.
В WebGL клиентском билде в Streaming Assets, на линукс сервере - прямо в папке с билдом.

Первая строка относится для символов, которым будет запиковаться 1 символ запрещённого слова. Рекомендую ставить 1 знак, изначально стоит *.
Все остальные строки (начиная со 2) хранят по одному запрещённому слову, то есть 1 запрещённое слово - 1 строка.

Обращайте внимание, что слово запиковается даже если слова написано слитно или повторяются (например запрещённое слово - знак, а предложение нетзнака, то будет нет****а.
Для клиентского список запрещённых слов можно не делать, но тогда он не будет запиковать свои сообщения при отправке. Для сервера - обязательно, он запиковает отправленные клиентом сообщения для других игроков.

Клиент:

EntryPoint:
1. Имплементируйте IClientsNicknamesProviderService (сервис, выдающий никнейм по Network Connection'y игрока).
2. Зарегистрируйте сервисы через метод ChatEntryPointTools.RegisterServices.

Если вы хотите, чтобы над игроками появлялись сообщения:

Добавьте на игрока компонент PlayerChatTextView и прокиньте все ссылки.

При создании персонажа:
1. Вызовите _chatFactory.GetChatTextControllerAsync(playerChatTextView.ChatTextView, mainCamera).
2. Создайте PlayerNicknameTextController(playerChatTextView.NicknameText, clientsNicknamesProviderService.Nicknames[networkConnection]);
3. Создайте и сохраните playerChatTextController, ChatConfig можно получить через chatFactory.GetChatConfigAsync(). 
4. Подпишите chatsDataProviderService.AddedMessageData += DisplayMessageOverPlayer, где DisplayMessageOverPlayer - playerChatTextController.StartOrContinueShowingText(chatTextData.Content), chatTextData - это 3 параметр.

Сервер: 

EntryPoint:
1. Имплементируйте IServerChatsDataProvider (он предоставляет чат по Network Connection'у игрока) и создайте экземпляр.
2. Вызовите ChatEntryPointTools.SubscribeServices.

При удалении комнаты (для очистки ненужного чата):
Вызовите serverChatsDataProvider.TryRemovingHubMessagesData.

При инициализации игрока:
Вызовите serverChatsDataProvider.SendInitializationChatsDataForClient, передав network connection игрока и server manager (fishnet).
