using NetCasbin.Util;
using System;
using System.Collections.Generic;

namespace NetCasbin.Model
{
    public class Model : Policy
    {
        private static readonly IDictionary<string, string> SectionNameMap = new Dictionary<string, string>() {
            { PermConstants.Section.RequestSection, PermConstants.Section.RequestSectionName},
            { PermConstants.Section.PolicySection, PermConstants.Section.PolicySectionName},
            { PermConstants.Section.RoleSection, PermConstants.Section.RoleSectionName},
            { PermConstants.Section.PolicyEffeftSection, PermConstants.Section.PolicyEffeftSectionName},
            { PermConstants.Section.MatcherSection, PermConstants.Section.MatcherSectionName}
        };

        private bool LoadAssertion(Config.Config cfg, string sec, string key)
        {
            var secName = SectionNameMap[sec];
            var value = cfg.GetString($"{secName}::{key}");
            return AddDef(sec, key, value);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sec">"p" or "g"</param>
        /// <param name="key">The policy type, "p", "p2", .. or "g", "g2", ..</param>
        /// <param name="value">The policy rule, separated by ", ".</param>
        /// <returns>Succeeds or not.</returns>
        public bool AddDef(string sec, string key, string value)
        {
            var ast = new Assertion
            {
                Key = key,
                Value = value
            };

            if (string.IsNullOrEmpty(ast.Value))
            {
                return false;
            }

            if (sec.Equals(PermConstants.Section.RequestSection) || sec.Equals(PermConstants.Section.PolicySection))
            {
                var tokens = ast.Value.Split(new string[] { ", " }, StringSplitOptions.None);
                for (var i = 0; i < tokens.Length; i++)
                {
                    tokens[i] = $"{key}_{tokens[i]}";
                }
                ast.Tokens = tokens;
            }
            else
            {
                ast.Value = Utility.RemoveComments(Utility.EscapeAssertion(ast.Value));
            }

            if (!Model.ContainsKey(sec))
            {
                var assertionMap = new Dictionary<string, Assertion>
                {
                    [key] = ast
                };
                Model.Add(sec, assertionMap);
            }
            else
            {
                Model[sec].Add(key, ast);
            }
            return true;
        }

        private string GetKeySuffix(int i)
        {
            if (i == 1)
            {
                return string.Empty;
            }
            return i.ToString();
        }

        private void LoadSection(Config.Config cfg, string sec)
        {
            var i = 1;
            while (true)
            {
                if (!LoadAssertion(cfg, sec, sec + GetKeySuffix(i)))
                {
                    break;
                }
                else
                {
                    i++;
                }
            }
        }

        public void LoadModel(string path)
        {
            var cfg = Config.Config.NewConfig(path);

            LoadSection(cfg, PermConstants.Section.RequestSection);
            LoadSection(cfg, PermConstants.Section.PolicySection);
            LoadSection(cfg, PermConstants.Section.RoleSection);
            LoadSection(cfg, PermConstants.Section.PolicyEffeftSection);
            LoadSection(cfg, PermConstants.Section.MatcherSection);
        }

        public void LoadModelFromText(string text)
        {
            var cfg = Config.Config.NewConfigFromText(text);

            LoadSection(cfg, PermConstants.Section.RequestSection);
            LoadSection(cfg, PermConstants.Section.PolicySection);
            LoadSection(cfg, PermConstants.Section.RoleSection);
            LoadSection(cfg, PermConstants.Section.PolicyEffeftSection);
            LoadSection(cfg, PermConstants.Section.MatcherSection);
        }
    }
}
