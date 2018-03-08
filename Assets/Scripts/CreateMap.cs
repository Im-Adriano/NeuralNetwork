using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateMap : MonoBehaviour {

    public int gridWidth = 11;
    public int gridHeight = 11;

    float hexWidth = .866f;
    float hexHeight = 1f;
    public float gap = 0.0f;

    Vector2 startPos;

    void Start()
    {
        AddGap();
        CalcStartPos();
        CreateGrid();
    }

    void AddGap()
    {
        hexWidth += hexWidth * gap;
        hexHeight += hexHeight * gap;
    }

    void CalcStartPos()
    {
        float offset = 0;
        if (gridHeight / 2 % 2 != 0)
        {
            offset = hexWidth / 2;
        }

        float x = -hexWidth * (gridWidth / 2) - offset;
        float y = hexHeight * .75f * (gridHeight / 2);
        startPos = new Vector2(x, y);
    }

    Vector2 CalcWorldPos(Vector2 gridPos)
    {
        float offset = 0;
        if (gridPos.y % 2 != 0)
        {
            offset = hexWidth / 2;
        }

        float x = startPos.x + gridPos.x * hexWidth + offset;
        float y = startPos.y - gridPos.y * hexHeight * .75f;

        return new Vector2(x, y);
    }

    void CreateGrid()
    {
        for(int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                GameObject hex = Instantiate(Resources.Load("Obstacle") as GameObject);
              
                Vector2 gridPos = new Vector2(x, y);
                hex.transform.position = CalcWorldPos(gridPos);
               
                hex.name = "Obstacle " + x + " , " + y;
              

            }
        }
    }
}
