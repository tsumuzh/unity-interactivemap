using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.IO;

[Serializable]
public class RegionData
{
    public int id;
    public string name;
    public string capital;
}

public class LoadData
{
    public RegionData[] regionData;
}

public class InteractiveMapManager : MonoBehaviour
{
    private int regionCount = 9; //Region Count
    [SerializeField] int width = 1024; //Map Width
    [SerializeField] int height = 1024; //Map Height
    [SerializeField] Camera mainCamera;
    [SerializeField] SpriteRenderer spriteRenderer_mapOverLay;
    [SerializeField] TextMeshProUGUI regionDataLabel;
    private Texture2D mapData;
    private Texture2D[] regionTextures;
    private RegionData[] regionData;
    private int selectedRegionID = -1; //if no region selected, -1

    void Start()
    {
        regionTextures = new Texture2D[regionCount];
        mapData = Resources.Load<Texture2D>("Textures/mapdata");
        for (int i = 0; i < regionCount; i++)
        {
            regionTextures[i] = Resources.Load<Texture2D>("Textures/RegionMap/" + i.ToString());
        }
        LoadData loadData = new LoadData();
        loadData.regionData = new RegionData[regionCount];
        string str = Resources.Load<TextAsset>("regiondata").ToString();
        LoadData json = JsonUtility.FromJson<LoadData>(str);
        regionData = json.regionData;
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            CheckPixelColor();
        }
    }
    private void CheckPixelColor()
    {
        RaycastHit rayHit;
        if (Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition), out rayHit))
        {
            int x = (int)(rayHit.point.x * 100);
            int y = (int)(rayHit.point.y * 100);
            if (mapData.GetPixel(x, y) == Color.black) return;
            else GetRegion(x, y);
        }
    }
    private void GetRegion(int x, int y)
    {
        for (int i = 0; i < regionTextures.Length; i++)
        {
            if (regionTextures[i].GetPixel(x, y) == Color.white)
            {
                selectedRegionID = i;
                spriteRenderer_mapOverLay.sprite = Resources.Load<Sprite>("Textures/RegionMap/" + i.ToString());
                regionDataLabel.text = "名前: " + regionData[i].name + "\n首都: " + regionData[i].capital;
                StartCoroutine(OverlayMapBlinker());
                return;
            }
        }
    }
    private IEnumerator OverlayMapBlinker()
    {
        float t = 0;
        while (true)
        {
            spriteRenderer_mapOverLay.color = new Color(1, 1, 1, Mathf.Lerp(0, 0.7f, -(t - 1) * (t - 1) + 1));
            t += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
            if (t > 2) t = 0;
        }
    }
}
