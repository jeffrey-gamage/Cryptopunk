using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchAndDestroy : MonoBehaviour
{
    [SerializeField] float minOpacity = 0.3f;
    [SerializeField] float opacityDelta = 0.5f;
    UnityEngine.UI.Text enemyActivityWarning;
    bool flashUp = false;
    float opacity = 0f;
    // Start is called before the first frame update
    void Start()
    {
        enemyActivityWarning = GetComponent<UnityEngine.UI.Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if(DungeonManager.instance.isPlayerTurn)
        {
            opacity = Mathf.Max(0f, opacity - Time.deltaTime * opacityDelta);
        }
        else
        {
            if (flashUp)
            {
                opacity = Mathf.Min(1f, opacity + Time.deltaTime * opacityDelta);
                flashUp = !(opacity >= 1f);
            }
            else
            {
                opacity = Mathf.Max(minOpacity, opacity - Time.deltaTime * opacityDelta);
                flashUp = opacity <= minOpacity;
            }
        }
        enemyActivityWarning.color = new Color(enemyActivityWarning.color.r, enemyActivityWarning.color.g, enemyActivityWarning.color.b, opacity);
    }
}
