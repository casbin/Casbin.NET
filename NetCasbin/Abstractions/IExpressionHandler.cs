using System;
using System.Collections.Generic;
using DynamicExpresso;

namespace NetCasbin.Abstractions
{
    internal interface IExpressionHandler
    {
        public IDictionary<string, int> RequestTokens { get; }

        public IDictionary<string, int> PolicyTokens { get; }

        public IReadOnlyList<object> RequestValues { get; set; }

        public IReadOnlyList<string> PolicyValues { get; set; }

        public IDictionary<string, Parameter> Parameters { get; } 

        public void SetFunction(string name, Delegate function);

        public void SetGFunctions();

        public bool Invoke(string expressionString);
        public bool Invoke<T1>(string expressionString, T1 value1);
        public bool Invoke<T1, T2>(string expressionString, T1 value1, T2 value2);
        public bool Invoke<T1, T2, T3>(string expressionString, T1 value1, T2 value2, T3 value3);
        public bool Invoke<T1, T2, T3, T4>(string expressionString, T1 value1, T2 value2, T3 value3, T4 value4);
        public bool Invoke<T1, T2, T3, T4, T5>(string expressionString, T1 value1, T2 value2, T3 value3, T4 value4, T5 value5);
    }
}
