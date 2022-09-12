using System;
using System.Collections.Generic;
using Casbin.Model;
using Casbin.UnitTests.Extensions;

namespace Casbin.UnitTests.ParallelTest
{
    public class DefaultRandomEntropyPool<TRequest> : IEntropyPool<TRequest> where TRequest : IRequestValues
    {
        private readonly string _randomStringSource = "abcdefghijklmnopqrstuvwxyz0123456789";
        private readonly int _defaultRandomStringLength = 3;
        /// <summary>
        /// A dictionary to save the random element pools for each category.
        /// </summary>
        public Dictionary<string, List<string>> _elementPools;
        private Func<List<string>, TRequest> _convertFunc;
        public DefaultRandomEntropyPool(Func<List<string>, TRequest> list2RequestFunc)
        {
            _convertFunc = list2RequestFunc;
            _elementPools = new Dictionary<string, List<string>>();
        }
        public DefaultRandomEntropyPool(Func<List<string>, TRequest> list2RequestFunc, Dictionary<string, List<string>> elementPools)
        {
            _convertFunc = list2RequestFunc;
            _elementPools = elementPools;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="categoryAndCount"> The first element is the category, the second is the count
        /// of elements you want to put in the element pool.</param>
        public DefaultRandomEntropyPool(Func<List<string>, TRequest> list2RequestFunc, params KeyValuePair<string, int>[] categoryAndCount)
        {
            _convertFunc = list2RequestFunc;
            _elementPools = new Dictionary<string, List<string>>();
            for (int i = 0; i < categoryAndCount.Length; i++)
            {
                List<string> values = new List<string>();
                for (int j = 0; j < categoryAndCount[i].Value; j++)
                {
                    values.Add(GetRandomString(_defaultRandomStringLength));
                }
                _elementPools.Add(categoryAndCount[i].Key, values);
            }
        }
        public DefaultRandomEntropyPool<TRequest> Add(string key, int count)
        {
            List<string> values = new List<string>();
            while(count-- > 0)
            {
                values.Add(GetRandomString(_defaultRandomStringLength));
            }
            _elementPools.Add(key, values);
            return this;
        }
        public DefaultRandomEntropyPool<TRequest> Add(string key, string value)
        {
            if(_elementPools.ContainsKey(key))
            {
                _elementPools[key].Add(value);
            }
            else
            {
                _elementPools.Add(key, new List<string>() { value });
            }
            return this;
        }
        public TRequest Get()
        {
            List<string> res = new List<string>();
            foreach(var keyValue in _elementPools)
            {
                Random rd = new Random();
                res.Add(keyValue.Value[rd.Next(0, keyValue.Value.Count)]);
            }
            return _convertFunc(res);
        }
        public void Add(TRequest request)
        {
            var enumerator = _elementPools.GetEnumerator();
            for (int i = 0; i < request.Count; i++)
            {
                enumerator.MoveNext();
                enumerator.Current.Value.Add(request[i]);
            }
        }
        private string GetRandomString(int length)
        {
            string res = "";
            while(length-- > 0)
            {
                Random rd = new Random();
                res += _randomStringSource[rd.Next(0, _randomStringSource.Length)];
            }
            return res;
        }
    }

    public static class DefaultRandomEntropyPool
    {
        public static DefaultRandomEntropyPool<RequestValues<string>> Create(params KeyValuePair<string, int>[] categoryAndCount)
        {
            return new DefaultRandomEntropyPool<RequestValues<string>>(x => Request.CreateValues(x[0]), categoryAndCount);
        }
        public static DefaultRandomEntropyPool<RequestValues<string, string>> Create2(params KeyValuePair<string, int>[] categoryAndCount)
        {
            return new DefaultRandomEntropyPool<RequestValues<string, string>>(x => Request.CreateValues(x[0], x[1]), categoryAndCount);
        }
        public static DefaultRandomEntropyPool<RequestValues<string, string, string>> Create3(params KeyValuePair<string, int>[] categoryAndCount)
        {
            return new DefaultRandomEntropyPool<RequestValues<string, string, string>>(
                x => Request.CreateValues(x[0], x[1], x[2]), categoryAndCount
            );
        }
        public static DefaultRandomEntropyPool<RequestValues<string, string, string, string>> 
            Create4(params KeyValuePair<string, int>[] categoryAndCount)
        {
            return new DefaultRandomEntropyPool<RequestValues<string, string, string, string>>(
                x => Request.CreateValues(x[0], x[1], x[2], x[3]), categoryAndCount
            );
        }
        public static DefaultRandomEntropyPool<RequestValues<string, string, string, string, string>> 
            Create5(params KeyValuePair<string, int>[] categoryAndCount)
        {
            return new DefaultRandomEntropyPool<RequestValues<string, string, string, string, string>>(
                x => Request.CreateValues(x[0], x[1], x[2], x[3], x[4]), categoryAndCount
            );
        }
        public static DefaultRandomEntropyPool<RequestValues<string, string, string, string, string, string>> 
            Create6(params KeyValuePair<string, int>[] categoryAndCount)
        {
            return new DefaultRandomEntropyPool<RequestValues<string, string, string, string, string, string>>(
                x => Request.CreateValues(x[0], x[1], x[2], x[3], x[4], x[5]), categoryAndCount
            );
        }
        public static DefaultRandomEntropyPool<RequestValues<string, string, string, string, string, string, string>> 
            Create7(params KeyValuePair<string, int>[] categoryAndCount)
        {
            return new DefaultRandomEntropyPool<RequestValues<string, string, string, string, string, string, string>>(
                x => Request.CreateValues(x[0], x[1], x[2], x[3], x[4], x[5], x[6]), categoryAndCount
            );
        }
        public static DefaultRandomEntropyPool<RequestValues<string, string, string, string, string, string, string, string>> 
            Create8(params KeyValuePair<string, int>[] categoryAndCount)
        {
            return new DefaultRandomEntropyPool<RequestValues<string, string, string, string, string, string, string, string>>(
                x => Request.CreateValues(x[0], x[1], x[2], x[3], x[4], x[5], x[6], x[7]), categoryAndCount
            );
        }
        public static DefaultRandomEntropyPool<RequestValues<string, string, string, string, string, string, string, string, string>> 
            Create9(params KeyValuePair<string, int>[] categoryAndCount)
        {
            return new DefaultRandomEntropyPool<RequestValues<string, string, string, string, string, string, string, string, string>>(
                x => Request.CreateValues(x[0], x[1], x[2], x[3], x[4], x[5], x[6], x[7], x[8]), categoryAndCount
            );
        }
        public static DefaultRandomEntropyPool<RequestValues<string, string, string, string, string, string, string, string, string, string>> 
            Create10(params KeyValuePair<string, int>[] categoryAndCount)
        {
            return new DefaultRandomEntropyPool<RequestValues<string, string, string, string, string, string, string, string, string, string>>(
                x => Request.CreateValues(x[0], x[1], x[2], x[3], x[4], x[5], x[6], x[7], x[8], x[9]), categoryAndCount
            );
        }
        public static DefaultRandomEntropyPool<RequestValues<string, string, string, string, string, string, string, string, string, string, string>> 
            Create11(params KeyValuePair<string, int>[] categoryAndCount)
        {
            return new DefaultRandomEntropyPool<RequestValues<string, string, string, string, string, string, string, string, string, string, string>>(
                x => Request.CreateValues(x[0], x[1], x[2], x[3], x[4], x[5], x[6], x[7], x[8], x[9], x[10]), categoryAndCount
            );
        }
        public static DefaultRandomEntropyPool<RequestValues<string, string, string, string, string, string, string, string, string, string, string, string>> 
            Create12(params KeyValuePair<string, int>[] categoryAndCount)
        {
            return new DefaultRandomEntropyPool<RequestValues<string, string, string, string, string, string, string, string, string, string, string, string>>(
                x => Request.CreateValues(x[0], x[1], x[2], x[3], x[4], x[5], x[6], x[7], x[8], x[9], x[10], x[11]), categoryAndCount
            );
        }
    }
}