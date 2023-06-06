using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RewardsUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI rewardsText;

    public void SetRewardsText(string Text)
    {
        rewardsText.text = Text;
    }

    public void OnClick()
    {
        transform.gameObject.SetActive(false);
        CameraController.instance.CameraIslands();
    }
}
