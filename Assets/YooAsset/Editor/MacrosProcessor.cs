using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using UnityEditor;
using UnityEngine;

namespace YooAsset.Editor
{
    [InitializeOnLoad]
    class YooMacrosProcessor : AssetPostprocessor
    {
        static readonly List<string> YooAssertMacros = new List<string>()
        {
            "YOO_VERSION_2",
            "YOO_VERSION_2_3_4",
            "YOO_VERSION_2_3_4_OR_NEWER",
        };
        static string OnGeneratedCSProject(string path, string content)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(content);

            if (!IsCSProjectReferenced(xmlDoc.DocumentElement))
            {
                return content;
            }

            if (!ProcessDefineConstants(xmlDoc.DocumentElement, YooAssertMacros))
            {
                return content;
            }

            StringWriter sw = new StringWriter();
            XmlWriter writer = XmlWriter.Create(sw, new XmlWriterSettings()
            {
                Indent = true,
            });
            xmlDoc.WriteTo(writer);
            writer.Flush();

            return sw.ToString();
        }

        static bool ProcessDefineConstants(XmlElement element, List<string> macros)
        {
            if (element == null)
            {
                return false;
            }

            bool processed = false;
            if (macros == null || macros.Count == 0)
            {
                return processed;
            }

            foreach (XmlNode node in element.ChildNodes)
            {
                if (node.Name != "PropertyGroup")
                {
                    continue;
                }

                foreach (XmlNode childNode in node.ChildNodes)
                {
                    if (childNode.Name != "DefineConstants")
                    {
                        continue;
                    }

                    string[] defines = childNode.InnerText.Split(';');
                    HashSet<string> hs = new HashSet<string>(defines);

                    string tmpMacro = string.Empty;
                    foreach (string macro in macros)
                    {
                        tmpMacro = macro.Trim();
                        if (string.IsNullOrEmpty(tmpMacro))
                            continue;
                        //加入YooAsset定义的宏
                        hs.Add(tmpMacro);
                    }
                    //更新节点InnerText
                    childNode.InnerText = string.Join(";", hs.ToArray());

                    processed = true;
                }
            }

            return processed;
        }

        static bool IsCSProjectReferenced(XmlElement element)
        {
            if (element == null)
            {
                return false;
            }

            foreach (XmlNode node in element.ChildNodes)
            {
                if (node.Name != "ItemGroup")
                {
                    continue;
                }
                foreach (XmlNode childNode in node.ChildNodes)
                {
                    if (childNode.Name != "Reference" && childNode.Name != "ProjectReference")
                    {
                        continue;
                    }
                    //工程引用了YooAsset
                    string include = childNode.Attributes["Include"].Value;
                    if (include.Contains("YooAsset"))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }

}
