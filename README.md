﻿# API для Высоцкого
## Предметная область
- [Термины](/docs/Domain.md)
## Ресурсы
- /api/categories - категории работ
- /api/employee - сотрудники УК
- /api/notifications - уведомления/новости от УК
- /api/owners - Арендаторы и владельцы помещений
- /api/rooms - помещения
- /api/tasks - заявки

## Стандартный формат ответа
(в случае успеха)
```json
{
  "status": "ok",
  "result": {}
}
```
- status - ok
- result - undefined | null | Object | Array | number | string | boolean

(в случае ошибки)
```json
{
  "status": "error",
  "error": {
    "message": "ошибка"
  }
}
```
- status - error
- error - { message: string }

Возможны следующие коды ответов:
- 200 - нет ошибок
- 201 - объект создан
- 400 - криво составлен запрос
- 401 - не предоставлен токен там, где он требуется 
- 403 - не достаточно прав
- 404 - несуществующий ID (если передаёются в URL), либо другой некорректный урл
- 500 - необработанное исключение

## Стандартная пагинация
Запрос (QUERY)
- pageSize - размер страницы (1 <= pageSize <= 50). По-умолчанию = 50
- pageNumber - номер страницы (1 <= pageNumber). По-умолчанию = 1
Ответ:
  TBD
## Стандартный поиск
Запрос (QUERY)
- q - поисковой запрос

Алгоритм поиска не определён

Ответ - массив или пагинация (если есть)
