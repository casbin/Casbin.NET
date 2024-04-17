using System;
using System.Collections.Generic;
using System.Threading;

namespace Casbin.Model;

public partial class DefaultPolicyStore
{
    internal class Node
    {
        private readonly PolicyAssertion _assertion;

        public List<IPolicyValues> Policy = new();
        public Node(PolicyAssertion assertion) => _assertion = assertion;
        public HashSet<string> PolicyTextSet { get; } = new();
        public ReaderWriterLockSlim Lock { get; } = new();

        public void RefreshPolicyStringSet()
        {
            PolicyTextSet.Clear();
            foreach (IPolicyValues policy in GetPolicy())
            {
                PolicyTextSet.Add(policy.ToText());
            }
        }

        public Iterator Iterate() => new(this);

        public IEnumerable<IPolicyValues> GetPolicy()
        {
            Iterator iterator = new(this);
            List<IPolicyValues> policyList = new();
            while (iterator.GetNext(out IPolicyValues policyValues))
            {
                policyList.Add(policyValues);
            }

            return policyList;
        }

        public IReadOnlyList<IPolicyValues> SetPolicy(List<IPolicyValues> valuesList) =>
            Interlocked.Exchange(ref Policy, valuesList);

        public bool ContainsPolicy(IPolicyValues values)
            => PolicyTextSet.Contains(values.ToText());

        public bool ValidatePolicy(IPolicyValues values)
        {
            if (_assertion.Section is PermConstants.Section.RoleSection)
            {
                return _assertion.Tokens.Count <= values.Count;
            }

            return _assertion.Tokens.Count == values.Count;
        }

        public bool TryAddPolicy(IPolicyValues values)
        {
            if (ValidatePolicy(values) is false)
            {
                return false;
            }

            if (ContainsPolicy(values))
            {
                return false;
            }

            if (_assertion.TryGetPriorityIndex(out int index))
            {
                return TryAddPolicyByPriority(values, index);
            }

            Lock.EnterWriteLock();
            try
            {
                Policy.Add(values);
            }
            finally
            {
                Lock.ExitWriteLock();
            }

            PolicyTextSet.Add(values.ToText());
            return true;
        }

        public bool TryUpdatePolicy(IPolicyValues oldValues, IPolicyValues newValues)
        {
            if (ValidatePolicy(newValues) is false)
            {
                return false;
            }

            if (ContainsPolicy(oldValues) is false)
            {
                return false;
            }

            Lock.EnterWriteLock();
            try
            {
                for (int i = 0; i < Policy.Count; i++)
                {
                    IPolicyValues v = Policy[i];
                    if (v.Equals(oldValues) is false)
                    {
                        continue;
                    }

                    Policy.RemoveAt(i);
                    Policy.Insert(i, newValues);
                    PolicyTextSet.Remove(oldValues.ToText());
                    PolicyTextSet.Add(newValues.ToText());
                    return true;
                }
            }
            finally
            {
                Lock.ExitWriteLock();
            }

            return true;
        }

        public bool TryRemovePolicy(IPolicyValues values)
        {
            if (ContainsPolicy(values) is false)
            {
                return false;
            }

            Lock.EnterWriteLock();
            try
            {
                for (int i = 0; i < Policy.Count; i++)
                {
                    IPolicyValues v = Policy[i];
                    if (v.Equals(values) is false)
                    {
                        continue;
                    }

                    Policy.RemoveAt(i);
                    PolicyTextSet.Remove(values.ToText());
                    return true;
                }
            }
            finally
            {
                Lock.ExitWriteLock();
            }

            return true;
        }

        public void ClearPolicy()
        {
            Interlocked.Exchange(ref Policy, new List<IPolicyValues>());
            PolicyTextSet.Clear();
        }

        private bool TrySortPoliciesByPriority()
        {
            if (_assertion.TryGetPriorityIndex(out int priorityIndex) is false)
            {
                return false;
            }

            int PolicyComparison(IPolicyValues p1, IPolicyValues p2)
            {
                string priorityString1 = p1[priorityIndex];
                string priorityString2 = p2[priorityIndex];

                if (int.TryParse(priorityString1, out int priority1) is false
                    || int.TryParse(priorityString2, out int priority2) is false)
                {
                    return string.CompareOrdinal(priorityString1, priorityString2);
                }

                return priority1 - priority2;
            }

            Lock.EnterWriteLock();
            try
            {
                Policy.Sort(PolicyComparison);
            }
            finally
            {
                Lock.ExitWriteLock();
            }

            return true;
        }

        private bool TrySortPoliciesBySubjectHierarchy(Dictionary<string, int> subjectHierarchyMap)
        {
            if (_assertion.TryGetDomainIndex(out int domainIndex) is false)
            {
                domainIndex = -1;
            }

            if (_assertion.TryGetSubjectIndex(out int subjectIndex) is false)
            {
                return false;
            }

            int PolicyComparison(IPolicyValues p1, IPolicyValues p2)
            {
                string domain1 = "", domain2 = "";
                if (domainIndex != -1)
                {
                    domain1 = p1[domainIndex];
                    domain2 = p2[domainIndex];
                }

                string name1 = RoleAssertion.GetNameWithDomain(domain1, p1[subjectIndex]);
                string name2 = RoleAssertion.GetNameWithDomain(domain2, p2[subjectIndex]);

                return subjectHierarchyMap[name1] - subjectHierarchyMap[name2];
            }


            Lock.EnterWriteLock();
            try
            {
                Policy.Sort(PolicyComparison);
            }
            finally
            {
                Lock.ExitWriteLock();
            }

            return true;
        }

        private bool TryAddPolicyByPriority(IPolicyValues values, int priorityIndex)
        {
            if (int.TryParse(values[priorityIndex], out int priority) is false)
            {
                return false;
            }

            bool LastLessOrEqualPriority(IPolicyValues v)
            {
                return int.Parse(v[priorityIndex]) <= priority;
            }

            Lock.EnterWriteLock();
            try
            {
                int lastIndex = Policy.FindLastIndex(LastLessOrEqualPriority);
                Policy.Insert(lastIndex + 1, values);
            }
            finally
            {
                Lock.ExitWriteLock();
            }

            PolicyTextSet.Add(values.ToText());
            return true;
        }


        internal bool TrySortPolicyByPriority()
        {
            if (_assertion.TryGetPriorityIndex(out int priorityIndex) is false)
            {
                return false;
            }

            int PolicyComparison(IPolicyValues p1, IPolicyValues p2)
            {
                string priorityString1 = p1[priorityIndex];
                string priorityString2 = p2[priorityIndex];

                if (int.TryParse(priorityString1, out int priority1) is false
                    || int.TryParse(priorityString2, out int priority2) is false)
                {
                    return string.CompareOrdinal(priorityString1, priorityString2);
                }

                return priority1 - priority2;
            }

            Lock.EnterWriteLock();
            try
            {
                Policy.Sort(PolicyComparison);
            }
            finally
            {
                Lock.ExitWriteLock();
            }

            return true;
        }

        internal bool TrySortPoliciesBySubjectHierarchy(IDictionary<string, int> subjectHierarchyMap,
            Func<string, string, string> nameFormatter)
        {
            if (_assertion.TryGetDomainIndex(out int domainIndex) is false)
            {
                domainIndex = -1;
            }

            if (_assertion.TryGetSubjectIndex(out int subjectIndex) is false)
            {
                return false;
            }


            int PolicyComparison(IPolicyValues p1, IPolicyValues p2)
            {
                string domain1 = "", domain2 = "";
                if (domainIndex != -1)
                {
                    domain1 = p1[domainIndex];
                    domain2 = p2[domainIndex];
                }

                string name1 = nameFormatter(domain1, p1[subjectIndex]);
                string name2 = nameFormatter(domain2, p2[subjectIndex]);

                return subjectHierarchyMap[name1] - subjectHierarchyMap[name2];
            }

            Lock.EnterWriteLock();
            try
            {
                Policy.Sort(PolicyComparison);
            }
            finally
            {
                Lock.ExitWriteLock();
            }

            return true;
        }
    }
}
