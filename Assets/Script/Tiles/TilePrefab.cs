using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class TilePrefab : MonoBehaviour {
    public int x;
    public int y;
    private TileType _type;
    private MatchUnityGame _matchUnityGame;
    private CodeMove MoveComponent;
    private CodeColor ColorComponent;
    public AnimationClip clearAnimation;

    public int getX {
        get { return x; }
        set { x = value; }
    }

    public int getY {
        get { return y; }
        set { y = value; }
    }

    public MatchUnityGame GetMatchUnityGame {
        get { return _matchUnityGame; }
    }

    public TileType GetType {
        get { return _type; }
    }

    public CodeMove getMoveComponent {
        get {
            if (MoveComponent == null || MoveComponent.TilePrefabCompent == null) {
                MoveComponent = GetComponent<CodeMove>();
                MoveComponent.initMoveCompent(this);
            }
            return MoveComponent;
        }
    }

    public CodeColor getColorComponent {
        get {
            if (ColorComponent == null) {
                ColorComponent = GetComponent<CodeColor>();
            }
            return ColorComponent;
        }
    }

    public void initPos(int x, int y, MatchUnityGame matchUnityGame , TileType tileT) {
        this.x = x;
        this.y = y;
        _matchUnityGame = matchUnityGame;
        _type = tileT;
    }

    // 清除方塊
    public void ClearCode() {
        StartCoroutine(clear());
    }

    private IEnumerator clear() {
        Animator a = GetComponent<Animator>();
        if (a)
        {
            a.Play(clearAnimation.name);

            yield return new WaitForSeconds(clearAnimation.length);

            Destroy(gameObject);
        }
    }
}
