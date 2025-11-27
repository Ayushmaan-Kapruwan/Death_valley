using UnityEngine;
using TMPro;

public class ObjectiveManager : MonoBehaviour
{
  public TextMeshProUGUI objectiveText;

  [TextArea]
  public string[] objectives;
  private int currentObjectiveIndex = 0;

  void Start()
  {
    UpdateObjectiveText();
  }

  public void AdvanceObjective()
  {
    if (currentObjectiveIndex < objectives.Length -1)
    {
      currentObjectiveIndex++;
      UpdateObjectiveText();
    }
    else
    {
      objectiveText.text = "Objetives completed";
    }
  }

  void UpdateObjectiveText()
  {
    objectiveText.text = objectives[currentObjectiveIndex];
  }
}
