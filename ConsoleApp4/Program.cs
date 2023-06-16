using System;
using System.Collections.Generic;

namespace ConsoleApp4
{
    public class MemoryManager
    {
        private const int MaxSegmentSize =1024; // Максимальный размер сегмента памяти

        private class MemoryBlock
        {
            public int Size { get; set; }
            public bool IsUsed { get; set; }
            public int Address { get; set; }
        }

        private readonly List<MemoryBlock> memoryBlocks;
        private int totalReservedMemory;

        public MemoryManager()
        {
            memoryBlocks = new List<MemoryBlock>();
            totalReservedMemory = 0;
        }

        public void AllocateMemory(int size)
        {
            if (size <= 0 || size > MaxSegmentSize - totalReservedMemory)
            {
                Console.WriteLine("Неудачное выделение памяти. Размер блока превышает доступное свободное место.");
                return;
            }

            int availableAddress = GetAvailableAddress();
            if (availableAddress == -1)
            {
                Console.WriteLine("Неудачное выделение памяти. Недостаточно свободного места.");
                return;
            }

            MemoryBlock block = new MemoryBlock
            {
                Size = size,
                IsUsed = true,
                Address = availableAddress
            };

            memoryBlocks.Add(block);
            totalReservedMemory += size;
            Console.WriteLine($"Выделена память блоком размером {size} на адресе {availableAddress}.");

            LogMemoryUsage();
        }

        public void FreeMemory(int address)
        {
            MemoryBlock block = memoryBlocks.Find(b => b.Address == address && b.IsUsed);
            if (block != null)
            {
                block.IsUsed = false;
                totalReservedMemory -= block.Size;
                Console.WriteLine($"Освобождена память блока размером {block.Size} на адресе {block.Address}.");

                LogMemoryUsage();

                OptimizeMemory();
            }
            else
            {
                Console.WriteLine($"Неудачное освобождение памяти. Блок на адресе {address} не найден или уже освобожден.");
            }
        }

        private int GetAvailableAddress()
        {
            int address = 0;
            foreach (MemoryBlock block in memoryBlocks)
            {
                if (!block.IsUsed)
                {
                    return block.Address;
                }
                address = block.Address + block.Size;
            }
            return (address < MaxSegmentSize) ? address : -1;
        }

        private void OptimizeMemory()
        {
            memoryBlocks.RemoveAll(block => !block.IsUsed);

            int currentAddress = 0;
            foreach (MemoryBlock block in memoryBlocks)
            {
                block.Address = currentAddress;
                currentAddress += block.Size;
            }
        }

        public void LogMemoryUsage()
        {
            Console.WriteLine("Текущее использование памяти:");
            foreach (MemoryBlock block in memoryBlocks)
            {
                Console.WriteLine($"Блок памяти размером {block.Size} на адресе {block.Address}. Статус: {(block.IsUsed ? "используется" : "не используется")}.");
            }

            Console.WriteLine($"Общий размер памяти, зарезервированный транслятором: {totalReservedMemory}.");
            Console.WriteLine();
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            MemoryManager memoryManager = new MemoryManager();

            // Запросить пять блоков памяти разного размера
            memoryManager.AllocateMemory(100);
            memoryManager.AllocateMemory(200);
            memoryManager.AllocateMemory(150);
            memoryManager.AllocateMemory(300);
            memoryManager.AllocateMemory(250);

            // Освободить память для 2 и 4 блока
            memoryManager.FreeMemory(200);
            memoryManager.FreeMemory(800);

            // Освободить память для 3 блока
            memoryManager.FreeMemory(300);

            // Запросить память чуть большего размера, чем можно выделить для одного сегмента
            memoryManager.AllocateMemory(1200);

            // Запросить три раза памяти размером в половину из предыдущего пункта
            memoryManager.AllocateMemory(600);
            memoryManager.AllocateMemory(600);
            memoryManager.AllocateMemory(600);

            // Вывести статистику использования памяти
            memoryManager.LogMemoryUsage();

            Console.ReadLine();
        }
    }
}
