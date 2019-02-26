/****************** UI 曲线动画控制器（动画完成时可回调函数） ****************************
 * 
 *   Leon Kim
 *   
 *   可用曲线 调整 动画表现方式，支持多个物体,多种动画混合 （比如 物体 移动位置的同时 播放旋转）
 *   
 *   
 *   单个物体时： （可无代码, 直接在TweenController组件上 选勾 自定义物体 并且拖拽即可）
 *   可用代码 控制开启
 *   StartTween()  (当勾选 自定义物体里面的 Play On Start时 运行时会调 StartTween()函数并开始播放)
 *   
 *   
 *   多个物体时：（以下都是代码接口）
 *   InitTweenType() 所有动画类型 还原成 初始开启模式
 *   OpenTweenType() 选择开启哪些动画类型 比如只开启 缩放和移动动画
 *   
 *   (1) StartMove()  
 *   (2) StartScale() 
 *   (3) StartRotate() 
 *   (4) StartAlpha()
 *   注：
 *   以上函数参数中 
 *   isOpenOnlyType = true 只播放当前动画（比如只移动或只旋转 不播放其他动画） 
 *   isOpenOnlyType = false 播放手动开启的动画 和 使用手动设置的值
 *   
 *   
 *   注:
 *   (0) 每个物体完成动画时 都会回调 Lua 函数 并且返回此物体
 *   (1) 挂此脚本的控件需 Anchor填充状态,
 *   (2) 从StartMove 传过来的物体 都在此控件层下(obj.SetParent(transform))
 *   (3) 此控件是动画控制器 本身不参与动画
 **/

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;

public enum TweenType
{
    None,
    Normal,
}
public class TweenController : MonoBehaviour
{

    private static TweenController _Instance = null;
    public static TweenController Instance
    {
        get
        {
            return _Instance;
        }
    }

    //存储 物体信息
    private List<TweenObjAttribute> objAttributeList = new List<TweenObjAttribute>();

    private Action<GameObject> callBackFunc;

    public float time = 1;   //动画进行时间
    public float delayTime = 0; //延迟时间

    //** 额外参数
    public bool isUseAdditionalParam = false;
    public bool playOnStart = false;
    public GameObject animGameObj;
    private RectTransform animGameObjRectTrans;

    private bool isTweening = false;
    private float updateTime = 0f;

    //** Position 动画相关
    public TweenType moveType = TweenType.None;
    public AnimationCurve xCurve = AnimationCurve.Linear(0, 0, 1, 1);
    public AnimationCurve yCurve = AnimationCurve.Linear(0, 0, 1, 1);
    public Vector3 fromPos;
    public Vector3 toPos;

    //** 偏移 动画相关
    public TweenType offsetType = TweenType.None;
    public AnimationCurve offsetCurve = AnimationCurve.Linear(0, 0, 1, 1);
    public float offset;

    //** Scale 动画相关
    public TweenType scaleType = TweenType.None;
    public AnimationCurve scaleCurve = AnimationCurve.Linear(0, 0, 1, 1);
    public Vector3 fromScale = Vector3.one;
    public Vector3 toScale = Vector3.one;

    //** Rotate 动画相关
    public TweenType rotateType = TweenType.None;
    public AnimationCurve rotateCurve = AnimationCurve.Linear(0, 0, 1, 1);
    public Vector3 fromRotate = Vector3.zero;
    public Vector3 toRotate = Vector3.zero;

    //** Alpha 动画相关
    public TweenType alphaType = TweenType.None;
    public AnimationCurve alphaCurve = AnimationCurve.Linear(0, 0, 1, 1);
    public float fromAlpha;
    public float toAlpha;



    private List<TweenType> beginTypeList = new List<TweenType>();

    private bool isAwake = false;
    private void Awake()
    {
        isAwake = true;
        _Instance = this;
    }


    private void Start()
    {
        //初始化用
        beginTypeList.Add(moveType);
        beginTypeList.Add(scaleType);
        beginTypeList.Add(rotateType);
        beginTypeList.Add(alphaType);

        if (playOnStart)
        {
            StartTween();
        }

    }

    //还原成默认 动画类型
    public void InitTweenType()
    {
        moveType = beginTypeList[0];
        scaleType = beginTypeList[1];
        rotateType = beginTypeList[2];
        alphaType = beginTypeList[3];
    }

    //选择性 动画类型开启方式 (移动，缩放，旋转，Alpha)
    public void OpenTweenType(bool isOpenMoveType, bool isOpenScaleType, bool isOpenRotateType, bool isOpenAlphaType)
    {
        moveType = isOpenMoveType ? TweenType.Normal : TweenType.None;
        scaleType = isOpenScaleType ? TweenType.Normal : TweenType.None;
        rotateType = isOpenRotateType ? TweenType.Normal : TweenType.None;
        alphaType = isOpenAlphaType ? TweenType.Normal : TweenType.None;
    }

    //普通 开始动画 (用于 自定义物体)
    public void StartTween()
    {
        StartTween(null);
    }
    //开始 带回调的 动画  （用于 自定义物体）
    public void StartTween(Action<GameObject> callBackFunc)
    {
        if (!isAwake && callBackFunc != null)
        {
            Debug.LogError(string.Format("This GameObject({0}) is not activated", this.gameObject.name));
        }

        if (!isUseAdditionalParam) return;

        if (animGameObj == null) return;

        if (moveType != TweenType.None)
        {
            animGameObj.transform.GetComponent<RectTransform>().anchoredPosition = fromPos;
        }
        if (scaleType != TweenType.None)
        {
            animGameObj.transform.localScale = fromScale;
        }
        if (rotateType != TweenType.None)
        {
            animGameObj.transform.localRotation = Quaternion.Euler(fromRotate);
        }
        if (alphaType != TweenType.None)
        {
            CanvasGroup g = animGameObj.GetComponent<CanvasGroup>();
            if (g == null)
            {
                g = animGameObj.AddComponent<CanvasGroup>();
            }
            g.alpha = fromAlpha;
        }

        animGameObjRectTrans = animGameObj.GetComponent<RectTransform>();

        updateTime = 0;
        isTweening = true;
    }

    //-------------------------------------------------------------->>>  移动动画 接口  <<<--------------------------------------------------------------------------
    //isOpenOnlyType == true 仅开启移动动画 其他动画关闭 
    //isOpenOnlyType == false 用手动设置的动画开启 和 除Position外 用手动设置的值

    //参数 (物体，开始Pos，目标Pos，是否仅开启移动动画)
    public void StartMove(GameObject obj, Vector3 fromPos, Vector3 toPos, bool isOpenOnlyType)
    {
        StartMove(obj, fromPos, toPos, isOpenOnlyType, null);
    }
    //参数 (物体，开始Pos，目标Pos，是否仅开启移动动画，回调函数)
    public void StartMove(GameObject obj, Vector3 fromPos, Vector3 toPos, bool isOpenOnlyType, Action<GameObject> callBackFunc)
    {
        StartMove(obj, fromPos, toPos, time, delayTime, isOpenOnlyType, callBackFunc);
    }
    //参数 (物体，开始Pos，目标Pos, 持续时间，延迟时间，是否仅开启移动动画，回调函数)
    public void StartMove(GameObject obj, Vector3 fromPos, Vector3 toPos, float timeLength, float delayTime, bool isOpenOnlyType, Action<GameObject> callBackFunc)
    {
        StartMove(obj, fromPos, toPos, timeLength, delayTime, isOpenOnlyType, false, false, callBackFunc);
    }
    public void StartMove(GameObject obj, Vector3 fromPos, Vector3 toPos, float timeLength, float delayTime, bool isOpenOnlyType, bool isCounterX, bool isCounterY, Action<GameObject> callBackFunc)
    {
        StartMove(obj, fromPos, toPos, timeLength, delayTime, isOpenOnlyType, false, false, callBackFunc, 0);
    }
    //参数 (物体，开始Pos，目标Pos, 持续时间，延迟时间，是否仅开启移动动画，x曲线是否反轨迹运动，y曲线是否反轨迹运动，回调函)
    public void StartMove(GameObject obj, Vector3 fromPos, Vector3 toPos, float timeLength, float delayTime, bool isOpenOnlyType, bool isCounterX, bool isCounterY, Action<GameObject> callBackFunc, float offset)
    {
        if (!isAwake)
        {
            Debug.LogError(string.Format("This GameObject({0}) is not activated", this.gameObject.name));
        }

        //obj.transform.SetParent(transform);

        TweenObjAttribute attribute = isOpenOnlyType ? new TweenObjAttribute(1) : new TweenObjAttribute(moveType, scaleType, rotateType, alphaType, this);

        attribute.Obj = obj;
        //attribute.RectTrans = obj.GetComponent<RectTransform>();
        attribute.FromPos = fromPos;
        attribute.ToPos = toPos;
        attribute.UpdateTime = 0;
        attribute.Time = timeLength;
        attribute.DelayTime = delayTime;
        //obj.transform.localPosition = fromPos;
        attribute.IsCounterX = isCounterX;
        attribute.IsCounterY = isCounterY;
        attribute.Offset = offset;
        attribute.CallBackFunc = callBackFunc;

        objAttributeList.Add(attribute);

        attribute.IsTweening = true;
    }

    //支持偏移的动画 参数 (物体，开始Pos，目标Pos, 持续时间，延迟时间，是否仅开启移动动画，偏移值，回调函数)
    public void StartOffsetMove(GameObject obj, Vector3 fromPos, Vector3 toPos, float timeLength, float delayTime, float offset, bool isOpenOnlyType, Action<GameObject> callBackFunc)
    {
        StartMove(obj, fromPos, toPos, timeLength, delayTime, isOpenOnlyType, false, false, callBackFunc, offset);
    }
    //-------------------------------------------------------------->>>  缩放动画 接口  <<<--------------------------------------------------------------------------
    //isOpenOnlyType == true 仅开启移动动画 其他动画关闭 
    //isOpenOnlyType == false 用手动设置的动画开启 和 除Scale外 用手动设置的值

    //参数 (物体，开始Scale，目标Scale，是否仅开启 Scale 动画)
    public void StartScale(GameObject obj, Vector3 fromScale, Vector3 toScale, bool isOpenOnlyType)
    {
        StartScale(obj, fromScale, toScale, isOpenOnlyType, null);
    }
    //参数 (物体，开始Scale，目标Scale，是否仅开启 Scale 动画, 回调函数)
    public void StartScale(GameObject obj, Vector3 fromScale, Vector3 toScale, bool isOpenOnlyType, Action<GameObject> callBackFunc)
    {
        StartScale(obj, fromScale, toScale, time, delayTime, isOpenOnlyType, callBackFunc);
    }
    //参数 (物体，开始Scale，目标Scale， 持续时间，延迟时间，是否仅开启 Scale 动画, 回调函数)
    public void StartScale(GameObject obj, Vector3 fromScale, Vector3 toScale, float timeLength, float delayTime, bool isOpenOnlyType, Action<GameObject> callBackFunc)
    {
        if (!isAwake)
        {
            Debug.LogError(string.Format("This GameObject({0}) is not activated", this.gameObject.name));
        }

        //obj.transform.SetParent(transform);

        TweenObjAttribute attribute = isOpenOnlyType ? new TweenObjAttribute(2) : new TweenObjAttribute(moveType, scaleType, rotateType, alphaType, this);

        attribute.Obj = obj;

        attribute.FromScale = fromScale;
        attribute.ToScale = toScale;

        attribute.UpdateTime = 0;
        attribute.Time = timeLength;
        attribute.DelayTime = delayTime;
        //attribute.Obj.transform.GetComponent<RectTransform>().anchoredPosition = fromPos;
        attribute.CallBackFunc = callBackFunc;

        objAttributeList.Add(attribute);

        attribute.IsTweening = true;
    }

    //-------------------------------------------------------------->>> 旋转动画 接口  <<<--------------------------------------------------------------------------
    //isOpenOnlyType == true 仅开启移动动画 其他动画关闭 
    //isOpenOnlyType == false 用手动设置的动画开启 和 除Rotate外 用手动设置的值

    //参数 (物体，开始Rotate，目标Rotate，是否仅开启 Rotate 动画)
    public void StartRotate(GameObject obj, Vector3 fromRotate, Vector3 toRotate, bool isOpenOnlyType)
    {
        StartRotate(obj, fromRotate, toRotate, isOpenOnlyType, null);
    }
    //参数 (物体，开始Rotate，目标Rotate，是否仅开启 Rotate 动画, 回调函数)
    public void StartRotate(GameObject obj, Vector3 fromRotate, Vector3 toRotate, bool isOpenOnlyType, Action<GameObject> callBackFunc)
    {
        StartRotate(obj, fromRotate, toRotate, time, delayTime, isOpenOnlyType, callBackFunc);
    }
    //参数 (物体，开始Rotate，目标Rotate， 持续时间，延迟时间，是否仅开启 Rotate 动画, 回调函数)
    public void StartRotate(GameObject obj, Vector3 fromRotate, Vector3 toRotate, float timeLength, float delayTime, bool isOpenOnlyType, Action<GameObject> callBackFunc)
    {
        if (!isAwake)
        {
            Debug.LogError(string.Format("This GameObject({0}) is not activated", this.gameObject.name));
        }

        //obj.transform.SetParent(transform);

        TweenObjAttribute attribute = isOpenOnlyType ? new TweenObjAttribute(3) : new TweenObjAttribute(moveType, scaleType, rotateType, alphaType, this);

        attribute.Obj = obj;

        attribute.FromRotate = fromRotate;
        attribute.ToRotate = toRotate;

        attribute.UpdateTime = 0;
        attribute.Time = timeLength;
        attribute.DelayTime = delayTime;
        attribute.Obj.transform.GetComponent<RectTransform>().anchoredPosition = fromPos;
        attribute.CallBackFunc = callBackFunc;

        objAttributeList.Add(attribute);

        attribute.IsTweening = true;
    }

    //-------------------------------------------------------------->>>  Alpha动画 接口  <<<--------------------------------------------------------------------------
    //isOpenOnlyType == true 仅开启移动动画 其他动画关闭 
    //isOpenOnlyType == false 用手动设置的动画开启 和 除Alpha外 用手动设置的值

    //参数 (物体，开始Alpha，目标alpha，是否仅开启 Alpha 动画)
    public void StartAlpha(GameObject obj, float fromAlpha, float toAlpha, bool isOpenOnlyType)
    {
        StartAlpha(obj, fromAlpha, toAlpha, isOpenOnlyType, null);
    }
    //参数 (物体，开始Alpha，目标alpha，是否仅开启 Alpha 动画, 回调函数)
    public void StartAlpha(GameObject obj, float fromAlpha, float toAlpha, bool isOpenOnlyType, Action<GameObject> callBackFunc)
    {
        StartAlpha(obj, fromAlpha, toAlpha, time, delayTime, isOpenOnlyType, callBackFunc);
    }
    //参数 (物体，开始Alpha，目标alpha， 持续时间，延迟时间，是否仅开启 Alpha 动画, 回调函数)
    public void StartAlpha(GameObject obj, float fromAlpha, float toAlpha, float timeLength, float delayTime, bool isOpenOnlyType, Action<GameObject> callBackFunc)
    {
        if (!isAwake)
        {
            Debug.LogError(string.Format("This GameObject({0}) is not activated", this.gameObject.name));
        }

        //obj.transform.SetParent(transform);

        TweenObjAttribute attribute = isOpenOnlyType ? new TweenObjAttribute(4) : new TweenObjAttribute(moveType, scaleType, rotateType, alphaType, this);

        attribute.Obj = obj;

        attribute.FromAlpha = fromAlpha;
        attribute.ToAlpha = toAlpha;

        attribute.UpdateTime = 0;
        attribute.Time = timeLength;
        attribute.DelayTime = delayTime;
        attribute.Obj.transform.GetComponent<RectTransform>().anchoredPosition = fromPos;
        attribute.CallBackFunc = callBackFunc;

        objAttributeList.Add(attribute);

        attribute.IsTweening = true;
    }

    public void ClearTween()
    {
        DisposeAll();
        objAttributeList.Clear();
    }

    private void Update()
    {
        //** 自定义物体 动画
        if (isTweening && animGameObj)
        {
            updateTime += Time.deltaTime;

            //判断 是否动画完成
            if (updateTime - delayTime > time)
            {
                StopNormalTween();
            }
            else if (updateTime - delayTime >= 0)
            {
                float t = (updateTime - delayTime) / time;

                UpdateMove(moveType, animGameObj, fromPos, toPos, t);
                UpdateScale(scaleType, animGameObj, fromScale, toScale, t);
                UpdateRotate(rotateType, animGameObj, fromRotate, toRotate, t);
                //UpdateFade(alphaType, animGameObj, fromAlpha, toAlpha, t);
            }
        }

        //** 代码里指定的物体 动画
        if (objAttributeList.Count <= 0) return;

        for (int i = 0; i < objAttributeList.Count; i++)
        {
            TweenObjAttribute objAttribute = objAttributeList[i];
            if (!objAttribute.IsTweening) continue;

            objAttribute.UpdateTime += Time.deltaTime;

            //判断 是否动画完成
            if (objAttribute.UpdateTime - objAttribute.DelayTime > objAttribute.Time)
            {
                StopTween(objAttribute);
            }
            else if (objAttribute.UpdateTime - objAttribute.DelayTime >= 0)
            {
                if (!objAttribute.Obj.activeSelf)
                    objAttribute.Obj.SetActive(true);

                float t = (objAttribute.UpdateTime - objAttribute.DelayTime) / objAttribute.Time;

                UpdateMove(objAttribute.MoveType, objAttribute.Obj, objAttribute.FromPos, objAttribute.ToPos, t, objAttribute.IsCounterX, objAttribute.IsCounterY, objAttribute.Offset);
                UpdateScale(objAttribute.ScaleType, objAttribute.Obj, objAttribute.FromScale, objAttribute.ToScale, t);
                UpdateRotate(objAttribute.RotateType, objAttribute.Obj, objAttribute.FromRotate, objAttribute.ToRotate, t);
                //UpdateFade(objAttribute.AlphaType, objAttribute.Obj, objAttribute.FromAlpha, objAttribute.ToAlpha, t);
            }
        }
    }

    //移动(Move)动画
    private void UpdateMove(TweenType type, GameObject obj, Vector3 fromPos, Vector3 toPos, float t, bool isCounterX = false, bool isCounterY = false, float offset = 0)
    {
        if (type == TweenType.None)
            return;

        Vector3 move = toPos - fromPos;

        float xVar = xCurve.Evaluate(t);
        float yVar = yCurve.Evaluate(t);
        float val = t / time;
        move.x *= !isCounterX ? val - (val - xVar) : val + (val - xVar);
        move.y *= !isCounterY ? val - (val - yVar) : val + (val - yVar);

        //偏移值
        float offsetVal = offsetCurve.Evaluate(t);
        float oVal = offset * offsetVal;

        float a = Vector3.Angle(move, Vector3.left) - 90;
        float offsetY = oVal * Mathf.Sin(a * Mathf.Deg2Rad);
        float offsetX = oVal * Mathf.Cos(a * Mathf.Deg2Rad);

        obj.transform.localPosition = fromPos + move + new Vector3(-offsetX, offsetY, 0);
    }
    //缩放(Scale)动画
    private void UpdateScale(TweenType type, GameObject obj, Vector3 fromScale, Vector3 toScale, float t)
    {
        if (type == TweenType.None)
            return;

        float val = scaleCurve.Evaluate(t);
        Vector3 s = fromScale * (1 - val) + toScale * val;
        obj.transform.localScale = s;
    }
    //旋转(Rotate)动画
    private void UpdateRotate(TweenType type, GameObject obj, Vector3 fromRotate, Vector3 toRotate, float t)
    {
        if (type == TweenType.None)
            return;

        float val = rotateCurve.Evaluate(t);
        Vector3 s = fromRotate * (1 - val) + toRotate * val;
        obj.transform.localRotation = Quaternion.Euler(s);
    }

    //透明(Alpha)动画
    //Dictionary<GameObject, AnimatedAlpha> canvasGroups = new Dictionary<GameObject, AnimatedAlpha>();
    //private void UpdateFade(TweenType type, GameObject obj, float fromAlpha, float toAlpha, float t)
    //{
    //    if (type == TweenType.None)
    //        return;

    //    AnimatedAlpha g = null;
    //    if (!canvasGroups.ContainsKey(obj))
    //    {
    //        g = obj.GetComponent<AnimatedAlpha>();
    //        canvasGroups[obj] = g;
    //    }
    //    else
    //    {
    //        g = canvasGroups[obj];
    //    }

    //    if (g == null)
    //        return;

    //    float val = alphaCurve.Evaluate(t);
    //    g.alpha = fromAlpha * (1 - val) + toAlpha * val;
    //}


    private void StopNormalTween()
    {
        if (moveType != TweenType.None)
        {
            animGameObj.transform.localPosition = toPos;
        }
        if (scaleType != TweenType.None)
        {
            //animGameObj.transform.localScale = toScale;
        }
        if (rotateType != TweenType.None)
        {
            animGameObj.transform.localRotation = Quaternion.Euler(toRotate);
        }
        //if (alphaType != TweenType.None)
        //{
        //    AnimatedAlpha g = animGameObj.GetComponent<AnimatedAlpha>();
        //    if (g != null)
        //    {
        //        g.alpha = toAlpha;
        //    }
        //}

        updateTime = 0;
        isTweening = false;

        if (callBackFunc != null)
        {
            callBackFunc(animGameObj);
        }
    }

    private void StopTween(TweenObjAttribute objManager)
    {
        if (objManager.Obj == null)
            return;

        if (objManager.MoveType != TweenType.None)
        {
            objManager.Obj.transform.localPosition = objManager.ToPos;
        }
        if (objManager.ScaleType != TweenType.None)
        {
            objManager.Obj.transform.localScale = toScale;
        }
        if (objManager.RotateType != TweenType.None)
        {
            objManager.Obj.transform.localRotation = Quaternion.Euler(toRotate);
        }
        //if (objManager.AlphaType != TweenType.None)
        //{
        //    AnimatedAlpha g = objManager.Obj.GetComponent<AnimatedAlpha>();
        //    if (g != null)
        //    {
        //        g.alpha = objManager.ToAlpha;
        //    }
        //}
        objManager.IsTweening = false;
        objManager.UpdateTime = 0;

        if (objManager.CallBackFunc != null)
        {
            objManager.CallBackFunc(objManager.Obj);
        }

        objAttributeList.Remove(objManager);
    }

    private void OnDestroy()
    {
        DisposeAll();
    }

    private void Dispose()
    {
        for (int i = 0, length = objAttributeList.Count; i < length; i++)
        {
            objAttributeList[i].Dispose();
            objAttributeList[i].Obj = null;
        }
        objAttributeList.Clear();
    }

    public void DisposeAll()
    {
        Dispose();
    }
}
