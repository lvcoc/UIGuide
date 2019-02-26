using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;

public class TweenObjAttribute
{
    private GameObject obj;     //物体
    private RectTransform rectTrans;

    private Vector3 fromPos;    //初始 Vector3 值 
    private Vector3 toPos;      //目标 Vector3 值

    private Vector3 fromScale;    //初始 Vector3 值 
    private Vector3 toScale;      //目标 Vector3 值

    private Vector3 fromRotate;    //初始 Vector3 值 
    private Vector3 toRotate;      //目标 Vector3 值

    private float fromAlpha = 0; //初始Alpha值
    private float toAlpha = 0;  //目标Alpha值

    private float updateTime = 0;  //实时更新的时间
    private bool isTweening = false;    //是否开启动画
    private float time = 1;  //时间长度
    private float delayTime = 0;  //延迟时间
    private bool isCounterX = false; //是否反曲线运动
    private bool isCounterY = false; //是否反曲线运动
    private float offset = 0; //偏移值

    private TweenType moveType = TweenType.None;
    private TweenType scaleType = TweenType.None;
    private TweenType rotateType = TweenType.None;
    private TweenType alphaType = TweenType.None;

    public Action<GameObject> CallBackFunc;

    public TweenObjAttribute()
    {

    }
    public TweenObjAttribute(TweenType moveType, TweenType scaleType, TweenType rotateType, TweenType alphaType, TweenController tweenController)
    {
        this.moveType = moveType;
        this.scaleType = scaleType;
        this.rotateType = rotateType;
        this.alphaType = alphaType;

        fromPos = tweenController.fromPos;
        toPos = tweenController.toPos;

        fromScale = tweenController.fromScale;
        toScale = tweenController.toScale;

        fromRotate = tweenController.fromRotate;
        toRotate = tweenController.toRotate;

        fromAlpha = tweenController.fromAlpha;
        toAlpha = tweenController.toAlpha;
    }

    //仅开启 某类型动画 (1 = 移动，2 = 缩放，3 = 旋转，4 = Alpha, 其他 = 默认)
    public TweenObjAttribute(int type)
    {
        switch (type)
        {
            case 1:
                moveType = TweenType.Normal;
                scaleType = TweenType.None;
                rotateType = TweenType.None;
                alphaType = TweenType.None;
                break;
            case 2:
                moveType = TweenType.None;
                scaleType = TweenType.Normal;
                rotateType = TweenType.None;
                alphaType = TweenType.None;
                break;
            case 3:
                moveType = TweenType.None;
                scaleType = TweenType.None;
                rotateType = TweenType.Normal;
                alphaType = TweenType.None;
                break;
            case 4:
                moveType = TweenType.None;
                scaleType = TweenType.None;
                rotateType = TweenType.None;
                alphaType = TweenType.Normal;
                break;
            default:
                break;
        }
    }

    public GameObject Obj
    {
        get
        {
            return obj;
        }

        set
        {
            obj = value;
        }
    }

    public float UpdateTime
    {
        get
        {
            return updateTime;
        }

        set
        {
            updateTime = value;
        }
    }

    public bool IsTweening
    {
        get
        {
            return isTweening;
        }

        set
        {
            isTweening = value;
        }
    }

    public float Time
    {
        get
        {
            return time;
        }

        set
        {
            time = value;
        }
    }

    public float DelayTime
    {
        get
        {
            return delayTime;
        }

        set
        {
            delayTime = value;
        }
    }

    public bool IsCounterX
    {
        get
        {
            return isCounterX;
        }

        set
        {
            isCounterX = value;
        }
    }

    public bool IsCounterY
    {
        get
        {
            return isCounterY;
        }

        set
        {
            isCounterY = value;
        }
    }

    public float FromAlpha
    {
        get
        {
            return fromAlpha;
        }

        set
        {
            fromAlpha = value;
        }
    }

    public float ToAlpha
    {
        get
        {
            return toAlpha;
        }

        set
        {
            toAlpha = value;
        }
    }

    public TweenType MoveType
    {
        get
        {
            return moveType;
        }

        set
        {
            moveType = value;
        }
    }

    public TweenType ScaleType
    {
        get
        {
            return scaleType;
        }

        set
        {
            scaleType = value;
        }
    }

    public TweenType RotateType
    {
        get
        {
            return rotateType;
        }

        set
        {
            rotateType = value;
        }
    }

    public TweenType AlphaType
    {
        get
        {
            return alphaType;
        }

        set
        {
            alphaType = value;
        }
    }

    public Vector3 FromPos
    {
        get
        {
            return fromPos;
        }

        set
        {
            fromPos = value;
        }
    }

    public Vector3 ToPos
    {
        get
        {
            return toPos;
        }

        set
        {
            toPos = value;
        }
    }

    public float Offset
    {
        get
        {
            return offset;
        }

        set
        {
            offset = value;
        }
    }

    public Vector3 FromScale
    {
        get
        {
            return fromScale;
        }

        set
        {
            fromScale = value;
        }
    }

    public Vector3 ToScale
    {
        get
        {
            return toScale;
        }

        set
        {
            toScale = value;
        }
    }

    public Vector3 FromRotate
    {
        get
        {
            return fromRotate;
        }

        set
        {
            fromRotate = value;
        }
    }

    public Vector3 ToRotate
    {
        get
        {
            return toRotate;
        }

        set
        {
            toRotate = value;
        }
    }

    public RectTransform RectTrans
    {
        get
        {
            return rectTrans;
        }

        set
        {
            rectTrans = value;
        }
    }

    public void Dispose()
    {

    }
}
