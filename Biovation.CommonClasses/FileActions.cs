using System;
using Biovation.Domain;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace Biovation.CommonClasses
{
    public static class FileActions
    {

        public static ResultViewModel<string> JsonReader(params string[] inputs)
        {
            try
            {
                if (inputs.Any())
                {
                    var appSettingsJson = JObject.Parse(File.ReadAllText(inputs.FirstOrDefault()));
                    var tags = inputs.Skip(1).ToArray();
                    var tmp = appSettingsJson[inputs.FirstOrDefault() ?? string.Empty];
                    //foreach (var tag in tags)
                    //{
                    //    if (tmp != null && tmp[tag] != null)
                    //    {
                    //        if (tags.LastOrDefault() != tag)
                    //        {
                    //            tmp = tmp[tag];
                    //        }
                    //        else
                    //        {
                    //            return new ResultViewModel<string>()
                    //            {
                    //                Success = true,
                    //                Data = tmp[tag].ToString(),
                    //            };
                    //        }

                    //    }
                    //}
                    if (inputs.Length == 3)
                    {
                        return new ResultViewModel<string>()
                        {
                            Success = true,
                            Data = appSettingsJson[inputs[1]][inputs[2]].ToString(),
                        };

                    }
                    else if (inputs.Length == 2)
                    {
                        return new ResultViewModel<string>()
                        {
                            Success = true,
                            Data = appSettingsJson[inputs[1]].ToString(),
                        };
                    }

                    return new ResultViewModel<string>()
                    {
                        Success = false
                    };
                }
                return new ResultViewModel<string>()
                {
                    Success = false
                };

            }
            catch (Exception e)
            {
                return new ResultViewModel<string>()
                {
                    Success = false,
                    Message = e.ToString()
                };
            }
        }

        public static ResultViewModel JsonWriter(params string[] inputs)
        {
            try
            {
                if (inputs.Any())
                {
                    var appSettingsJson = JObject.Parse(File.ReadAllText(inputs.FirstOrDefault()));
                    var tags = inputs.Take(inputs.Length - 1).Skip(1).ToArray();
                    //JToken tmp = appSettingsJson;
                    //foreach (var tag in tags)
                    //{
                    //    tmp = tmp[tag];
                    //    if (tmp != null )
                    //    {
                    //        if (tags.LastOrDefault() == tag)
                    //        {
                    //            tmp[tag] = inputs.LastOrDefault();
                    //            return new ResultViewModel()
                    //            {
                    //                Success = true,
                    //            };
                    //        }

                    //    }
                    //}
                    if (inputs.Length == 4)
                    {
                        appSettingsJson[inputs[1]][inputs[2]] = inputs.LastOrDefault();
                    }
                    var output = JsonConvert.SerializeObject(appSettingsJson
                        , Formatting.Indented);
                    try
                    {
                        File.WriteAllText(
                            inputs.FirstOrDefault(), output);
                    }
                    catch (Exception)
                    {
                        // ignored
                    }
                    return new ResultViewModel()
                    {
                        Success = true
                    };
                }
                return new ResultViewModel()
                {
                    Success = false
                };

            }
            catch (Exception e)
            {
                return new ResultViewModel()
                {
                    Success = false,
                    Message = e.ToString()
                };
            }
        }
    }
}
