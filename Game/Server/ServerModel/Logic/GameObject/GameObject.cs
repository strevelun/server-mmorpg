using ServerModel.Logic.Component;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServerModel.Logic
{
    public class GameObject
    {
        private Dictionary<Type, IComponent> _components = new Dictionary<Type, IComponent>();
        private GameObject _parenet = null;

        public void OnInit()
        {
            foreach (var component in _components.Values)
            {
                component.OnInit();
            }
        }

        public void OnRelease()
        {
            foreach (var component in _components.Values)
            {
                component.OnRelease();
            }
        }

        public void AddComponent<T>(T instance) where T : class, IComponent
        {
            this._components.Add(typeof(T), instance);

            instance.GameObject = this;
        }
    }
}
