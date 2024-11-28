using System.Collections;
using System.Collections.Generic;
using Script;
using UnityEngine;
using UnityEngine.Serialization;

public class MatchUnityGame : MonoBehaviour {
    public int height;
    public int width;
    public float fillTime = 0.1f;
    public GameObject bgCode;
    public MatchGameMode mode;

    public Tile[] normalCodes;

    public Dictionary<TileType, GameObject> piecePrefabDict;

    public Sprite five;

    public Sprite fow;

    public Sprite lianxu;

    private TilePrefab[,] gridCodes;

    private TilePrefab _startTile;

    private TilePrefab _endTile;

    private SystemUI _systemUIComponent;

    private GameOver _gameOverComponent;
    
    private int _score;
		
    public int getScore(){
        return _score;
    }
		
    public void addScore(int score){
        _score += score;
    }

    public SystemUI getSystemUI {
        get { return _systemUIComponent; }
    }

    private void Awake()
    {
        piecePrefabDict = new Dictionary<TileType, GameObject>();
        
        Board board = new Board(height, width);

        _systemUIComponent = GetComponent<SystemUI>();

        _gameOverComponent = GetComponent<GameOver>();

        _gameOverComponent.initGameOver(60 , this);
        // 初始化網格大小
        gridCodes = new TilePrefab[height , width];

        // 初始化預設 Object
        for (int i = 0; i < normalCodes.Length; i++)
        {
            if (!piecePrefabDict.ContainsKey(normalCodes[i].type))
            {
                piecePrefabDict.Add(normalCodes[i].type, normalCodes[i].codePrefab);
            }
        }
    }

    void Start () {
        GridDraw();
        // 建立空節點
        InitializeGrid();
        StartCoroutine(fillAll());
    }
    
    private void InitializeGrid()
    {
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                SpawnNewPiece(i, j, TileType.Empty);
            }
        }
    }

    // 繪製網格
    private void GridDraw() {
        for (int i = 0; i < height; i++) {
            for (int j = 0; j < width; j++) {
                GameObject bGCode = Instantiate(bgCode, NumToVector(i, j), Quaternion.identity);
                bGCode.transform.parent = transform;
            }
        }
    }
    
    // 生成新的方塊
    public void SpawnNewPiece(int x, int y , TileType type) {

        GameObject code = Instantiate(piecePrefabDict[type], NumToVector(x , y) , Quaternion.identity);

        code.transform.parent = transform;

        TilePrefab tilePrefab = code.GetComponent<TilePrefab>();

        tilePrefab.initPos(x ,y , this , type);

        gridCodes[x, y] = tilePrefab;
    }

    // 將數字轉換成座標
    public Vector2 NumToVector(int x, int y) {
        Vector2 v = new Vector2(x - (int)(height*0.5f + 0.5f) , (int)(width*0.5f - 1.5f) - y);
        return v;
    }

    // 填充所有空的節點
    public IEnumerator fillAll() {
        bool isClear = true;
        int count = 0;
        while (isClear) {
            yield return new WaitForSeconds(fillTime);
            while (!fillRow())
            {
                yield return new WaitForSeconds(fillTime);
            }
            isClear = clearAllCode();
            count++;
        }
        if (count >= 2) {
            _systemUIComponent.PlayScore(lianxu);
            _gameOverComponent.AddScore(count * 5);
            _gameOverComponent.AddTime((int)(count * 1.5f));
        }
    }

    // 以行填充節點
    private bool fillRow() {
        // 判斷是否需要填充
        bool isNeed = true;

        for (int i = 0; i < height; i++)
        {
            for (int y = 0; y < width; y++)
            {
                // 判斷是否為空
                if (gridCodes[i, y].GetType == TileType.Empty)
                {
                    isNeed = false;

                    Destroy(gridCodes[i, y].gameObject);

                    GameObject code = Instantiate(piecePrefabDict[TileType.Normal], NumToVector(i, -1), Quaternion.identity);

                    code.transform.parent = transform;

                    TilePrefab tilePrefab = code.GetComponent<TilePrefab>();

                    tilePrefab.initPos(i, -1, this, TileType.Normal);

                    tilePrefab.getColorComponent.setRangeSprite();

                    for (int c = y-1; c >= 0; c--)
                    {
                        gridCodes[i, c].getMoveComponent.MoveToPos(i, c + 1, fillTime);

                        gridCodes[i, c + 1] = gridCodes[i, c];
                    }

                    tilePrefab.getMoveComponent.MoveToPos(i, 0, fillTime);

                    gridCodes[i, 0] = tilePrefab;

                    break;
                }
            }
        }

        return isNeed;
    }

    // 清除所有滿足條件的方塊
    public bool clearAllCode() {
        bool isClear = false;
        for (int x = 0; x < height; x ++) {
            for (int y = 0; y < width; y++)
            {
                if (gridCodes[x, y].GetType != TileType.Empty) {
                    List<TilePrefab> cle = GetMatch(gridCodes[x , y] , x , y);
                    if (cle.Count > 0) {
                        isClear = true;
                        UpdateScore(cle.Count);
                        foreach (TilePrefab c in cle) {
                            SpawnNewPiece(c.getX, c.getY, TileType.Empty);
                            c.ClearCode();
                        }
                    }
                }
            }
        }

        return isClear;
    }
    
    // 配對當前指定對象是否有消除的對象
    public List<TilePrefab> GetMatch(TilePrefab tile , int x , int y) {
        List<TilePrefab> codeMatch = new List<TilePrefab>();
        List<TilePrefab> horizontalCode = new List<TilePrefab>();
        List<TilePrefab> verticalCode = new List<TilePrefab>();
        bool isH = false;
        bool isV = false;

        if (tile.GetType == TileType.Empty) {
            return codeMatch;
        }

        //右遍历
        for (int i = x + 1; i < height; i++) {
            if (tile.getX == i && tile.getY == y) {
                break;
            }
            if (gridCodes[i, y].GetType != TileType.Empty &&  gridCodes[i , y].getColorComponent.GetTileColor == tile.getColorComponent.GetTileColor)
            {
                horizontalCode.Add(gridCodes[i, y]);
            }
            else {
                break;
            }
        }

        //左遍历
        for (int i = x - 1; i >= 0; i--)
        {
            if (tile.getX == i && tile.getY == y)
            {
                break;
            }
            if (gridCodes[i, y].GetType != TileType.Empty &&  gridCodes[i, y].getColorComponent.GetTileColor == tile.getColorComponent.GetTileColor)
            {
                horizontalCode.Add(gridCodes[i, y]);
            }
            else
            {
                break;
            }
        }

        //下遍历
        for (int i = y + 1; i < width; i++)
        {
            if (tile.getX == x && tile.getY == i)
            {
                break;
            }
            if (gridCodes[x , i].GetType != TileType.Empty && gridCodes[x, i].getColorComponent.GetTileColor == tile.getColorComponent.GetTileColor)
            {
                verticalCode.Add(gridCodes[x, i]);
            }
            else
            {
                break;
            }
        }
        //上遍历
        for (int i = y - 1; i >= 0; i--)
        {
            if (tile.getX == x && tile.getY == i)
            {
                break;
            }
            if (gridCodes[x, i].GetType != TileType.Empty && gridCodes[x, i].getColorComponent.GetTileColor == tile.getColorComponent.GetTileColor)
            {
                verticalCode.Add(gridCodes[x , i]);
            }
            else
            {
                break;
            }
        }

        if (horizontalCode.Count >= 2) {
            isH = true;
            foreach (TilePrefab c in horizontalCode) {
                //Debug.Log("X 軸  顏色 : " + c.getColorComponent.getColor + "座標: " + c.getX + "," + c.getY);
                codeMatch.Add(c);
            }
        }

        if (verticalCode.Count >= 2) {
            isV = true;
            foreach (TilePrefab c in verticalCode)
            {
                //Debug.Log("Y 軸   顏色 : " + c.getColorComponent.getColor + "座標: " + c.getX + "," + c.getY);
                codeMatch.Add(c);
            }
        }

        if (isH || isV) {
            //Debug.Log("顏色 : " + code.getColorComponent.getColor + "座標: " + x + "," + y);

            codeMatch.Add(tile);
            
        }

        return codeMatch;
    }

    public void DownCode(TilePrefab sTile) {
        _startTile = sTile;
    }

    public void EnterCode(TilePrefab eTile) {
        _endTile = eTile;
    }

    //滑動手势判断
    public void UpCode() {
        if (GetComponent<SystemUI>().IsGame) {
            if (_startTile != null && _endTile != null)
            {
                int X = _endTile.getX - _startTile.getX;
                int Y = _endTile.getY - _startTile.getY;

                // X 軸滑動
                if (Mathf.Abs(X) > Mathf.Abs(Y))
                {
                    // 右滑
                    if (X > 0)
                    {
                        Debug.Log("向右滑動");
                        _endTile = gridCodes[_startTile.getX + 1, _startTile.getY];
                    }
                    // 左滑
                    else if (X < 0)
                    {
                        Debug.Log("向左滑動");
                        _endTile = gridCodes[_startTile.getX - 1, _startTile.getY];
                    }
                }
                // Y 軸滑動
                else if (Mathf.Abs(X) < Mathf.Abs(Y))
                {
                    if (Y > 0)
                    {
                        Debug.Log("向下滑動");
                        _endTile = gridCodes[_startTile.getX, _startTile.getY + 1];
                    }
                    else if (Y < 0)
                    {
                        Debug.Log("向上滑動");
                        _endTile = gridCodes[_startTile.getX, _startTile.getY - 1];
                    }
                }
                else
                {
                    _endTile = null;
                }
                StartCoroutine(MoveCode());

                _startTile = null;
                _endTile = null;
            }
        }

    }

    private IEnumerator MoveCode()
    {
        if (_startTile != null && _endTile != null)
        {
            List<TilePrefab> startCodeS = GetMatch(_startTile, _endTile.getX, _endTile.getY);
            List<TilePrefab> endCodeS = GetMatch(_endTile, _startTile.getX, _startTile.getY);
            if (startCodeS.Count > 0 || endCodeS.Count > 0)
            {
                // 交換位置 
                SwapCodes(_startTile, _endTile);

                yield return new WaitForSeconds(fillTime);

                UpdateScore(startCodeS.Count);
                UpdateScore(endCodeS.Count);

                foreach (TilePrefab endC in endCodeS)
                {
                    SpawnNewPiece(endC.getX, endC.getY, TileType.Empty);
                    endC.ClearCode();
                }
                foreach (TilePrefab startC in startCodeS)
                {
                    SpawnNewPiece(startC.getX, startC.getY, TileType.Empty);
                    startC.ClearCode();
                }

                yield return new WaitForSeconds(fillTime * 2);

                // 填充空的節點
                StartCoroutine(fillAll());
            }
            else
            {
                _startTile.getMoveComponent.MoveBackToPos(_endTile.getX, _endTile.getY, fillTime);
                _endTile.getMoveComponent.MoveBackToPos(_startTile.getX, _startTile.getY, fillTime);
            }

        }
    }
    
    private void SwapCodes(TilePrefab start, TilePrefab end)
    {
        int startX = end.getX;
        int startY = end.getY;
        int endX = start.getX;
        int endY = start.getY;

        gridCodes[startX, startY] = start;
        gridCodes[endX, endY] = end;

        start.getMoveComponent.MoveToPos(startX, startY, fillTime);
        end.getMoveComponent.MoveToPos(endX, endY, fillTime);
    }
    
    
    // 分數增加
    public void UpdateScore(int codeCount)
    {
        switch (codeCount)
        {
            case 4:
                _gameOverComponent.AddScore(8);
                _gameOverComponent.AddTime(5);
                _systemUIComponent.PlayScore(fow);
                break;
            case 5:
                _gameOverComponent.AddScore(15);
                _gameOverComponent.AddTime(6);
                _systemUIComponent.PlayScore(five);
                break;
            case > 5:
                _gameOverComponent.AddScore((int)(codeCount*2.5f));
                _gameOverComponent.AddTime(15);
                break;
            default:
                _gameOverComponent.AddScore(3);
                break;
        }
    }
}
