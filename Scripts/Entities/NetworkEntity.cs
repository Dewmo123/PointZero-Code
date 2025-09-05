using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Scripts.Entities
{
    public abstract class NetworkEntity : MonoBehaviour
    {
        public int Index { get; protected set; } = 0;

        protected Dictionary<Type, IEntityComponent> _components;

        protected void InitEntity()
        {
            _components = new Dictionary<Type, IEntityComponent>();
            AddComponents();
            InitializeComponents();
        }

        private void AddComponents()
        {
            GetComponentsInChildren<IEntityComponent>().ToList()
                .ForEach(component => _components.Add(component.GetType(), component));
        }

        private void InitializeComponents()
        {
            _components.Values.ToList().ForEach(component => component.Initialize(this));
        }

        public T GetCompo<T>(bool isDerived = false) where T : IEntityComponent
        {
            if (_components.TryGetValue(typeof(T), out IEntityComponent component))
                return (T)component;

            if (!isDerived) return default(T);

            Type findType = _components.Keys.FirstOrDefault(type => type.IsSubclassOf(typeof(T)));
            if (findType != null)
                return (T)_components[findType];

            return default(T);
        }
    }
}
