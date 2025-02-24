﻿namespace SharpEdus;

public class Threads
{
    class Counter
    {
        public int X;
        public int Y;
        public Counter() { }
        public Counter(int x, int y) { X = x; Y = y; }
        public void Count()
        {
            for (int i = 1; i < 9; i++)
            {
                Console.WriteLine("Вторичный поток:");
                Console.WriteLine(i * X * Y);
                Thread.Sleep(400);
            }
        }
    }

    static volatile bool stop;

    static void Main_()
    {
        //General_();
        //Join_();
        //BreakThread_();
        //ThreadPriorities_();
        //ThreadPool_();
        //SecondaryThreads_();
        //Volatile_();
        //AccessStaticVariables_();
        //Lock_();
        //Mutex_();
        //Semaphore_();
        //AutoResetEvent_();
        //ManualResetEvent_();
        //CountdownEvent_();
        //RegisteredWaitHandle_();
        //EventWaitHandle_();
        //SendArgumentToThread_();
        //SendSeveralArgumentsToThreadNotSequre_();
        //SendSeveralArgumentsToThreadSequre_();
    }

    static void General_()
    {
        // Каждому потоку выделяется квант времени

        // Статусы потока содержатся в перечислении ThreadState:
        // Unstarted        - еще не был запущен
        // Running          - запущен и работает
        // Background       - выполняется в фоновом режиме
        // StopRequested    - получил запрос на остановку
        // Stopped          - завершен
        // SuspendRequested - получил запрос на приостановку
        // Suspended        - приостановлен
        // WaitSleepJoin    - заблокирован методами Sleep или Join
        // AbortRequested   - для потока вызван метод Abort, но остановка еще не произошла
        // Aborted          - остановлен, но еще окончательно не завершен

        object lockCompleted = new object();
        void F1()
        {
            lock (lockCompleted) 
            { 
                Console.WriteLine("F1"); 
            }
        }

        var thread = new Thread(F1);
        //new Thread(F1).Start(); // Эквивалентно предыдущей записи
        thread.Name = "Thread 1";
        thread.Start();

        // CurrentThread - поток, выполняющий данный метод
        Console.WriteLine("IsAlive " + Thread.CurrentThread.IsAlive);
        Console.WriteLine("IsBackground " + Thread.CurrentThread.IsBackground);
        Console.WriteLine("ThreadState " + Thread.CurrentThread.ThreadState);
    }

    static void BreakThread_()
    {
        void F1()
        {
            for (; ; )
                Console.WriteLine("for");
        }

        Console.WriteLine("Start" + Thread.CurrentThread.ManagedThreadId);
        var thread = new Thread(F1);
        thread.Start();
        Thread.Sleep(2000);
        thread.Interrupt(); // Через 2 секунды метод прервется
                            // Abort устарел
        Console.WriteLine("End" + Thread.CurrentThread.ManagedThreadId);
    }

    static void ThreadPriorities_()
    {
        // Приоритеты потоков, располагаются в перечислении ThreadPriority
        // Влияет на время, выделяемое потоку процессором

        // - Lowest
        // - BelowNormal
        // - Normal - по умолчанию
        // - AboveNormal
        // - Highest

        static void F1()
        {
            Console.WriteLine("Start " + Thread.CurrentThread.ManagedThreadId + " " + Thread.CurrentThread.Priority);

            for (int i = 0; i < 3; i++)
            {
                Console.WriteLine("-- Loop " + Thread.CurrentThread.ManagedThreadId + " " + Thread.CurrentThread.Priority);
            }

            Console.WriteLine("End " + Thread.CurrentThread.ManagedThreadId + " " + Thread.CurrentThread.Priority);
        }

        var thread = new Thread[5];

        for (int i = 0; i < 5; i++)
        {
            thread[i] = new Thread(F1);
        }

        thread[0].Priority = ThreadPriority.Lowest;
        thread[1].Priority = ThreadPriority.Normal;
        thread[2].Priority = ThreadPriority.Highest;
        thread[3].Priority = ThreadPriority.BelowNormal;
        thread[4].Priority = ThreadPriority.AboveNormal;

        for (int i = 0; i < 5; i++)
        {
            thread[i].Start();
        }
    }

    static void ThreadPool_()
    {
        void F1() // Cколько потоков доступно в пуле
        {
            int availableThreads, availableIOThreads;
            ThreadPool.GetAvailableThreads(out availableThreads, out availableIOThreads);
            Console.WriteLine("Threads in pool " + availableThreads);
            Console.WriteLine("Free threads in pool " + availableIOThreads);
        }

        void F2(object state)
        {
            Thread.CurrentThread.Name = "1";
            Console.WriteLine("Start thread " + Thread.CurrentThread.Name);
            Thread.Sleep(1000);
            Console.WriteLine("End thread " + Thread.CurrentThread.Name);
        }

        void F3(object state)
        {
            Thread.CurrentThread.Name = "2";
            Console.WriteLine("Start thread " + Thread.CurrentThread.Name);
            Thread.Sleep(1000);
            Console.WriteLine("End thread " + Thread.CurrentThread.Name);
        }

        Console.WriteLine("Start");
        ThreadPool.QueueUserWorkItem(new WaitCallback(F2));
        F1();
        ThreadPool.QueueUserWorkItem(F3); // Помещаем F3 в очередь пула потоков
        Thread.Sleep(1000);
        Console.WriteLine("End");
    }

    static void ForegroundBackground_()
    {
        // Есть два варианта работы вторичных потоков:
        // - Foreground - будет работать после завршения первичного потока (по умолчанию)
        // - Background - завершает работу вместе с первичным потоком

        object lockCompleted = new object();
        void F1()
        {
            lock (lockCompleted) 
                Console.WriteLine("F1"); 
        }

        var thread = new Thread(F1);
        thread.IsBackground = true;
        thread.Start();
    }

    // Синхронизация

    // Атрибут означает, что каждый поток работает со своей статической переменной, а не с общей для всех
    [ThreadStatic]
    static int counter = 0;

    static void Join_()
    {
        // Заставляет первичный поток ждать завершения вторичного
        // С Join будет: 1 2 3
        // Без Join:     1 3 2
        
        void F1()
        {
            Thread.Sleep(500);
            Console.WriteLine("F1: " + Thread.CurrentThread.ManagedThreadId);
        }

        Console.WriteLine("Start: " + Thread.CurrentThread.ManagedThreadId);

        var thread = new Thread(F1);
        thread.Start();
        thread.Join();

        Console.WriteLine("End: " + Thread.CurrentThread.ManagedThreadId);
    }

    static void AccessStaticVariables_()
    {
        static void F1()
        {
            if (counter < 10)
            {
                counter++;
                Console.WriteLine("Start " + Thread.CurrentThread.ManagedThreadId + ", counter " + counter);
                var thread = new Thread(F1); // Плохо - каждый раз создаем новый поток
                thread.Start();
                thread.Join();
            }
        }

        Console.WriteLine("Start " + Thread.CurrentThread.ManagedThreadId);
        var thread = new Thread(F1);
        thread.Start();
        thread.Join();
        Console.WriteLine("End " + Thread.CurrentThread.ManagedThreadId);
    }

    static void Interlocked_()
    {
        var count_1 = 0;
        var count_2 = 0;

        // Результат каждый раз будет разный
        Task.WaitAll
        (
            Task.Run(() => { for (int i = 0; i < 10000; i++) count_1++; }),
            Task.Run(() => { for (int i = 0; i < 10000; i++) count_1++; }),
            Task.Run(() => { for (int i = 0; i < 10000; i++) count_1++; }),
            Task.Run(() => { for (int i = 0; i < 10000; i++) count_1++; })
        );

        // Результат каждый раз будет одинаковый - делать так при работе с потоками
        Task.WaitAll
        (
            Task.Run(() => { for (int i = 0; i < 10000; i++) Interlocked.Increment(ref count_2); }),
            Task.Run(() => { for (int i = 0; i < 10000; i++) Interlocked.Increment(ref count_2); }),
            Task.Run(() => { for (int i = 0; i < 10000; i++) Interlocked.Increment(ref count_2); }),
            Task.Run(() => { for (int i = 0; i < 10000; i++) Interlocked.Increment(ref count_2); })
        );

        Console.WriteLine(count_1);
        Console.WriteLine(count_2);
    }

    static void Lock_()
    {
        // Внутри lock может одновременно работать один поток
        // locker - Это пустой объект-заглушка типа object
        
        // Внутри lock нельзя вызывать await, т.к. возможна ситуация, что объект синхронизации
        // берется одним потоком, а отпускается другим, а это нарушает логику

        // Может привести к deadlock

        object locker = new object();
        string value = string.Empty;
        void F1()
        {
            Thread.Sleep(1000);
            lock (locker) { value = "Updating value"; }
        }

        Task.Factory.StartNew(F1);
        Console.WriteLine("Main thread is waiting");

        lock (locker)
        {
            value = "Updating value in main thread";
            Console.WriteLine("Value: " + value);
        }

        Thread.Sleep(1000);
        Console.WriteLine("Released worker thread");
    }

    static void Deadlock_()
    {
        /*
        
        Возможен дедлок, если сначала будет вызван Method1(),
        а затем в другом потоке будет вызван Method3()
        Оба метода используют блокировку lock1, а Method2() использует блокировку lock2,
        что может привести к блокировке обоих потоков

        public class DeadlockExample
        {
            private object lock1 = new object();
            private object lock2 = new object();

            public void Method1()
            {
                lock (lock1)
                {
                    Method2();
                }
            }

            public void Method2()
            {
                lock (lock2)
                {
                    Method3();
                }
            }

            public void Method3()
            {
                lock (lock1)
                {
                    // do something
                }
            }
        }

        */
    }

    static void Semaphore_()
    {
        // Позволяет ограничить количество потоков, имеющих доступ к определенному ресурсу
        // Достигается путем создания счетчика, который уменьшается, когда поток получает доступ к ресурсу,
        // и увеличивается, когда поток освобождает ресурс
        // Если счетчик равен нулю - другие потоки должны ждать, пока ресурс освободится

        // Может быть полезным, например, для ограничения количества запросов к БД
        // SemaphoreSlim меньше нагружает процессор и работает в рамках одного процесса

        Semaphore semaphore;

        void F1(object number)
        {
            semaphore.WaitOne(); // Ожидание получения семафора
            Console.WriteLine("Begin " + Thread.CurrentThread.ManagedThreadId);
            Thread.Sleep(2000);
            Console.WriteLine("End " + Thread.CurrentThread.ManagedThreadId);
            semaphore.Release(); // Освобождаем семафор
        }

        // 1й аргумент - начальное количество слотов
        // 2й аргумент - максимальное количество слотов
        // 3й аргумент - имя семафора
        semaphore = new Semaphore(2, 4, "Semaphore");
        semaphore.Release(2); // Убираем 2 из предыдущей строки, теперь семафор могут использовать максимальное число потоков - 4
        
        for (int i = 0; i < 5; i++)
        {
            new Thread(F1).Start(i);
        }
    }
    
    static void Mutex_()
    {
        // Позволяет блокировать доступ к ресурсу не только в пределах одного процесса, но и между процессами
        // Работает путем создания объекта блокировки, который может быть захвачен только одним потоком
        // в любой момент времени
        // Если другой поток попытается захватить блокировку, пока она уже занята,
        // он будет заблокирован и будет ждать, пока блокировка не будет освобождена первым потоком
        // Блокировка может быть освобождена явным вызовом функции или автоматически при завершении
        // выполнения кода, который ее захватил

        // Когда выполнение дойдет до mutex.WaitOne(), поток будет ожидать, пока не освободится мьютекс
        // После освобождения продолжит работу
        // Нет межпроцессорной синхронизации

        var mutex = new Mutex(false, "Mutex");

        void F1()
        {
            mutex.WaitOne();
            Console.WriteLine("F1");
            F2();                   // Запускаем метод, который тоже использует данный конкретный mutex
                                    // Пока F6_2() не отработает, F6_2 будет ждать
            mutex.ReleaseMutex();
        }

        void F2()
        {
            mutex.WaitOne();        // Приостанавливаем выполнение потока, пока не будет получен mutex
            Console.WriteLine("F2");
            mutex.ReleaseMutex();   // После выполнения всех действий, когда мьютекс не нужен,
                                    // поток освобождает
        }

        var thread = new Thread[5];
        for (int i = 0; i < 5; i++)
        {
            thread[i] = new Thread(F1)
            {
                Name = i.ToString()
            };

            thread[i].Start();
        }
    }

    // Обработка событий

    static void AutoResetEvent_()
    {
        // Если в программе несколько объектов AutoResetEvent, можем использовать для отслеживания
        // состояния этих объектов методы WaitAll и WaitAny, которые в качестве параметра принимают
        // массив объектов класса WaitHandle - базового класса для AutoResetEvent
        // AutoResetEvent.WaitAll(new WaitHandle[] { waitHandler });

        // false - установка в несигнальное состояние
        var autoResetEvent = new AutoResetEvent(false);
        
        void F1()
        {
            Console.WriteLine("1");
            autoResetEvent.WaitOne();   // Один поток продолжает работу, остальные ждут вызова Set()
            Console.WriteLine("2");
            autoResetEvent.WaitOne();
            Console.WriteLine("3");
        }

        void F2()
        {
            Console.WriteLine("1 Begin");
            autoResetEvent.WaitOne();
            Console.WriteLine("1 End");
        }

        void F3()
        {
            Console.WriteLine("2 Begin");
            autoResetEvent.WaitOne();
            Console.WriteLine("2 End");
        }

        var thread = new Thread(F1);
        thread.Start();
        autoResetEvent.Set();   // Уведомляем ожидающие потоки, что объект
                                // снова находится в сигнальном состоянии и следующий
                                // поток может начать работу, а остальные снова ждут
        autoResetEvent.Set();

        new Thread(F2).Start();
        new Thread(F3).Start();
        autoResetEvent.Set();
        autoResetEvent.Set();
    }

    static void ManualResetEvent_()
    {
        // ManualResetEventSlim работает на уровне потоков, а не процессов

        // false - установка в несигнальное состояние
        var manualResetEvent = new ManualResetEvent(false);

        void F1()
        {
            Console.WriteLine("1 Begin");
            manualResetEvent.WaitOne();
            //resetEvent.WaitOne();
            Console.WriteLine("1 End");
        }

        void F2()
        {
            Console.WriteLine("2 BEGIN");
            manualResetEvent.WaitOne();
            Console.WriteLine("2 END");
        }

        new Thread(F1).Start();
        new Thread(F2).Start();
        manualResetEvent.Set();     // Сигнал всем потокам

        //Task.Factory.StartNew(F10_1);
        //Task.Factory.StartNew(F10_2);
        //resetEvent.Set();
    }

    static void CountdownEvent_()
    {
        var countdown = new CountdownEvent(5);

        void F1() 
        { 
            Console.WriteLine("F1"); 
        }

        Task.Factory.StartNew(F1);
        Task.Factory.StartNew(F1);
        Task.Factory.StartNew(F1);

        countdown.Wait();
    }

    static void RegisteredWaitHandle_()
    {
        void F1(object state, bool istTmeOut) 
        { 
            Console.WriteLine("Signal"); 
        }

        var auto = new AutoResetEvent(false);
        var callback = new WaitOrTimerCallback(F1);
        RegisteredWaitHandle registeredWaitHandle = ThreadPool.RegisterWaitForSingleObject(
            auto,       // от кого ждать сигнал
            callback,   // что выполнять
            null,       // 1й аргумент callback метода
            2000,       // интервал между вызовами callback метода
            true);      // true - вызвать callback метод 1 раз,
                        // false - вызвать с интервалом
        auto.Set();
        registeredWaitHandle.Unregister(auto);
    }

    static void EventWaitHandle_()
    {
        EventWaitHandle handle = null;
        void F1()
        {
            handle.WaitOne(); // Приостановка потока

            while (true)
            {
                Console.WriteLine("1");
                Thread.Sleep(300);
            }
        }

        handle = new EventWaitHandle(
            false,                      // несигнальное состояние
            EventResetMode.ManualReset, // тип события
            "GlobalEvent::GUID");       // имя объекта синхронизации в ОС. Если объект с таким
                                        // именем существует, будет получена ссылка на него

        var thread = new Thread(F1) 
        { 
            IsBackground = true 
        };

        thread.Start();
    }

    // Передача аргументов

    static void SendArgumentToThread_()
    {
        void F1(object x)
        {
            for (int i = 1; i < 9; i++)
            {
                int n = (int)x;

                Console.WriteLine("Второй поток:");
                Console.WriteLine(i * n);
                Thread.Sleep(400);
            }
        }

        // Передача в метод F16_1 аргумента типа int
        var thread = new Thread(new ParameterizedThreadStart(F1));
        thread.Start(5);

        for (int i = 1; i < 9; i++)
        {
            Console.WriteLine("Main thread: ");
            Console.WriteLine(i * i);
            Thread.Sleep(300);
        }

        // Альтернативный способ запуска
        //new Thread(() =>
        //{
        //    Thread.Sleep(1000);
        //    Console.WriteLine("thread1");
        //}).Start();
    }

    static void SendSeveralArgumentsToThreadNotSequre_()
    {
        // Для передачи в поток нескльких аргументов нужен класс
        // Метод Thread.Start не является типобезопасным и мы можем передать в него любой тип,
        // а потом придется приводить переданный объект к нужному типу
        
        void F1(object obj)
        {
            for (int i = 1; i < 9; i++)
            {
                Counter c = (Counter)obj;
                Console.WriteLine("Второй поток:");
                Console.WriteLine(i * c.X * c.Y);
            }
        }

        Counter counter = new Counter();
        counter.X = 4;
        counter.Y = 5;

        var thread = new Thread(new ParameterizedThreadStart(F1));
        thread.Start(counter);
    }

    static void SendSeveralArgumentsToThreadSequre_()
    {
        // Рекомендуется объявлять все используемые методы и переменные в специальном классе,
        // а в основной программе запускать поток через ThreadStart
        
        var counter = new Counter(5, 4);
        var thread = new Thread(new ThreadStart(counter.Count));
        thread.Start();
    }
}
