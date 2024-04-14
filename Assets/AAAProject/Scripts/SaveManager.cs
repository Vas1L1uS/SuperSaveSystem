using UnityEngine;

namespace AAAProject.Scripts
{
    public static class SaveManager
    {
        /// <summary>
        /// Записывает загруженные данные в публичные поля класса или структуры
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item">Класс или структура</param>
        /// <param name="hasKey">Найден ли ключ?</param>
        /// <param name="key">Ключ</param>
        public static void Load<T>(ref T item, string key, out bool hasKey)
        {
            hasKey = false;

            if (!PlayerPrefs.HasKey(key))
            {
                return;
            }
            
            var type = item.GetType();
            string json = PlayerPrefs.GetString(key);
            item = (T)JsonUtility.FromJson(json, type);
            hasKey = true;
        }

        /// <summary>
        /// Записывает загруженные данные в переменную
        /// </summary>
        /// <param name="item">Переменная в которую запишется значение</param>
        /// <param name="hasKey">Найден ли ключ?</param>
        /// <param name="key">Ключ</param>
        public static void Load(ref int item, string key, out bool hasKey)
        {
            hasKey = false;

            if (!PlayerPrefs.HasKey(key))
            {
                return;
            }

            item = PlayerPrefs.GetInt(key);
            hasKey = true;
        }

        /// <summary>
        /// Записывает загруженные данные в переменную
        /// </summary>
        /// <param name="item">Переменная в которую запишется значение</param>
        /// <param name="hasKey">Найден ли ключ?</param>
        /// <param name="key">Ключ</param>
        public static void Load(ref float item, string key, out bool hasKey)
        {
            hasKey = false;

            if (!PlayerPrefs.HasKey(key))
            {
                return;
            }
            
            item = PlayerPrefs.GetFloat(key);
            hasKey = true;
        }

        /// <summary>
        /// Записывает загруженные данные в переменную
        /// </summary>
        /// <param name="item">Переменная в которую запишется значение</param>
        /// <param name="hasKey">Найден ли ключ?</param>
        /// <param name="key">Ключ</param>
        public static void Load(ref string item, string key, out bool hasKey)
        {
            hasKey = false;

            if (!PlayerPrefs.HasKey(key))
            {
                return;
            }

            item = PlayerPrefs.GetString(key);
            hasKey = true;
        }
        
        /// <summary>
        /// Записывает загруженные данные в переменную
        /// </summary>
        /// <param name="item">Переменная в которую запишется значение</param>
        /// <param name="hasKey">Найден ли ключ?</param>
        /// <param name="key">Ключ</param>
        public static void Load(ref bool item, string key, out bool hasKey)
        {
            hasKey = false;
            
            if (!PlayerPrefs.HasKey(key))
            {
                return;
            }
            
            int result = PlayerPrefs.GetInt(key);

            if (result != 0)
            {
                item = true;
            }
            else
            {
                item = false;
            }

            hasKey = true;
        }

        /// <summary>
        /// Записывает загруженные данные в публичные поля класса или структуры
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item">Класс</param>
        /// <param name="key">Ключ</param>
        public static void Load<T>(ref T item, string key)
        {
            var type = item.GetType();
            if (!PlayerPrefs.HasKey(key))
            {
                return;
            }

            string json = PlayerPrefs.GetString(key);
            item = (T)JsonUtility.FromJson(json, type);
        }

        /// <summary>
        /// Записывает загруженные данные в переменную
        /// </summary>
        /// <param name="item">Переменная в которую запишется значение</param>
        /// <param name="key">Ключ</param>
        public static void Load(ref int item, string key)
        {
            if (PlayerPrefs.HasKey(key))
            {
                item = PlayerPrefs.GetInt(key);
            }
        }

        /// <summary>
        /// Записывает загруженные данные в переменную
        /// </summary>
        /// <param name="item">Переменная в которую запишется значение</param>
        /// <param name="key">Ключ</param>
        public static void Load(ref float item, string key)
        {
            if (PlayerPrefs.HasKey(key))
            {
                item = PlayerPrefs.GetFloat(key);
            }
        }

        /// <summary>
        /// Записывает загруженные данные в переменную
        /// </summary>
        /// <param name="item">Переменная в которую запишется значение</param>
        /// <param name="key">Ключ</param>
        public static void Load(ref string item, string key)
        {
            if (PlayerPrefs.HasKey(key))
            {
                item = PlayerPrefs.GetString(key);
            }
        }
        
        /// <summary>
        /// Записывает загруженные данные в переменную
        /// </summary>
        /// <param name="item">Переменная в которую запишется значение</param>
        /// <param name="key">Ключ</param>
        public static void Load(ref bool item, string key)
        {
            if (PlayerPrefs.HasKey(key))
            {
                int result = PlayerPrefs.GetInt(key);

                if (result != 0)
                {
                    item = true;
                }
                else
                {
                    item = false;
                }
            }
        }

        /// <summary>
        /// Сохраняет значения только в публичные поля класса или структуры
        /// </summary>
        /// <typeparam name="T">Класс или переменная</typeparam>
        /// <param name="item">Класс</param>
        /// <param name="key">Ключ</param>
        public static void Save<T>(T item, string key)
        {
            string json = JsonUtility.ToJson(item);

            PlayerPrefs.SetString(key, json);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// Сохраняет значения переменной по ключу
        /// </summary>
        /// <param name="item">Класс</param>
        /// <param name="key">Ключ</param>
        public static void Save(int item, string key)
        {
            PlayerPrefs.SetInt(key, item);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// Сохраняет значения переменной по ключу
        /// </summary>
        /// <param name="item">Класс</param>
        /// <param name="key">Ключ</param>
        public static void Save(float item, string key)
        {
            PlayerPrefs.SetFloat(key, item);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// Сохраняет значения переменной по ключу
        /// </summary>
        /// <param name="item">Класс</param>
        /// <param name="key">Ключ</param>
        public static void Save(string item, string key)
        {
            PlayerPrefs.SetString(key, item);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// Сохраняет значения переменной по ключу
        /// </summary>
        /// <param name="item">Класс</param>
        /// <param name="key">Ключ</param>
        public static void Save(bool item, string key)
        {
            int result = 0;

            if (item)
            {
                result = 1;
            }

            PlayerPrefs.SetInt(key, result);
            PlayerPrefs.Save();
        }
    }
}