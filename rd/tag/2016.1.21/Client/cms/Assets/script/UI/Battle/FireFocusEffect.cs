﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FireFocusEffect : MonoBehaviour
{
	RectTransform rectTrans = null;
	public GameObject effectGo;
	Transform effectTargetTrans;
	float offsetH = 0.0f;
    bool isActive = false;

	void Start()
	{
		rectTrans = transform as RectTransform;
		BindListener ();
        //modify:xiaolong 2015-8-27 16:39:11
        //effectGo = Util.FindChildByName (gameObject, "fireFocusImage");
        //if (null == effectGo)
        //{
        //    Logger.LogError ("Canpt Find SubGameobject: " + "fireFocusImage");
        //} 
        //else 
        //{
        //    effectGo.SetActive(false);
        //}
        effectGo.SetActive(false);
	}

	void OnDestroy()
	{
		UnBindListener();
	}

	void BindListener()
	{
        GameEventMgr.Instance.AddListener<BattleObject>(GameEventList.ShowFireFocus, OnShow);
		GameEventMgr.Instance.AddListener(GameEventList.HideFireFocus, OnHide);
	}
	
	void UnBindListener()
	{
        GameEventMgr.Instance.RemoveListener<BattleObject>(GameEventList.ShowFireFocus, OnShow);
		GameEventMgr.Instance.RemoveListener(GameEventList.HideFireFocus, OnHide);
	}

    void OnEnable()
    {
        if (isActive)
        {
            try
            {
                StartCoroutine("RefreshEffectCo");
            }
            catch
            {
            }
        }
    }

	void OnShow(BattleObject bo)
    {
        //modify:xiaolong 2015-8-27 16:40:02
        //effectGo.SetActive(true);
        isActive = true;
        //even not active, save the new target trans
        offsetH = 0.5f;
        if (bo.unit.isBoss)
        {
            offsetH = 1.1f;
        }
        Transform targetTrans = bo.gameObject.transform;
        string wpName = bo.unit.attackWpName;
        if (!bo.unit.isBoss)
        {
            wpName = "xgwp";
        }
        if (!string.IsNullOrEmpty(wpName))
        {
            WeakPointRuntimeData wpRuntimeData = null;
            //WeakPointData wp = null;

            if (bo.wpGroup.allWpDic.TryGetValue(wpName, out wpRuntimeData))
            {
                targetTrans = wpRuntimeData.wpMirrorTarget.gameObject.transform;
                offsetH = 0.0f;

            }
        }
        effectTargetTrans = targetTrans;

        if (!gameObject.activeInHierarchy)
            return;

        effectGo.SetActive(true);

        try
        {
            StopCoroutine("RefreshEffectCo");
        }
        catch
        {
        }

        try
        {
            StartCoroutine("RefreshEffectCo");
        }
        catch
        {
        }

	}

	IEnumerator RefreshEffectCo()
	{
		while (true) 
		{
			if(null != effectTargetTrans)
			{
				Vector3 unitPosition = effectTargetTrans.position + new Vector3 (0, offsetH, 0);
				var screenPos = BattleCamera.Instance.CameraAttr.WorldToScreenPoint (unitPosition);
				rectTrans.anchoredPosition = new Vector2 (screenPos.x / UIMgr.Instance.CanvasAttr.scaleFactor, screenPos.y / UIMgr.Instance.CanvasAttr.scaleFactor);
			}
			yield return new WaitForEndOfFrame();
		}
	}

	void OnHide()
    {
        try
        {
            StopCoroutine("RefreshEffectCo");
        }
        catch
        {
        }
        //modify:xiaolong 2015-8-27 16:40:29
        //effectGo.SetActive(false);
        isActive = false;
        effectGo.SetActive(false);
	}


}
