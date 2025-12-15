using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

[System.Serializable]
public class RankData
{
    public string player_name;
    public int stage_level;
    public int reaction_ms;
}

public class RankingManager : MonoBehaviour
{
    string url = "http://localhost:3000/api/rank";// 포스트맨에서는 127.0.0.1로 바꿔서 하면좋다.

    public GameObject rankingPanel;
    public Transform rankingContent;
    public GameObject rankRowPrefab;

    public void SaveRanking(string name, int stage, int ms)
    {
        StartCoroutine(PostRanking(name, stage, ms));
    }

    IEnumerator PostRanking(string name, int stage, int ms)
    {
        RankData data = new RankData { player_name = name, stage_level = stage, reaction_ms = ms };
        string json = JsonUtility.ToJson(data);

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            OpenRankingBoard();
        }
         
    }

    public void OpenRankingBoard()
    {
        rankingPanel.SetActive(true);
        StartCoroutine(GetRankingData());
    }

    public void CloseRankingBoard()
    {
        rankingPanel.SetActive(false);
    }

    IEnumerator GetRankingData()
    {
        foreach (Transform child in rankingContent)
        {
            Destroy(child.gameObject);
        }

        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string jsonResult = request.downloadHandler.text;
            RankData[] rankList = JsonHelper.FromJson<RankData>(jsonResult);
            for (int i = 0; i < rankList.Length; i++)
            {
                GameObject row = Instantiate(rankRowPrefab, rankingContent);
                RankRow rowScript = row.GetComponent<RankRow>();
                if (rowScript != null)
                {
                    rowScript.SetData(i + 1, rankList[i].player_name, rankList[i].stage_level, rankList[i].reaction_ms);
                }
            }
        }
        
    }
}

public static class JsonHelper
{
    public static T[] FromJson<T>(string json)
    {
        string newJson = "{ \"Items\": " + json + "}";
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);
        return wrapper.Items;
    }

    [System.Serializable]
    private class Wrapper<T>
    {
        public T[] Items;
    }
}