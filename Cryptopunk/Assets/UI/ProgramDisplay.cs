using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgramDisplay : MonoBehaviour
{
    [SerializeField] Text nameDisplay;
    [SerializeField] Text sizeDisplay;
    [SerializeField] Text speedDisplay;
    [SerializeField] Text powerDisplay;
    [SerializeField] Text rangeDisplay;
    [SerializeField] Text breachDisplay;
    [SerializeField] Text sightDisplay;
    [SerializeField] Text keywordDisplay;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Program.selectedProgram)
        {
            nameDisplay.text = Program.selectedProgram.name;
            sizeDisplay.text = Program.selectedProgram.size.ToString() + " / " + Program.selectedProgram.maxSize.ToString();
            speedDisplay.text = Program.selectedProgram.movesLeft.ToString() + " / " + Program.selectedProgram.speed.ToString();
            powerDisplay.text = Program.selectedProgram.power.ToString();
            rangeDisplay.text = Program.selectedProgram.range.ToString();
            breachDisplay.text = Program.selectedProgram.breach.ToString();
            sightDisplay.text = Program.selectedProgram.sight.ToString();
            string keywords = "";
            foreach(string keyword in Program.selectedProgram.keywords)
            {
                keywords += keyword + "\n";
            }
            keywordDisplay.text = keywords;
        }
    }
}
