﻿using System;

namespace KafkaEdu
{
    class Program
    {
        // Какфка содержит в себе несколько брокеров сообщений, которые вместе образуют кластер
        // Они принимают сообщения и ставят их в очередь, пока они не будут обработаны
        // Отправляют сообщения продьюсеры, а принимаю консьюмеры
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }
    }
}
