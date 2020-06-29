using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NetCasbin.Persist
{
    public interface IAdapter
    {
        void LoadPolicy(Model.Model model);

        Task LoadPolicyAsync(Model.Model model);

        void SavePolicy(Model.Model model);

        Task SavePolicyAsync(Model.Model model);

        void AddPolicy(string sec, string ptype, IList<string> rule);

        Task AddPolicyAsync(string sec, string ptype, IList<string> rule);

        void RemovePolicy(string sec, string ptype, IList<string> rule);

        Task RemovePolicyAsync(string sec, string ptype, IList<string> rule);

        void RemoveFilteredPolicy(string sec, string ptype, int fieldIndex, params string[] fieldValues);

        Task RemoveFilteredPolicyAsync(string sec, string ptype, int fieldIndex, params string[] fieldValues);
    }
}
