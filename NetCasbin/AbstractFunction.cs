using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

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
