using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NetCasbin
{
    public class Config
    {
        private static readonly String DEFAULT_SECTION = "default";
        private static readonly String DEFAULT_COMMENT = "#";
        private static readonly String DEFAULT_COMMENT_SEM = ";";

        // Section:key=value
        private readonly IDictionary<String, IDictionary<String, String>> _data;

        private Config()
        {
            _data = new Dictionary<String, IDictionary<String, String>>();
        }

        public static Config NewConfig(String configFilePath)
        {
            Config c = new Config();
            c.Parse(configFilePath);
            return c;
        }

        public static Config NewConfigFromText(String text)
        {
            Config c = new Config();
            c.ParseBuffer(new StringReader(text));
            return c;
        }

        private bool AddConfig(string section, string option, string value)
        {
            if (string.IsNullOrEmpty(section))
            {
                section = DEFAULT_SECTION;
            }

            if (!_data.ContainsKey(section))
            {
                _data.Add(section, new Dictionary<String, String>());
            }

            Boolean ok = _data[section].ContainsKey(option);
            _data[section].Add(option, value);
            return !ok;
        }

        private void Parse(String configFilePath)
        {
            using (StreamReader sr = new StreamReader(configFilePath))
            {
                ParseBuffer(sr);
            }
        }

        private void ParseBuffer(TextReader reader)
        {
            String section = "";
            int lineNum = 0;
            String line;
            while (true)
            {
                lineNum++;
                try
                {
                    if ((line = reader.ReadLine()) != null)
                    {
                        if (string.IsNullOrEmpty(line))
                        {
                            continue;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
                catch (IOException e)
                {
                    throw new Exception("IO error occurred");
                }

                line = line.Trim();

                if (line.StartsWith(DEFAULT_COMMENT))
                {
                    continue;
                }
                else if (line.StartsWith(DEFAULT_COMMENT_SEM))
                {
                    continue;
                }
                else if (line.StartsWith("[") && line.EndsWith("]"))
                {
                    section = line.Substring(1, line.Length - 2);
                }
                else
                {
                    String[] optionVal = line.Split("=".ToCharArray(), 2);
                    if (optionVal.Length != 2)
                    {
                        throw new Exception(
                                String.Format("parse the content error : line {0} , {1} = ? ", lineNum, optionVal[0]));
                    }
                    String option = optionVal[0].Trim();
                    String value = optionVal[1].Trim();
                    AddConfig(section, option, value);
                }
            }
        }

        public Boolean getBool(String key)
        {
            return Boolean.Parse(Get(key));
        }

        public int GetInt(String key)
        {
            return int.Parse(Get(key));
        }

        public float GetFloat(String key)
        {
            return float.Parse(Get(key));
        }

        public String GetString(String key)
        {
            return Get(key);
        }

        public String[] GetStrings(String key)
        {
            String v = Get(key);
            if (string.IsNullOrEmpty(v))
            {
                return null;
            }
            return v.Split(',');
        }

        public void Set(String key, String value)
        {

            if (String.IsNullOrEmpty(key))
            {
                throw new Exception("key is empty");
            }

            String section = "";
            String option;

            String[] keys = key.ToLower().Split(new String[] { "::" }, StringSplitOptions.None);
            if (keys.Length >= 2)
            {
                section = keys[0];
                option = keys[1];
            }
            else
            {
                option = keys[0];
            }
            AddConfig(section, option, value);
        }

        public String Get(String key)
        {
            String section;
            String option;

            String[] keys = key.ToLower().Split(new String[] { "::" }, StringSplitOptions.None);
            if (keys.Length >= 2)
            {
                section = keys[0];
                option = keys[1];
            }
            else
            {
                section = DEFAULT_SECTION;
                option = keys[0];
            }

            Boolean ok = _data.ContainsKey(section) && _data[section].ContainsKey(option);
            if (ok)
            {
                return _data[section][option];
            }
            else
            {
                return "";
            }
        }
    }
}
