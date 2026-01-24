# Проект: **CRM-платформа для специалистов**

## 📌 Цель проекта

Создать микросервисную CRM-систему, в которой:

- специалисты (пользователи) могут управлять своими клиентами и назначать сессии;
- микросервисы взаимодействуют через очередь сообщений (RabbitMQ);
- используется Redis для кэширования данных;
- уведомления и побочные действия обрабатываются асинхронно.

🧱 Общая архитектура
| Сервис | Назначение |
| --- | --- |
| **UserService** | Управление пользователями (регистрация, авторизация) |
| **CRMService** | Работа с клиентами и сессиями (встречами) |
| **NotificationService** | Обработка событий из очереди: логика уведомлений |
| **Redis** | Кэширование данных о сессиях |
| **RabbitMQ** | Обмен событиями между сервисами |

## 🔧 Функциональные требования

### 📍 `UserService`

- Регистрация и аутентификация
- Предоставление `UserId` в заголовке (например, `X-User-Id`)

### 📍 `CRMService`

- CRUD для клиентов и сессий
- Все операции выполняются от имени пользователя (`UserId`)
- Проверка доступа: пользователь может работать только со своими данными

### 🧠 Redis-кэш

- Кэшировать список сессий по `ClientId`
- Удалять/обновлять кэш при изменении сессий

### 📤 Публикация событий в RabbitMQ

- `ClientCreatedEvent`
- `SessionPlannedEvent`

### 📍 `NotificationService`

- Подписан на очередь `crm.events` через RabbitMQ
- Слушает события:
  | Routing Key | Событие | Поведение |
| --- | --- | --- |
| `client.created` | `ClientCreatedEvent` | Эмуляция welcome email (лог в консоль) |
| `session.planned` | `SessionPlannedEvent` | Эмуляция напоминания (лог в консоль) |

## ⚙️ Технологии

- .NET 8 WebAPI
- PostgreSQL (через EF Core или Dapper)
- RabbitMQ (через RabbitMQ.Client или MassTransit)
- Redis (StackExchange.Redis или IDistributedCache)
- Serilog
- Swagger/OpenAPI
- Docker + docker-compose

## 🐳 Docker и окружение

### `docker-compose.yml` включает:

- Redis
- RabbitMQ (с UI на `localhost:15672`)
- CRMService
- UserService
- NotificationService

---

# Project: **CRM platform for specialists**

## 📌 Project goal

Create a microservice-based CRM system where:

- specialists (users) can manage their clients and schedule sessions;
- microservices communicate via a message queue (RabbitMQ);
- Redis is used for data caching;
- notifications and side effects are processed asynchronously.

🧱 General architecture
| Service | Purpose |
| --- | --- |
| **UserService** | User management (registration, authorization) |
| **CRMService** | Working with clients and sessions (appointments) |
| **NotificationService** | Event processing from the queue: notification logic |
| **Redis** | Caching session data |
| **RabbitMQ** | Event exchange between services |

## 🔧 Functional requirements

### 📍 `UserService`

- Registration and authentication
- Providing `UserId` in the header (for example, `X-User-Id`)

### 📍 `CRMService`

- CRUD for clients and sessions
- All operations are performed on behalf of the user (`UserId`)
- Access control: the user can only work with their own data

### 🧠 Redis cache

- Cache a list of sessions by `ClientId`
- Delete/update the cache when sessions are modified

### 📤 Publishing events in RabbitMQ

- `ClientCreatedEvent`
- `SessionPlannedEvent`

### 📍 `NotificationService`

- Subscribed to the `crm.events` queue via RabbitMQ
- Listens to events:
  | Routing Key | Event | Behavior |
| --- | --- | --- |
| `client.created` | `ClientCreatedEvent` | Emulating a welcome email (console log) |
| `session.planned` | `SessionPlannedEvent` | Emulation of a reminder (console log) |

## ⚙️ Technologies

- .NET 8 WebAPI
- PostgreSQL (via EF Core or Dapper)
- RabbitMQ (via RabbitMQ.Client or MassTransit)
- Redis (StackExchange.Redis or IDistributedCache)
- Serilog
- Swagger/OpenAPI
- Docker + docker-compose

## 🐳 Docker and environment

### `docker-compose.yml` includes:

- Redis
- RabbitMQ (with UI on `localhost:15672`)
- CRMService
- UserService
- NotificationService
