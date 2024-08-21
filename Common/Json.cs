using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
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

        public static string FileExtensionClear(string file_name)
        {
            string[] str = file_name.Split('.');
            int i = str.Length;

            while (i > 0)
            {
                if (str[i - 1] != "json")
                {
                    break;
                }
                i--;
            }

            file_name = "";
            for (int j = 0; j < i; j++)
            {
                file_name += str[j];
            }

            return file_name + ".json";
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

                    JsonSerializer.Serialize(stream, arg, inType, options);
                    trace("json file is save:\r" + fileName);
                    stream.Close();
                    return -1;
                }
            }
            catch (Exception e)
            {
                trace("json file save error:\r" + e);
            }
            return 0;
        }

        public static int Open<TObject>(string fileName, out TObject arg)
        {
            return Open(fileName, out arg, null);
        }

        public static object Deserialize(string fileName, Type outType)
        {
            if (fileName == null)
            {
                trace("json file open error: file_name = null");
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

                    var result = JsonSerializer.Deserialize(stream, outType, options);

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

        public static int Open<TObject>(string file_name, out TObject arg, JsonConverter converter)
        {
            arg = default(TObject);

            if (file_name == null)
            {
                trace("json file open error: file_name = null");
                return -1;
            }

            if (!File.Exists(file_name))
            {
                trace("json file open error: file not found");
                return -1;
            }

            try
            {
                using (FileStream stream = new FileStream(file_name, FileMode.Open))
                {
                    var options = new JsonSerializerOptions();
                    options.PropertyNameCaseInsensitive = true;
                    options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                    options.AllowTrailingCommas = true;

                    if (converter != null)
                    {
                        options.Converters.Add(converter);
                    }

                    var result = JsonSerializer.DeserializeAsync<TObject>(stream, options);
                    arg = result.Result;
                    trace("json file is open:\r" + file_name);
                    return 0;
                }
            }
            catch (Exception e)
            {
                trace("json file save error:\r" + e);
            }

            return -1;
        }
    }
}
