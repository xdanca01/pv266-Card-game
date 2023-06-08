using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class RewardsUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI rewardsText;
    [SerializeField] Image image;

    public void SetRewardsText(string Text)
    {
        rewardsText.text = Text;
    }

    public void SetUI(string artwork)
    {
        image.sprite = Resources.Load<Sprite>("Artwork/"+artwork);
    }

    public void OnClick()
    {
        transform.gameObject.SetActive(false);
        CameraController.instance.CameraIslands();
    }
}
