# Задача 1
Решение:
|                               |         T1        | T2            | T3            | T4                 |
| ----------------------------- | ----------------- | ------------- | ------------- | ------------------ |
| Процесс 1                     | Begin Transaction |    0 --> 1    |               | Commit Transaction |
| Процесс 2 (Read commited)     |                   |               |        0      |                    |
| Процесс 3 (Read uncommited)   |                   |               |        1      |                    |
[Источник](https://habr.com/ru/articles/469415/)

# Задача 2
Используется сайт [sqliteonline.com](https://sqliteonline.com/), БД MS SQL 0.15.1 beta. На данном сайте numeric выводится как число с плавающей точкой, имеющий всегда нули после точки в размере 8 штук. Вместо "15.00000000" будет писаться "15.000" (сокращение 5 нулей)
<details>
  <summary>1. Создание таблицы</summary>

  ```SQL
  CREATE TABLE dbo.tblClosePrice (
    -- Дата на которую берётся цена
    PriceDate DATE NOT NULL
    -- ИД актива
    , PriceAssetId INT NOT NULL
    -- Цена закрытия
    , ClosePrice NUMERIC (19, 8) NOT NULL
    , CONSTRAINT PK_tblClosePrice PRIMARY KEY CLUSTERED (PriceAssetId, PriceDate)
  );
  ```
  
</details>

<details>
  <summary>2. Вставка некоторых данных (в целях тестирования)</summary>
  
  ```SQL
  INSERT
	INTO dbo.tblClosePrice
    VALUES
    	  ('20210618 10:54:29 AM', 1, 15) 
        , ('20210719 11:14:03 AM', 5, 16)
        , ('20210728 9:34:04 PM', 5, 17)
        , ('20220618 10:04:19 AM', 1, 18)
  ```
</details>

<details>
  <summary>3. Вывод внесённых данных</summary>
  
  ```SQL
  SELECT * FROM dbo.tblClosePrice
  ```
  **Вывод:**
  | PriceDate  | PriceAssetId | ClosePrice |
  | ---------- | ------------ | ---------- |
  | 2021-06-18 |       1      |   15.000   |
  | 2022-06-18 |       1      |   18.000   |
  | 2021-07-19 |       5      |   16.000   |
  | 2021-07-28 |       5      |   17.000   |
</details>

<details>
  <summary>4. Выборка данных</summary>

  ```SQL
  SELECT DATEPART(month, PriceDate) AS month, MAX(ClosePrice) as max, MIN(ClosePrice) as min
  FROM dbo.tblClosePrice
  GROUP BY DATEPART(month, PriceDate)
  ```
  **Вывод:**
  | month  |    min    |    max    |
  | ------ | --------- | --------- |
  |    6   |  15.000   |  18.000   |
  |    7   |  16.000   |  17.000   |
</details>

[Источник](https://www.geeksforgeeks.org/how-to-group-by-day-date-hour-month-or-year-in-sql-server/)
