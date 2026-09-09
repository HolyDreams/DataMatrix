# SpecEng.TestWork

## Настройка перед запуском

Перед запуском нужно прописать параметры:

### DataMatrix.Web — `appsettings.json`

- `AppSettings.ApiHttpClientSettings.BaseAddress` - Адрес до апи
- `AppSettings.ApiHttpClientSettings.MaxTimeout` - Максимальное время на ожидание ответа
- `AppSettings.FileSettings.Directory` - Временная папка, в которой будут хранится файлы кодов
- `AppSettings.CleanupWorkerSettings.DoWorkDelay` - Делей между запусками работы воркера
- `AppSettings.CleanupWorkerSettings.FileLastUpdate` - Сколько прошло времени, с момента его обновления, для его удаления

## Сборка фронтенда

В `DataMatrix.Web` перед первым запуском нужно установить зависимости и собрать бандл:

```
npm install
npm run build
```

## Доп. инфо

В API я доработал БД, там не было данных о ролях, из за чего при почти любом вызове, падало с ошибкой 403.

Как работает мой сервис думаю интуитивно понятно.
