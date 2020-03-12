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

        void AddPolicy(String sec, String ptype, IList<String> rule);

        Task AddPolicyAsync(String sec, String ptype, IList<String> rule);

        void RemovePolicy(String sec, String ptype, IList<String> rule);

        void RemoveFilteredPolicy(String sec, String ptype, int fieldIndex, params string[] fieldValues);
    }
}
