using DbUp.Engine;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Biovation.CommonClasses.Migration
{
    public class ScriptPreprocessor : IScriptPreprocessor
    {/// <summary>
     /// 

        /// </summary>
        /// <param name="contents"></param>
        /// <returns></returns>
        public string Process(string contents)
        {
            contents = contents.Replace(
                @"SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO", "");

            var parser = new TSql150Parser(true);
            var fragments = parser.Parse(new StringReader(contents), out IList<ParseError> errors);

            var options = new SqlScriptGeneratorOptions
            {
                SqlVersion = SqlVersion.Sql110,
                KeywordCasing = KeywordCasing.Uppercase,
                IndentViewBody = true,
                AlignClauseBodies = true,
                AlignColumnDefinitionFields = true,
                NewLineBeforeFromClause = true,
                NewLineBeforeJoinClause = true,
                AlignSetClauseItem = true,
                MultilineSelectElementsList = true,
                AsKeywordOnOwnLine = true,
                MultilineViewColumnsList = true
                //IncludeSemicolons = false
            };

            var scriptGen = new Sql100ScriptGenerator(options);
            scriptGen.GenerateScript(fragments, out var script);

            var resultContent = string.Empty;
            var partedScripts = Regex.Split(script, "^\\s*GO\\s*$", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            foreach (var splittedScript in partedScripts)
            {
                if (splittedScript.Trim().Length > 0)
                {
                    string partialScript;

                    var firstLine = splittedScript.Split('\r', '\n').FirstOrDefault(line => !string.IsNullOrEmpty(line) && !string.IsNullOrWhiteSpace(line));
                    var procedureCheck = new Regex(@"(CREATE|ALTER)\s+(PROCEDURE|PROC|TRIGGER|VIEW|FUNCTION)\b\s*(\[?\w+\d*\b\]?\.?)?(\[?\w+\d*\]?)\s*", RegexOptions.IgnoreCase);
                    var tableCheck = new Regex(@"(CREATE|ALTER)\s+(TABLE)\b\s*(\[?\w+\d*\b\]?\.?)?(\[?\w+\d*\]?)\s*", RegexOptions.IgnoreCase);

                    if (firstLine == null) continue;

                    var spMatch = procedureCheck.Match(firstLine);
                    var tableMatch = tableCheck.Match(firstLine);
                    if (spMatch.Success)
                    {
                        var existenceCheckPhrase = @"IF OBJECT_ID('"
                                                   +
                                                   (spMatch.Groups[3].Value.IndexOf(".", StringComparison.Ordinal) > -1
                                                       ? spMatch.Groups[3].Value
                                                         + spMatch.Groups[4].Value
                                                       : spMatch.Groups[4].Value) + "') IS NOT NULL "
                                                   + Environment.NewLine + " DROP " + spMatch.Groups[2].Value + " "
                                                   +
                                                   (spMatch.Groups[3].Value.IndexOf(".", StringComparison.Ordinal) > -1
                                                       ? spMatch.Groups[3].Value
                                                         + spMatch.Groups[4].Value
                                                       : spMatch.Groups[4].Value)
                                                   + Environment.NewLine + "  GO " + Environment.NewLine;

                        const string alterPattern = @"(ALTER)\s+(PROCEDURE|PROC|TRIGGER|VIEW|FUNCTION)\b\s*";
                        partialScript = existenceCheckPhrase +
                                   Regex.Replace(splittedScript, alterPattern, " CREATE " + spMatch.Groups[2].Value + " ",
                                       RegexOptions.None);
                    }

                    else if (tableMatch.Success)
                    {
                        var existenceCheckPhrase =
                            $@"IF OBJECT_ID('{(tableMatch.Groups[3].Value.IndexOf(".", StringComparison.Ordinal) > -1
                                ? tableMatch.Groups[3].Value + tableMatch.Groups[4].Value
                                : tableMatch.Groups[4].Value)}{(tableMatch.Groups[1].Value.ToUpper() == "CREATE"
                                ? $"') IS NULL {Environment.NewLine}BEGIN"
                                : $"') IS NOT NULL {Environment.NewLine}BEGIN")}{Environment.NewLine}{Environment.NewLine}";

                        //var goPhraseMatches = Regex.Matches(partialScripts, @"\WGO\W", RegexOptions.IgnoreCase);
                        //if (goPhraseMatches.Count != 0)
                        //{
                        //    for (var i = 0; i < goPhraseMatches.Count; i++)
                        //    {
                        //        var goPhraseIndex = goPhraseMatches[i].Index;
                        //        partialScripts = partialScripts.Insert(goPhraseIndex - 1, "END" + Environment.NewLine);
                        //        partialScripts = partialScripts.Insert(goPhraseIndex + 8, Environment.NewLine + existenceCheckPhrase);
                        //        goPhraseMatches = Regex.Matches(script, @"\WGO\W", RegexOptions.IgnoreCase);
                        //    }
                        //}

                        partialScript = existenceCheckPhrase + splittedScript + Environment.NewLine + "END";
                    }
                    else
                    {
                        partialScript = splittedScript;
                    }
                    
                    resultContent += partialScript + $"{Environment.NewLine}GO{Environment.NewLine}{Environment.NewLine}";
                }
            }

            //contents = contents.Replace(";", "");
            return resultContent;
        }
    }
}
