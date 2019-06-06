using System;
using System.Reflection;

namespace NetCasbin
{
    public abstract class AbstractFunction
    {
        private readonly string _name;

        public string Name => _name;

        public AbstractFunction(string name)
        {
            _name = name;
        }

        public ParameterInfo[] InputParameters => GetFunc().Method.GetParameters();

        public Type ReturnType => GetFunc().Method.ReturnType;

        protected abstract Delegate GetFunc();

        public static implicit operator Delegate(AbstractFunction aviator)
        {
            return aviator.GetFunc();
        }
    }
}
