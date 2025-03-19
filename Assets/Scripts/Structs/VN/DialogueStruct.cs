// BURGER
using System.Collections.Generic;
using System;

[Serializable]
public class DialogueStruct
{
    public BaseDialogueStruct baseDialogue;
    public List<ConditionalChoicesStruct> conditionalChoices; // instead of being mapped to list of CDS, make it mapped to indexes in the list
    public int jumpToIndex = -1;
    public bool endOfScene;
    public string flag;
}
