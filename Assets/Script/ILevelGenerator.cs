using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILevelGenerator
{
    public delegate void PutElement(string type, Vector2 position);

    void GeneratorMap(PutElement callback);


}
