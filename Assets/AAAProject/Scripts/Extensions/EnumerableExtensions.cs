using System;
using System.Collections.Generic;
using System.Linq;

namespace AAAProject.Scripts.Extensions
{
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Получить следующий индекс элемента в списке
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <param name="currentIndex">Текущий индекс</param>
        /// <returns>Следующий индекс</returns>
        public static int GetNextIndex<T>(this List<T> input, int currentIndex)
        {
            if (currentIndex == input.Count - 1)
            {
                return 0;
            }
            else
            {
                return currentIndex + 1;
            }
        }

        /// <summary>
        /// Получить следующий индекс элемента в массиве
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <param name="currentIndex">Текущий индекс</param>
        /// <returns>Следующий индекс</returns>
        public static int GetNextIndex<T>(this T[] input, int currentIndex)
        {
            if (currentIndex == input.Length - 1)
            {
                return 0;
            }
            else
            {
                return currentIndex + 1;
            }
        }

        /// <summary>
        /// Получить предыдущий индекс элемента в списке
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <param name="currentIndex">Текущий индекс</param>
        /// <returns>Предыдущий индекс</returns>
        public static int GetPreviousIndex<T>(this List<T> input, int currentIndex)
        {
            if (currentIndex == 0)
            {
                return input.Count - 1;
            }
            else
            {
                return currentIndex - 1;
            }
        }

        /// <summary>
        /// Получить предыдущий индекс элемента в массиве
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <param name="currentIndex">Текущий индекс</param>
        /// <returns>Предыдущий индекс</returns>
        public static int GetPreviousIndex<T>(this T[] input, int currentIndex)
        {
            if (currentIndex == 0)
            {
                return input.Length - 1;
            }
            else
            {
                return currentIndex - 1;
            }
        }

        /// <summary>
        /// Переместить элементы на шаг -1
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        public static void StepLeftElements<T>(this List<T> input)
        {
            var first = input.First();

            for (int i = 0; i < input.Count - 1; i++)
            {
                input[i] = input[i + 1];
            }

            input[input.Count - 1] = first;
        }

        /// <summary>
        /// Переместить элементы на шаг -1
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        public static void StepLeftElements<T>(this T[] input)
        {
            var first = input.First();

            for (int i = 0; i < input.Length - 1; i++)
            {
                input[i] = input[i + 1];
            }

            input[input.Length - 1] = first;
        }

        /// <summary>
        /// Переместить элементы на шаг +1
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        public static void GetListStepRightElements<T>(this List<T> input)
        {
            for (int i = 0; i < input.Count - 1; i++)
            {
                input[i + 1] = input[i];
            }
        }

        /// <summary>
        /// Переместить элементы на шаг +1
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        public static void GetListStepRightElements<T>(this T[] input)
        {
            for (int i = 0; i < input.Length - 1; i++)
            {
                input[i + 1] = input[i];
            }
        }

        /// <summary>
        /// Переместить элементы на указаный шаг
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        public static void GetArrayStepElements<T>(this List<T> input, int steps)
        {
            if (steps >= input.Count) throw new Exception("Невозможно переместить элементы на шаг првышающиц размер списка!");

            var temp = input;

            int i = 0;

            if (steps < 0) i = steps;

            for (; i < input.Count - 1; i++)
            {

                if (steps < 0)
                {
                    if (i + steps < 0)
                    {
                        input[input.Count + (i + steps)] = temp[i];
                        continue;
                    }
                }
                else
                {
                    if (i + steps >= input.Count)
                    {
                        input[input.Count - (i + steps)] = temp[i];
                        continue;
                    }
                }

                input[i + steps] = temp[i];
            }
        }

        /// <summary>
        /// Переместить элементы на указаный шаг
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        public static void GetArrayStepElements<T>(this T[] input, int steps)
        {
            if (steps >= input.Length) throw new Exception("Невозможно переместить элементы на шаг првышающиц размер массива!");

            var temp = input;

            int i = 0;

            if (steps < 0) i = steps;

            for (; i < input.Length - 1; i++)
            {

                if (steps < 0)
                {
                    if (i + steps < 0)
                    {
                        input[input.Length + (i + steps)] = temp[i];
                        continue;
                    }
                }
                else
                {
                    if (i + steps >= input.Length)
                    {
                        input[input.Length - (i + steps)] = temp[i];
                        continue;
                    }
                }

                input[i + steps] = temp[i];
            }
        }

        public static void SetLast<T>(this List<T> input, T value)
        {
            input[input.Count - 1] = value;
        }

        public static void SetLast<T>(this T[] input, T value)
        {
            input[input.Length - 1] = value;
        }
    }
}