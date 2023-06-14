﻿using System;
using System.Collections.Generic;
using System.IO;

namespace Casbin.Config
{
    public class DefaultConfig : IConfig
    {
        private static readonly string _defaultSection = "default";
        private static readonly string _defaultComment = "#";
        private static readonly string _defaultCommentSem = ";";
        private static readonly string _defaultFeed = "\\";

        // Section:key=value
        private readonly IDictionary<string, IDictionary<string, string>> _data;

        private DefaultConfig()
        {
            _data = new Dictionary<string, IDictionary<string, string>>();
        }

        /// <summary>
        /// Creates an empty default configuration representation.
        /// </summary>
        /// <returns>The constructor of Config.</returns>
        public static IConfig Create()
        {
            return new DefaultConfig();
        }

        /// <summary>
        /// Creates an empty default configuration representation from file.
        /// </summary>
        /// <param name="configFilePath">The path of the model file.</param>
        /// <returns>The constructor of Config.</returns>
        public static IConfig CreateFromFile(string configFilePath)
        {
            var config = new DefaultConfig();
            config.Parse(configFilePath);
            return config;
        }

        /// <summary>
        /// Creates an empty default configuration representation from text.
        /// </summary>
        /// <param name="text">The model text.</param>
        /// <returns>The constructor of Config.</returns>
        public static IConfig CreateFromText(string text)
        {
            var config = new DefaultConfig();
            config.ParseBuffer(new StringReader(text));
            return config;
        }

        public string Get(string key)
        {
            string section;
            string option;

            var keys = key.ToLower().Split(new string[] { "::" }, StringSplitOptions.None);
            if (keys.Length >= 2)
            {
                section = keys[0];
                option = keys[1];
            }
            else
            {
                section = _defaultSection;
                option = keys[0];
            }

            bool ok = _data.ContainsKey(section) && _data[section].ContainsKey(option);
            if (ok)
            {
                return _data[section][option];
            }
            else
            {
                return string.Empty;
            }
        }

        public bool GetBool(string key)
        {
            return bool.Parse(Get(key));
        }

        public int GetInt(string key)
        {
            return int.Parse(Get(key));
        }

        public float GetFloat(string key)
        {
            return float.Parse(Get(key));
        }

        public string GetString(string key)
        {
            return Get(key);
        }

        public string[] GetStrings(string key)
        {
            string v = Get(key);
            if (string.IsNullOrEmpty(v))
            {
                return null;
            }
            return v.Split(',');
        }

        public void Set(string key, string value)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new Exception("key is empty");
            }

            string section = string.Empty;
            string option;

            var keys = key.ToLower().Split(new string[] { "::" }, StringSplitOptions.None);
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

        /// <summary>
        /// Adds a new section->key:value to the configuration.
        /// </summary>
        /// <param name="section"></param>
        /// <param name="option"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private bool AddConfig(string section, string option, string value)
        {
            if (string.IsNullOrEmpty(section))
            {
                section = _defaultSection;
            }

            if (!_data.ContainsKey(section))
            {
                _data.Add(section, new Dictionary<string, string>());
            }

            bool ok = _data[section].ContainsKey(option);
            _data[section].Add(option, value);
            return !ok;
        }

        private void Parse(string configFilePath)
        {
            using (var sr = new StreamReader(configFilePath))
            {
                ParseBuffer(sr);
            }
        }

        private void ParseBuffer(TextReader reader)
        {
            string section = string.Empty;
            int lineNum = 0;
            string line;
            bool inSuccessiveLine = false;
            string option = string.Empty;
            string processedValue = string.Empty;
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
                    throw new IOException("IO error occurred", e);
                }

                line = line.Trim();

                if (line.StartsWith(_defaultComment))
                {
                    continue;
                }
                else if (line.StartsWith(_defaultCommentSem))
                {
                    continue;
                }
                else if (line.StartsWith("[") && line.EndsWith("]"))
                {
                    section = line.Substring(1, line.Length - 2);
                }
                else
                {
                    if (inSuccessiveLine == false)
                    {
                        var optionVal = line.Split("=".ToCharArray(), 2);
                        if (optionVal.Length != 2)
                        {
                            throw new Exception(
                                    string.Format("parse the content error : line {0} , {1} = ? ", lineNum, optionVal[0]));
                        }
                        option = optionVal[0].Trim();
                        string value = optionVal[1].Trim();
                        int commentStartIdx = value.IndexOf(PermConstants.PolicyCommentChar);
                        string lineProcessedValue = (commentStartIdx == -1 ? value : value.Remove(commentStartIdx)).Trim();
                        if (lineProcessedValue.EndsWith(_defaultFeed))
                        {
                            inSuccessiveLine = true;
                            processedValue = lineProcessedValue.Substring(0, lineProcessedValue.Length - 1);
                        }
                        else
                        {
                            inSuccessiveLine = false;
                            processedValue = lineProcessedValue;
                        }
                    }
                    else
                    {
                        string value = line.Trim();
                        int commentStartIdx = value.IndexOf(PermConstants.PolicyCommentChar);
                        string lineProcessedValue = (commentStartIdx == -1 ? value : value.Remove(commentStartIdx)).Trim();
                        if (lineProcessedValue.EndsWith(_defaultFeed))
                        {
                            inSuccessiveLine = true;
                            processedValue += lineProcessedValue.Substring(0, lineProcessedValue.Length - 1);
                        }
                        else
                        {
                            inSuccessiveLine = false;
                            processedValue += lineProcessedValue;
                        }
                    }

                    if (inSuccessiveLine == false)
                    {
                        AddConfig(section, option, processedValue);
                    }
                }
            }
        }

    }
}
