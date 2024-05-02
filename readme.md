# Примечания
Используется сайт [sqliteonline.com](https://sqliteonline.com/), БД MS SQL 0.15.1 beta

# Задача 1
Решение:
|                               |         T1        | T2            | T3            | T4                 |
| ----------------------------- | ----------------- | ------------- | ------------- | ------------------ |
| Процесс 1                     | Begin Transaction |    0 --> 1    |               | Commit Transaction |
| Процесс 2 (Read commited)     |                   |               |        0      |                    |
| Процесс 3 (Read uncommited)   |                   |               |        1      |                    |

[Источник](https://habr.com/ru/articles/469415/), хотя задачу можно и так решить по определениям "commited" и "uncommited". В программирование _всегда_ говорящие ключевые слова и термины 

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

# Задача 2
<details>
  <summary>0. Создание таблиц, заполнение данными</summary>

  ```SQL
  BEGIN TRAN
  CREATE TABLE tblClient (
    ClientId int PRIMARY KEY
    , Name varchar(255)
  )
  CREATE TABLE tblClientPayments (
    ClientId int NOT NULL
    , PaymentDate date
    , PaymentSum int
    , CONSTRAINT FK_ClientId FOREIGN KEY (ClientId)
          REFERENCES tblClient (ClientId)
          ON DELETE CASCADE
  )
  INSERT INTO tblClient VALUES
    (1, 'Иванов Иван Иванович')
      , (2, 'Петров Петр Петрович')
      , (3, 'Кузнецова Анна Андреевна')
      , (4, 'Тихонова Светлана Анатольевна')
  INSERT INTO tblClientPayments VALUES 
    (1, '2022-02-01', 4000)
    , (1, '2022-02-05', 6000)
    , (3, '2022-03-02', 10000)
    , (4, '2022-03-09', 1000)
    , (1, '2022-03-16', 2000)
  COMMIT TRAN
  ```
</details>

<details>
  <summary>1. Выбрать всех клиентов (ClientId, Name) и посчитать по каждому общую сумму платежей (TotalSum)</summary>
  
  Немного информации: используется [RIGHT JOIN](https://learn.microsoft.com/ru-ru/sql/relational-databases/performance/joins?view=sql-server-ver16#fundamentals), где правая таблица имеет информацию о клиентах, однако клиенты могут совершить ни одного платежа. Для такого случая используется функция [ISNULL](https://learn.microsoft.com/ru-ru/sql/t-sql/functions/isnull-transact-sql?view=sql-server-ver16), которая заменяет NULL на 0
  ```SQL
  SELECT client.ClientId as ClientId
    , MAX(client.Name) as Name
    , ISNULL(SUM(payment.PaymentSum), 0)
  FROM tblClientPayments as payment
  RIGHT JOIN tblClient as client On payment.ClientId = client.ClientId
  GROUP BY client.ClientId
  ```
</details>

<details>
  <summary>2. Выбрать всех клиентов (ClientId, Name, TotalSum), у которых либо есть платежи после 05.03.2022, либо сумма всех платежей превышает 7000</summary>

  ```SQL
  SELECT
	client.ClientId as ClientId
    , MAX(client.Name) as Name
    , ISNULL(SUM(payment.PaymentSum), 0)
  FROM tblClientPayments as payment
  RIGHT JOIN tblClient as client On payment.ClientId = client.ClientId
  GROUP BY client.ClientId
  HAVING
    ISNULL(SUM(payment.PaymentSum), 0) > 7000
      OR
    MAX(payment.PaymentDate) > '2022-03-05'
  ```
</details>

<details>
  <summary>3. Выбрать постоянных клиентов (ClientId, Name), то есть клиентов, у которых есть платежи по крайней мере в двух разных календарных месяцах одного года</summary>

  1. Группировка сначала по клиенту, потом по году. В итоге мы имеем таблицу с каждым клиентом за год
  2. Исключаем строки, в которых количество месяцев в году (в которых были оплаты) не превышает 1

  ```SQL
  SELECT
  client.ClientId as ClientId
    , MAX(client.Name) as Name
  FROM tblClientPayments as payment
  RIGHT JOIN tblClient as client On payment.ClientId = client.ClientId
  GROUP BY client.ClientId, DATEPART(YEAR, payment.PaymentDate)
  HAVING COUNT(DISTINCT MONTH(payment.PaymentDate)) > 1
  ```
</details>

<details>
  <summary>4. Измените платёж Кузнецовой Анны Андреевны от 02.03.2022, применив к нему скидку 10% (Уменьшите на 10%). Напишите SQL запрос</summary>

  - Для производительности "уменьшите на 10%" заменено на "оставьте 90%" ( `x*0.9 = x-(x*0.1)` ).
  - Транзакция и SELECT-ы выполняют вспомогательную роль для удобства

  ```SQL
  BEGIN TRAN
  SELECT c.ClientId, c.Name, p.PaymentDate, p.PaymentSum FROM tblClientPayments p LEFT JOIN tblClient c ON p.ClientId=c.ClientId

  UPDATE payment
  SET payment.PaymentSum = payment.PaymentSum*0.9
  FROM tblClientPayments payment
    RIGHT JOIN tblClient AS client
      ON payment.ClientId = client.ClientId
  WHERE
    client.Name = 'Кузнецова Анна Андреевна'
      AND
      payment.PaymentDate = '2022-03-02'

  SELECT c.ClientId, c.Name, p.PaymentDate, p.PaymentSum FROM tblClientPayments p LEFT JOIN tblClient c ON p.ClientId=c.ClientId
  ROLLBACK TRAN
  ```
</details>

<details>
  <summary>5. Добавьте платёж Петрова Петра Петровича за текущую дату на сумму 18000</summary>

  **ВНИМАНИЕ!**. На сайте [sqliteonline.com](https://sqliteonline.com/) вся кириллица заменяется знаками вопроса, и поменять это никак нельзя! Поэтому запрос ниже добавляет две строчки, а не одну, так как строчки "Иванов Иван Иванович" и "Петров Петр Петрович" имеют одинаковую длину, которые потом превращаются в знаки вопроса и делает их равными.
  - Так же, как и в прошлой задаче, транзакция и SELECT-ы приводятся для удобства
  ```SQL
  BEGIN TRAN
  SELECT c.ClientId, c.Name, p.PaymentDate, p.PaymentSum FROM tblClientPayments p LEFT JOIN tblClient c ON p.ClientId=c.ClientId

  INSERT
      INTO tblClientPayments (ClientId, PaymentDate, PaymentSum)
      SELECT client.ClientId, getdate(), 18000
        FROM tblClient client
        WHERE client.Name = 'Петров Петр Петрович'

  SELECT c.ClientId, c.Name, p.PaymentDate, p.PaymentSum FROM tblClientPayments p LEFT JOIN tblClient c ON p.ClientId=c.ClientId
  ROLLBACK TRAN
  ```
</details>

# Задача 3
Думаю, что это задача на оконные функции [LEAD/LAG](http://www.sql-tutorial.ru/ru/book_lag_and_lead_functions.html). Так оно и было, но легче от этого не стало. По какой-то причине числа в типе numeric сходили с ума и выдавал абсолютно нелогичные ответы. Я посидел, подумал, и пришёл к выводу, что лучше оставить всё как есть. Буду разбираться с этим, когда кандидатуру одобрят. А пока, пускай таблица остаётся с NUMERIC, а функция работает с REAL. Выборка включает в себя подзапрос, так как применить WHERE на оконных функциях невозможно
<details>
  <summary>1. Создание функции для нахождения процента</summary>
  Функция конвертирует (при получении, не в самой функции) NUMERIC в REAL для нормальной работы

  ```SQL
  CREATE FUNCTION FindPercent (
    @current REAL,
    @previous REAL
  )
  RETURNS REAL
  AS
  BEGIN
    RETURN ((@previous-@current)/@previous*100);
  END
  ```
</details>

<details>
  <summary>2. Выборка</summary>

  ```SQL
  SELECT * FROM (
    SELECT
    t.PriceDate as DateT
      , t.PriceAssetId AssetId
      , t.ClosePrice PriceT
      , LAG(t.ClosePrice)OVER w AS PriceT1
      , dbo.FindPercent(t.ClosePrice, LAG(t.ClosePrice) OVER w) AS Divergence
      --, (t.ClosePrice/LAG(t.ClosePrice) OVER w) - 1 AS Divergence
      FROM dbo.tblClosePrice t
    WINDOW w AS (PARTITION BY t.PriceAssetId ORDER BY t.PriceAssetId)
  ) AS s1 WHERE ABS(s1.Divergence) > 30
  ```
</details>

# Задача 4

# Задача 5
Так как в данной задаче не указано, что именно нужно сделать, предполагается, что нужно определить количество замкнутых вагонов. Решение см. в [файле C#](Task5/Program.cs). Сам алгоритм начинается со слов "Начало алгоритма" в консоли и описан в статичном методе [Train.Steps()](Task5/Program.cs#L20)

## Текстовое описание решения
В списке у каждого действия (кроме 6.3.4, так как данное действие выполняется самим ЯП) есть ссылка на соответствующие действие в решение данной задачи на языке C#
1. Текущий вагон горит? Если да, то гасим свет, если нет, то зажигаем его [→](Task5/Program.cs#L26)
2. Запоминаем свет в текущем вагоне как "_целевой свет_" (**ЦС**) [→](Task5/Program.cs#L28)
3. Создаём счётчик вагонов. Предполагаем, что их всего 1. [→](Task5/Program.cs#L30)
4. Переходим к следующему вагону [→](Task5/Program.cs#L32)
5. Свет вагона **не** совпадает с ЦС: [→](Task5/Program.cs#L35)
   1. Переключаем свет в вагоне [→](Task5/Program.cs#L36)
   2. Увеличиваем счётчик поездов на 1 [→](Task5/Program.cs#L38)
6. Свет вагона совпадает с ЦС: [→](Task5/Program.cs#L40)
   1. Переключаем свет в текущем вагоне [→](Task5/Program.cs#L41)
   2. Возвращаемся назад на предполагаемое количество вагонов [→](Task5/Program.cs#L43)
   3. Свет вагона совпадает с ЦС [→](Task5/Program.cs#L45)
      1. Возвращаемся вперёд на предполагаемое количество вагонов [→](Task5/Program.cs#L47)
      2. Переключаем свет в текущем вагоне (которое равно ЦС) [→](Task5/Program.cs#L49)
      3. Увеличиваем счётчик поездов на 1 [→](Task5/Program.cs#L51)
      4. Возвращаемся к шагу _**4**_
   4. Свет вагона НЕ совпадает с ЦС [→](Task5/Program.cs#L53)
      1. Мы нашли количество вагонов. Предполагаемое количество вагонов и подтверждено [→](Task5/Program.cs#L56)

## Ограничения
- Минимум вагонов: **2** (включительно)
- При количестве вагонов больше 10 нумерация вагонов (косметическая) съезжает направо. Фикс данной проблемы предусматривает требует дополнительного кода, который автор посчитал нецелесообразным. Хотя алгоритм и будет работать, читать его вывод будет трудновато
## Сложность
Вычислено автором без всяких тестов (предположительная оценка на основе кода алгоритма)
- Лучший: **O(n)**
- Худший: **O(n²)**

## Примечания
- Используется _net8.0_ Для запуска можно просто ввести 2 команды: `cd Task5` и `dotnet run`
- Все переменные hardcoded в файле. Инструкции по их изменению:
  - Количество вагонов: можно изменить в строчке [114](Task5/Program.cs#L114)
  - Знаки включенного/выключенного света в вагоне: изменяется в строчке [81](Task5/Program.cs#L81)
## Пример вывода

```text
> Selecting length 7
         0   1   2   3   4   5   6
        [-] [+] [-] [+] [+] [+] [-] : Начало алгоритма
        [+] [+] [-] [+] [+] [+] [-] : Переключаем свет в текущем вагоне
        [+] [+] [-] [+] [+] [+] [-] : Решено, что все вагоны будут гореть
        [+] [+] [-] [+] [+] [+] [-] : Переход в следующий вагон
        [+] [+] [-] [+] [+] [+] [-] : Текущий вагон горит
        [+] [-] [-] [+] [+] [+] [-] : Переключаем свет в текущем вагоне
        [+] [-] [-] [+] [+] [+] [-] : Возвращаемся назад на 1 вагонов
        [+] [-] [-] [+] [+] [+] [-] : В текущем вагоне свет не изменился
        [+] [-] [-] [+] [+] [+] [-] : Возвращаемся вперёд на 1 вагонов
        [+] [+] [-] [+] [+] [+] [-] : Переключаем свет в текущем вагоне
        [+] [+] [-] [+] [+] [+] [-] : Увеличено количество вагонов на 1. Текущее: 2
        [+] [+] [-] [+] [+] [+] [-] : Переход в следующий вагон
        [+] [+] [-] [+] [+] [+] [-] : Текущий вагон потух
        [+] [+] [+] [+] [+] [+] [-] : Переключаем свет в текущем вагоне
        [+] [+] [+] [+] [+] [+] [-] : Увеличено количество вагонов на 1. Текущее: 3
        [+] [+] [+] [+] [+] [+] [-] : Переход в следующий вагон
        [+] [+] [+] [+] [+] [+] [-] : Текущий вагон горит
        [+] [+] [+] [-] [+] [+] [-] : Переключаем свет в текущем вагоне
        [+] [+] [+] [-] [+] [+] [-] : Возвращаемся назад на 3 вагонов
        [+] [+] [+] [-] [+] [+] [-] : В текущем вагоне свет не изменился
        [+] [+] [+] [-] [+] [+] [-] : Возвращаемся вперёд на 3 вагонов
        [+] [+] [+] [+] [+] [+] [-] : Переключаем свет в текущем вагоне
        [+] [+] [+] [+] [+] [+] [-] : Увеличено количество вагонов на 1. Текущее: 4
        [+] [+] [+] [+] [+] [+] [-] : Переход в следующий вагон
        [+] [+] [+] [+] [+] [+] [-] : Текущий вагон горит
        [+] [+] [+] [+] [-] [+] [-] : Переключаем свет в текущем вагоне
        [+] [+] [+] [+] [-] [+] [-] : Возвращаемся назад на 4 вагонов
        [+] [+] [+] [+] [-] [+] [-] : В текущем вагоне свет не изменился
        [+] [+] [+] [+] [-] [+] [-] : Возвращаемся вперёд на 4 вагонов
        [+] [+] [+] [+] [+] [+] [-] : Переключаем свет в текущем вагоне
        [+] [+] [+] [+] [+] [+] [-] : Увеличено количество вагонов на 1. Текущее: 5
        [+] [+] [+] [+] [+] [+] [-] : Переход в следующий вагон
        [+] [+] [+] [+] [+] [+] [-] : Текущий вагон горит
        [+] [+] [+] [+] [+] [-] [-] : Переключаем свет в текущем вагоне
        [+] [+] [+] [+] [+] [-] [-] : Возвращаемся назад на 5 вагонов
        [+] [+] [+] [+] [+] [-] [-] : В текущем вагоне свет не изменился
        [+] [+] [+] [+] [+] [-] [-] : Возвращаемся вперёд на 5 вагонов
        [+] [+] [+] [+] [+] [+] [-] : Переключаем свет в текущем вагоне
        [+] [+] [+] [+] [+] [+] [-] : Увеличено количество вагонов на 1. Текущее: 6
        [+] [+] [+] [+] [+] [+] [-] : Переход в следующий вагон
        [+] [+] [+] [+] [+] [+] [-] : Текущий вагон потух
        [+] [+] [+] [+] [+] [+] [+] : Переключаем свет в текущем вагоне
        [+] [+] [+] [+] [+] [+] [+] : Увеличено количество вагонов на 1. Текущее: 7
        [+] [+] [+] [+] [+] [+] [+] : Переход в следующий вагон
        [+] [+] [+] [+] [+] [+] [+] : Текущий вагон горит
        [-] [+] [+] [+] [+] [+] [+] : Переключаем свет в текущем вагоне
        [-] [+] [+] [+] [+] [+] [+] : Возвращаемся назад на 7 вагонов
        [-] [+] [+] [+] [+] [+] [+] : В текущем вагоне свет изменился => мы сделали полный круг за 7 вагонов
> Finished. Answer: 7
```
