GRANT ALL PRIVILEGES ON DATABASE dbo TO postgres;

create SCHEMA dbo;
 
ALTER ROLE postgres SET search_path TO dbo;


CREATE TABLE dbo."TaskStatus"
(
    "StatusId" serial primary key,
    "Description" text not null
);

CREATE TABLE dbo."UserRole"
(
    "Id" SERIAL PRIMARY KEY,
    "Description" text not null
);

CREATE TABLE dbo."User"(
    "Id" SERIAL PRIMARY KEY,
    "Username" varchar (30) not null,
    "Email"    varchar (30) not null,
    "Role"     integer null,
    CONSTRAINT "FK_User_Roles" 
    FOREIGN KEY ("Role") 
    REFERENCES dbo."UserRole"("Id")
    ON DELETE SET NULL
);

CREATE TABLE dbo."Task" (
    "Id" SERIAL PRIMARY KEY,
    "Title" varchar(56) not null,
    "Description" text,
    "Status" integer not null,
    "CreatedAt" TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP not null,
    "CreatedBy" integer null,
    CONSTRAINT "FK_Task_TaskStatus" 
    FOREIGN KEY ("Status") 
    REFERENCES dbo."TaskStatus"("StatusId")
    ON DELETE SET NULL,

    CONSTRAINT "FK_Task_User"
    FOREIGN KEY ("CreatedBy")
    REFERENCES dbo."User"("Id")
    ON DELETE SET NULL
);

CREATE TABLE dbo."TaskAssigned" (
    "TaskId" integer not null,
    "UserId" integer not null,
    CONSTRAINT "task_assign_pkey" PRIMARY KEY ("TaskId", "UserId"),

    CONSTRAINT "FK_Task_TaskAssign" 
    FOREIGN KEY ("TaskId") 
    REFERENCES dbo."Task"("Id")
    ON DELETE CASCADE,

    CONSTRAINT "FK_User_TaskAssign" 
    FOREIGN KEY ("UserId") 
    REFERENCES dbo."User"("Id")
    ON DELETE SET NULL
);


INSERT INTO dbo."UserRole" ("Id","Description")
VALUES
(0,'User'),
(1,'Admin');

INSERT INTO dbo."TaskStatus" ("StatusId", "Description")
VALUES
(1, 'Задача только создана, но не назначена'),
(2, 'Над задачей работает один специалист'),
(3, 'Над задачей работает несколько специалистов'),
(4, 'Выполнение задачи приостановлено'),
(5, 'Задача не назначена, и не завершена'), --Кто то заболел, или уволился
(6, 'Задача завершена'),
(7, 'Задача отменена');


INSERT INTO dbo."User" ("Username", "Email", "Role")
VALUES 
('alexey_alexeev', 'alexey.i@company.com', 0),
('marina_smirnova', 'marina.s@company.com', 0),
('dmitry_lopux', 'dmitry.p@company.com', 0),
('olga_lopux', 'olga.v@company.com', 1),
('ivan_ivanov', 'ivan.k@company.com', 0),
('ekaterina_fedorova', 'ekaterina.f@company.com', 0);

INSERT INTO dbo."Task" ("Title", "Description", "Status", "CreatedBy")
VALUES 
('Акция "Черная пятница"', 'Подготовить скидки 50% на топ-100 товаров', 2, 4),
('SEO-оптимизация категорий', 'Добавить мета-теги для 20 основных категорий', 2, 4),
('Обновление описаний товаров', 'Переписать 50 старых описаний под новый стиль', 1, 4),
('Возврат товара #45678', 'Оформить возврат для клиента client@mail.com', 3, 6),
('Анализ конкурентов', 'Сравнить цены на 10 ключевых товаров', 6, 3);


INSERT INTO dbo."TaskAssigned" ("TaskId", "UserId")
VALUES 
(1, 2),  
(1, 3),  
(2, 4),  
(3, 2),  
(4, 6),  
(5, 3);
 
