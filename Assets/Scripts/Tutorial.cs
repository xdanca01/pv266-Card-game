using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Tutorial : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _textRef;
    [SerializeField] GameObject blockingPanel;
    [SerializeField] GameObject continueButton;
    List<string> tutorial1 = new List<string> { "Welcome to the tutorial", "You can place your hero by clicking on the hero and\n the wanted place on battlefield", "After placing your heroes on the battlefield\n you can click on one of his actions.", "First and second actions are attacks, third action is heal.", "Good luck!" };
    List<string> tutorial2 = new List<string> { "Welcome to the second tutorial", "As you can see there are multiple tiles,\nwhere you can place your hero.", "The battlefields are specific for each map\n and it's tiles can be occupied by\n other objects, which can be used as cover.", "Try to place one of your heroes behind the tree.",  "Good luck!"};
    List<string> tutorial3 = new List<string> { "Welcome to the third tutorial", "Now we will tell you something about upgrades.", "You can receive upgrades by buying them in store,\n getting them as reward or the upgrade can be already\n part of the battlefield.", "You can place your upgrades directly on your hero by\n clicking on the upgrade and after that clicking on the hero.", "You can see upgrades placed on hero by hovering over them.", "Good luck!"};
    List<string> texts = new List<string> { "Tutorial 1", "Tutorial 2", "Tutorial 3" };
    public static Tutorial instance { get; private set; }
    int cnt = 0;
    int cntTutorial = 0;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }

    public void ContinueTutorial()
    {
        if (cnt == 0)
        {
            Tutorial1();
        }
        else if (cnt == 1)
        {
            Tutorial2();
        }
        else if (cnt == 2)
        {
            Tutorial3();
        }
        else
        {
            _textRef.transform.parent.gameObject.SetActive(false);
        }
    }

    public void Tutorial1()
    {
        blockingPanel.SetActive(true);
        continueButton.SetActive(true);
        _textRef.text = tutorial1[cntTutorial];
        ++cntTutorial;
        if (cntTutorial >= tutorial1.Count)
        {
            ++cnt;
            cntTutorial = 0;
            blockingPanel.SetActive(false);
            continueButton.SetActive(false);
        }
    }

    public void Tutorial2()
    {
        blockingPanel.SetActive(true);
        continueButton.SetActive(true);
        _textRef.text = tutorial2[cntTutorial];
        ++cntTutorial;
        if (cntTutorial >= tutorial2.Count)
        {
            ++cnt;
            cntTutorial = 0;
            blockingPanel.SetActive(false);
            continueButton.SetActive(false);
        }
    }

    public void Tutorial3()
    {
        blockingPanel.SetActive(true);
        continueButton.SetActive(true);
        _textRef.text = tutorial3[cntTutorial];
        ++cntTutorial;
        if (cntTutorial >= tutorial3.Count)
        {
            ++cnt;
            cntTutorial = 0;
            blockingPanel.SetActive(false);
            continueButton.SetActive(false);
        }
    }
}
