using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;

public class WorldConfig : MonoBehaviour
{
    public Config config;
    public string worldsPath;
    private void Start()
    {
        if (!Application.isEditor)
        {
            if (!Directory.Exists(Path.GetFullPath(Path.Combine(Application.dataPath, @"..\")) + @"\Saves"))
            {
                Directory.CreateDirectory(Path.GetFullPath(Path.Combine(Application.dataPath, @"..\")) + @"\Saves");
            }
            worldsPath = Path.GetFullPath(Path.Combine(Application.dataPath, @"..\"));
        }
        else
        {
            worldsPath = Application.persistentDataPath;
        }
        if (File.Exists(worldsPath + "/Config.xml"))
        {
            XmlSerializer s = new XmlSerializer(typeof(Config));
            using (Stream reader = new FileStream(worldsPath + "/Config.xml", FileMode.Open))
            {
                config = (Config)s.Deserialize(reader);
            }
        }
    }
}
