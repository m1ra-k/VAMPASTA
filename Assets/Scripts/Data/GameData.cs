using System.Collections.Generic;
using System;

[Serializable]
public class GameData
{
    // last scene
    // dialogue
    public string scene;
    public int dialougeIndex;
    public int money;

    // route conditions not related to trust | rather, depending on player actions (satisfied by some bool)
}