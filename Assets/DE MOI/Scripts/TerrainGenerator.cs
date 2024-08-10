using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[ExecuteInEditMode]
public class TerrainGenerator : MonoBehaviour
{
    public static TerrainGenerator instance;

    [System.Serializable]
    public class DetailLevel
    {
        public enum Overlay
        {
            Add,
            Multiply
        };
        public Vector2 resolution;
        public float intensity;
        public float power;
        public Overlay overlayType;
        public Vector2 offset;
    }
    [System.Serializable]
    public class _show_line
    {
        public Terrain[] a;
    }
    public bool update = false;
    public bool alwaysUpdate = false;
    public string seed;
    public Terrain ground;
    public _show_line[] snow;
    public float heightMaximizer = 0;
    public float distanceZeroToCenter = 0;
    public DetailLevel[] detailLevels;

    public Transform treeParent;
    public Transform treePrefab;
    public Transform bushParent;
    public Transform bushPrefab;
    public Transform deadBushPrefab;

    public float timeUntilSpwonAnotherNPC;
    public Transform NPC_parent;
    public Transform deerPrefab;
    public Transform hunterPrefab;
    private float lastNpcSpawned = 0;

    public Transform[] importantPlaces;
    public float distanceToPlace;

    public Transform goodEndTree;
    public Transform badEndTree;

    float[,] heights;
    float[][][,] snowheights;
    float[,,] textures;

    private float groundDiameter;

    public float getHeight(float i, float j, float diameter)
    {
        float rez = 1;
        foreach (DetailLevel dl in detailLevels)
        {
            float val = Mathf.Pow(Mathf.PerlinNoise(dl.offset.x + i * dl.resolution.x, dl.offset.y + j * dl.resolution.y), dl.power) * dl.intensity;
            if (dl.overlayType == DetailLevel.Overlay.Add)
                rez += val;
            else
                rez *= val;
        }
        float x = i - (ground.terrainData.heightmapResolution / 2);
        float y = j - (ground.terrainData.heightmapResolution / 2);

        float distance = x * x + y * y;
        distance = distance / diameter;
        distance = Mathf.Atan(distance + distanceZeroToCenter) * Mathf.PI;

        rez = rez * heightMaximizer * distance + 0.01f;
        if (rez < 0)
            rez = 0;
        return rez;
    }
    float calculateGroundHeignt(float x, float y)
    {
        //x -= 1f / 1.5f;
        //y -= 1f / 1.5f;
        if (x < 0)
            x = 0;
        if (y < 0)
            y = 0;
        if (x >= heights.GetLength(0))
            x = heights.GetLength(0) - 1;
        if (y >= heights.GetLength(1))
            y = heights.GetLength(1) - 1;
        float dx = x - (int)x;
        float dy = y - (int)y;

        float v11, v12, v21, v22;
        v11 = heights[(int)x, (int)y];
        if ((int)y + 1 < heights.GetLength(1))
            v12 = heights[(int)x, (int)y + 1];
        else v12 = v11;
        if ((int)x + 1 < heights.GetLength(0))
            v21 = heights[(int)x + 1, (int)y];
        else v21 = v11;
        if ((int)x + 1 < heights.GetLength(0) && (int)y + 1 < heights.GetLength(1))
            v22 = heights[(int)x + 1, (int)y + 1];
        else v22 = v11;

        v11 = (1 - dy) * v11 + dy * v12;
        v21 = (1 - dy) * v21 + dy * v22;

        return (1 - dx) * v11 + dx * v21;
    }

    internal float getShowHeight(Vector3 position)
    {
        Vector3 poz1 = new Vector3(position.x, 0, position.z);
        //position -= ground.transform.position;
        position.x /= ground.terrainData.size.x;
        position.z /= ground.terrainData.size.z;
        position *= ground.terrainData.heightmapResolution;
        float groundHeight = calculateGroundHeignt(position.z, position.x);
        //float groundHeight = ground.SampleHeight(position);

        poz1.x /= snow[0].a[0].terrainData.size.x;
        poz1.z /= snow[0].a[0].terrainData.size.z;
        int k = (int)(poz1.x);
        int l = (int)(poz1.z);
        poz1.x -= k;
        poz1.z -= l;
        poz1 *= snow[0].a[0].terrainData.heightmapResolution;
        //float diference = 1.0f * ground.terrainData.heightmapResolution / snow.terrainData.heightmapResolution;
        int x = (int)(poz1.x);
        int y = (int)(poz1.z);
        float snowHeight = snowheights[l][k][y, x];
        //float snowHeight = snow.SampleHeight(position);


        //float[,] newH = new float[1, 1];
        //newH[0, 0] = groundHeight;
        //snowheights[y, x] = newH[0, 0];
        //snow.terrainData.SetHeights(x, y, newH);
        //Debug.Log(x + " " + y + " " + k + " " + l);
        //Debug.Log(snowHeight + " " + groundHeight);
        if (snowHeight - groundHeight < 0)
            return 0;
        return snowHeight - groundHeight;
    }

    internal void walkedOver(Vector3 position, float meltingSpeed, bool updateTerrain = false)
    {
        if (snowheights != null)
        {
            position.x /= snow[0].a[0].terrainData.size.x;
            position.z /= snow[0].a[0].terrainData.size.z;
            int k = (int)(position.x);
            int l = (int)(position.z);
            position.x -= k;
            position.z -= l;
            position *= snow[0].a[0].terrainData.heightmapResolution;
            //float diference = 1.0f * ground.terrainData.heightmapResolution / snow.terrainData.heightmapResolution;
            int x = (int)(position.x);
            int y = (int)(position.z);
            //Debug.Log(x + " " + y);
            //snowheights[y + 8, x + 8] = 0;
            float[,] newH = new float[1, 1];
            //Debug.Log(l + " " + k + " " + x + " " + y);
            newH[0, 0] = snowheights[l][k][y, x] - meltingSpeed;
            if (newH[0, 0] < 0)
                newH[0, 0] = 0;
            snowheights[l][k][y, x] = newH[0, 0];
            if (updateTerrain)
                snow[l].a[k].terrainData.SetHeights(x, y, newH);

            if (x == 0 && k != 0)
            {
                //Debug.Log(">1");
                snowheights[l][k - 1][y, snowheights[l][k - 1].GetLength(1) - 1] = newH[0, 0];
                if (updateTerrain)
                    snow[l].a[k - 1].terrainData.SetHeights(snowheights[l][k - 1].GetLength(1) - 1, y, newH);
            }
            if(y == 0 && l != 0)
            {
                //Debug.Log(">2");
                snowheights[l-1][k][snowheights[l-1][k].GetLength(0) - 1, x] = newH[0, 0];
                if (updateTerrain)
                    snow[l-1].a[k].terrainData.SetHeights(x, snowheights[l - 1][k].GetLength(0) - 1, newH);
            }
            if (x == 0 && k != 0 && y == 0 && l != 0)
            {
                //Debug.Log(">12");
                snowheights[l - 1][k - 1][snowheights[l - 1][k - 1].GetLength(0) - 1, snowheights[l - 1][k - 1].GetLength(1) - 1] = newH[0, 0];
                if (updateTerrain)
                    snow[l - 1].a[k - 1].terrainData.SetHeights(snowheights[l - 1][k - 1].GetLength(1) - 1, snowheights[l - 1][k - 1].GetLength(0) - 1, newH);
            }
            if (x == snowheights[l][k].GetLength(1) - 1 && k != snow[l].a.Length - 1)
            {
                //Debug.Log(">3");
                snowheights[l][k + 1][y, 0] = newH[0, 0];
                if (updateTerrain)
                    snow[l].a[k + 1].terrainData.SetHeights(0, y, newH);
            }
            if (y == snowheights[l][k].GetLength(0) - 1 && l != snow.Length - 1)
            {
                //Debug.Log(">4");
                snowheights[l + 1][k][0, x] = newH[0, 0];
                if (updateTerrain)
                    snow[l + 1].a[k].terrainData.SetHeights(x, 0, newH);
            }
            if (y == snowheights[l][k].GetLength(0) - 1 && l != snow.Length - 1 && x == snowheights[l][k].GetLength(1) - 1 && k != snow[l].a.Length - 1)
            {
                //Debug.Log(">34");
                snowheights[l + 1][k + 1][0, 0] = newH[0, 0];
                if (updateTerrain)
                    snow[l + 1].a[k + 1].terrainData.SetHeights(0, 0, newH);
            }
        }
    }

    public void bleedOver(Vector3 position)
    {
        if (snowheights != null)
        {
            position.x /= snow[0].a[0].terrainData.size.x;
            position.z /= snow[0].a[0].terrainData.size.z;
            int k = (int)(position.x);
            int l = (int)(position.z);
            position.x -= k;
            position.z -= l;
            position *= snow[0].a[0].terrainData.alphamapResolution;
            //float diference = 1.0f * ground.terrainData.heightmapResolution / snow.terrainData.heightmapResolution;
            int x = (int)(position.x);
            int y = (int)(position.z);
            //Debug.Log(x + " " + y);
            //snowheights[y + 8, x + 8] = 0;
            float[,,] newA = new float[1, 1, 2];
            newA[0, 0, 1] = 0.2f + 0.4f * Random.value;
            newA[0, 0, 0] = 1 - newA[0, 0, 1];
            snow[l].a[k].terrainData.SetAlphamaps(x, y, newA);
        }
    }
    public void generateHeight()
    {
        heights = new float[ground.terrainData.heightmapResolution, ground.terrainData.heightmapResolution];
        snowheights = new float[snow.Length][][,];
        textures = new float[snow[0].a[0].terrainData.alphamapWidth, snow[0].a[0].terrainData.alphamapHeight, snow[0].a[0].terrainData.alphamapLayers];
        groundDiameter = ground.terrainData.size.magnitude / 2;

        for (int i = 0; i < snowheights.Length; i++)
        {
            snowheights[i] = new float[snow[0].a.Length][,];
            for (int j = 0; j < snowheights[i].Length; j++)
            {
                snowheights[i][j] = new float[snow[0].a[0].terrainData.heightmapResolution, snow[0].a[0].terrainData.heightmapResolution];
                Terrain top, bot, lef, rig;
                top = bot = lef = rig = null;
                if (i != 0)
                    bot = snow[i - 1].a[j];
                if (i != snowheights.Length - 1)
                    top = snow[i + 1].a[j];
                if (j != 0)
                    lef = snow[i].a[j - 1];
                if (j != snowheights[i].Length - 1)
                    rig = snow[i].a[j + 1];
                snow[i].a[j].SetNeighbors(lef, top, rig, bot);
            }
        }
        ground.SetNeighbors(null, null, null, null);

        float diam = ground.terrainData.heightmapResolution * ground.terrainData.heightmapResolution / 4;
        for (int i = 0; i < heights.GetLength(0); i++)
            for (int j = 0; j < heights.GetLength(1); j++)
            {
                heights[i, j] = getHeight(i, j, diam);
            }
        float diference = 1.0f * ground.terrainData.heightmapResolution / (snow[0].a[0].terrainData.heightmapResolution * snow.Length);
        for (int k = 0; k < snowheights.Length; k++)
            for (int l = 0; l < snowheights[k].Length; l++)
            {
                for (int i = 0; i < snowheights[k][l].GetLength(0); i++)
                    for (int j = 0; j < snowheights[k][l].GetLength(1); j++)
                    {
                        float x = (i + k * snowheights[k][l].GetLength(0)) * diference;
                        float y = (j + l * snowheights[k][l].GetLength(1)) * diference;

                        snowheights[k][l][i, j] = 1.01f * calculateGroundHeignt(x, y) + 0.001f;
                        if (i == 0 && k != 0)
                        {
                            snowheights[k][l][i, j] = (snowheights[k][l][i, j] + snowheights[k-1][l][snowheights[k][l].GetLength(0) - 1, j]) / 2;
                            snowheights[k - 1][l][snowheights[k][l].GetLength(0) - 1, j] = snowheights[k][l][i, j];
                        }
                        if (j==0 && l != 0)
                        {
                            snowheights[k][l][i, j] = (snowheights[k][l][i, j] + snowheights[k][l - 1][i, snowheights[k][l].GetLength(1) - 1]) / 2;
                            snowheights[k][l - 1][i, snowheights[k][l].GetLength(1) - 1] = snowheights[k][l][i, j];
                        }
                        //snowheights[i, j] = calculateGroundHeignt(x, y);
                        //snowheights[i, j] = heights[(int)x, (int)y];
                        //snowheights[i, j] = getHeight(i * diference, j * diference, diam);
                    }
                //snow[k].a[l].terrainData.SetHeights(0, 0, snowheights[k][l]);
            }
        ground.terrainData.SetHeights(0, 0, heights);

        //Debug.Log(textures.GetLength(0) + " " + textures.GetLength(1) + " " + heights.GetLength(0) + " " + heights.GetLength(1) + " " + ground.terrainData.alphamapResolution);

        for (int i = 0; i < textures.GetLength(0); i++)
            for (int j = 0; j < textures.GetLength(1); j++)
            {
                textures[i, j, 0] = 1;
            }
        for (int k = 0; k < snowheights.Length; k++)
            for (int l = 0; l < snowheights[k].Length; l++)
            {
                snow[k].a[l].terrainData.SetHeights(0, 0, snowheights[k][l]);
                snow[k].a[l].terrainData.SetAlphamaps(0, 0, textures);
            }

        while (treeParent.childCount > 0)
        {
            var child = treeParent.GetChild(0);
            //if (Application.isEditor)
            //    DestroyImmediate(child.gameObject);
            //else
                Destroy(child.gameObject);
        }

        for (int i = 10; i < ground.terrainData.size.x - 10; i += 5 + Random.Range(0, 5))
        {
            for (int j = 10; j < ground.terrainData.size.z - 10; j += 5 + Random.Range(0, 5))
            {
                if (j >= ground.terrainData.size.z / 2 - 10 && j <= ground.terrainData.size.z / 2 + 10)
                    if (i >= ground.terrainData.size.x / 2 - 10 && i <= ground.terrainData.size.x / 2 + 10)
                        continue;
                Transform newTr = Instantiate(treePrefab, treeParent);
                //float x = ground.terrainData.heightmapResolution * i / ground.terrainData.size.x;
                //float y = ground.terrainData.heightmapResolution * j / ground.terrainData.size.z;
                newTr.position = new Vector3(i - 2f + Random.value * 4, ground.SampleHeight(new Vector3(i, 100, j)), j - 2f + Random.value * 4);
                Vector3 rot = newTr.localRotation.eulerAngles;
                rot.y += Random.value * 180;
                newTr.localRotation = Quaternion.Euler(rot);
                rot = new Vector3(Random.value * 0.5f + 0.8f, 0, 0);
                rot.z = rot.y = rot.x;
                rot.y *= Random.value * 0.4f + 0.8f;
                rot.x *= newTr.localScale.x;
                rot.y *= newTr.localScale.y;
                rot.z *= newTr.localScale.z;
                newTr.localScale = rot;
            }
        }

        while (bushParent.childCount > 0)
        {
            var child = bushParent.GetChild(0);
            if (Application.isEditor)
                DestroyImmediate(child.gameObject);
            else
                Destroy(child.gameObject);
        }

        for (int i = 10; i < ground.terrainData.size.x - 10; i += 10 + Random.Range(0, 10))
        {
            for (int j = 10; j < ground.terrainData.size.z - 10; j += 10 + Random.Range(0, 10))
            {
                if (j >= ground.terrainData.size.z / 2 - 10 && j <= ground.terrainData.size.z / 2 + 10)
                    if (i >= ground.terrainData.size.x / 2 - 10 && i <= ground.terrainData.size.x / 2 + 10)
                        continue;

                Vector3 pozition = new Vector3(i - 3f + Random.value * 6, 0, j - 3f + Random.value * 6);
                float distToCenter = 2 * Vector3.Distance(pozition, ground.terrainData.size / 2) / groundDiameter;

                if (Random.value > distToCenter)
                    continue;

                Transform newTr = null;
                if (Random.value < 0.1)
                    newTr = Instantiate(deadBushPrefab, bushParent);
                else
                    newTr = Instantiate(bushPrefab, bushParent);
                //float x = ground.terrainData.heightmapResolution * i / ground.terrainData.size.x;
                //float y = ground.terrainData.heightmapResolution * j / ground.terrainData.size.z;
                pozition.y = ground.SampleHeight(new Vector3(i, 100, j));
                newTr.position = pozition;
                Vector3 rot = newTr.localRotation.eulerAngles;
                rot.y += Random.value * 180;
                newTr.localRotation = Quaternion.Euler(rot);
            }
        }
    }

    void Start()
    {
        instance = this;
        //Random.InitState(seed.GetHashCode());
        for (int i = 0; i < detailLevels.Length; i++)
            detailLevels[i].offset = new Vector2(Random.value * 100, Random.value * 100);
        generateHeight();
    }

    private void Update()
    {
        if (update || alwaysUpdate)
        {
            update = false;
            Random.InitState(seed.GetHashCode());
            for (int i = 0; i < detailLevels.Length; i++)
                detailLevels[i].offset = new Vector2(Random.value * 100, Random.value * 100);
            generateHeight();
            //for (float x = 0; x < 100; x += .5f)
            //    for (float y = 0; y < 100; y += .5f)
            //        getShowHeight(new Vector3(x, 0, y));
        }
        float minDistance = groundDiameter;
        if (PlayerController.instance)
        {
            for (int i = 0; i < importantPlaces.Length; i++)
            {
                float dist = Vector3.Distance(PlayerController.instance.transform.localPosition, importantPlaces[i].localPosition);
                if (dist < minDistance)
                    minDistance = dist;
                if (dist < distanceToPlace)
                {
                    if (!importantPlaces[i].gameObject.activeSelf)
                        importantPlaces[i].gameObject.SetActive(true);
                }
                else
                {
                    if (importantPlaces[i].gameObject.activeSelf)
                        importantPlaces[i].gameObject.SetActive(false);
                }
            }
            minDistance /= groundDiameter;
            minDistance *= 2;
            if (minDistance > 1.5f)
                minDistance = 1.5f;

            lastNpcSpawned += Time.deltaTime * (Random.value / 2 + 0.5f) * minDistance;
            if (lastNpcSpawned > timeUntilSpwonAnotherNPC)
            {
                lastNpcSpawned = 0;
                Transform npc = null;
                if (Random.value > 0.7)
                    npc = Instantiate(hunterPrefab, NPC_parent);
                else npc = Instantiate(deerPrefab, NPC_parent);

                float rig = 1 - 2 * Random.value;
                Vector3 poz = -PlayerController.instance.transform.forward;
                poz *= 2 + Random.value * 5;
                poz = rig * poz + PlayerController.instance.transform.right * (1 - rig) * 2;
                poz += PlayerController.instance.transform.position;
                npc.position = poz + Vector3.up * 5;
            }
        }
    }
}
