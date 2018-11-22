using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Board Composition", menuName = "Board/Composition")]
public class BoardComposition : ScriptableObject
{
    public string Name;

    public string Description;

    public Sprite PreviewImage;

    public List<CompositionEntity> Entities;
}
