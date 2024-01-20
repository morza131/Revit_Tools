using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MyHelpfullTools
{
    public static class MyHelpfullTools
    {
        public static List<string> GetPathsFromTXT(this string listLinksPath)
        {
            string filePath = listLinksPath;
            string listOfPaths = String.Empty;
            using (StreamReader reader = new StreamReader(filePath))
            {
                listOfPaths = reader.ReadToEnd();
            }
            List<string> paths = listOfPaths.Split('\n', '\r').ToList();
            paths.RemoveAll(p => p == "");
            return paths;
        }
        public static string GetEnumDescription(this Enum value)
        {
            Type type = value.GetType();
            string name = Enum.GetName(type, value);
            if (name !=null)
            { 
                FieldInfo fieldInfo = type.GetField(name);
                if (fieldInfo != null)
                {
                    DescriptionAttribute attr = Attribute.GetCustomAttribute(fieldInfo, typeof(DescriptionAttribute)) as DescriptionAttribute;
                    if (attr != null)
                    { 
                        return attr.Description;
                    }
                }
            }
            return null;
        }
    }
}
