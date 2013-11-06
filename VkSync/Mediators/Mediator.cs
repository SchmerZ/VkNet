using System;
using System.Collections.Generic;

namespace VkSync.Mediators
{
    public class Mediator<T>
    {
        #region Fields

        static readonly Mediator<T> _instance = new Mediator<T>();

        #endregion

        #region Ctor

        private Mediator()
        {
            Listeners = new Dictionary<T, List<Action<object>>>();
        }

        #endregion

        #region Properties

        public static Mediator<T> Instance
        {
            get
            {
                return _instance;
            }
        }

        private Dictionary<T, List<Action<object>>> Listeners
        {
            get;
            set;
        }

        #endregion

        #region Public Methods

        public void Register(T message, Action<object> callback)
        {
            if (!Listeners.ContainsKey(message))
            {
                Listeners.Add(message, new List<Action<object>> { callback });
            }
            else
            {
                Listeners[message].Add(callback);
            }
        }

        public void Notify(T message, object args)
        {
            if (Listeners.ContainsKey(message))
            {
                foreach (var callback in Listeners[message])
                {
                    callback(args);
                }
            }
        }

        #endregion
    }
}