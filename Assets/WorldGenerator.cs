
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WorldGenerator : MonoBehaviour
{
    // Width and height of the texture in pixels.
    public int pixWidth;
    public int pixHeight;

    // The origin of the sampled area in the plane.
    public float xOrg;
    public float yOrg;
    public float fullest;
    public float shm;
    // The number of cycles of the basic noise pattern that are repeated
    // over the width and height of the texture.
    public float scale = 1.0F;
    public int upScale = 2;
    public float heightScale;
    public int octaves;
    public float persistance;
    public float lacunar;
    public int dopOffcetX, dopOffcetY;
    public int upWidthScale;
    [HideInInspector]
    public int offcetX, offcetY;
    [HideInInspector]
    public Texture2D noiseTex;

    [HideInInspector]
    public Texture2D noiseTex2;
    [HideInInspector]
    public Color[] pix;

    public Material matDiff;

    public int startY = -199;
    [HideInInspector]
    public bool startGen = false;

    public Vector2Int positions = new Vector2Int(0, -200);
    public Tilemap back, main, decor;

    public TileBase[] tileBases;
    public Dungeon[] dungeons;
    public int chunksHorizontalCount, chunksVerticalCount;


    public List<List<Chunk>> chunks = new List<List<Chunk>>();

    public GameObject[] trees;

    public List<GameObject> lastTree;

    public List<Ore> legacyOres = new List<Ore>();

    public Transform player;

    public string name, path;

    public TMPro.TMP_Text loadText;
    public GameObject overlay, saveOverlay;
    public WorldConfig config;
    [Space]

    public Chunk playerChunk;
    public Vector2Int playerChunkPos;

    public int fps;

    IEnumerator gen;
    
    public System.Random rnd;

    void Start()
    {
        rnd = new System.Random(int.Parse(System.DateTime.Now.ToString("T").Replace(":", "")));
        QualitySettings.vSyncCount = 0;  // VSync must be disabled
        name = PlayerPrefs.GetString("Name");
        dopOffcetX = PlayerPrefs.GetInt("Offx");
        dopOffcetX = PlayerPrefs.GetInt("Offy");
        shm = PlayerPrefs.GetFloat("Smh");
        var savesForlder = "";

        if (!Application.isEditor)
        {
            savesForlder = Path.Combine(Application.dataPath, @"..\") + @"\Saves\";
        }
        else
        {
            savesForlder = Application.persistentDataPath + @"/Saves\";
        }



        path = savesForlder + PlayerPrefs.GetString("Path");
        heightScale = PlayerPrefs.GetInt("Height");
        chunksHorizontalCount = PlayerPrefs.GetInt("SizeX");
        chunksVerticalCount = PlayerPrefs.GetInt("SizeY");
        fullest = PlayerPrefs.GetFloat("Caves");
        upScale = PlayerPrefs.GetInt("upScale");

        offcetX = offcetX + dopOffcetX;
        offcetY = offcetY + dopOffcetY;

        player.transform.position = new Vector3((chunksHorizontalCount * pixWidth) / 2, player.transform.position.y, player.transform.position.z);

        // Set up the texture and a Color array to hold pixels during processing.
        //chunkBiomesTexture = _CalcBiomes();
        noiseTex = new Texture2D(pixWidth, pixHeight);
        pix = new Color[noiseTex.width * noiseTex.height];
        //rend.material.mainTexture = noiseTex;
        positions.x = 0;
        xOrg = positions.x;

        for (int x = 0; x < chunksHorizontalCount; x++)
        {
            chunks.Add(new List<Chunk>());
            positions.y = startY;
            yOrg = startY;
            for (int y = 0; y < chunksVerticalCount; y++)
            {
                GameObject ch = new GameObject();
                ch.name = x + "-" + y;
                ch.transform.position = new Vector3(positions.x, positions.y, 0);
                chunks[x].Add(new Chunk(null, positions, ch) { generated = false, biom = Random.Range(0, 3), chunkTexture = null});
                //Generate(chunks[x][y].chunkTexture, positions);  
                ch.SetActive(true);
                positions.y -= pixHeight;
                yOrg = positions.y;
            }
            positions.x += pixWidth;
            xOrg = positions.x;
        }
        for (int x = 0; x < chunksHorizontalCount; x++)
        {
            chunks[x][1].biom = chunks[x][0].biom;
        }


        StartLoad();
        gen = GenChunks();
        StartCoroutine(gen);
    }


    public void StartLoad()
    {
        if (File.ReadAllText(path) != "")
        {
            Load();
        }
        else
        {
            StartCoroutine(startChunks());
        }
    }
    private void FixedUpdate()
    {
        int generated = 0;
        for (int x = 0; x < chunksHorizontalCount; x++)
        {
            for (int y = 0; y < chunksVerticalCount; y++)
            {
                if (chunks[x][y].enumerator != null)
                {
                    generated++;
                }
            }
        }
        Debug.LogError(generated);
        Application.targetFrameRate = fps;
        if (gen == null)
        {
            gen = GenChunks();
            StartCoroutine(gen);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F6))
        {
            StartCoroutine(Save());
        }
    }
    public Vector2Int GetChunkByPos(Transform obj)
    {
        Vector2Int positions = new Vector2Int(0, 0);
        positions.x = 0;
        for (int x = 0; x < chunksHorizontalCount; x++)
        {
            positions.y = -199;
            for (int y = 0; y < chunksVerticalCount; y++)
            {
                if (obj.position.x > positions.x && obj.position.x < positions.x + pixWidth)
                {
                    if (obj.position.y - (pixHeight) < positions.y && obj.position.y > positions.y - (pixHeight))
                    {
                        return new Vector2Int(x, y);
                    }
                }
                positions.y -= pixHeight;
            }
            positions.x += pixWidth;
        }
        return new Vector2Int();
    }


    IEnumerator startChunks()
    {

        bool genAll = PlayerPrefs.GetInt("AllGen") == 1;
        if (genAll == false)
        {
            player.GetComponent<Rigidbody2D>().isKinematic = true;
            int y = 0;
            for (int x = (int)(chunksHorizontalCount * 0.3f); x < chunksHorizontalCount - (int)(chunksHorizontalCount * 0.3f); x++)
            {
                player.transform.position = chunks[x][y].pos + new Vector2(pixWidth / 2, pixHeight / 2);
                yield return new WaitForSeconds(0.6f);
                player.transform.position = chunks[x][y + 1].pos + new Vector2(pixWidth / 2, pixHeight / 2);
                yield return new WaitForSeconds(0.6f);
                chunks[x][y].objects.SetActive(false);
                chunks[x][y + 1].objects.SetActive(false);
            }
            player.transform.position = new Vector3(player.transform.position.x, 0);
            startGen = true;
            player.GetComponent<Rigidbody2D>().isKinematic = false;
        }
        else
        {

            yield return new WaitForSeconds(1);
            player.GetComponent<Rigidbody2D>().isKinematic = true;
            for (int x = 0; x < chunksHorizontalCount; x++)
            {
                for (int y = 0; y < chunksVerticalCount; y++)
                {
                    yield return new WaitForSeconds(0.001f);
                    positions = chunks[x][y].pos;
                    playerChunk = chunks[x][y];
                    playerChunkPos = new Vector2Int(x, y);
                    if (chunks[x][y].generated == false)
                    {
                        chunks[x][y].chunkTexture = _CalcNoise(new Vector2Int(x, y));
                        Generate(chunks[x][y].chunkTexture, new Vector2Int(x, y), chunks[x][y].biom, new Vector2Int(x, y));
                        chunks[x][y].generated = true;
                        chunks[x][y].objects.SetActive(false);
                        chunks[x][y].enumerator = GenerationUpdate(chunks[x][y].pos, new Vector2Int(x, y), true, chunks[x][y].biom);

                        StartCoroutine(chunks[x][y].enumerator);
                    }
                    chunks[x][y].objects.SetActive(false);
                }
            }
            player.transform.position = new Vector3(player.transform.position.x, 0);
            startGen = true;
            player.GetComponent<Rigidbody2D>().isKinematic = false;
        }


        StartCoroutine(Save());
    }

    IEnumerator GenChunks()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.2f);

            positions.x = 0;
            for (int x = 0; x < chunksHorizontalCount; x++)
            {
                positions.y = -199;
                for (int y = 0; y < chunksVerticalCount; y++)
                {
                    if (player.position.x + Camera.main.orthographicSize * 2 > positions.x && player.position.x - Camera.main.orthographicSize * 2 < positions.x + pixWidth)
                    {
                        if (player.position.y - (pixHeight + Camera.main.orthographicSize * 2) < positions.y && player.position.y > positions.y - (pixHeight + Camera.main.orthographicSize * 2))
                        {
                            // X Y
                            playerChunk = chunks[x][y];
                            playerChunkPos = new Vector2Int(x, y);
                            if (chunks[x][y].generated == false)
                            {
                                overlay.SetActive(true);
                                loadText.text = "";
                                if (chunks[x][y].chunkTexture == null)
                                {
                                    if (!config.config.eng)
                                        loadText.text = "Генерация [" + x + ":" + y + "]: Расчёт шума [1/3]";
                                    else
                                        loadText.text = "Generation [" + x + ":" + y + "]: Noise calc [1/3]";
                                    yield return new WaitForSeconds(0.01f);

                                    chunks[x][y].chunkTexture = _CalcNoise(new Vector2Int(x, y));
                                }
                                if (!config.config.eng)
                                    loadText.text = "Генерация [" + x + ":" + y + "]: Генерация каркаса [2/3]";
                                else
                                    loadText.text = "Generation [" + x + ":" + y + "]: Shapes generation [2/3]";
                                yield return new WaitForSeconds(0.01f);
                                Generate(chunks[x][y].chunkTexture, new Vector2Int(x, y), chunks[x][y].biom, new Vector2Int(x,y));
                                if (!config.config.eng)
                                    loadText.text = "Генерация [" + x + ":" + y + "]: Генерация окружения [3/3]";
                                else
                                    loadText.text = "Generation [" + x + ":" + y + "]: Environment generation [3/3]";
                                yield return new WaitForSeconds(0.01f);
                                chunks[x][y].generated = true;
                                chunks[x][y].enumerator = GenerationUpdate(chunks[x][y].pos, new Vector2Int(x, y), true, chunks[x][y].biom);
                                StartCoroutine(chunks[x][y].enumerator);
                                

                                yield return new WaitForSeconds(0.01f);
                                player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                                overlay.SetActive(false);
                            }
                            chunks[x][y].objects.SetActive(true);

                        }
                        else
                        {
                            chunks[x][y].objects.SetActive(false);
                        }
                    }
                    else
                    {

                        chunks[x][y].objects.SetActive(false);
                    }
                    positions.y -= pixHeight;
                }
                positions.x += pixWidth;
            }
            overlay.SetActive(false);
        }
    }

    void Generate(Texture2D tex, Vector2 pos, int biom, Vector2Int cords)
    {
        List<Vector3Int> upempty = new List<Vector3Int>(), downempty = new List<Vector3Int>();
        for (int x = 0; x < tex.width; x++)
        {
            for (int y = 0; y < tex.height; y++)
            {
                if (tex.GetPixel(x, y + 1) == Color.white || tex.GetPixel(x, y + 1) == Color.red)   upempty.Add(new Vector3Int(positions.x, positions.y, 0) + new Vector3Int(x, y,0));
                if (tex.GetPixel(x, y - 1) == Color.white || tex.GetPixel(x, y - 1) == Color.red) downempty.Add(new Vector3Int(positions.x, positions.y, 0) + new Vector3Int(x, y,0));

                if (back.GetTile(new Vector3Int(positions.x, positions.y, 0) + new Vector3Int(x, y, 0)) == null && main.GetTile(new Vector3Int(positions.x, positions.y, 0) + new Vector3Int(x, y, 0)) == null)
                {
                    if (biom == 0)
                    {
                        if (pos.y < chunksVerticalCount - 2)
                        {
                            if (tex.GetPixel(x, y) != Color.red)
                            {
                                back.SetTile(new Vector3Int(positions.x, positions.y, 0) + new Vector3Int(x, y, 0), tileBases[0]);
                            }
                            if (tex.GetPixel(x, y) == Color.black && positions.y < 0)
                            {
                                main.SetTile(new Vector3Int(positions.x, positions.y, 0) + new Vector3Int(x, y, 0), tileBases[1]);
                                back.SetTile(new Vector3Int(positions.x, positions.y, 0) + new Vector3Int(x, y, 0), tileBases[1]);
                            }
                            if (tex.GetPixel(x, y) == Color.black && positions.y > -200 - RandomRange(0, 10))
                            {
                                main.SetTile(new Vector3Int(positions.x, positions.y, 0) + new Vector3Int(x, y, 0), tileBases[0]);
                                back.SetTile(new Vector3Int(positions.x, positions.y, 0) + new Vector3Int(x, y, 0), tileBases[0]);
                            }
                        }

                        if (pos.y == chunksVerticalCount - 2)
                        {
                            //print(tex.GetPixel(x, y));
                            if (tex.GetPixel(x, y) == Color.white)
                            {
                                if (RandomRange(y, tex.height) == tex.height - 1)
                                {
                                    back.SetTile(new Vector3Int(positions.x, positions.y, 0) + new Vector3Int(x, y, 0), tileBases[0]);
                                }
                                else
                                {
                                    back.SetTile(new Vector3Int(positions.x, positions.y, 0) + new Vector3Int(x, y, 0), tileBases[3]);
                                }
                            }
                            if (tex.GetPixel(x, y) == Color.black && positions.y < 0)
                            {
                                int rn = RandomRange(y, tex.height);
                                if (rn < y + 10 && rn > y - 10)
                                {
                                    main.SetTile(new Vector3Int(positions.x, positions.y, 0) + new Vector3Int(x, y, 0), tileBases[1]);
                                    back.SetTile(new Vector3Int(positions.x, positions.y, 0) + new Vector3Int(x, y, 0), tileBases[1]);
                                }
                                else
                                {
                                    main.SetTile(new Vector3Int(positions.x, positions.y, 0) + new Vector3Int(x, y, 0), tileBases[3]);
                                    back.SetTile(new Vector3Int(positions.x, positions.y, 0) + new Vector3Int(x, y, 0), tileBases[3]);
                                }
                            }
                        }
                        if (pos.y == chunksVerticalCount - 1)
                        {
                            //print(tex.GetPixel(x, y));
                            if (tex.GetPixel(x, y) == Color.white)
                            {
                                back.SetTile(new Vector3Int(positions.x, positions.y, 0) + new Vector3Int(x, y, 0), tileBases[3]);
                            }
                            if (tex.GetPixel(x, y) == Color.black && positions.y < 0)
                            {
                                main.SetTile(new Vector3Int(positions.x, positions.y, 0) + new Vector3Int(x, y, 0), tileBases[3]);
                                back.SetTile(new Vector3Int(positions.x, positions.y, 0) + new Vector3Int(x, y, 0), tileBases[3]);
                            }

                        }
                    }
                    if (biom == 1)
                    {
                        if (pos.y < chunksVerticalCount - 2)
                        {
                            if (tex.GetPixel(x, y) != Color.red)
                            {
                                back.SetTile(new Vector3Int(positions.x, positions.y, 0) + new Vector3Int(x, y, 0), tileBases[6]);
                            }
                            if (tex.GetPixel(x, y) == Color.black && positions.y < 0)
                            {
                                main.SetTile(new Vector3Int(positions.x, positions.y, 0) + new Vector3Int(x, y, 0), tileBases[7]);
                                back.SetTile(new Vector3Int(positions.x, positions.y, 0) + new Vector3Int(x, y, 0), tileBases[7]);
                            }
                            if (tex.GetPixel(x, y) == Color.black && positions.y > -200 - RandomRange(0, 10))
                            {
                                main.SetTile(new Vector3Int(positions.x, positions.y, 0) + new Vector3Int(x, y, 0), tileBases[6]);
                                back.SetTile(new Vector3Int(positions.x, positions.y, 0) + new Vector3Int(x, y, 0), tileBases[6]);
                            }
                        }

                        if (pos.y == chunksVerticalCount - 2)
                        {
                            //print(tex.GetPixel(x, y));
                            if (tex.GetPixel(x, y) == Color.white)
                            {
                                if (RandomRange(y, tex.height) == tex.height - 1)
                                {
                                    back.SetTile(new Vector3Int(positions.x, positions.y, 0) + new Vector3Int(x, y, 0), tileBases[0]);
                                }
                                else
                                {
                                    back.SetTile(new Vector3Int(positions.x, positions.y, 0) + new Vector3Int(x, y, 0), tileBases[3]);
                                }
                            }
                            if (tex.GetPixel(x, y) == Color.black && positions.y < 0)
                            {
                                int rn = RandomRange(y, tex.height);
                                if (rn < y + 10 && rn > y - 10)
                                {
                                    main.SetTile(new Vector3Int(positions.x, positions.y, 0) + new Vector3Int(x, y, 0), tileBases[1]);
                                    back.SetTile(new Vector3Int(positions.x, positions.y, 0) + new Vector3Int(x, y, 0), tileBases[1]);
                                }
                                else
                                {
                                    main.SetTile(new Vector3Int(positions.x, positions.y, 0) + new Vector3Int(x, y, 0), tileBases[3]);
                                    back.SetTile(new Vector3Int(positions.x, positions.y, 0) + new Vector3Int(x, y, 0), tileBases[3]);
                                }
                            }
                        }
                        if (pos.y == chunksVerticalCount - 1)
                        {
                            //print(tex.GetPixel(x, y));
                            if (tex.GetPixel(x, y) == Color.white)
                            {
                                back.SetTile(new Vector3Int(positions.x, positions.y, 0) + new Vector3Int(x, y, 0), tileBases[3]);
                            }
                            if (tex.GetPixel(x, y) == Color.black && positions.y < 0)
                            {
                                main.SetTile(new Vector3Int(positions.x, positions.y, 0) + new Vector3Int(x, y, 0), tileBases[3]);
                                back.SetTile(new Vector3Int(positions.x, positions.y, 0) + new Vector3Int(x, y, 0), tileBases[3]);
                            }

                        }
                    }
                    if (biom == 2)
                    {
                        if (pos.y < chunksVerticalCount - 2)
                        {
                            if (tex.GetPixel(x, y) != Color.red)
                            {
                                back.SetTile(new Vector3Int(positions.x, positions.y, 0) + new Vector3Int(x, y, 0), tileBases[16]);
                            }
                            if (tex.GetPixel(x, y) == Color.black && positions.y < 0)
                            {
                                main.SetTile(new Vector3Int(positions.x, positions.y, 0) + new Vector3Int(x, y, 0), tileBases[16]);
                                back.SetTile(new Vector3Int(positions.x, positions.y, 0) + new Vector3Int(x, y, 0), tileBases[16]);
                            }
                            if (tex.GetPixel(x, y) == Color.black && positions.y > -200 - RandomRange(0, 10))
                            {
                                main.SetTile(new Vector3Int(positions.x, positions.y, 0) + new Vector3Int(x, y, 0), tileBases[16]);
                                back.SetTile(new Vector3Int(positions.x, positions.y, 0) + new Vector3Int(x, y, 0), tileBases[16]);
                            }
                        }

                        if (pos.y == chunksVerticalCount - 2)
                        {
                            //print(tex.GetPixel(x, y));
                            if (tex.GetPixel(x, y) == Color.white)
                            {
                                if (RandomRange(y, tex.height) == tex.height - 1)
                                {
                                    back.SetTile(new Vector3Int(positions.x, positions.y, 0) + new Vector3Int(x, y, 0), tileBases[16]);
                                }
                                else
                                {
                                    back.SetTile(new Vector3Int(positions.x, positions.y, 0) + new Vector3Int(x, y, 0), tileBases[3]);
                                }
                            }
                            if (tex.GetPixel(x, y) == Color.black && positions.y < 0)
                            {
                                int rn = RandomRange(y, tex.height);
                                if (rn < y + 10 && rn > y - 10)
                                {
                                    main.SetTile(new Vector3Int(positions.x, positions.y, 0) + new Vector3Int(x, y, 0), tileBases[16]);
                                    back.SetTile(new Vector3Int(positions.x, positions.y, 0) + new Vector3Int(x, y, 0), tileBases[16]);
                                }
                                else
                                {
                                    main.SetTile(new Vector3Int(positions.x, positions.y, 0) + new Vector3Int(x, y, 0), tileBases[3]);
                                    back.SetTile(new Vector3Int(positions.x, positions.y, 0) + new Vector3Int(x, y, 0), tileBases[3]);
                                }
                            }
                        }
                        if (pos.y == chunksVerticalCount - 1)
                        {
                            //print(tex.GetPixel(x, y));
                            if (tex.GetPixel(x, y) == Color.white)
                            {
                                back.SetTile(new Vector3Int(positions.x, positions.y, 0) + new Vector3Int(x, y, 0), tileBases[3]);
                            }
                            if (tex.GetPixel(x, y) == Color.black && positions.y < 0)
                            {
                                main.SetTile(new Vector3Int(positions.x, positions.y, 0) + new Vector3Int(x, y, 0), tileBases[3]);
                                back.SetTile(new Vector3Int(positions.x, positions.y, 0) + new Vector3Int(x, y, 0), tileBases[3]);
                            }

                        }
                    }
                }
                if (tex.GetPixel(x, y) != Color.red)
                {
                    for (int i = 0; i < dungeons.Length; i++)
                    {
                        if (dungeons[i].startSpawnHeight > positions.y + y && dungeons[i].endSpawnHeight < positions.y + y)
                        {
                            if (dungeons[i].biomes.Contains(biom))
                            {
                                if (RandomRange(1, dungeons[i].maxRandom) == 1)
                                {
                                    var dst = Vector3.Distance(new Vector3Int(positions.x + dungeons[i].dungeon.GetComponent<DangeSpawn>().size.x / 2, positions.y + dungeons[i].yoffcet + dungeons[i].dungeon.GetComponent<DangeSpawn>().size.y / 2, 0) + new Vector3Int(x, y, 0), player.transform.position);
                                    if (dst > dungeons[i].dungeon.GetComponent<DangeSpawn>().size.y && dst > dungeons[i].dungeon.GetComponent<DangeSpawn>().size.x)
                                        Instantiate(dungeons[i].dungeon, new Vector3Int(positions.x, positions.y + dungeons[i].yoffcet, 0) + new Vector3Int(x, y, 0), Quaternion.identity).GetComponent<DangeSpawn>().Spawn();
                                }
                            }
                        }
                    }
                }
            }
        }
        chunks[cords.x][cords.y].upempty = upempty;
        chunks[cords.x][cords.y].downEmpty = downempty;
    } //NO WHILE

    public int RandomRange(int mn, int mx)
    {
        return rnd.Next(mn, mx);
    }
    public float RandomRange(float mn, float mx)
    {
        double range = (double)mx - (double)mn;
        double sample = rnd.NextDouble();
        double scaled = (sample * range) + mn;
        float f = (float)scaled;
        return f;
    }

    IEnumerator GenerationUpdate(Vector2Int pos, Vector2Int cords, bool genOres, int biom)
    {
        if (genOres)
        {
            positions = chunks[cords.x][cords.y].pos;
            for (int i = 0; i < legacyOres.Count; i++)
            {
                if (cords.y <= legacyOres[i].maxSpawnY && cords.y >= legacyOres[i].minSpawnY)
                {
                    int count = Random.Range(3, 6);
                    for (int c = 0; c < count; c++)
                    {
                        int structsize = (int)Random.Range(legacyOres[i].min, legacyOres[i].max);
                        int size = 80 * structsize;
                        Vector2 ellise = new Vector2(Random.Range(1f, 5f), Random.Range(1f, 5f));
                        var StartPoint = new Vector3Int(Random.Range(chunks[cords.x][cords.y].pos.x, chunks[cords.x][cords.y].pos.x + pixWidth), Random.Range(chunks[cords.x][cords.y].pos.y, chunks[cords.x][cords.y].pos.y + pixWidth), 0);
                        List<Vector2Int> oldPoints = new List<Vector2Int>();
                        for (int k = 0; k < size; k++)
                        {
                            var point = Vector2Int.CeilToInt((Random.insideUnitCircle * ellise) * structsize);
                            int it = 0;
                            while (oldPoints.Contains(point))
                            {
                                point = Vector2Int.CeilToInt(Random.insideUnitCircle * structsize);
                                it++;
                                if (it > 100) break;
                            }
                            if (main.GetTile(Vector3Int.CeilToInt(new Vector3(point.x, point.y))  + StartPoint) != null)
                            {
                                main.SetTile(Vector3Int.CeilToInt(new Vector3(point.x, point.y)) + StartPoint, legacyOres[i].tile);
                            }
                            oldPoints.Add(point);
                        }
                    }

                }
            }
        }
        for (int x = chunks[cords.x][cords.y].pos.x-2; x < chunks[cords.x][cords.y].pos.x + pixWidth; x++)
        {
            for (int y = chunks[cords.x][cords.y].pos.y; y < chunks[cords.x][cords.y].pos.y + pixHeight; y++)
            {
                if (x > pos.x && x < pos.x + pixWidth)
                {
                    if (pos.y < y && y > pos.y - pixHeight)
                    {
                        if (pos.x + pixWidth < x)
                        {
                            break;
                        }
                        if (y <= pos.y - pixHeight)
                        {
                            break;
                        }
                        bool down = chunks[cords.x][cords.y].downEmpty.Contains(new Vector3Int(x, y, 0));
                        bool up = chunks[cords.x][cords.y].upempty.Contains(new Vector3Int(x, y, 0));

                        var tile = main.GetTile(new Vector3Int(x, y, 0));
                        if (main.GetTile(new Vector3Int(x,y+1,0)) == null)
                        {
                            if (biom == 0)
                            {
                                if (tile == tileBases[0])
                                {
                                    main.SetTile(new Vector3Int(x, y, 0), tileBases[2]);
                                    back.SetTile(new Vector3Int(x, y, 0), tileBases[0]);
                                }
                            }
                            if (biom == 1)
                            {
                                if (tile == tileBases[6])
                                {
                                    main.SetTile(new Vector3Int(x, y, 0), tileBases[8]);
                                    back.SetTile(new Vector3Int(x, y, 0), tileBases[6]);
                                }
                            }
                        }

                        if (y > -199)
                        {
                            if (main.GetTile(new Vector3Int(x,y+1,0)) == null)
                            {
                                if (RandomRange(0, 800) == 2)
                                {
                                    GameObject woter = Instantiate(GetComponent<WorldManager>().allEntities[20].gameObject, main.CellToWorld(new Vector3Int(x + 1, y, 0)) + new Vector3(0, 0.5f, 0), Quaternion.identity);
                                    SetParet(woter, cords);
                                }
                            }
                        }
                        
                        if (y < (chunksVerticalCount - 1 * pixHeight))
                        {
                            if (main.GetTile(new Vector3Int(x,y+1,0)) == null)
                            {
                                var tile1 = main.GetTile(new Vector3Int(x, y, 0));
                                if (tile1 == tileBases[3])
                                {

                                    main.SetTile(new Vector3Int(x, y, 0), tileBases[4]);
                                }
                            }
                        }
                        if (y > -500)
                        {
                            if (main.GetTile(new Vector3Int(x,y+1,0)) == null)
                            {
                                var tile1 = main.GetTile(new Vector3Int(x, y, 0));
                                if (tile1 == tileBases[1])
                                {
                                    main.SetTile(new Vector3Int(x, y, 0), tileBases[2]);
                                    back.SetTile(new Vector3Int(x, y, 0), tileBases[0]);
                                }
                            }
                        }



                        {//DECOR
                            tile = main.GetTile(new Vector3Int(x, y, 0));
                            if (biom == 0 || biom == 1)
                            {
                                ///Декор (Камешки)
                                if (tile == tileBases[0] || tile == tileBases[1] || tile == tileBases[7] || tile == tileBases[6])
                                {
                                    int rn = Random.Range(1, 6);
                                    if (rn == 1)
                                        decor.SetTile(new Vector3Int(x, y, 0), tileBases[12]);
                                    else
                                    if (rn == 4)
                                        decor.SetTile(new Vector3Int(x, y, 0), tileBases[13]);
                                }
                                if (y > -250)
                                {
                                    //Trava
                                    if (main.GetTile(new Vector3Int(x, y + 1, 0)) == null && (tile != null) && tile == tileBases[2])
                                    {
                                        int rn = Random.Range(0, 5);
                                        if (rn <= 2)
                                            decor.SetTile(new Vector3Int(x, y + 1, 0), tileBases[10]);
                                        else
                                        if (rn == 3)
                                            decor.SetTile(new Vector3Int(x, y + 1, 0), tileBases[11]);
                                    }
                                    if (main.GetTile(new Vector3Int(x, y + 1, 0)) == null && (tile != null) && tile == tileBases[8])
                                    {
                                        int rn = Random.Range(0, 7);
                                        if (rn <= 4)
                                            decor.SetTile(new Vector3Int(x, y + 1, 0), tileBases[15]);
                                        else
                                        if (rn == 5)
                                            decor.SetTile(new Vector3Int(x, y + 1, 0), tileBases[14]);
                                    }
                                }
                            }
                            else if (biom == 2)
                            {
                                tile = main.GetTile(new Vector3Int(x, y, 0));
                                if (tile == tileBases[0] || tile == tileBases[1] || tile == tileBases[7] || tile == tileBases[6])
                                {
                                    int rn = Random.Range(1, 6);
                                    if (rn == 1)
                                        decor.SetTile(new Vector3Int(x, y, 0), tileBases[12]);
                                    else
                                    if (rn == 4)
                                        decor.SetTile(new Vector3Int(x, y, 0), tileBases[13]);
                                }
                                if ((main.GetTile(new Vector3Int(x, y + 1, 0)) == null) && (tile != null) && tile == tileBases[16])
                                {
                                    if (y > -199)
                                    {
                                        if (Random.Range(0, 5) == 1)
                                        {
                                            int id = RandomRange(18, 25);
                                            if (id == 21) { id++; }
                                            decor.SetTile(new Vector3Int(x, y + 1, 0), tileBases[id]);
                                        }
                                    }
                                }
                                else
                                {
                                    if (tile == tileBases[16])
                                    {
                                        if (Random.Range(0, 8) == 1)
                                        {
                                            decor.SetTile(new Vector3Int(x, y, 0), tileBases[RandomRange(18, 22)]);
                                        }
                                    }
                                }
                            }

                            if (biom == 1)
                            {

                                tile = main.GetTile(new Vector3Int(x, y, 0));
                                if (tile == tileBases[0])
                                {
                                    main.SetTile(new Vector3Int(x, y, 0), tileBases[6]);
                                    tile = tileBases[6];
                                }
                                if (main.GetTile(new Vector3Int(x, y + 1, 0)) == null && (tile != null) && (tile == tileBases[6] || tile == tileBases[2] || tile == tileBases[0] || tile == tileBases[6]))
                                {

                                    main.SetTile(new Vector3Int(x, y, 0), tileBases[8]);
                                }
                            }
                        }

                        //// Переходы биомов
                        ///
                        tile = main.GetTile(new Vector3Int(x, y, 0));
                       
                        if (biom == 0 || biom == 1 || biom == 2)
                        {
                            if (cords.x != 0)
                            {
                                if (chunks[cords.x - 1][cords.y].biom != chunks[cords.x][cords.y].biom)
                                {
                                    if (x == pos.x + pixWidth - 1 || x == pos.x + 1)
                                    {
                                        var tmptile = main.GetTile(new Vector3Int(x, y, 0));
                                        if (tmptile == tileBases[2])
                                            tmptile = tileBases[0];
                                        if (tmptile == tileBases[8])
                                            tmptile = tileBases[6];

                                        if (tmptile != null)
                                        {
                                            var r = RandomRange(2, 6);
                                            for (int i = r; i > -r; i--)
                                            {
                                                if (back.GetTile(new Vector3Int(x + i, y, 0)) != null)
                                                {
                                                    back.SetTile(new Vector3Int(x + i, y, 0), tmptile);
                                                }
                                                if (main.GetTile(new Vector3Int(x + i, y, 0)) != null)
                                                {
                                                    if (main.GetTile(new Vector3Int(x + i, y + 1, 0)) == null)
                                                    {
                                                        if (tmptile == tileBases[0])
                                                            tmptile = tileBases[2];
                                                        if (tmptile == tileBases[6])
                                                            tmptile = tileBases[8];
                                                        main.SetTile(new Vector3Int(x + i, y, 0), tmptile);
                                                    }
                                                    else
                                                    {
                                                        if (tmptile == tileBases[2])
                                                            tmptile = tileBases[0];
                                                        if (tmptile == tileBases[8])
                                                            tmptile = tileBases[6];
                                                        main.SetTile(new Vector3Int(x + i, y, 0), tmptile);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            if (cords.y >= 1)
                            {
                                if (cords.x != 0)
                                {
                                    if (chunks[cords.x][cords.y - 1].biom != chunks[cords.x][cords.y].biom)
                                    {
                                        if (y == pos.y + pixHeight - 1 || x == pos.y + 1)
                                        {
                                            var tmptile = main.GetTile(new Vector3Int(x, y + 1, 0));
                                            if (tmptile == tileBases[2])
                                                tmptile = tileBases[0];
                                            if (tmptile != null)
                                            {
                                                var r = RandomRange(2, 6);
                                                for (int i = -r; i < r; i++)
                                                {
                                                    if (main.GetTile(new Vector3Int(x, y + i, 0)) != null)
                                                    {
                                                        main.SetTile(new Vector3Int(x, y + i, 0), tmptile);
                                                        back.SetTile(new Vector3Int(x, y + i, 0), tmptile);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        //TRAVA
                        tile = main.GetTile(new Vector3Int(x, y, 0));
                        if (biom == 1)
                        {
                            if (tile == tileBases[2])
                            {
                                main.SetTile(new Vector3Int(x, y, 0), tileBases[8]);
                                tile = tileBases[8];
                            }
                            if (main.GetTile(new Vector3Int(x, y + 1, 0)) == null && tile == tileBases[6])
                            {
                                decor.SetTile(new Vector3Int(x, y + 1, 0), null);
                                main.SetTile(new Vector3Int(x, y, 0), tileBases[8]);
                                tile = tileBases[8];
                            }
                            if (tile == tileBases[0])
                            {
                                main.SetTile(new Vector3Int(x, y, 0), tileBases[6]);
                                tile = tileBases[6];
                            }
                        }
                        if (biom == 2)
                        {
                            if (tile == tileBases[2])
                            {
                                main.SetTile(new Vector3Int(x, y, 0), tileBases[17]);
                                tile = tileBases[17];
                            }
                        }
                        tile = main.GetTile(new Vector3Int(x, y, 0));
                        ///




                        if (!up && !down) continue;
                        /// 
                        /// 
                        ///Спавт ентити и Ад
                        if (y < -700)
                        {
                            int rnd = RandomRange(0, 40);

                            if (rnd == 1)
                            {
                                if (main.GetTile(new Vector3Int(x,y+1,0)) == null && (main.GetTile(new Vector3Int(x + 1, y + 1, 0)) == null) && (tile == tileBases[1]) && (main.GetTile(new Vector3Int(x + 1, y, 0)) != null))
                                {
                                    if (back.GetTile(new Vector3Int(x, y + 1, 0)) != null)
                                    {
                                        GameObject g = Instantiate(GetComponent<WorldManager>().allEntities[14].gameObject, main.CellToWorld(new Vector3Int(x, y + 1, 0)) + new Vector3(0.5f, 0), Quaternion.identity);
                                        SetParet(g, cords);
                                    }
                                }
                            }
                            if (rnd == 2)
                            {
                                if (down && (main.GetTile(new Vector3Int(x - 1, y - 1, 0)) == null) && (tile == tileBases[1]) && (main.GetTile(new Vector3Int(x - 1, y, 0)) != null))
                                {
                                    if (back.GetTile(new Vector3Int(x, y - 1, 0)) != null)
                                    {
                                        GameObject g = Instantiate(GetComponent<WorldManager>().allEntities[15].gameObject, main.CellToWorld(new Vector3Int(x, y, 0)) - new Vector3(0.5f, 0), GetComponent<WorldManager>().allEntities[15].transform.rotation);
                                        SetParet(g, cords);
                                    }
                                }
                            }
                            if (rnd == 3)
                            {
                                if (down && (main.GetTile(new Vector3Int(x - 1, y - 1, 0)) == null) && (tile == tileBases[3]) && (main.GetTile(new Vector3Int(x - 1, y, 0)) != null))
                                {
                                    if (back.GetTile(new Vector3Int(x, y - 1, 0)) != null)
                                    {
                                        GameObject g = Instantiate(GetComponent<WorldManager>().allEntities[16].gameObject, main.CellToWorld(new Vector3Int(x, y, 0)) - new Vector3(0.5f, 0), GetComponent<WorldManager>().allEntities[15].transform.rotation);
                                        SetParet(g, cords);
                                    }
                                }
                            }

                            if (RandomRange(0, 400) == 5)
                            {
                                if ((main.GetTile(new Vector3Int(x, y + 1, 0)) == null) && (tile != null))
                                {
                                    GameObject lava = Instantiate(GetComponent<WorldManager>().allEntities[19].gameObject, main.CellToWorld(new Vector3Int(x + 1, y, 0)) + new Vector3(0, 0.5f, 0), Quaternion.identity);
                                    SetParet(lava, cords);
                                }
                            }
                        }

                        if (y < -230)
                        {
                            if (RandomRange(0, 500) == 5)
                            {
                                if ((main.GetTile(new Vector3Int(x, y + 2, 0)) == null) && (tile == null) && (back.GetTile(new Vector3Int(x, y, 0)) != null) && ((back.GetTile(new Vector3Int(x, y, 0)) == tileBases[6])))
                                {

                                    GameObject g = Instantiate(GetComponent<WorldManager>().allEntities[23].gameObject, main.CellToWorld(new Vector3Int(x, y + 2, 0)), Quaternion.identity);
                                    SetParet(g, cords);
                                }
                            }
                            if (RandomRange(0, 20) == 5)
                            {
                                if (main.GetTile(new Vector3Int(x,y+1,0)) == null && ((tile == tileBases[7])))
                                {
                                    GameObject g = Instantiate(GetComponent<WorldManager>().allEntities[21].gameObject, main.CellToWorld(new Vector3Int(x, y + 1, 0)), Quaternion.identity);
                                    SetParet(g, cords);
                                }
                            }
                            if (RandomRange(0, 6) == 5)
                            {
                                if (down && ((tile == tileBases[0])))
                                {
                                    GameObject g = Instantiate(GetComponent<WorldManager>().allEntities[10].gameObject, main.CellToWorld(new Vector3Int(x, y - 1, 0)), Quaternion.identity);
                                    SetParet(g, cords);
                                }
                            }
                            if (main.GetTile(new Vector3Int(x,y+1,0)) == null && (tile != null))
                            {
                                int rnd = RandomRange(0, 400);
                                if (rnd == 5)
                                {
                                    GameObject g = Instantiate(GetComponent<WorldManager>().allEntities[11].gameObject, main.CellToWorld(new Vector3Int(x, y + 1, 0)), Quaternion.identity);
                                    SetParet(g, cords);
                                }
                                if (rnd == 6)
                                {
                                    GameObject g = Instantiate(GetComponent<WorldManager>().allEntities[17].gameObject, main.CellToWorld(new Vector3Int(x, y + 1, 0)), Quaternion.identity);
                                    SetParet(g, cords);
                                }
                                if (rnd == 7)
                                {
                                    GameObject g = Instantiate(GetComponent<WorldManager>().allEntities[18].gameObject, main.CellToWorld(new Vector3Int(x, y + 1, 0)), Quaternion.identity);
                                    SetParet(g, cords);
                                }
                            }
                            if (main.GetTile(new Vector3Int(x,y+1,0)) == null && (main.GetTile(new Vector3Int(x + 1, y + 1, 0)) == null) && (tile != null) && (main.GetTile(new Vector3Int(x + 1, y, 0)) != null))
                            {
                                if (RandomRange(0, 200) == 5)
                                {
                                    GameObject g = Instantiate(GetComponent<WorldManager>().allEntities[12].gameObject, main.CellToWorld(new Vector3Int(x, y + 1, 0)), Quaternion.identity);
                                    SetParet(g, cords);
                                }
                            }
                        }

                        int tree = RandomRange(0, 10);
                        if (y > -200)
                        {
                            tile = main.GetTile(new Vector3Int(x, y, 0));
                            if (biom == 0)
                            {
                                if (tree == 1)
                                {
                                    NoEmptyTree();
                                    if (tile == tileBases[2] && up)
                                    {
                                        bool can = true;
                                        for (int i = 0; i < lastTree.Count; i++)
                                        {
                                            if (lastTree != null)
                                            {
                                                if (Vector3.Distance(lastTree[i].transform.position, new Vector3(x + 0.5f, y + 1, 0)) < 10)
                                                {
                                                    can = false;
                                                }
                                            }
                                        }

                                        RaycastHit2D hit = Physics2D.Raycast(new Vector3(x + 0.5f, y + 2, 0), Vector2.up, 10);
                                        if (hit.collider != null) { print(hit.collider.name); can = false; }

                                        if (can)
                                        {
                                            GameObject tree_ = Instantiate(trees[0], new Vector3(x + 0.5f, y + 1, 0), Quaternion.identity);
                                            tree_.GetComponent<Tree>().pos = new Vector3Int(x, y + 1, 0);
                                            lastTree.Add(tree_);
                                            SetParet(tree_, cords);
                                        }
                                    }
                                }
                            }
                            if (tree == 7)
                            {
                                if (Random.Range(0, 3) == 1)
                                {
                                    if ((tile == tileBases[0] || tile == tileBases[2]) && up)
                                    {
                                        if (biom == 0)
                                        {
                                            GameObject tree_ = Instantiate(GetComponent<WorldManager>().allEntities[35], new Vector3(x + 0.5f, y + 1, 0), Quaternion.identity);
                                            tree_.GetComponent<Tree>().pos = new Vector3Int(x, y + 1, 0);
                                        }
                                    }
                                    if ((tile == tileBases[8] || tile == tileBases[7]) && up)
                                    {
                                        if (biom == 1)
                                        {
                                            GameObject tree_ = Instantiate(GetComponent<WorldManager>().allEntities[36], new Vector3(x + 0.5f, y + 1, 0), Quaternion.identity);
                                            tree_.GetComponent<Tree>().pos = new Vector3Int(x, y + 1, 0);
                                        }
                                    }
                                }
                            }
                            if (biom == 1)
                            {
                                if (tree == 2)
                                {
                                    if (tile == tileBases[8] && up)
                                    {
                                        bool can = true;
                                        for (int i = 0; i < lastTree.Count; i++)
                                        {
                                            if (lastTree[i] != null)
                                            {
                                                if (Vector3.Distance(lastTree[i].transform.position, new Vector3(x + 0.5f, y + 1, 0)) < 10)
                                                {
                                                    can = false;
                                                }
                                            }
                                        }

                                        RaycastHit2D hit = Physics2D.Raycast(new Vector3(x + 0.5f, y + 2, 0), Vector2.up, 10);

                                        if (hit.collider != null) { print(hit.collider.name); can = false; }
                                        if (can)
                                        {
                                            GameObject tree_ = Instantiate(trees[1], new Vector3(x + 0.5f, y + 1, 0), Quaternion.identity);
                                            tree_.GetComponent<Tree>().pos = new Vector3Int(x, y + 1, 0);
                                            lastTree.Add(tree_);
                                            SetParet(tree_, cords);
                                        }
                                    }
                                }

                            }
                            if (biom == 2)
                            {
                                if (tree == 3)
                                {
                                    NoEmptyTree();
                                    if (tile == tileBases[16] && up)
                                    {
                                        bool can = true;
                                        for (int i = 0; i < lastTree.Count; i++)
                                        {
                                            if (lastTree != null)
                                            {
                                                if (Vector3.Distance(lastTree[i].transform.position, new Vector3(x + 0.5f, y + 1, 0)) < 10)
                                                {
                                                    can = false;
                                                }
                                            }
                                        }

                                        RaycastHit2D hit = Physics2D.Raycast(new Vector3(x + 0.5f, y + 2, 0), Vector2.up, 10);
                                        if (hit.collider != null) { print(hit.collider.name); can = false; }

                                        if (can)
                                        {
                                            GameObject tree_ = Instantiate(trees[RandomRange(2,4)], new Vector3(x + 0.5f, y + 1, 0), Quaternion.identity);
                                            tree_.GetComponent<Tree>().pos = new Vector3Int(x, y + 1, 0);
                                            lastTree.Add(tree_);
                                            SetParet(tree_, cords);
                                        }
                                    }
                                }
                            }
                        }
                        if (y < -200)
                        {
                            tile = main.GetTile(new Vector3Int(x, y, 0));
                         
                            if (tree == 4)
                            {
                                if ((tile == tileBases[0] || tile == tileBases[2]) && up)
                                {
                                    bool can = true;
                                    for (int i = 0; i < lastTree.Count; i++)
                                    {
                                        if (lastTree[i] != null)
                                        {
                                            if (Vector3.Distance(lastTree[i].transform.position, new Vector3(x + 0.5f, y + 1, 0)) < 10)
                                            {
                                                can = false;
                                            }
                                        }
                                    }
                                    for (int i = 0; i < 4; i++)
                                    {
                                        if (main.GetTile(new Vector3Int(x, y + 1 + i, 0)) != null)
                                            can = false;
                                    }
                                    if (can)
                                    {
                                        GameObject tree_ = Instantiate(GetComponent<WorldManager>().allEntities[13], new Vector3(x + 0.5f, y + 1, 0), Quaternion.identity);
                                        tree_.GetComponent<Tree>().pos = new Vector3Int(x, y + 1, 0);
                                        lastTree.Add(tree_);
                                        SetParet(tree_, cords);
                                    }
                                }
                            }
                            if (tree == 5)
                            {
                                if ((tile == tileBases[8] || tile == tileBases[7]) && up)
                                {
                                    bool can = true;
                                    for (int i = 0; i < lastTree.Count; i++)
                                    {
                                        if (lastTree[i] != null)
                                        {
                                            if (Vector3.Distance(lastTree[i].transform.position, new Vector3(x + 0.5f, y + 1, 0)) < 10)
                                            {
                                                can = false;
                                            }
                                        }
                                    }
                                    for (int i = 0; i < 4; i++)
                                    {
                                        if (main.GetTile(new Vector3Int(x, y + 1 + i, 0)) != null)
                                            can = false;
                                    }
                                    if (can)
                                    {
                                        GameObject tree_ = Instantiate(GetComponent<WorldManager>().allEntities[24], new Vector3(x + 0.5f, y + 1, 0), Quaternion.identity);
                                        tree_.GetComponent<Tree>().pos = new Vector3Int(x, y + 1, 0);
                                        lastTree.Add(tree_);
                                        SetParet(tree_, cords);
                                    }
                                }
                            }
                        }
                        if (y < 0 && y > -200)
                        {
                            tile = main.GetTile(new Vector3Int(x, y, 0));
                            if (tree == 6)
                            {
                                if (biom != 2)
                                {
                                    if (tile == tileBases[2] && up)
                                    {
                                        bool can = true;
                                        for (int i = 0; i < lastTree.Count; i++)
                                        {
                                            if (lastTree[i] != null)
                                            {
                                                if (Vector3.Distance(lastTree[i].transform.position, new Vector3(x + 0.5f, y + 1, 0)) < 10)
                                                {
                                                    can = false;
                                                }
                                            }
                                        }
                                        for (int i = 0; i < 4; i++)
                                        {
                                            if (main.GetTile(new Vector3Int(x, y + 1 + i, 0)) != null)
                                                can = false;
                                        }
                                        if (can)
                                        {
                                            GameObject tree_ = Instantiate(GetComponent<WorldManager>().allEntities[34], new Vector3(x + 0.5f, y + 1, 0), Quaternion.identity);
                                            tree_.GetComponent<Tree>().pos = new Vector3Int(x, y + 1, 0);
                                            SetParet(tree_, cords);
                                        }
                                    }
                                }
                            }
                        }


                    }
                }
            }
        }
        chunks[cords.x][cords.y].enumerator = null;
        yield return null;
    } //NO WHILE

    public void NoEmptyTree()
    {
        for (int i = 0; i < lastTree.Count; i++)
        {
            if (lastTree[i] == null)
            {
                lastTree.RemoveAt(i);
                NoEmptyTree();
                return;
            }
        }
    }



    public void SetParet(GameObject obj, Vector2Int cords)
    {
        obj.transform.parent = chunks[cords.x][cords.y].objects.transform;
    }
    public Texture2D _CalcNoise(Vector2Int cords)
    {

        float maxNoise = float.MinValue;
        float minNoise = float.MaxValue;
        for (float y = 0; y < noiseTex.height; y++)
        {
            for (float x = 0; x < noiseTex.width; x++)
            {
                float amplitude = 1;
                float freq = 1;
                float noiseHeight = 0;

                for (int i = 0; i < octaves; i++)
                {
                    float xCoord = (((cords.x * pixWidth) +  (x / noiseTex.width)  * (scale * 2)) + shm + offcetX ) * freq;
                    float yCoord = (((cords.y * pixHeight) + (y / noiseTex.height) * (scale * 2)) + shm + offcetY ) * freq;

                    float sample = Mathf.PerlinNoise(xCoord, yCoord) * 2 - 1;

                    noiseHeight += sample * amplitude;
                    amplitude *= persistance;
                    freq *= lacunar;
                }

                if (noiseHeight > maxNoise)
                {
                    maxNoise = noiseHeight;
                }
                if (noiseHeight < minNoise)
                {
                    minNoise = noiseHeight;
                }
                pix[(int)y * noiseTex.width + (int)x] = new Color(noiseHeight, noiseHeight, noiseHeight);
            }
        }
        for (int y = 0; y < noiseTex.height; y++)
        {
            for (int x = 0; x < noiseTex.width; x++)
            {
                
                float sample = pix[(int)y * noiseTex.width + (int)x].r;
                sample = Mathf.InverseLerp(minNoise, maxNoise, sample);

                if (sample < fullest * (cords.y == 0 ? 1.3f : 1)) sample = 0; else sample = 1;
                pix[(int)y * noiseTex.width + (int)x] = new Color(sample, sample, sample);
            }
        }
        noiseTex.SetPixels(pix);

        if (cords.y == 0)
        {
            float secondY = 0f;
            List<float> line = new List<float>();
            for (float y = 0; y < 1; y++)
            {
                for (float x = 0; x < noiseTex.width; x++)
                {
                    float xCoord = ((cords.x * pixWidth) + (x / noiseTex.width) * (upScale / upWidthScale)) + shm + offcetX;
                    float yCoord = ((cords.y * pixHeight) + y / noiseTex.height * (upScale)) + shm + offcetY;
                    float sample = Mathf.PerlinNoise(xCoord, yCoord);
                    line.Add(sample * heightScale);
                }
                secondY = y;
            }
            float rnd = Mathf.PerlinNoise(cords.x, cords.y) + 2 - 1;
            for (int i = 0; i < octaves; i++)
            {
                for (float y = 0; y < 1; y++)
                {
                    for (float x = 0; x < noiseTex.width; x++)
                    {
                        float xCoord = (((cords.x * pixWidth * i) + (x / noiseTex.width) * (upScale / upWidthScale))) + shm + offcetX;
                        float yCoord = (((cords.y * pixHeight * i) + y / noiseTex.height * (upScale))) + shm + offcetY;
                        float sample = Mathf.PerlinNoise(xCoord, yCoord);
                        line[(int)x] = (line[(int)x] * sample * (heightScale * rnd)) + upScale;
                    }
                    secondY = y;
                }
            }


            for (int x = 0; x < noiseTex.width; x++)
            {
                for (secondY = noiseTex.height - 1; secondY > 0; secondY--)
                {
                    if (secondY > line[x])
                    {
                        noiseTex.SetPixel(x, (int)secondY, Color.red);
                    }
                }
            }

            print("LandScape");
            noiseTex.Apply();
            noiseTex2 = noiseTex;
            return noiseTex;
        }

        noiseTex.Apply();
        return noiseTex;
    } //NO WHILE

    public Texture2D _CalcNoiseOre(float minRange, int currY, Ore ore)
    {
        Texture2D texture2D = new Texture2D(pixWidth/2, pixHeight/2);
        var s = Random.Range(0, 1000);
        var xr = Random.Range(0, 1000);
        var yr = Random.Range(0, 1000);

        for (float y = 0; y < texture2D.height/2; y++)
        {
            for (float x = 0; x < texture2D.width/2; x++)
            {
                float xCoord = (xr + x / texture2D.width * scale*2) + s;
                float yCoord = (yr + y / texture2D.height * scale*2) + s;
                float sample = Mathf.PerlinNoise(xCoord + s, yCoord + s);
                if (sample > minRange)
                {
                    pix[(int)y * texture2D.width + (int)x] = new Color(0, 0, 0, 0);
                }
                else
                {
                    if (currY >= ore.minSpawnY && currY < ore.maxSpawnY)
                    {
                        pix[(int)y * texture2D.width + (int)x] = new Color(1,1,1);
                    }
                    else
                    {
                        pix[(int)y * texture2D.width + (int)x] = new Color(0, 0, 0, 0);
                    }
                }
            }
        }
        texture2D.SetPixels(pix);
        texture2D.Apply();
        TextureScaler.scale(texture2D, pixWidth, pixHeight, FilterMode.Point);
        return texture2D;
    } //NO WHILE


    [System.Serializable]
    public class Chunk
    {
        public Texture2D chunkTexture;
        public List<Ore> ores = new List<Ore>();
        public Vector2Int pos;
        public bool generated = false;
        public int biom = 0;
        public GameObject objects;
        public List<Vector3Int> upempty= new List<Vector3Int>(), downEmpty = new List<Vector3Int>();
        public System.Collections.IEnumerator enumerator;

        public Chunk(Texture2D tx, Vector2Int pos, GameObject objects)
        {
            this.chunkTexture = tx;
            this.pos = pos;
            this.objects = objects;
        }
    }
    [System.Serializable]
    public class Ore
    {
        public string name;
        public Texture2D texture;
        public float min, max;
        public int minSpawnY, maxSpawnY;
        public TileBase tile;
    }
    [System.Serializable]
    public class Dungeon {
        public GameObject dungeon;
        public int maxRandom;
        public int yoffcet;
        public int startSpawnHeight, endSpawnHeight;
        public List<int> biomes = new List<int>();
    }


    public void GoScene(int id)
    {
        Application.LoadLevel(id);
    }

    public IEnumerator Save()
    {
        saveOverlay.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.OpenWrite(path);
        WorldData data = new WorldData();
        WorldManager worldManager = FindObjectOfType<WorldManager>();

        data.name = PlayerPrefs.GetString("Name");
        data.desc = PlayerPrefs.GetString("Desc");//

        data.smh = PlayerPrefs.GetFloat("Smh");
        data.height = PlayerPrefs.GetInt("Height");

        data.sizex = PlayerPrefs.GetInt("SizeX");
        data.sizey = PlayerPrefs.GetInt("SizeY");
        data.caves = PlayerPrefs.GetFloat("Caves");
        data.upScale = PlayerPrefs.GetInt("upScale");



        data.playerX = player.localPosition.x;
        data.playerY = player.localPosition.y;

        data.offcetX = offcetY;
        data.offcetY = offcetY;

        data.startX = player.GetComponent<PlayerStart>().playerStart.x;
        data.startY = player.GetComponent<PlayerStart>().playerStart.y;
        data.startGlobalX = player.GetComponent<PlayerStart>().playerStartGlobal.x;
        data.startGlobalY = player.GetComponent<PlayerStart>().playerStartGlobal.y;

        data.firstBoss = FindObjectOfType<WorldManager>().firstBoss;
        data.secondBoss = FindObjectOfType<WorldManager>().secondBoss;
        data.thirdBoss = FindObjectOfType<WorldManager>().thirdBoss;
        data.fourBoss = FindObjectOfType<WorldManager>().fourBoss;
        data.time = FindObjectOfType<TimeCycle>().timeLine;

        data.helmet = player.GetComponent<Armor>().helmet;
        data.body = player.GetComponent<Armor>().body;
        data.hands = player.GetComponent<Armor>().hands;
        data.backpack = player.GetComponent<Armor>().backpack;
        data.boots = player.GetComponent<Armor>().foots;
        data.hook = player.GetComponent<Armor>().hook;

        data.dopHp = player.GetComponent<PlayerStats>().localPlayer.dopHealth;
        data.hp = player.GetComponent<PlayerStats>().localPlayer.health;

        data.firstStart = false;
        //data.player = player.GetComponent<PlayerStats>().localPlayer;

        List<Item> inv = player.GetComponent<PlayerStats>().localPlayer.inventory;
        for (int i = 0; i < inv.Count; i++)
        {
            if (inv[i].secondName != "_None_")
            {
                data.inventoryItems.Add(new InventoryItemData() { id = inv[i].secondName, value = inv[i].value, hotbar = inv[i].inInventory });
            }
        }
        var worldLive = FindObjectOfType<WorldLive>();

        for (int i = 0; i < worldLive.mobsSpawned.Count; i++)
        {
            if (worldLive.mobsSpawned[i] != null)
            {
                string mname = worldLive.mobsSpawned[i].GetComponent<MobStats>().name;
                for (int j = 0; j < worldLive.mobs.Count; j++)
                {
                    if (mname == worldLive.mobs[j].name)
                    {
                        data.mobs.Add(new MobsData() { id = j, hp = worldLive.mobsSpawned[i].GetComponent<MobStats>().hp, x = worldLive.mobsSpawned[i].transform.position.x, y = worldLive.mobsSpawned[i].transform.position.y });
                        break;
                    }
                }
            }
        }
        for (int i = 0; i < worldLive.mobsPieceSpawned.Count; i++)
        {
            if (worldLive.mobsPieceSpawned[i] != null)
            {
                string mname = worldLive.mobsPieceSpawned[i].GetComponent<MobStats>().name;
                for (int j = 0; j < worldLive.mobs.Count; j++)
                {
                    if (mname == worldLive.mobs[j].name)
                    {
                        data.mobs.Add(new MobsData() { id = j, hp = worldLive.mobsPieceSpawned[i].GetComponent<MobStats>().hp, x = worldLive.mobsPieceSpawned[i].transform.position.x, y = worldLive.mobsPieceSpawned[i].transform.position.y });
                        break;
                    }
                }
            }
        }


        for (int cx = 0; cx < chunks.Count; cx++)
        {
            for (int cy = 0; cy < chunks[cx].Count; cy++)
            {
                if (chunks[cx][cy].generated == true)
                {
                    chunks[cx][cy].objects.SetActive(true);
                }
            }
        }

        for (int cx = 0; cx < chunks.Count; cx++)
        {
            for (int cy = 0; cy < chunks[cx].Count; cy++)
            {
                if (chunks[cx][cy].generated == true)
                {
                    data.generatedChunks.Add(new ChunkData() { x = cx, y = cy, biom = chunks[cx][cy].biom });
                    for (int x = 0; x < pixWidth; x++)
                    {
                        for (int y = 0; y < pixHeight; y++)
                        {
                            var v3 = new Vector3Int(chunks[cx][cy].pos.x + x, chunks[cx][cy].pos.y + y, 0);
                            var tile = main.GetTile(v3);
                            if (tile != null)
                            {
                                data.blocksMain.Add(new BlockData() { id = worldManager.GetTileID(tile), x = v3.x, y = v3.y });
                            }
                            tile = back.GetTile(v3);
                            if (tile != null)
                            {
                                data.blocksBack.Add(new BlockData() { id = worldManager.GetTileID(tile), x = v3.x, y = v3.y });
                            }
                            var dec = decor.GetTile(v3 + new Vector3Int(0, 1, 0));
                            if (dec != null)
                            {
                                data.blocksDecor.Add(new BlockData() { id = worldManager.GetTileID(dec), x = v3.x, y = v3.y + 1 });
                            }

                        }
                    }
                }
            }
        }


        Entity[] entities = FindObjectsOfType<Entity>();
        for (int i = 0; i < entities.Length; i++)
        {
            if (entities[i].gameObject.active == true)
            {
                for (int p = 0; p < worldManager.allEntities.Count; p++)
                {
                    if (worldManager.allEntities[p].GetComponent<Entity>() != null)
                    {
                        if (worldManager.allEntities[p].GetComponent<Entity>().entityName == entities[i].entityName)
                        {
                            List<object> list = new List<object>();
                            if (entities[i].TryGetComponent(out Chest chest))
                            {
                                for (int g = 0; g < chest.itemsIn.Count; g++)
                                {
                                    list.Add(new InventoryItemData() { hotbar = -1, id = chest.itemsIn[g].secondName, value = chest.itemsIn[g].value });
                                }

                            }
                            data.entites.Add(new EntitesData { x = entities[i].transform.position.x, y = entities[i].transform.position.y, id = p, items = list, chunkX = entities[i].chunk.x, chunkY = entities[i].chunk.y, data = entities[i].data });
                            break;
                        }
                    }
                }
            }
        }
        GameObject[] npcs = GameObject.FindGameObjectsWithTag("NPC");
        for (int i = 0; i < npcs.Length; i++)
        {
            for (int p = 0; p < worldManager.NPCs.Count; p++)
            {
                if (worldManager.NPCs[p].GetComponent<NPC>().NPCName == npcs[i].GetComponent<NPC>().NPCName)
                {
                    var n = new NPCData() { id = p, x = npcs[i].transform.position.x, y = npcs[i].transform.position.y };
                    if (p == 0)
                    {
                        if (npcs[i].TryGetComponent(out GuidFirstDialog guid))
                        {
                            n.data = npcs[i].GetComponent<GuidFirstDialog>().used.ToString();
                        }
                        else
                        {
                            n.data = "true";
                        }
                    }
                    data.NPC.Add(n);
                    break;
                }
            }
        }
        bf.Serialize(file, data);
        file.Close();

        saveOverlay.SetActive(false);
    }

    public void Load()
    {
        if (File.Exists(path))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(path, FileMode.Open);
            WorldData data = (WorldData)bf.Deserialize(file);
            WorldManager worldManager = FindObjectOfType<WorldManager>();
            file.Close();

            player.GetComponent<PlayerStart>().enabled = false;
            player.localPosition = new Vector3(data.playerX, data.playerY, player.localPosition.z);

            player.GetComponent<Armor>().helmet = data.helmet;
            player.GetComponent<Armor>().body = data.body;
            player.GetComponent<Armor>().hands = data.hands;
            player.GetComponent<Armor>().backpack = data.backpack;
            player.GetComponent<Armor>().foots = data.boots;
            player.GetComponent<Armor>().hook = data.hook;

            player.GetComponent<PlayerStats>().localPlayer.dopHealth = data.dopHp;
            player.GetComponent<PlayerStats>().localPlayer.health = data.hp;

            offcetY = data.offcetX;
            offcetY = data.offcetY;

            player.GetComponent<PlayerStart>().playerStart.x = data.startX;
            player.GetComponent<PlayerStart>().playerStart.y = data.startY;
            player.GetComponent<PlayerStart>().playerStartGlobal.x = data.startGlobalX;
            player.GetComponent<PlayerStart>().playerStartGlobal.y = data.startGlobalY;

            FindObjectOfType<WorldManager>().firstBoss = data.firstBoss;
            FindObjectOfType<WorldManager>().secondBoss = data.secondBoss;
            FindObjectOfType<WorldManager>().secondBoss = data.thirdBoss;
            FindObjectOfType<WorldManager>().fourBoss = data.fourBoss;

            FindObjectOfType<TimeCycle>().timeLine = data.time;

            var inv = player.GetComponent<PlayerStats>().localPlayer.inventory;
            for (int i = 0; i < data.inventoryItems.Count; i++)
            {
                if (data.inventoryItems[i].id != "_None_")
                {
                    Item item = worldManager.GetItemBySecondName(data.inventoryItems[i].id);
                    item.value = data.inventoryItems[i].value;
                    item.inInventory = data.inventoryItems[i].hotbar;
                    inv.Add(item);
                }
            }
            var worldLive = FindObjectOfType<WorldLive>();

            for (int i = 0; i < data.mobs.Count; i++)
            {
                GameObject obj = Instantiate(worldLive.mobs[data.mobs[i].id].prefab, new Vector3(data.mobs[i].x, data.mobs[i].y), Quaternion.identity);
                obj.GetComponent<MobStats>().hp = data.mobs[i].hp;

                if (worldLive.mobs[data.mobs[i].id].agressive)
                {
                    worldLive.mobsSpawned.Add(obj);
                }
                else
                {
                    worldLive.mobsPieceSpawned.Add(obj);
                }
            }

            for (int i = 0; i < data.generatedChunks.Count; i++)
            {
                chunks[data.generatedChunks[i].x][data.generatedChunks[i].y].generated = true;
                chunks[data.generatedChunks[i].x][data.generatedChunks[i].y].biom = data.generatedChunks[i].biom;
            }
            for (int x = 0; x < data.blocksMain.Count; x++)
            {
                main.SetTile(new Vector3Int(data.blocksMain[x].x, data.blocksMain[x].y, 0), worldManager.allTiles[data.blocksMain[x].id]);
            }
            for (int x = 0; x < data.blocksBack.Count; x++)
            {
                back.SetTile(new Vector3Int(data.blocksBack[x].x, data.blocksBack[x].y, 0), worldManager.allTiles[data.blocksBack[x].id]);
            }
            for (int x = 0; x < data.blocksDecor.Count; x++)
            {
                decor.SetTile(new Vector3Int(data.blocksDecor[x].x, data.blocksDecor[x].y, 0), worldManager.allTiles[data.blocksDecor[x].id]);
            }

            for (int i = 0; i < data.entites.Count; i++)
            {
                GameObject object_ = Instantiate(worldManager.allEntities[data.entites[i].id], new Vector3(data.entites[i].x, data.entites[i].y, worldManager.allEntities[data.entites[i].id].transform.position.z), Quaternion.identity);
                object_.GetComponent<Entity>().data = data.entites[i].data;
                SetParet(object_, new Vector2Int(data.entites[i].chunkX, data.entites[i].chunkY));
                if (object_.TryGetComponent(out Chest chest))
                {
                    for (int p = 0; p < data.entites[i].items.Count; p++)
                    {
                        var item = worldManager.GetItemBySecondName((data.entites[i].items[p] as InventoryItemData).id);
                        item.value = (data.entites[i].items[p] as InventoryItemData).value;
                        chest.itemsIn.Add(item);
                    }
                }

                if (object_.transform.tag == "Tree")
                {
                    object_.GetComponent<Tree>().pos = new Vector3Int(Mathf.FloorToInt(data.entites[i].x), (int)data.entites[i].y, 0);
                    lastTree.Add(object_.gameObject);
                }
                if (object_.TryGetComponent(out Fluid component))
                {
                    component.skip = true;
                }
            }
            for (int i = 0; i < data.NPC.Count; i++)
            {
                GameObject npc = Instantiate(worldManager.NPCs[data.NPC[i].id], new Vector3(data.NPC[i].x, data.NPC[i].y, worldManager.NPCs[data.NPC[i].id].transform.position.z), Quaternion.identity);
                npc.GetComponent<NPC>().firstStart = false;
                npc.GetComponent<NPC>().start = true;
                if (data.NPC[i].id == 0)
                {
                    if (data.NPC[i].data.ToLower() == "true")
                    {
                        Destroy(npc.GetComponent<GuidFirstDialog>());
                    }
                }
            }
        }
    }

    [System.Serializable]
    public class BlockData
    {
        public int x, y;
        public int id;
    }
    [System.Serializable]
    public class EntitesData
    {
        public float x, y;
        public int id;
        public int chunkX, chunkY;
        public string data;
        public List<object> items = new List<object>();
    }
    [System.Serializable]
    public class ChunkData
    {
        public int x, y, biom;
    }
    [System.Serializable]
    public class InventoryItemData
    {
        public string id;
        public int value;
        public int hotbar;
    }
    [System.Serializable]
    public class NPCData
    {
        public int id;
        public float x, y;
        public string data;
    }
    [System.Serializable]
    public class MobsData
    {
        public int id;
        public float x, y;
        public float hp;
    }


    [System.Serializable]
    public class WorldData
    {

        public string name, desc;
        public int height;
        public float smh, caves;
        public string date;
        public string path;
        public int sizex, sizey;
        public int upScale;

        /// <summary>
        /// 
        /// </summary>




        public float time = 0;
        public int dopHp = 0, hp = 100;
        public float playerX = 0, playerY = 0;
        public int offcetX, offcetY;
        public float startX, startY;
        public float startGlobalX, startGlobalY;
        public string helmet, body, hands, backpack, boots, hook;
        public bool firstStart = true, firstBoss = false, secondBoss = false, thirdBoss = false, fourBoss = false;
        public List<InventoryItemData> inventoryItems = new List<InventoryItemData>();
        public List<NPCData> NPC = new List<NPCData>();
        public List<BlockData> blocksMain = new List<BlockData>();
        public List<BlockData> blocksDecor = new List<BlockData>();
        public List<BlockData> blocksBack = new List<BlockData>();
        public List<EntitesData> entites = new List<EntitesData>();
        public List<ChunkData> generatedChunks = new List<ChunkData>();
        public List<MobsData> mobs = new List<MobsData>();
    }
}
