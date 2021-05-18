using System;
using Biovation.Domain;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Linq;

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
                    foreach (var tag in tags)
                    {
                        if (tmp != null && tmp[tag] != null)
                        {
                            if (tags.LastOrDefault() != tag)
                            {
                                tmp = tmp[tag];
                            }
                            else
                            {
                                return new ResultViewModel<string>()
                                {
                                    Success = true,
                                    Data = tmp[tag].ToString(),
                                };
                            }

                        }
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
                    var tags = inputs.Skip(1).ToArray();
                    var tmp = appSettingsJson[inputs.FirstOrDefault() ?? string.Empty];
                    foreach (var tag in tags)
                    {
                        if (tmp != null && tmp[tag] != null)
                        {
                            if (tags.LastOrDefault() != tag)
                            {
                                tmp = tmp[tag];
                            }
                            else
                            {
                                tmp = tag;
                                return new ResultViewModel()
                                {
                                    Success = true,
                                };
                            }

                        }
                    }
                    return new ResultViewModel()
                    {
                        Success = false
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
