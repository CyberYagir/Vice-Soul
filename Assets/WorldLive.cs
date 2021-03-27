using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WorldLive : MonoBehaviour
{
    public Transform localPlayer;

    public Tilemap main;

    public Vector2Int fullsize, camSize;



    public List<Mob> mobs = new List<Mob>();

    public int mobsCount;
    public List<GameObject> mobsSpawned = new List<GameObject>();
    public List<GameObject> mobsPieceSpawned = new List<GameObject>();

    public float waitTime = 5;

    [Header("Time")]
    [Header("Управлят временем суток (фонами)")]
    public TimeCycle timeCycle;
    public Renderer backgroundsSky;
    public Renderer[] backgrounds;
    public Texture2D[] biomForests;
    public List<Color> stBackgrounds = new List<Color>();
    public Light sun;
    public Color stColor;

    public float intensivity;

    public WorldGenerator generator;
    public WorldGenerator.Chunk oldchunk;

    private void Start()
    {
        oldchunk.biom = -1;
        backgroundsSky.material.EnableKeyword("_EMISSION");
        stColor = backgroundsSky.material.GetColor("_EmissionColor");
        StartCoroutine(whil());
        for (int i = 0; i < backgrounds.Length; i++)
        {
            backgrounds[i].material.EnableKeyword("_EMISSION");
            stBackgrounds.Add(backgrounds[i].material.GetColor("_EmissionColor"));
        }
    }
    private void FixedUpdate()
    {
        if (oldchunk.biom != generator.playerChunk.biom)
        {
            oldchunk = generator.playerChunk;
            StartCoroutine(changeBiomBack());
        }
        if (timeCycle.hours >= 5 && timeCycle.hours <= 20)
        {
            if (intensivity < 0)
            {
                intensivity += Time.deltaTime / 5;
            }
            backgroundsSky.material.SetColor("_EmissionColor", stColor * intensivity);
            
        }
        if (timeCycle.hours < 5 || timeCycle.hours > 20)
        {
            if (intensivity > -5)
            {
                intensivity -= Time.deltaTime/5;
            }
            backgroundsSky.material.SetColor("_EmissionColor", stColor * intensivity);
        }
        for (int i = 0; i < stBackgrounds.Count; i++)
        {
            if (intensivity > -0.9f)
            {
                backgrounds[i].material.color = new Color(intensivity + 1, intensivity + 1, intensivity + 1, backgrounds[i].material.color.a);
            }
        }
        sun.color = new Color(intensivity + 1, intensivity + 1, intensivity + 1, 1);
    }

    IEnumerator changeBiomBack()
    {
        backgrounds[3].material.mainTexture = backgrounds[1].material.mainTexture;
        backgrounds[3].material.mainTexture = biomForests[generator.playerChunk.biom];
        backgrounds[3].material.color = new Color(intensivity + 1, intensivity + 1, intensivity + 1, 0);
        
        for (int i = 0; i < 5; i++)
        {
            backgrounds[3].material.color = new Color(intensivity + 1, intensivity + 1, intensivity + 1, (1f/5f)*i);
            yield return new WaitForSeconds(0.5f / 5f);
        }
        backgrounds[1].material.mainTexture = biomForests[generator.playerChunk.biom];
        
    }
    IEnumerator whil()
    {
        while (true)
        {
            yield return new WaitForSeconds(waitTime);
            Spawn();
        }
    }

    void Spawn()
    {
        for (int i = 0; i < mobsSpawned.Count; i++)
        {
            if (mobsSpawned[i] != null)
            {
                if (Vector2.Distance(localPlayer.transform.position, mobsSpawned[i].transform.position) > 80)
                {
                    Destroy(mobsSpawned[i].gameObject);
                    mobsSpawned.RemoveAt(i);
                    break;
                }
            }
            else
            {
                mobsSpawned.RemoveAt(i);
            }
        }
        if (mobsSpawned.Count <= mobsCount)
        {
            Vector3Int ppos = main.WorldToCell(localPlayer.position);
            for (int x = -fullsize.x; x < fullsize.x; x++)
            {
                for (int y = -fullsize.y; y < fullsize.y; y++)
                {
                    if (x < -camSize.x || x > camSize.x)
                    {
                        if (y > -camSize.y || y < camSize.y)
                        {
                            var newpos = ppos + new Vector3Int(x, y, 0);

                            if (main.GetTile(newpos) != null)
                            {
                                if (main.GetTile(ppos + new Vector3Int(x, y + 1, 0)) == null)
                                {
                                    int trys = 0;
                                    int id = Random.Range(0, mobs.Count);
                                    int curr = Random.Range(0, mobs[id].chance);
                                    while (trys < 10)
                                    {
                                        if (curr == 5)
                                        {
                                            if (mobs[id].y-mobs[id].yrange < localPlayer.position.y + fullsize.y && mobs[id].y + mobs[id].yrange > localPlayer.position.y - fullsize.y)
                                            {
                                                bool canSpawn = false;
                                                if (mobs[id].spawnTime == Mob.time.AllTime)
                                                {
                                                    canSpawn = true;
                                                }else
                                                if (mobs[id].spawnTime == Mob.time.Day && FindObjectOfType<TimeCycle>().isNight == false)
                                                {
                                                    canSpawn = true;
                                                }else
                                                if (mobs[id].spawnTime == Mob.time.Night && FindObjectOfType<TimeCycle>().isNight == true)
                                                {
                                                    canSpawn = true;
                                                }
                                                if (canSpawn)
                                                {
                                                    if (mobs[id].biomID == -1 || mobs[id].biomID == GetComponent<WorldGenerator>().playerChunk.biom)
                                                    {
                                                        GameObject gameObject = Instantiate(mobs[id].prefab, ppos + new Vector3(x, y + mobs[id].spawnYOffcet, 0), Quaternion.identity);
                                                        if (mobs[id].agressive == true)
                                                        {
                                                            mobsSpawned.Add(gameObject);
                                                        }
                                                        else
                                                        {
                                                            mobsPieceSpawned.Add(gameObject);
                                                        }
                                                    }
                                                }
                                                break;
                                            }
                                        }
                                        trys++;
                                        id = Random.Range(0, mobs.Count);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}


[System.Serializable]
public class Mob {
    public string name;
    public GameObject prefab;
    public int y, yrange;
    [Range(5, 1000)]
    public int chance;
    public float spawnYOffcet;
    public bool agressive = true;
    public int biomID;
    public enum time {Day, Night, AllTime};
    public time spawnTime;
}

