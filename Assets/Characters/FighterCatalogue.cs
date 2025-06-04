using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Fighter Catalogue", menuName = "Fighter Catalogue", order = 1)]
public class FighterCatalogue : ScriptableObject
{
    public Fighter[] fighters;
}
