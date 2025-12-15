using UnityEngine;
using UnityEngine.UI;

public class RankRow : MonoBehaviour
{
    public Text rankText;  

    public void SetData(int rank, string name, int stage, int ms)
    {
        rankText.text = $"{rank}µî. {name} | Stage: {stage} | Time: {ms}ms";
    }
}