using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.SceneManagement;
using System.Linq;

public class Menu : MonoBehaviour
{
    [Header("Load")]
    public List<World> worlds;
    public Config config;
    public GameObject worldItem, worldHolder, loadScreen;


    [Header("Create")]
    public TMP_Text SHM_text;
    public TMP_Text OFFX_text, OFFY_text, Height_text, Sizex_text, Sizey_text, Caves_text, upScale_text;
    public Slider SHM_slider, OFFX_slider, OFFY_slider, Height_slider, Sizex_slider, Sizey_slider, Caves_slider, upScale_slider;
    public TMP_InputField tb_name, tb_desc;
    public Toggle genAllWorld;
    [Space]
    public GameObject createButton;

    public int selectedIndex;

    public List<GameObject> backgrounds;

    public GameObject playWButton, deleteWButton;
    [Header("Options")]
    public TMP_Dropdown resolution, qual;
    public Toggle fullscreen, eng;
    [Header("Saves")]
    public string savesForlder;
    public string folderToDelete;
    public string worldsPath;
    bool onCreate = false;
    [Header("Add World")]
    public Transform addContent;
    public Transform addItem;
    public GameObject Error;

    public void UpdateAddItems()
    {
        List<string> names = new List<string>();
        var files = Directory.GetFiles(savesForlder,"*.world");

        for (int i = 0; i < files.Length; i++)
        {
            if (File.ReadAllText(files[i]) == "")
            {
                continue;
            }
            bool ex = false;
            for (int j = 0; j < worlds.Count; j++)
            {
                if (worlds[j].path == Path.GetFileName(files[i]))
                {
                    ex = true; break;
                }
                
            }
            if (ex == false)
            {
                names.Add(Path.GetFileName(files[i]));
            }
        }

        foreach (Transform item in addContent)
        {
            Destroy(item.gameObject);
        }

        for (int i = 0; i < names.Count; i++)
        {
            var g = Instantiate(addItem, addContent);
            g.GetComponentInChildren<TMP_Text>().text = names[i];
            g.gameObject.SetActive(true);
        }


    }
    public void AddWorld()
    {
        AddWorldItem namebtn = FindObjectsOfType<AddWorldItem>().ToList().Find(x => x.select == true);

        selectedIndex = -1;
        onCreate = true;
        var p = Path.GetFileNameWithoutExtension(namebtn.GetComponentInChildren<TMP_Text>().text) + ".world";
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(savesForlder + p, FileMode.Open);
        WorldGenerator.WorldData data = new WorldGenerator.WorldData();
        try
        {
            data = (WorldGenerator.WorldData)bf.Deserialize(file);
            file.Dispose();
        }
        catch (System.Exception)
        {
            file.Dispose();
            Error.SetActive(true);
            return;
        }

        worlds.Add(new World(
            data.name,
            data.desc,
            data.smh,
            data.offcetX,
            data.offcetY,
            System.DateTime.Now.ToString(),
            p,
            data.height,
            data.sizex,
            data.sizey,
            data.caves, data.upScale, false));

        SaveWorlds();
        UpdateWorlds();
        UpdateAddItems();
        onCreate = false;
    }



    private void Start()
    {
        Time.timeScale = 1;
        SHM_slider.value = 5;
        OFFX_slider.value = Random.Range(0, 1000);
        OFFY_slider.value = Random.Range(0, 1000);
        backgrounds[Random.Range(0, backgrounds.Count)].SetActive(true);

        if (!Application.isEditor && !SystemInfo.operatingSystem.Contains("Android"))
        {
            if (!Directory.Exists(Path.GetFullPath(Path.Combine(Application.dataPath, @"..\")) + @"\Saves"))
            {
                PlayerPrefs.DeleteAll();
                Screen.SetResolution(1920,1080, false);
                QualitySettings.SetQualityLevel(0);
                Directory.CreateDirectory(Path.GetFullPath(Path.Combine(Application.dataPath, @"..\")) + @"\Saves");
            }
            //savesForlder = Path.GetFullPath(Path.Combine(Application.dataPath, @"..\")) + @"\Saves";
            //worldsPath = Path.GetFullPath(Path.Combine(Application.dataPath, @"..\"));

            savesForlder = Path.Combine(Application.dataPath, @"..\") + @"\Saves\";
            worldsPath = Path.Combine(Application.dataPath, @"..\");
        }
        else
        {
            //savesForlder = Application.persistentDataPath + @"/Saves";
            //worldsPath = Application.persistentDataPath;

            savesForlder = Application.persistentDataPath + @"/Saves\";
            worldsPath = Application.persistentDataPath + @"\";
        }
        if (!Directory.Exists(savesForlder + @"\Deleted worlds"))
        {
            Directory.CreateDirectory( savesForlder + @"\Deleted worlds");
        }
        folderToDelete = savesForlder + @"\Deleted worlds\";

        LoadConfig();
        LoadWorlds();
        UpdateWorlds();

        UpdateAddItems();
    }


    public void UpdFon()
    {
        int active = 0;
        for (int i = 0; i < backgrounds.Count; i++)
        {
            if (backgrounds[i].active == true)
            {
                active = i;
            }
        }
        active++;
        if (active > backgrounds.Count - 1)
        {
            active = 0;
        }
        for (int i = 0; i < backgrounds.Count; i++)
        {
            backgrounds[i].SetActive(false);
        }
        backgrounds[active].SetActive(true);
    }


    private void FixedUpdate()
    {
        if (genAllWorld.isOn) {
            Sizex_slider.minValue = 10;
            Sizey_slider.minValue = 8;
        }
        else
        {

            Sizex_slider.minValue = 15;
            Sizey_slider.minValue = 8;
        }

        if (selectedIndex != -1)
        {
            playWButton.SetActive(true);
            if (onCreate == false)
            {
                deleteWButton.SetActive(true);
            }
        }
        else
        {
            playWButton.SetActive(false);
            deleteWButton.SetActive(false);
        }
        if (config.eng == false)
        {
            SHM_text.text = "Процент шума: " + SHM_slider.value.ToString("F2");
            OFFX_text.text = "Смещение X: " + OFFX_slider.value.ToString();
            OFFY_text.text = "Смещение Y: " + OFFY_slider.value.ToString();
            Height_text.text = "Высота: " + Height_slider.value.ToString();
            Sizex_text.text = "Чанков по X: " + Sizex_slider.value.ToString();
            Sizey_text.text = "Чанков по Y: " + Sizey_slider.value.ToString();
            Caves_text.text = "Пещерность: " + Caves_slider.value.ToString();
            upScale_text.text = "Плавность ланшафта: " + upScale_slider.value.ToString();
        }
        else
        {
            SHM_text.text = "Noise %: " + SHM_slider.value.ToString("F2");
            OFFX_text.text = "Offcet X: " + OFFX_slider.value.ToString();
            OFFY_text.text = "Offcet Y: " + OFFY_slider.value.ToString();
            Height_text.text = "Height: " + Height_slider.value.ToString();
            Sizex_text.text = "Chunks X: " + Sizex_slider.value.ToString();
            Sizey_text.text = "Chunks Y: " + Sizey_slider.value.ToString();
            Caves_text.text = "Caves: " + Caves_slider.value.ToString();
            upScale_text.text = "Smoothness: " + upScale_slider.value.ToString();
        }
        bool exist = false;
        for (int i = 0; i < worlds.Count; i++)
        {
            if (worlds[i].name.Replace(" ", "").ToLower() == tb_name.text.Replace(" ", "").ToLower())
            {
                exist = true;
            }
        }
        if (exist == false)
        {
            createButton.SetActive(true);
        }
        else
        {
            createButton.SetActive(false);
        }
    }

    public void Play()
    {
        SaveWorlds();

        PlayerPrefs.SetString("Name", worlds[selectedIndex].name);
        PlayerPrefs.SetString("Desc", worlds[selectedIndex].desc);
        PlayerPrefs.SetInt("Offx", worlds[selectedIndex].offx);
        PlayerPrefs.SetInt("Offy", worlds[selectedIndex].offy);
        PlayerPrefs.SetFloat("Smh", worlds[selectedIndex].smh);
        PlayerPrefs.SetString("Path", worlds[selectedIndex].path);
        PlayerPrefs.SetString("Start", worlds[selectedIndex].firstStart.ToString());
        PlayerPrefs.SetInt("Height", worlds[selectedIndex].height);
        PlayerPrefs.SetInt("SizeX", worlds[selectedIndex].sizex);
        PlayerPrefs.SetInt("SizeY", worlds[selectedIndex].sizey);
        PlayerPrefs.SetFloat("Caves", worlds[selectedIndex].caves);
        PlayerPrefs.SetInt("upScale", worlds[selectedIndex].upScale);
        PlayerPrefs.SetInt("AllGen", worlds[selectedIndex].allGen == true ? 1 : 0);

        loadScreen.SetActive(true);
        StartCoroutine(waitToPlay());
    }

    IEnumerator waitToPlay()
    {
        yield return new WaitForSeconds(1);
        if (File.Exists(savesForlder + worlds[selectedIndex].path) && File.ReadAllText(savesForlder + worlds[selectedIndex].path) != "")
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(savesForlder + worlds[selectedIndex].path, FileMode.Open);
            WorldGenerator.WorldData data = (WorldGenerator.WorldData)bf.Deserialize(file);
            WorldManager worldManager = FindObjectOfType<WorldManager>();
            file.Close();
            if (data.firstStart == false)
            {
                yield return new WaitForSeconds(2);
                StartCoroutine(LoadYourAsyncScene(1));
                yield break;
            }
            else
            {
                StartCoroutine(LoadYourAsyncScene(2));
                yield break;
            }
        }
        if (File.ReadAllText(savesForlder + worlds[selectedIndex].path) == "")
        {
            StartCoroutine(LoadYourAsyncScene(2));
            yield break;
        }
    }

    IEnumerator LoadYourAsyncScene(int scene)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(scene);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    public void CreateWorld()
    {

        selectedIndex = -1;
        onCreate = true;
        var p = tb_name.text + ".world";
        File.Create(savesForlder + p).Dispose();

        worlds.Add(new World(
            tb_name.text, 
            tb_desc.text, 
            SHM_slider.value, 
            (int)OFFX_slider.value, 
            (int)OFFY_slider.value, 
            System.DateTime.Now.ToString(), 
            p, 
            (int)Height_slider.value, 
            (int)Sizex_slider.value, 
            (int)Sizey_slider.value, 
            Caves_slider.value, (int)upScale_slider.value, genAllWorld.isOn));

        SaveWorlds();
        UpdateWorlds();
        onCreate = false;
    }

    public void RemoveWorld()
    {
        if (File.Exists(savesForlder + worlds[selectedIndex].path))
        {
            print(("(" + System.DateTime.Now.ToString() + ") " + Path.GetFileName(savesForlder + worlds[selectedIndex].path)));
            File.Move(savesForlder + worlds[selectedIndex].path, folderToDelete + "/" + ("("+ System.DateTime.Now.ToString().Replace(':', '-') + ") " + worlds[selectedIndex].path));
        }
        worlds.RemoveAt(selectedIndex);
        selectedIndex = -1;
        UpdateWorlds();
        SaveWorlds();
    }

    public void UpdateWorlds()
    {
        foreach (Transform item in worldHolder.transform)
        {
            Destroy(item.gameObject);
        }
        for (int i = 0; i < worlds.Count; i++)
        {
            GameObject item = Instantiate(worldItem, worldHolder.transform);
            WorldItem witem = item.GetComponent<WorldItem>();
            witem.name.text = worlds[i].name;
            witem.desc.text = worlds[i].desc;
            witem.info.text = worlds[i].date;
            witem.size.text = (new FileInfo(savesForlder + worlds[i].path).Length/1024+1) + "KB";
            witem.index = i;
        }
    }

    public void SaveWorlds()
    {
        XmlSerializer s = new XmlSerializer(typeof(Worlds));
        TextWriter myWriter = new StreamWriter(worldsPath +  "/Worlds.xml");
        Worlds scene = new Worlds();

        scene.worlds = worlds.ToArray();
       
        s.Serialize(myWriter, scene);
        myWriter.Close();

    }

    public void LoadWorlds()
    {
        if (File.Exists(worldsPath + "/Worlds.xml"))
        {
            XmlSerializer s = new XmlSerializer(typeof(Worlds));
            using (Stream reader = new FileStream(worldsPath + "/Worlds.xml", FileMode.Open))
            {
                // Call the Deserialize method to restore the object's state.
                Worlds w = (Worlds)s.Deserialize(reader);
                for (int i = 0; i < w.worlds.Length; i++)
                {
                    //Debug.LogError(savesForlder + w.worlds[i].path);
                    if (File.Exists(savesForlder + w.worlds[i].path))
                    {
                        worlds.Add(w.worlds[i]);
                    }
                }
                UpdateWorlds();
            }
        }
    }




    public void OpenPathWorlds()
    {
        if (Application.isEditor)
        {
            OpenPath(savesForlder);
        }
        else
        {
            OpenPath(savesForlder);
        }
    }

    public void OpenPathDeleted()
    {
        if (Application.isEditor)
        {
            OpenPath(folderToDelete);
        }
        else
        {
            OpenPath(folderToDelete);
        }
    }

    public void OpenPath(string path)
    {
        Application.OpenURL(path);
    }

    public void SaveConfig() {
        XmlSerializer s = new XmlSerializer(typeof(Config));
        TextWriter myWriter = new StreamWriter(worldsPath + "/Config.xml");
        Config cfg = new Config();
        cfg.screenWidth = int.Parse(resolution.options[resolution.value].text.Split('x')[0]);
        cfg.screenHeight = int.Parse(resolution.options[resolution.value].text.Split('x')[1]);
        cfg.full = fullscreen.isOn;
        cfg.eng = eng.isOn;
        cfg.selRes = resolution.value;
        cfg.qual = qual.value;
        QualitySettings.SetQualityLevel(qual.value);
        s.Serialize(myWriter, cfg);
        myWriter.Close();
        LoadConfig();
    }

    public void LoadConfig()
    {
        if (File.Exists(worldsPath + "/Config.xml"))
        {
            XmlSerializer s = new XmlSerializer(typeof(Config));
            using (Stream reader = new FileStream(worldsPath + "/Config.xml", FileMode.Open))
            {
                config = (Config)s.Deserialize(reader);
                fullscreen.isOn = config.full;
                eng.isOn = config.eng;
                resolution.value = config.selRes;
                qual.value = config.qual;
                QualitySettings.SetQualityLevel(qual.value);
                Screen.SetResolution(config.screenWidth, config.screenHeight, config.full);
                UpdateWorlds();
            }
        }
        else
        {
            fullscreen.isOn = true;
            eng.isOn = true;
            qual.value = 0;
            QualitySettings.SetQualityLevel(0);
            resolution.value = 2;
            SaveConfig();
        }
    }


    public void Exit()
    {
        Application.Quit();
    }
}
[System.Serializable]
public class Worlds {
    [XmlArrayItem(ElementName = "World",
             IsNullable = true,
             Type = typeof(World))]
    [XmlArray]
    public World[] worlds;

    public Worlds()
    {

    }
}



[System.Serializable]
public class Config { 
    public int screenWidth , screenHeight, selRes, qual;
    public bool full;
    public bool eng;
    
    public Config()
    {

    }
}


[System.Serializable]
public class World
{
    public string name, desc;
    public int offx, offy, height;
    public float smh, caves;
    public string date;
    public string path;
    public int sizex, sizey;
    public bool firstStart;
    public int upScale;
    public bool allGen;

    public World(string name, string desc, float smh, int offx, int offy, string date, 
        string path, int height, int sizex, int sizey, float caves, int upScale, bool allGen)
    {
        this.name = name;
        this.desc = desc;
        this.smh = smh;
        this.offx = offx;
        this.offy = offy;
        this.date = date;
        this.path = path;
        this.height = height;
        this.sizex = sizex;
        this.sizey = sizey;
        this.caves = caves;
        this.upScale = upScale;
        this.allGen = allGen;
    }
    public World()
    {

    }
}
