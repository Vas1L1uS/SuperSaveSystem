using System;

namespace AAAProject.Scripts.Extensions.Interfaces
{
    public interface INotifyDestroy
    {
        /// <summary>
        /// Вызывать в методе OnDestroy()
        /// </summary>
        event Action BeingDestroyed;
    }
}