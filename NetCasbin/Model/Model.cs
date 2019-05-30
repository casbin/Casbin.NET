using System;
using System.Collections.Generic;
using System.Text;

namespace NetCasbin
{
    public class Model : Policy
    {
        private static readonly IDictionary<string, string> sectionNameMap = new Dictionary<string, string>() {
            { "r", "request_definition"},
            { "p", "policy_definition"},
            { "g", "role_definition"},
            { "e", "policy_effect"},
            { "m", "matchers"},
        };

        public Model()
        {
            Model = new Dictionary<String, Dictionary<String, Assertion>>();
        }

        private Boolean LoadAssertion(Config cfg, String sec, String key)
        {
            var secName = sectionNameMap[sec];
            String value = cfg.GetString($"{secName}::{key}");
            return this.AddDef(sec, key, value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sec">"p" or "g"</param>
        /// <param name="key">the policy type, "p", "p2", .. or "g", "g2", ..</param>
        /// <param name="value">the policy rule, separated by ", ".</param>
        /// <returns>succeeds or not.</returns>
        public bool AddDef(string sec, string key, string value)
        {
            Assertion ast = new Assertion
            {
                Key = key,
                Value = value
            };

            if (string.IsNullOrEmpty(ast.Value))
            {
                return false;
            }

            if (sec.Equals("r") || sec.Equals("p"))
            {
                var tokens = ast.Value.Split(new string[] { ", " }, StringSplitOptions.None);
                for (int i = 0; i < tokens.Length; i++)
                {
                    tokens[i] = $"{key}_{tokens[i]}";
                }
                ast.Tokens = tokens;
            }
            else
            {
                ast.Value = Util.RemoveComments(Util.EscapeAssertion(ast.Value));
            }

            if (!Model.ContainsKey(sec))
            {
                var assertionMap = new Dictionary<String, Assertion>
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

        private String GetKeySuffix(int i)
        {
            if (i == 1)
            {
                return "";
            }
            return i.ToString();
        }

        private void LoadSection(Config cfg, String sec)
        {
            int i = 1;
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

        public void LoadModel(String path)
        {
            Config cfg = Config.NewConfig(path);

            LoadSection(cfg, "r");
            LoadSection(cfg, "p");
            LoadSection(cfg, "e");
            LoadSection(cfg, "m");
            LoadSection(cfg, "g");
        }

        public void LoadModelFromText(String text)
        {
            Config cfg = Config.NewConfigFromText(text);

            LoadSection(cfg, "r");
            LoadSection(cfg, "p");
            LoadSection(cfg, "e");
            LoadSection(cfg, "m");

            LoadSection(cfg, "g");
        }

    }
}
