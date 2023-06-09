using System;
using System.Collections.Generic;
using System.Linq;
using NetCasbin.Model;

namespace NetCasbin.Persist
{
    public static class Helper
    {
        [Obsolete("This item will remove at the next mainline version")]
        public delegate void LoadPolicyLineHandler<T, TU>(T t, TU u);

        [Obsolete("please use the extension method TryLoadPolicyLine of Model")]
        public static void LoadPolicyLine(string line, Model.Model model)
        {
            model.TryLoadPolicyLine(line);
        }

        public static bool TryLoadPolicyLine(this Model.Model model, string line)
        {
            if (string.IsNullOrEmpty(line))
            {
                return false;
            }

            if (line[0] == '#')
            {
                return false;
            }
            string[] tokens;
            if(line.Contains('\"'))
            {
                int leftPos = 0;
                bool inSegment = false, inDoubleQuotation = false;
                List<string> tokensTemp = new List<string>();
                for(int i  = 0; i < line.Length;i++)
                {
                    if (line[i] =='\"')
                    {
                        if(inSegment==false)
                        {
                            inSegment = true;
                        }
                        else
                        {
                            if(inDoubleQuotation==false)
                            {
                                inDoubleQuotation = true;
                            }
                            else
                            {
                                inDoubleQuotation = false;
                            }
                        }
                    }
                    else
                    {
                        if(inDoubleQuotation==true)
                        {
                            inDoubleQuotation=false;
                            inSegment = false;
                        }
                        if (inSegment == false&& line[i]==',')
                        {
                            tokensTemp.Add(line.Substring(leftPos, i - leftPos));
                            leftPos = i + 1;
                        }
                    }
                }
                tokensTemp.Add(line.Substring(leftPos));
                tokens= tokensTemp.Select(x => x.Trim()).ToArray();
                for(int i=0;i<tokens.Length;i++)
                {
                    string stringTemp = tokens[i];
                    if (stringTemp.Contains(',') && stringTemp[0] == '\"' && stringTemp[stringTemp.Length-1]=='\"')
                    {
                        stringTemp=stringTemp.Substring(1,stringTemp.Length-2);
                        stringTemp = stringTemp.Replace("\"\"", "\"");
                    }
                    tokens[i] = stringTemp;
                }
            }
            else
            {
                tokens = line.Split(',').Select(x => x.Trim()).ToArray();
            }
             
            return model.TryLoadPolicyLine(tokens);
        }

        public static bool TryLoadPolicyLine(this Model.Model model, IReadOnlyList<string> lineTokens)
        {
            string type = lineTokens[0];
            string sec = type.Substring(0, 1);
            return model.TryGetExistAssertion(sec, type, out Assertion assertion)
                   && assertion.TryAddPolicy(lineTokens.Skip(1).ToList());
        }
    }
}
