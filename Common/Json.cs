using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Text.Json;
using System.Xml.Linq;
using xLibV100.Components;

namespace xLibV100.Common
{
    public class Json
    {
        public static xAction<string> Tracer = xTracer.Message;

        private static void trace(string str)
        {
            Tracer?.Invoke(str);
        }

        public static bool CreateFolder(string path, string subpath)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(path);
            if (!dirInfo.Exists)
            {
                dirInfo.Create();
            }

            dirInfo.CreateSubdirectory(subpath);
            return true;
        }

        public static int Save(string fileName, in object arg, Type inType = null)
        {
            if (fileName == null)
            {
                trace("json file save error: file_name = null");
                return -1;
            }

            if (arg == null)
            {
                trace("json file save error: arg = null");
                return -1;
            }

            try
            {
                string directoryPath = Path.GetDirectoryName(fileName);

                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                using (FileStream stream = new FileStream(fileName, FileMode.Create))
                {
                    var options = new JsonSerializerOptions
                    {
                        WriteIndented = true
                    };

                    if (inType == null)
                    {
                        inType = arg.GetType();
                    }

                    System.Text.Json.JsonSerializer.Serialize(stream, arg, inType, options);
                    trace("json file is save:\r" + fileName);
                    stream.Close();
                }
            }
            catch (Exception e)
            {
                trace("json file save error:\r" + e);
                return -1;
            }

            return 0;
        }

        public static object Deserialize(string fileName, Type outType)
        {
            if (fileName == null)
            {
                trace("json file open error: fileName = null");
                return null;
            }

            if (!File.Exists(fileName))
            {
                trace("json file open error: file not found");
                return null;
            }

            try
            {
                using (FileStream stream = new FileStream(fileName, FileMode.Open))
                {
                    var options = new JsonSerializerOptions();
                    options.PropertyNameCaseInsensitive = true;
                    options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                    options.AllowTrailingCommas = true;

                    var result = System.Text.Json.JsonSerializer.Deserialize(stream, outType, options);

                    trace("json file is open:\r" + fileName);
                    return result;
                }
            }
            catch (Exception e)
            {
                trace("json file save error:\r" + e);
            }

            return null;
        }

        /*public static int Open<TObject>(string fileName, out TObject arg)
        {
            arg = default(TObject);

            if (fileName == null)
            {
                trace("json file open error: fileName = null");
                return -1;
            }

            if (!File.Exists(fileName))
            {
                trace("json file open error: file not found");
                return -1;
            }

            try
            {
                using (FileStream stream = new FileStream(fileName, FileMode.Open))
                {
                    var options = new JsonSerializerOptions();
                    options.PropertyNameCaseInsensitive = true;
                    options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                    options.AllowTrailingCommas = true;

                    var result = JsonSerializer.DeserializeAsync<TObject>(stream, options);
                    arg = result.Result;
                    trace("json file is open:\r" + fileName);
                    return 0;
                }
            }
            catch (Exception e)
            {
                trace("json file save error:\r" + e);
            }

            return -1;
        }*/

        public static int Open<TObject>(string fileName, out TObject arg)
        {
            arg = default;

            if (fileName == null)
            {
                trace("json file open error: fileName = null");
                return -1;
            }

            if (!File.Exists(fileName))
            {
                trace("json file open error: file not found");
                return -1;
            }

            try
            {
                using (StreamReader reader = new StreamReader(fileName))
                {
                    string jsonContent = reader.ReadToEnd();

                    JsonSerializerSettings settings = new JsonSerializerSettings
                    {
                        MissingMemberHandling = MissingMemberHandling.Ignore,
                        NullValueHandling = NullValueHandling.Ignore,
                        Formatting = Newtonsoft.Json.Formatting.Indented
                    };

                    arg = JsonConvert.DeserializeObject<TObject>(jsonContent, settings);
                    trace("json file is open:\r" + fileName);
                    return 0;
                }
            }
            catch (Exception e)
            {
                trace("json file open error:\r" + e);
            }

            return -1;
        }

        public static T Deserialize<T>(object element)
        {
            if (element is JObject jobject)
            {
                return jobject.ToObject<T>();
            }

            return default;
        }
    }
}
