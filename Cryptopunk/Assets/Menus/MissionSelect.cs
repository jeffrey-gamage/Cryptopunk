using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MissionSelect : MonoBehaviour
{
    [SerializeField] RectTransform[] anchors;
    [SerializeField] GameObject exploitPrefab;
    // Start is called before the first frame update
    void Start()
    {
        if (PersistentState.instance.hasMissionListBeenRefreshed)
        {
            int i = 0;
            foreach (RectTransform anchor in anchors)
            {
                Exploit exploit = Instantiate(exploitPrefab, anchor).GetComponent<Exploit>();
                exploit.corpName = PersistentState.instance.availableMissions[i].corpName;
                exploit.corpID = PersistentState.instance.availableMissions[i].corpID;
                exploit.vulnerability = PersistentState.instance.availableMissions[i].vulnerability;
                i++;
            }
        }
        else
        {
            PersistentState.instance.availableMissions = new List<PersistentState.ExploitRecord>();
            foreach (RectTransform anchor in anchors)
            {
                Exploit exploit = Instantiate(exploitPrefab, anchor).GetComponent<Exploit>();
                exploit.RandomizeExploit();
                PersistentState.ExploitRecord newRecord;
                newRecord.corpID = exploit.corpID;
                newRecord.corpName = exploit.corpName;
                newRecord.vulnerability = exploit.vulnerability;
                PersistentState.instance.availableMissions.Add(newRecord);
            }
            PersistentState.instance.hasMissionListBeenRefreshed= true;
        }

    }

    public void Back()
    {
        SceneManager.LoadScene("Desktop");
    }

}
