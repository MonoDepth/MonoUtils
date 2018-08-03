using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection;

namespace MonoUtilities.Ini
{
    public static class MonoIni
    {
        /***
         * Ini reads
        ***/
        public static string IniReadString(string path, string sectionName, string settingName, string defaultValue)
        {
            try
            {
                using (StreamReader rdr = new StreamReader(path, Encoding.UTF8))
                {
                    string _section = "";
                    while ((_section = rdr.ReadLine()) != null)
                    {
                        if (_section.Length <= 0 || _section[0] == '#')
                            continue;
                        if (_section.Trim().ToUpper() == $"[{sectionName.ToUpper()}]")
                        {
                            break;
                        }
                    }

                    if (_section == "")
                        return defaultValue;

                    string _setting = "";
                    while ((_setting = rdr.ReadLine()) != null)
                    {
                        if (_setting.Length <= 0 || _setting[0] == '#')
                            continue;
                        if (_setting.Split('=')[0].Trim().ToUpper() == settingName.ToUpper())
                        {
                            _setting = _setting.Substring(_setting.IndexOf('=') + 1).Trim();
                            return _setting;
                        }


                    }
                }
                return defaultValue;
            }
            catch
            {
                return defaultValue;
            }
        }

        public static float IniReadFloat(string path, string sectionName, string settingName, float defaultValue)
        {
            string _s = IniReadString(path, sectionName, settingName, "");
            if (_s == "" || !float.TryParse(_s, out float _t))
                return defaultValue;
            return _t;
        }
        public static int IniReadInteger(string path, string sectionName, string settingName, int defaultValue)
        {
            string _s = IniReadString(path, sectionName, settingName, "");
            if (_s == "" || !int.TryParse(_s, out int _t))
                return defaultValue;
            return _t;
        }
        public static double IniReadDouble(string path, string sectionName, string settingName, double defaultValue)
        {
            string _s = IniReadString(path, sectionName, settingName, "");
            if (_s == "" || !double.TryParse(_s, out double _t))
                return defaultValue;
            return _t;
        }
        public static decimal IniReadDecimal(string path, string sectionName, string settingName, decimal defaultValue)
        {
            string _s = IniReadString(path, sectionName, settingName, "");
            if (_s == "" || !decimal.TryParse(_s, out decimal _t))
                return defaultValue;
            return _t;
        }
        public static bool IniReadBool(string path, string sectionName, string settingName, bool defaultValue)
        {
            string _s = IniReadString(path, sectionName, settingName, "");
            if (_s == "" || !bool.TryParse(_s, out bool _t))
                return defaultValue;
            return _t;
        }

        public static T IniReadGeneric<T>(string path, string sectionName, string settingName, T defaultValue = default(T))
        {
            string _s = IniReadString(path, sectionName, settingName, "");
            T _t;
            if (_s == "")
                return defaultValue;
            try
            {
                _t = (T)Convert.ChangeType(_s, typeof(T));
            }
            catch
            {
                _t = defaultValue;
            }
            return _t;
        }


        /***
         * Ini writes
        ***/

        public static bool IniWriteString(string path, string sectionName, string settingName, string settingValue)
        {
            List<string> ini;
            if (File.Exists(path))
                ini = File.ReadAllLines(path).ToList();
            else
                ini = new List<string>();
            int currentLine = 0;

            string _section = "";
            for (int i = 0; i < ini.Count; i++)// ((_section = rdr.ReadLine()) != null)
            {
                if (ini[i].Trim().ToUpper() == $"[{sectionName.ToUpper()}]")
                {
                    if (ini[i].Length <= 0 || ini[i][0] == '#')
                        continue;
                    _section = ini[i];
                    currentLine = i + 1;
                    break;
                }
            }

            if (_section == "")
            {
                ini.Add($"[{sectionName}]");
                ini.Add($"{settingName}={settingValue}");
                ini.Add(Environment.NewLine);
                using (StreamWriter wrr = new StreamWriter(path, false, Encoding.UTF8))
                {
                    foreach (string s in ini)
                        wrr.WriteLine(s);
                }

                return true;
            }
            else
            {
                string _setting = "";
                for (int i = currentLine; i < ini.Count; i++)
                {
                    _setting = ini[i];
                    if (_setting.Length <= 0 || _setting[0] == '#')
                        continue;
                    if (!_setting.Contains('='))
                        continue;
                    _setting = _setting.Substring(0, _setting.IndexOf('='));
                    if (_setting.ToUpper() == settingName.ToUpper())
                    {
                        using (StreamWriter wrr = new StreamWriter(path, false, Encoding.UTF8))
                        {
                            //ini.RemoveAt(i);
                            ini[i] = $"{settingName}={settingValue}";
                            foreach (string s in ini)
                                wrr.WriteLine(s);
                        }
                        return true;
                    }
                    _setting = "";
                }
                if (_setting == "")
                {
                    ini.Insert(currentLine, $"{settingName}={settingValue}");
                    using (StreamWriter wrr = new StreamWriter(path, false, Encoding.UTF8))
                    {
                        foreach (string s in ini)
                            wrr.WriteLine(s);
                    }
                    return true;
                }
            }

            return false;
        }

        public static bool IniWriteFloat(string path, string sectionName, string settingName, float settingValue)
        {
            return IniWriteString(path, sectionName, settingName, settingValue.ToString());
        }
        public static bool IniWriteInteger(string path, string sectionName, string settingName, int settingValue)
        {
            return IniWriteString(path, sectionName, settingName, settingValue.ToString());
        }
        public static bool IniWriteDouble(string path, string sectionName, string settingName, double settingValue)
        {
            return IniWriteString(path, sectionName, settingName, settingValue.ToString());
        }
        public static bool IniWriteDecimal(string path, string sectionName, string settingName, decimal settingValue)
        {
            return IniWriteString(path, sectionName, settingName, settingValue.ToString());
        }
        public static bool IniWriteBool(string path, string sectionName, string settingName, bool settingValue)
        {
            return IniWriteString(path, sectionName, settingName, settingValue.ToString());
        }
        public static bool IniWriteGeneric<T>(string path, string sectionName, string settingName, T settingValue)
        {
            return IniWriteString(path, sectionName, settingName, settingValue.ToString());
        }

        public static string GetOrCreateProgramAppdataFolder(string programName)
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"/" + @programName;
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            return path;
        }

        public static void LoadFromIni<T>(string iniPath)
        {
            PropertyInfo[] settings = typeof(T).GetProperties(BindingFlags.Static | BindingFlags.Public);
            foreach (PropertyInfo setting in settings)
            {
                string[] settingInfo = setting.Name.Split('_');
                if (settingInfo.Length != 2)
                    continue;
                string section = Regex.Replace(settingInfo[0], @"(?<!^)(?=[A-Z])", " ");
                string settingName = settingInfo[1];

                MethodInfo method = typeof(MonoIni).GetMethod("IniReadGeneric").MakeGenericMethod(new[] { setting.PropertyType });
                object[] param = { iniPath, section, settingName, setting.GetValue(null) };
                object iniValue = method.Invoke(null, param);
                setting.SetValue(null, iniValue);
            }

        }

        public static void SaveToIni<T>(string iniPath)
        {
            
            PropertyInfo[] settings = typeof(T).GetProperties();
            
            foreach (PropertyInfo setting in settings)
            {
                string[] settingInfo = setting.Name.Split('_');
                if (settingInfo.Length != 2)
                    continue;
                string section = Regex.Replace(settingInfo[0], @"(?<!^)(?=[A-Z])", " ");
                string settingName = settingInfo[1];

                MethodInfo method = typeof(MonoIni).GetMethod("IniWriteGeneric").MakeGenericMethod(new[] { setting.PropertyType });
                object[] param = { iniPath, section, settingName, setting.GetValue(null) };
                method.Invoke(null, param);

            }
        }
    }
}
