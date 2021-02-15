using System;
using System.Reflection;

namespace NetCasbin.Abstractions
{
    public abstract class AbstractFunction
    {
        public string Name { get; }

        protected AbstractFunction(string name)
        {
            Name = name;
        }

        public ParameterInfo[] InputParameters => GetFunc().Method.GetParameters();

        public Type ReturnType => GetFunc().Method.ReturnType;

        protected abstract Delegate GetFunc();

        public static implicit operator Delegate(AbstractFunction thisClass)
        {
            return thisClass.GetFunc();
        }
    }
}
