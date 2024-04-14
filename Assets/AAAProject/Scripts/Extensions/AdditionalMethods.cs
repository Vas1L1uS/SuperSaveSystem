using System;

namespace AAAProject.Scripts.Extensions
{
    public static class AdditionalMethods
    {
        /// <summary>
        /// Получить зациклинное число в заданном диапазоне
        /// </summary>
        /// <param name="x">Число</param>
        /// <param name="min">Минимальный предел</param>
        /// <param name="max">Максимальный предел</param>
        /// <returns>Зациклинное число</returns>
        public static int GetCycleInt(int x, int min, int max)
        {
            int result = x;
            
            if (x > max)
            {
                result = min + (x - max - 1) % (max - min + 1);
            }
            else if (x < min)
            {
                result = max - (min - x - 1) % (max - min + 1);
            }

            return result;
        }
        
        /// <summary>
        /// Получить зациклинное число в заданном диапазоне
        /// </summary>
        /// <param name="x">Число</param>
        /// <param name="min">Минимальный предел</param>
        /// <param name="max">Максимальный предел</param>
        /// <returns>Зациклинное число</returns>
        public static float GetCycleFloat(float x, float min, float max)
        {
            float result = x;
            int intX = Convert.ToInt32(x * 1000);
            int intMin = Convert.ToInt32(min * 1000);
            int intMax = Convert.ToInt32(max * 1000);
            int intResult = GetCycleInt(intX, intMin, intMax);
            result = Convert.ToSingle(intResult) / 1000;

            return result;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bit"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        public static bool CheckBitInNumber(int bit, int number)
        {
            bit = (int)Math.Pow(2, bit);

            for (int i = 31; i >= 0; i--)
            {
                if (Math.Pow(2, i) == bit)
                {
                    if (number - Math.Pow(2, i) >= 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    if (number - Math.Pow(2, i) >= 0)
                    {
                        number -= (int)Math.Pow(2, i);
                        continue;
                    }
                    else
                    {
                        continue;
                    }
                }
            }

            return false;
        }
    }
}