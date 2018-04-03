
using UnityEngine;






// class for style options 
[System.Serializable]
public class SquareStyle
{
    public int Number;
    public Color32 SquareColor;
    public Color32 TextColor;
}


public class SquareStyleHolder : Singleton<SquareStyleHolder> {



    public  SquareStyle[] SquareStyles;



    private void Awake()
    {
      
    }
}
