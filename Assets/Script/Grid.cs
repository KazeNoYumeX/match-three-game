using System.Collections;
using System.Collections.Generic;
using Script;
using UnityEngine;

public class Grid : MonoBehaviour {

    public enum CodeType
    {
        Empty,
        Normal,
        Row,
    }

    [System.Serializable]
    public struct Codes
    {
        public CodeType type;
        public GameObject codePrefab;
    }

    public float fillTime = 0.1f;

    public int Grid_H = 0;

    public int Grid_V = 0;

    public GameObject bgCode;

    public Codes[] NomalCodes;

    public Dictionary<CodeType, GameObject> piecePrefabDict;

    public Sprite five;

    public Sprite fow;

    public Sprite lianxu;

    private CodePrefab[,] gridCodes;

    private CodePrefab StartCode;

    private CodePrefab EndCode;

    private SystemUI SystemUIComponent;

    private GameOver gameOverComponent;

    public SystemUI getSystemUI {
        get { return SystemUIComponent; }
    }

    private void Awake()
    {
        Player player = new Player();
        
        piecePrefabDict = new Dictionary<CodeType, GameObject>();

        SystemUIComponent = GetComponent<SystemUI>();

        gameOverComponent = GetComponent<GameOver>();

        gameOverComponent.initGameOver(60 , this, player);
        // 初始化網格大小
        gridCodes = new CodePrefab[Grid_H , Grid_V];

        // 初始化預設 Object
        for (int i = 0; i < NomalCodes.Length; i++)
        {
            if (!piecePrefabDict.ContainsKey(NomalCodes[i].type))
            {
                piecePrefabDict.Add(NomalCodes[i].type, NomalCodes[i].codePrefab);
            }
        }
    }

    void Start () {
        GridDraw();

        // 建立空節點
        for (int i = 0; i < Grid_H; i++)
        {
            for (int j = 0; j < Grid_V; j++)
            {
                SpawnNewPiece(i, j, CodeType.Empty);
            }
        }

        StartCoroutine(fillAll());
    }

    // 繪製網格
    private void GridDraw() {
        for (int i = 0; i < Grid_H; i++) {
            for (int j = 0; j < Grid_V; j++) {
                GameObject BGCode = Instantiate(bgCode, NumToVector(i, j), Quaternion.identity);
                BGCode.transform.parent = transform;
            }
        }
    }
    
    // 生成新的方塊
    public void SpawnNewPiece(int x, int y , CodeType type) {

        GameObject code = Instantiate(piecePrefabDict[type], NumToVector(x , y) , Quaternion.identity);

        code.transform.parent = transform;

        CodePrefab codePrefab = code.GetComponent<CodePrefab>();

        codePrefab.initPos(x ,y , this , type);

        gridCodes[x, y] = codePrefab;
    }

    // 將數字轉換成座標
    public Vector2 NumToVector(int x, int y) {
        Vector2 v = new Vector2(x - (int)(Grid_H*0.5f + 0.5f) , (int)(Grid_V*0.5f - 1.5f) - y);
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
            SystemUIComponent.playScore(lianxu);
            gameOverComponent.addScore(count * 5);
            gameOverComponent.addTime((int)(count * 1.5f));
        }
    }

    // 以行填充節點
    private bool fillRow() {
        // 判斷是否需要填充
        bool isNeed = true;

        for (int i = 0; i < Grid_H; i++)
        {
            for (int y = 0; y < Grid_V; y++)
            {
                // 判斷是否為空
                if (gridCodes[i, y].getCodeType == CodeType.Empty)
                {
                    isNeed = false;

                    Destroy(gridCodes[i, y].gameObject);

                    GameObject code = Instantiate(piecePrefabDict[CodeType.Normal], NumToVector(i, -1), Quaternion.identity);

                    code.transform.parent = transform;

                    CodePrefab codePrefab = code.GetComponent<CodePrefab>();

                    codePrefab.initPos(i, -1, this, CodeType.Normal);

                    codePrefab.getColorComponent.setRangeSprite();

                    for (int c = y-1; c >= 0; c--)
                    {
                        gridCodes[i, c].getMoveComponent.MoveToPos(i, c + 1, fillTime);

                        gridCodes[i, c + 1] = gridCodes[i, c];
                    }

                    codePrefab.getMoveComponent.MoveToPos(i, 0, fillTime);

                    gridCodes[i, 0] = codePrefab;

                    break;
                }
            }
        }

        return isNeed;
    }

    // 清除所有滿足條件的方塊
    public bool clearAllCode() {
        bool isClear = false;
        for (int x = 0; x < Grid_H; x ++) {
            for (int y = 0; y < Grid_V; y++)
            {
                if (gridCodes[x, y].getCodeType != CodeType.Empty) {
                    List<CodePrefab> cle = getMatch(gridCodes[x , y] , x , y);
                    if (cle.Count > 0) {
                        isClear = true;
                        scoreManager(cle.Count);
                        foreach (CodePrefab c in cle) {
                            SpawnNewPiece(c.getX, c.getY, CodeType.Empty);
                            c.ClearCode();
                        }
                    }
                }
            }
        }

        return isClear;
    }
    
    // 配對當前指定對象是否有消除的對象
    public List<CodePrefab> getMatch(CodePrefab code , int x , int y) {
        List<CodePrefab> codeMatch = new List<CodePrefab>();
        List<CodePrefab> horizontalCode = new List<CodePrefab>();
        List<CodePrefab> verticalCode = new List<CodePrefab>();
        bool isH = false;
        bool isV = false;

        if (code.getCodeType == CodeType.Empty) {
            return codeMatch;
        }

        //右遍历
        for (int i = x + 1; i < Grid_H; i++) {
            if (code.getX == i && code.getY == y) {
                break;
            }
            if (gridCodes[i, y].getCodeType != CodeType.Empty &&  gridCodes[i , y].getColorComponent.getColor == code.getColorComponent.getColor)
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
            if (code.getX == i && code.getY == y)
            {
                break;
            }
            if (gridCodes[i, y].getCodeType != CodeType.Empty &&  gridCodes[i, y].getColorComponent.getColor == code.getColorComponent.getColor)
            {
                horizontalCode.Add(gridCodes[i, y]);
            }
            else
            {
                break;
            }
        }

        //下遍历
        for (int i = y + 1; i < Grid_V; i++)
        {
            if (code.getX == x && code.getY == i)
            {
                break;
            }
            if (gridCodes[x , i].getCodeType != CodeType.Empty && gridCodes[x, i].getColorComponent.getColor == code.getColorComponent.getColor)
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
            if (code.getX == x && code.getY == i)
            {
                break;
            }
            if (gridCodes[x, i].getCodeType != CodeType.Empty && gridCodes[x, i].getColorComponent.getColor == code.getColorComponent.getColor)
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
            foreach (CodePrefab c in horizontalCode) {
                //Debug.Log("X 軸  顏色 : " + c.getColorComponent.getColor + "座標: " + c.getX + "," + c.getY);
                codeMatch.Add(c);
            }
        }

        if (verticalCode.Count >= 2) {
            isV = true;
            foreach (CodePrefab c in verticalCode)
            {
                //Debug.Log("Y 軸   顏色 : " + c.getColorComponent.getColor + "座標: " + c.getX + "," + c.getY);
                codeMatch.Add(c);
            }
        }

        if (isH || isV) {
            //Debug.Log("顏色 : " + code.getColorComponent.getColor + "座標: " + x + "," + y);

            codeMatch.Add(code);
            
        }

        return codeMatch;
    }

    public void DownCode(CodePrefab sCode) {
        StartCode = sCode;
    }

    public void EnterCode(CodePrefab eCode) {
        EndCode = eCode;
    }

    //滑動手势判断
    public void UpCode() {
        if (GetComponent<SystemUI>().IsGame) {
            if (StartCode != null && EndCode != null)
            {
                int X = EndCode.getX - StartCode.getX;
                int Y = EndCode.getY - StartCode.getY;

                // X 軸滑動
                if (Mathf.Abs(X) > Mathf.Abs(Y))
                {
                    // 右滑
                    if (X > 0)
                    {
                        Debug.Log("向右滑動");
                        EndCode = gridCodes[StartCode.getX + 1, StartCode.getY];
                    }
                    // 左滑
                    else if (X < 0)
                    {
                        Debug.Log("向左滑動");
                        EndCode = gridCodes[StartCode.getX - 1, StartCode.getY];
                    }
                }
                // Y 軸滑動
                else if (Mathf.Abs(X) < Mathf.Abs(Y))
                {
                    if (Y > 0)
                    {
                        Debug.Log("向下滑動");
                        EndCode = gridCodes[StartCode.getX, StartCode.getY + 1];
                    }
                    else if (Y < 0)
                    {
                        Debug.Log("向上滑動");
                        EndCode = gridCodes[StartCode.getX, StartCode.getY - 1];
                    }
                }
                else
                {
                    EndCode = null;
                }
                StartCoroutine(MoveCode());

                StartCode = null;
                EndCode = null;
            }
        }

    }

    private IEnumerator MoveCode()
    {
        if (StartCode != null && EndCode != null)
        {
            List<CodePrefab> startCodeS = getMatch(StartCode, EndCode.getX, EndCode.getY);
            List<CodePrefab> endCodeS = getMatch(EndCode, StartCode.getX, StartCode.getY);
            if (startCodeS.Count > 0 || endCodeS.Count > 0)
            {
                int StartX = EndCode.getX;
                int StartY = EndCode.getY;
                int EndX = StartCode.getX;
                int EndY = StartCode.getY;
                // 互換陣列中的位置 
                gridCodes[StartX, StartY] = StartCode;
                gridCodes[EndX, EndY] = EndCode;

                // 交換位置
                StartCode.getMoveComponent.MoveToPos(StartX, StartY, fillTime);
                EndCode.getMoveComponent.MoveToPos(EndX, EndY, fillTime);

                yield return new WaitForSeconds(fillTime);

                scoreManager(startCodeS.Count);
                scoreManager(endCodeS.Count);

                foreach (CodePrefab endC in endCodeS)
                {
                    SpawnNewPiece(endC.getX, endC.getY, CodeType.Empty);
                    endC.ClearCode();
                }
                foreach (CodePrefab startC in startCodeS)
                {
                    SpawnNewPiece(startC.getX, startC.getY, CodeType.Empty);
                    startC.ClearCode();
                }

                yield return new WaitForSeconds(fillTime * 2);

                // 填充空的節點
                StartCoroutine(fillAll());
            }
            else
            {
                StartCode.getMoveComponent.MoveBackToPos(EndCode.getX, EndCode.getY, fillTime);
                EndCode.getMoveComponent.MoveBackToPos(StartCode.getX, StartCode.getY, fillTime);
            }

        }
    }
    // 分數增加
    public void scoreManager(int CodeCount)
    {

        if (CodeCount == 4)
        {
            gameOverComponent.addScore(8);
            gameOverComponent.addTime(5);
            SystemUIComponent.playScore(fow);

        }
        else if (CodeCount == 5)
        {
            gameOverComponent.addScore(15);
            gameOverComponent.addTime(6);
            SystemUIComponent.playScore(five);
        }
        else if (CodeCount > 5)
        {
            gameOverComponent.addScore((int)(CodeCount*2.5f));
            gameOverComponent.addTime(15);
        }
        else {
            gameOverComponent.addScore(3);
        }
    }
}
