using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CodeMove : MonoBehaviour {
    private TilePrefab _tilePrefab;
    private IEnumerator _moveAnimation;
    private bool isMoveing = false;

    public bool IsMoveIng {
        get { return isMoveing; }
    }

    public TilePrefab TilePrefabCompent {
        get { return _tilePrefab; }
    }
    public void initMoveCompent(TilePrefab tile) {
        _tilePrefab = tile;
    }

    public void MoveToPos(int X , int Y , float fillTime) {

        if (_moveAnimation != null) {
            StopCoroutine(_moveAnimation);
        }
        _moveAnimation = Move(X , Y , fillTime);
        StartCoroutine(_moveAnimation);
    }

    public void MoveBackToPos(int X, int Y, float fillTime) {
        if (_moveAnimation != null)
        {
            StopCoroutine(_moveAnimation);
        }
        _moveAnimation = MoveBack(X , Y , fillTime);
        StartCoroutine(_moveAnimation);
    }

    private IEnumerator Move(int newx , int newy , float fillTime) {
        isMoveing = true;

        _tilePrefab.getX = newx;
        _tilePrefab.getY = newy;

        Vector3 startPos = transform.position;
        Vector3 endPos = _tilePrefab.GetMatchUnityGame.NumToVector(newx , newy);

        for (float t = 0; t <= fillTime; t += Time.deltaTime) {

            transform.position = Vector3.Lerp(startPos, endPos, t / fillTime);

            yield return 0;
        }

        transform.position = endPos;

        isMoveing = false;
    }

    private IEnumerator MoveBack(int newx, int newy, float fillTime) {
        isMoveing = true;
        Vector3 startPos = transform.position;
        Vector3 endPos = _tilePrefab.GetMatchUnityGame.NumToVector(newx, newy);

        for (float t = 0; t <= fillTime; t += Time.deltaTime)
        {
            transform.position = Vector3.Lerp(startPos, endPos, t / fillTime);
            yield return 0;
        }

        yield return new WaitForSeconds(fillTime);
        transform.position = endPos;

        for (float t = 0; t <= fillTime; t += Time.deltaTime)
        {
            transform.position = Vector3.Lerp(endPos, startPos, t / fillTime);
            yield return 0;
        }
        transform.position = startPos;
        isMoveing = false;
    }

    // 滑鼠按下時
    private void OnMouseDown()
    {
        if (!isMoveing)
        {
            TilePrefabCompent.GetMatchUnityGame.DownCode(TilePrefabCompent);
        }
    }
    // 滑鼠指向時
    private void OnMouseEnter()
    {
        if (!isMoveing) {
            TilePrefabCompent.GetMatchUnityGame.EnterCode(TilePrefabCompent);
        }
    }

    // 滑鼠抬起時
    private void OnMouseUp()
    {
        TilePrefabCompent.GetMatchUnityGame.UpCode();
    }
}
