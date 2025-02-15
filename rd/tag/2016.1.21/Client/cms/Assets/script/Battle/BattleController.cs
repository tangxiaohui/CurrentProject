﻿using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;
using DG.Tweening;

public enum ExitInstanceType
{
    Exit_Instance_OK,
    Exit_Instance_Next,
    Exit_Instance_Retry,

    Num_Exit_Instance_Type
}

public enum BattleType
{
    Normal,
    Boss,
    Rare,
}

public class BattleController : MonoBehaviour
{
    public static float floorHeight = 0.0f;
    public string instanceSpell;
    int curProcessIndex = 0;
    int maxProcessIndex = 0;
    int battleStartID = BattleConst.enemyStartID;
    public  BattleType battleType;
    InstanceData instanceData;
    PB.HSInstanceEnterRet curInstance = null;
    BattleLevelData curBattleLevel = null;
    int instanceStar = 0;
    GameObject occlusionY;
    GameObject occlusion1;
    GameObject occlusion2;
    Animator occlusionAnimator;
    public double beginChangeEnegyTime = 0;
    private float mirrorEnegy = 0;
    public  float MirrorEnegyAttr
    {
        get
        {
            return mirrorEnegy;
        }
        set
        {
            mirrorEnegy = value;
            if(mirrorEnegy < 0)
            {
                mirrorEnegy = 0;
            }
            if(mirrorEnegy > GameConfig.Instance.MirrorMaxEnegy)
            {
                mirrorEnegy = GameConfig.Instance.MirrorMaxEnegy;
            }
            if (uiBattle != null && uiBattle.m_MirrorDray != null)
            {
                uiBattle.m_MirrorDray.UpdateMirrorEnegy();
            }
        }
    }
    public MirrorState mirrorState = MirrorState.CannotUse;

    public int InstanceStar
    {
        get { return instanceStar; }
    }

    public InstanceData InstanceData
    {
        get { return instanceData; }
    }
    BattleProcess process;
    public BattleProcess Process
    {
        get { return process; }
    }
    bool isMouseOnUI = false;

    BattleGroup battleGroup;
    public BattleGroup BattleGroup
    {
        get { return battleGroup; }
    }

    bool mRevived;
    public bool IsRevived
    {
        set { mRevived = value; }
        get { return mRevived; }
    }

    //战斗胜利method
    MethodInfo victorMethod = null;

    static BattleController instance;
    public static BattleController Instance
    {
        get { return instance; }
    }

    public BattleObject curBattleScene
    {
        set;
        get;
    }
    private UIBattle uiBattle;
    public bool processStart;
    private UIScore mUIScore;
    public int mHuoliBeforeScore;
    private UILevelInfo mUILevelInfo;

	private	Dictionary<string,Transform> cameraNodeDic = new Dictionary<string, Transform>();
    private EnterInstanceParam curInstanceParam;
    private bool battleSuccess;
    //---------------------------------------------------------------------------------------------
    // Use this for initialization
    public void Init()
    {
        instance = this;
        BindListener();
        process = gameObject.AddComponent<BattleProcess>();
        process.Init();
        //battleGroup = new BattleGroup();
    }
    //---------------------------------------------------------------------------------------------
	void OnDestroy()
	{
        Destroy(process);
		UnBindListener ();
	}
    //---------------------------------------------------------------------------------------------
	void BindListener()
	{
        GameEventMgr.Instance.AddListener<Vector3>(GameEventList.MirrorClicked, OnMirrorClilced);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.INSTANCE_SETTLE_C.GetHashCode().ToString(), OnInstanceSettleResult);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.INSTANCE_SETTLE_S.GetHashCode().ToString(), OnInstanceSettleResult);
        GameEventMgr.Instance.AddListener<ProtocolMessage>(PB.code.PLAYER_REWARD_S.GetHashCode().ToString(), OnScoreReward);
	}
    //---------------------------------------------------------------------------------------------
	void UnBindListener()
	{
        GameEventMgr.Instance.RemoveListener<Vector3>(GameEventList.MirrorClicked, OnMirrorClilced);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.INSTANCE_SETTLE_C.GetHashCode().ToString(), OnInstanceSettleResult);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.INSTANCE_SETTLE_S.GetHashCode().ToString(), OnInstanceSettleResult);
        GameEventMgr.Instance.RemoveListener<ProtocolMessage>(PB.code.PLAYER_REWARD_S.GetHashCode().ToString(), OnScoreReward);
	}
    //---------------------------------------------------------------------------------------------
	void  OnMirrorClilced(Vector3 inputPos)
	{
		RaycastBattleObject (inputPos);
	}
    //---------------------------------------------------------------------------------------------
    void Update()
    {
		Vector3 inputPos = Input.mousePosition;
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            isMouseOnUI = EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId);
        }  
        else if (Input.GetMouseButtonDown(0))
        {
            isMouseOnUI = EventSystem.current.IsPointerOverGameObject();
        }

		if (Input.GetMouseButtonUp (0))
		{
            if (!isMouseOnUI)
            {
                if (!LastEvenType.Instance.IsDrag())
                {
                    RaycastBattleObject(inputPos);
                }
            }
            else 
            {
                GameObject curGo = EventSystem.current.currentSelectedGameObject;
                if (curGo != null && curGo.GetComponent<PetSwitchItem>() == null)
                {
                    GameEventMgr.Instance.FireEvent<int>(GameEventList.HideSwitchPetUI, BattleConst.closeSwitchPetUI);
                }
            }
		}
        int changeStep = 0;
        //mirror enegy
        if(battleType == BattleType.Boss)
        {
            if (mirrorState == MirrorState.Recover)
            {
                changeStep = 1;
            }
            else if (mirrorState == MirrorState.Consum)
            {
                changeStep = -1;
            }
        }
        
        if(changeStep != 0)
        {
            double curTime = GameTimeMgr.Instance.TimeStampAsMilliseconds();
            if (curTime - beginChangeEnegyTime > 200)
            {
                int times = (int)(curTime - beginChangeEnegyTime) / 200;
                if(changeStep == 1)
                {
                    MirrorEnegyAttr += times * GameConfig.Instance.RecoveryMirrorEnegyUnit;
                }
                else
                {
                    MirrorEnegyAttr -= times * GameConfig.Instance.ConsumMirrorEnegyUnit;
                }
                beginChangeEnegyTime += times * 200;
            }
            if(changeStep == -1 && MirrorEnegyAttr < 0.001)
            {
                GameEventMgr.Instance.FireEvent<bool,bool>(GameEventList.SetMirrorModeState, false,true);
                UIIm.Instance.ShowSystemHints(StaticDataMgr.Instance.GetTextByID("battle_zhaoyaojing_002"), (int)PB.ImType.PROMPT);
            }
        }
    }
    //---------------------------------------------------------------------------------------------
	void RaycastBattleObject(Vector3 inputPos)
	{
		RaycastHit hit;
        //Ray ray = BattleCamera.Instance.GetComponentInChildren<Camera>().ScreenPointToRay(inputPos);
        Ray ray = Camera.main.ScreenPointToRay(inputPos);
		
		if (Physics.Raycast(ray, out hit, 1000))
		{
			var battleGo = hit.collider.gameObject.GetComponent<BattleObject>();
			if (battleGo)
			{
				if(battleGo.unit.isVisible)
				{
					OnHitBattleObject(battleGo, null);
				}
				return;
			}
            if (hit.collider.gameObject.tag == "DropItem")
            {
                Transform parent = hit.collider.gameObject.transform.parent;
                DropItems dropItems = parent.GetComponent<DropItems>();
                if (dropItems != null)
                {
                    dropItems.OnHit();
                }
            }

			var wpTarget = hit.collider.gameObject.GetComponent<WeakpointTarget>();
			if(wpTarget)
			{
				BattleObject bo = wpTarget.battleObject;
				WeakPointRuntimeData wpRuntimeData = null;
				if(bo.wpGroup.allWpDic.TryGetValue(wpTarget.wpId,out wpRuntimeData))
				{
					if( wpRuntimeData.IsCanFireFocus())
					{
						OnHitBattleObject(bo,wpTarget.wpId);
						return;
					}
				}

			}
		}
           
		GameEventMgr.Instance.FireEvent<int>(GameEventList.HideSwitchPetUI, BattleConst.closeSwitchPetUI);
	}
    //---------------------------------------------------------------------------------------------
    void OnHitBattleObject(BattleObject battleGo, string weakpointName)
    {
        if (battleGo.camp == UnitCamp.Enemy)
        {
            //集火或者大招
			process.OnHitBattleObject(battleGo, weakpointName);
            GameEventMgr.Instance.FireEvent<int>(GameEventList.HideSwitchPetUI, BattleConst.closeSwitchPetUI);
           // Logger.LogWarning("hit enemy gameobject....");
        }
        else if (
            battleGo.camp == UnitCamp.Player &&
            process.SwitchingPet == false && 
            uiBattle.gameObject.activeSelf == true &&
            battleGo.unit.backUp == false
            )
        {
            //换宠
            ShowSwitchPetUIArgs args = new ShowSwitchPetUIArgs();
            args.targetId = battleGo.guid;
            GameEventMgr.Instance.FireEvent<EventArgs>(GameEventList.ShowSwitchPetUI, args);
        }
    }
    //---------------------------------------------------------------------------------------------
    public void StartBattlePrepare(EnterInstanceParam enterParam)
    {
        curInstanceParam = enterParam;
        //add player to load
        PbUnit pb = null;
        Dictionary<int, PbUnit> unitPbList = GameDataMgr.Instance.PlayerDataAttr.unitPbList;
        int count = enterParam.playerTeam.Count;
        for (int i = 0; i < count; ++i)
        {
            if (unitPbList.TryGetValue(enterParam.playerTeam[i], out pb))
            {
                AddUnitDataRequestInternal(pb.id);
            }
        }
        //add enemy to load
        curInstance = enterParam.instanceData;
        instanceData = StaticDataMgr.Instance.GetInstanceData(enterParam.instanceData.instanceId);
        count = curInstance.battle.Count;
        for (int index = 0; index < count; ++index)
        {
            PB.HSBattle curBattle = curInstance.battle[index];
            int monsterCount = curBattle.monsterCfgId.Count;
            for (int i = 0; i < curBattle.monsterCfgId.Count; ++i)
            {
                AddUnitDataRequestInternal(curBattle.monsterCfgId[i]);
            }
        }

        ActorEventService.Instance.AddResourceGroup("common");
        ActorEventService.Instance.AddResourceGroup("commonAttack");
        ActorEventService.Instance.AddResourceGroup("commonBuff");
        ActorEventService.Instance.AddResourceGroup("cure");
        ActorEventService.Instance.AddResourceGroup("commonDeBuff");
        ActorEventService.Instance.AddResourceGroup("commonPassive");

        //add scoreui
        UIScore.AddResourceRequest();
        mHuoliBeforeScore = GameDataMgr.Instance.PlayerDataAttr.HuoliAttr;

        ResourceMgr.Instance.AddAssetRequest(new AssetRequest("hintMessage"));
    }
    //---------------------------------------------------------------------------------------------
    public EnterInstanceParam GetCurrentInstance()
    {
        return curInstanceParam;
    }
    //---------------------------------------------------------------------------------------------
    private void AddUnitDataRequestInternal(string assetID)
    {
        ResourceMgr resMgr = ResourceMgr.Instance;
        UnitData unitRowData = StaticDataMgr.Instance.GetUnitRowData(assetID);
        if (unitRowData != null)
        {
            resMgr.AddAssetRequest(new AssetRequest(unitRowData.assetID));
        }

        //TODO:add spell request
        ActorEventService.Instance.AddResourceGroup(assetID);
    }
    //---------------------------------------------------------------------------------------------
    public void StartBattle()
    {
        mRevived = false;
        //ResourceMgr.Instance.UnloadCachedBundles(true);
        curProcessIndex = 0;
        processStart = false;
        battleStartID = BattleConst.enemyStartID;
        curInstance = curInstanceParam.instanceData;
        //battleType = (BattleType)proto.battleType;
        instanceData = StaticDataMgr.Instance.GetInstanceData(curInstanceParam.instanceData.instanceId);
        maxProcessIndex = curInstanceParam.instanceData.battle.Count;

        InstanceEntryRuntimeData instanceRuntimeData = InstanceMapService.Instance.GetRuntimeInstance(curInstanceParam.instanceData.instanceId);
        if (instanceRuntimeData!=null)
        {
            instanceStar = instanceRuntimeData.star;
        }
        if (!InitVictorMethod())
            return;
        AudioSystemMgr.Instance.PlayMusic(instanceData.instanceProtoData.backgroundmusic);
        //设置battlegroup 并且创建模型
        //battleGroup.SetEnemyList(proto.enemyList);
        GameDataMgr.Instance.PlayerDataAttr.SetMainUnits(curInstanceParam.playerTeam);

        //加载场景
        LoadBattleScene(instanceData.instanceProtoData.sceneID);
    }
    //---------------------------------------------------------------------------------------------
    public UIBattle GetUIBattle()
    {
        return uiBattle;
    }
    //---------------------------------------------------------------------------------------------
    public GameObject GetSceneRoot()
    {
        return (curBattleScene != null) ? curBattleScene.gameObject : null;
    }
    //---------------------------------------------------------------------------------------------
    void LoadBattleScene(string sceneName)
    {
        battleGroup = new BattleGroup();
        ////int index = sceneName.LastIndexOf('/');
        ////string assetbundle = sceneName.Substring(0, index);
        ////string assetname = sceneName.Substring(index + 1, sceneName.Length - index - 1);
        ////curBattleScene = ObjectDataMgr.Instance.CreateSceneObject(BattleConst.battleSceneGuid, "", "scene_root");
        ////OnSceneLoaded(null, null);

        //ResourceMgr.Instance.LoadLevelAsyn(sceneName, false, OnSceneLoaded);
        GameObject sceneRoot = GameObject.Find("Root");
        if (sceneRoot == null)
        {
            Logger.LogError("can not find pos slot parent!");
            sceneRoot = gameObject;
        }
        curBattleScene = ObjectDataMgr.Instance.AddSceneObject(BattleConst.battleSceneGuid, sceneRoot);
        
        //init scene spell if there has
        InstanceEntry entryData = StaticDataMgr.Instance.GetInstanceEntry(instanceData.instanceProtoData.id);
        if (entryData != null && string.IsNullOrEmpty(entryData.instanceSpell) == false)
        {
            curBattleScene.unit.spellList = new Dictionary<string, Spell>();
            instanceSpell = entryData.instanceSpell;
            SpellProtoType spellPt = StaticDataMgr.Instance.GetSpellProtoData(instanceSpell);
            if (spellPt != null)
            {
                if (
                    spellPt.category == (int)SpellType.Spell_Type_Defense ||
                    spellPt.category == (int)SpellType.Spell_Type_Passive ||
                    spellPt.category == (int)SpellType.Spell_Type_Beneficial ||
                    spellPt.category == (int)SpellType.Spell_Type_Negative ||
                    spellPt.category == (int)SpellType.Spell_Type_Lazy ||
                    spellPt.category == (int)SpellType.Spell_Type_PrepareDazhao ||
                    spellPt.category == (int)SpellType.Spell_Type_Hot
                    )
                {
                    curBattleScene.unit.spellList.Add(instanceSpell, new Spell(spellPt, 1));
                }
                else
                {
                    curBattleScene.unit.spellList.Add(instanceSpell, new Spell(spellPt, instanceData.instanceProtoData.level));
                }
            }
        }

        GameDataMgr.Instance.PlayerDataAttr.InitMainUnitList();
        battleGroup.SetPlayerList();
        SetCameraDefault();

        uiBattle = UIMgr.Instance.OpenUI_(UIBattle.ViewName) as UIBattle;
        uiBattle.Initialize();
        uiBattle.ShowUI(false);
        mUILevelInfo = UIMgr.Instance.OpenUI_(UILevelInfo.ViewName) as UILevelInfo;
        mUILevelInfo.SetInstanceName(entryData.NameAttr);
        //UIIm.Instance.transform.SetAsLastSibling();
        StartProcess(curProcessIndex);
    }
    //---------------------------------------------------------------------------------------------
    public void OnSceneLoaded(GameObject instance, System.EventArgs args)
    {
        GameObject sceneRoot = GameObject.Find("Root");
        if (sceneRoot == null)
        {
            Logger.LogError("can not find pos slot parent!");
            sceneRoot = gameObject;
        }
        curBattleScene = ObjectDataMgr.Instance.AddSceneObject(BattleConst.battleSceneGuid, sceneRoot);
        StartCoroutine(LoadBattleUI());
    }
    //---------------------------------------------------------------------------------------------
    public IEnumerator LoadBattleUI()
    {
        yield return new WaitForSeconds(1.0f);
        //SetCameraDefault();

        GameDataMgr.Instance.PlayerDataAttr.InitMainUnitList();
        battleGroup.SetPlayerList();

        //uiBattle = UIMgr.Instance.OpenUI_(UIBattle.ViewName) as UIBattle;
        //uiBattle.Initialize();
        //uiBattle.ShowUI();
        StartProcess(curProcessIndex);
    }
    //---------------------------------------------------------------------------------------------
    public void SetCameraDefault()
    {
        Transform defaultTrans = GetDefaultCameraNode();
        BattleCamera.Instance.transform.localPosition = defaultTrans.position;
        BattleCamera.Instance.transform.localRotation = defaultTrans.rotation;
        BattleCamera.Instance.transform.localScale = Vector3.Scale(transform.localScale, defaultTrans.localScale);
    }
    //---------------------------------------------------------------------------------------------
    public Transform GetDefaultCameraNode()
    {
        string cameraSlotName = "cameraNormal";
        if (battleType == BattleType.Boss)
        {
            cameraSlotName = "cameraBoss";
        }
		Transform defaultTrans = null;
		if (cameraNodeDic.TryGetValue (cameraSlotName, out defaultTrans))
		{
			return defaultTrans;
		}

        GameObject posParent = GetPositionRoot();
        GameObject cameraNode = Util.FindChildByName(posParent, cameraSlotName);
		cameraNodeDic.Add (cameraSlotName, cameraNode.transform);
		return cameraNode.transform;
    }
	//---------------------------------------------------------------------------------------------
	public	Transform GetPhyDazhaoCameraNode()
    {
		string name = "cameraPhyDazhao";
		Transform dazhaoTrans = null;
		if (cameraNodeDic.TryGetValue (name, out dazhaoTrans))
		{
			return dazhaoTrans;
		}

        GameObject posParent = GetPositionRoot();
		GameObject cameraNode = Util.FindChildByName(posParent,name);
		cameraNodeDic.Add (name, cameraNode.transform);
		return cameraNode.transform;
	}
    //---------------------------------------------------------------------------------------------
    public void UnLoadBattleScene(ExitInstanceType state)
    {
        UIMgr.Instance.OpenUI_(UILoading.ViewName);
        StartCoroutine(UnLoadBattleSceneInternal(state));
    }
    //---------------------------------------------------------------------------------------------
    IEnumerator UnLoadBattleSceneInternal(ExitInstanceType state)
    {
        yield return new WaitForEndOfFrame();
        //state 0 confirm back to instance choose
        //state 1 next back to instance info
        //state 2 retry back to team choose
        battleGroup.DestroyEnemys();
        curBattleScene.ClearEvent();
        ObjectDataMgr.Instance.RemoveBattleObject(BattleConst.battleSceneGuid);
        //Destroy(curBattleScene);
        List<BattleObject> playerUnitList = GameDataMgr.Instance.PlayerDataAttr.GetMainUnits();
        for (int i = 0; i < playerUnitList.Count; ++i)
        {
            playerUnitList[i].ClearEvent();
            ObjectDataMgr.Instance.RemoveBattleObject(playerUnitList[i].guid);
            //playerUnitList[i].gameObject.SetActive(false);
        }
        battleGroup = null;
        curBattleScene = null;

        process.Clear();
        GameMain.Instance.ChangeModule<BuildModule>((int)state);
        UIMgr.Instance.DestroyUI(mUIScore);
        UIMgr.Instance.DestroyUI(mUILevelInfo);
        UIMgr.Instance.DestroyUI(uiBattle);
        //ResourceMgr.Instance.LoadLevelAsyn("mainstage", false, null);
        //curInstanceParam = null;
    }
    //---------------------------------------------------------------------------------------------
    public GameObject GetSlotNode(UnitCamp camp, int slotID, bool isBoss)
    {
        if (curBattleScene == null)
        {
            Logger.LogError("battle scene is null");
            return GameMain.Instance.gameObject;
        }
        GameObject posParent = GetPositionRoot();

        string nodeName = "pos";
        if (isBoss)
        {
            nodeName = "bosspos";
        }
        else 
        {
            if (camp == UnitCamp.Enemy)
            {
                slotID = slotID + BattleConst.slotIndexMax + 1;
            }

            nodeName = nodeName + slotID.ToString();
        }

        GameObject slotNode = Util.FindChildByName(posParent, nodeName);
        if (slotNode != null)
        {
            string testName = slotNode.name;
            return slotNode;
        }

        return GameMain.Instance.gameObject;
    }
    //---------------------------------------------------------------------------------------------
    public GameObject GetPositionRoot()
    {
        GameObject posParent = Util.FindChildByName(curBattleScene.gameObject, "battlePosition" + curProcessIndex.ToString());
        if (posParent == null)
        {
            Logger.LogError("can not find pos slot parent!");
            posParent = curBattleScene.gameObject;
        }
        return posParent;
    }
    //---------------------------------------------------------------------------------------------
    /// <summary>
    /// 反射获取战斗/进程胜利的方法
    /// </summary>
    bool InitVictorMethod()
    {
        //获取战斗胜利的func
        //switch (battleType)
        //{ 
        //    case BattleType.Normal:
        //        {
        //            if (instanceData.normalValiVicMethod != null)
        //            {
        //                victorMethod = instanceData.normalValiVicMethod;
        //                break;
        //            }

        //            string funcName = instanceData.normalValiVic;
        //            var cls = typeof(NormalScript);
        //            victorMethod = cls.GetMethod(funcName);
        //            if (victorMethod == null)
        //            {
        //                Logger.LogErrorFormat("Instance {0}'s normalValiVic #{1}# can not find! Exit battle!", instanceData.id, funcName);
        //                return false;
        //            }
        //            instanceData.normalValiVicMethod = victorMethod;
        //            break;
        //        }
        //    case BattleType.Boss:
        //        {
        //            if (instanceData.bossValiVicMethod != null)
        //            {
        //                victorMethod = instanceData.bossValiVicMethod;
        //                break;
        //            }

        //            string funcName = instanceData.bossValiVic;
        //            var cls = typeof(BossScript);
        //            victorMethod = cls.GetMethod(funcName);
        //            if (victorMethod == null)
        //            {
        //                Logger.LogErrorFormat("Instance {0}'s bossValiVic #{1}# can not find! Exit battle!", instanceData.id, funcName);
        //                return false;
        //            }
        //            instanceData.bossValiVicMethod = victorMethod;

        //            foreach (var item in instanceData.bossProcess)
        //            {
        //                if (item.method != null)
        //                    break;

        //                funcName = item.func;
        //                var method = cls.GetMethod(funcName);
        //                if (method == null)
        //                {
        //                    Logger.LogErrorFormat("Instance {0}'s BossProcessValiVic #{1}# can not find! Exit battle!", instanceData.id, funcName);
        //                    return false;
        //                }
        //                item.method = method;
        //            }
        //            break;
        //        }
        //    case BattleType.Rare:
        //        {
        //            if (instanceData.rareValiVicMethod != null)
        //            {
        //                victorMethod = instanceData.rareValiVicMethod;
        //                break;
        //            }

        //            string funcName = instanceData.rareValiVic;
        //            var cls = typeof(RareScript);
        //            victorMethod = cls.GetMethod(funcName);
        //            if (victorMethod == null)
        //            {
        //                Logger.LogErrorFormat("Instance {0}'s rareValiVic #{1}# can not find! Exit battle!", instanceData.id, funcName);
        //                return false;
        //            }
        //            instanceData.rareValiVicMethod = victorMethod;

        //            foreach (var item in instanceData.rareProcess)
        //            {
        //                if (item.method != null)
        //                    break;

        //                funcName = item.func;
        //                var method = cls.GetMethod(funcName);
        //                if (method == null)
        //                {
        //                    Logger.LogErrorFormat("Instance {0}'s RareProcessValiVic #{1}# can not find! Exit battle!", instanceData.id, funcName);
        //                    return false;
        //                }
        //                item.method = method;
        //            }
        //            break;
        //        }
        //    default:
        //        Logger.LogError("Battle type error" + battleType);
        //        return false;
        //}

        return true;
    }
    //---------------------------------------------------------------------------------------------
    //void ShowUI()
    //{
    //    GameEventMgr.Instance.FireEvent(GameEventList.ShowBattleUI);
    //}
    public void ShowLevelInfo(bool isVisible)
    {
        if (isVisible == true)
        {
            uiBattle.ShowUI(false);
            UIIm.Instance.SetLevelVisible(false);
            mUILevelInfo.SetBattleLevelProcess(curProcessIndex + 1, maxProcessIndex);
        }
        else
        {
            uiBattle.ShowUI(true);
            UIIm.Instance.SetLevelVisible(true);
            mUILevelInfo.SetVisible(false);
        }
    }
    //---------------------------------------------------------------------------------------------
    void StartProcess(int index)
    {
        if (index < maxProcessIndex)
        {
            GameObject slotNode = BattleController.Instance.GetSlotNode(UnitCamp.Player, 0, false);
            floorHeight = slotNode.transform.position.y;

            process.ClearRewardItem();
            PB.HSBattle curBattle = curInstance.battle[index];
            curBattleLevel = StaticDataMgr.Instance.GetBattleLevelData(curBattle.battleCfgId);
            if (curBattleLevel.battleProtoData.id.Contains("Boss"))
            {
                battleType = BattleType.Boss;
            }
            else
            {
                battleType = BattleType.Normal;
            }

            List<PbUnit> pbList = new List<PbUnit>();

            int monsterCount = curBattle.monsterCfgId.Count;
            int dropCount = curBattle.monsterDrop.Count;
            if (monsterCount != dropCount)
            {
                Logger.LogError("monster count not equal to drop count!");
            }

            for (int i = 0; i < curBattle.monsterCfgId.Count; ++i)
            {
                PbUnit pbUnit = new PbUnit();
                //enemy use minus uid
                pbUnit.guid = --battleStartID;
                //if (StaticDataMgr.Instance.GetUnitRowData(instanceData.bossID) != null)
                //    pbUnit.id = instanceData.bossID;
                //else
                pbUnit.id = curBattle.monsterCfgId[i]; //instanceData.rareID;
                pbUnit.level = instanceData.instanceProtoData.level;
                pbUnit.camp = UnitCamp.Enemy;
                pbUnit.slot = i;
                pbUnit.lazy = BattleConst.defaultLazy;

                pbList.Add(pbUnit);

                if (i < dropCount)
                {
                    process.AddRewardItem(pbUnit.guid, curBattle.monsterDrop[i]);
                }
            }
            battleGroup.SetEnemyList(pbList);
            //if (index == 0)
            //{
            //    AudioSystemMgr.Instance.PlayMusic(BattleController.Instance.InstanceData.instanceProtoData.backgroundmusic);
            //}
            //TODO:动画1
            System.Action<float> preStartEvent = (delayTime) =>
            {
                process.StartProcess(index, curBattleLevel);
            };
            //Logger.Log("当前副本ID：" + curBattleLevel.battleProtoData.id);
            //demo_1_level2
            if (!string.IsNullOrEmpty(curBattleLevel.battleProtoData.preStartEvent) && InstanceStar == 0)
            {
                UISpeech.Open(curBattleLevel.battleProtoData.preStartEvent, preStartEvent);
            }
            else
            {
                preStartEvent(0.0f);
            }
        }
        else 
        {
            curBattleLevel = null;
            OnBattleOver(true);
        }
    }
    //---------------------------------------------------------------------------------------------
    public IEnumerator StartNextProcess(float delayTime)
    {
        if (delayTime > 0.0f)
        {
            yield return new WaitForSeconds(delayTime);
        }

        //对局切换过度
        battleGroup.DestroyEnemys();
        //int playerCount = battleGroup.PlayerFieldList.Count;
        //for (int index = 0; index < playerCount; ++index)
        //{
        //    BattleObject bo = battleGroup.PlayerFieldList[index];
        //    if (bo != null)
        //    {
        //        bo.unit.OnStartNextProcess();
        //    }
        //}
        List<BattleObject> playerUnitList = GameDataMgr.Instance.PlayerDataAttr.GetMainUnits();
        var itor = playerUnitList.GetEnumerator();
        while (itor.MoveNext())
        {
            itor.Current.unit.OnStartNextProcess();
        }

        MagicDazhaoController.Instance.ClearAll ();
		PhyDazhaoController.Instance.ClearAll ();
        //no use now, only the last process has mirror
		//GameEventMgr.Instance.FireEvent<bool,bool> (GameEventList.SetMirrorModeState, false,false);
		process.HideFireFocus ();

        //uiBattle.gameObject.BroadcastMessage("OnAnimationFinish");
        uiBattle.ShowUI(false);
        UIIm.Instance.SetLevelVisible(false);
        float waitTime = BattleConst.unitOutTime * 0.5f * GameSpeedService.Instance.GetBattleSpeed();
        Appearance(false, waitTime);
        IsOcclusion(true);
        //Fade.FadeOut(waitTime);
        ItemDropManager.Instance.ClearDropItem();
        GameEventMgr.Instance.FireEvent<int>(GameEventList.HideSwitchPetUI, BattleConst.closeSwitchPetUI);
        yield return new WaitForSeconds(waitTime);

        ++curProcessIndex;
        //Fade.FadeIn(waitTime);
        cameraNodeDic.Clear();        
        IsOcclusion(false);
        yield return new WaitForSeconds(waitTime);        
        battleGroup.RefreshPlayerPos();
        //uiBattle.ShowUI(true);
        //uiBattle.gameObject.BroadcastMessage("OnAnimationFinish");
        StartProcess(curProcessIndex);
        SetCameraDefault();
        Appearance(true, waitTime);
        //TODO: fade in and fade out && end event &&recover life etc
    }
    //---------------------------------------------------------------------------------------------
    public bool HasNextProcess()
    {
        return curProcessIndex + 1 < maxProcessIndex;
    }
    //---------------------------------------------------------------------------------------------
    public void IsOcclusion(bool goOut)
    {
         if (occlusionY == null)
         {
             occlusionY = GameObject.Find("UIRoot/OcclusionY");
             if (occlusionY==null)
             {
                 return;
             }
             occlusion1 = occlusionY.transform.FindChild("y1").gameObject;
             occlusion2 = occlusionY.transform.FindChild("y2").gameObject;
             occlusion1.SetActive(true);
             occlusion2.SetActive(true);
             occlusionAnimator = occlusionY.GetComponent<Animator>();
         }
         occlusionAnimator.SetBool("out", !goOut);
         occlusionAnimator.SetBool("go", goOut);
    }
    //---------------------------------------------------------------------------------------------
    public void Appearance(bool goOut, float moveTime)//move out/move in
    {
        BattleObject battleObj = null;

        Vector3 vec;//方向
        Vector3 newPos = new Vector3(0.0f, 0.0f, 0.0f);//位置
        Vector3 moveTag = new Vector3(0.0f, 0.0f, 0.0f);//移动目标
        for (int i = 0; i < battleGroup.PlayerFieldList.Count; i++)
        {
            battleObj = battleGroup.PlayerFieldList[i];
            if (battleObj == null)
            {
                continue;
            }
            Transform playerNode = GetSlotNode(UnitCamp.Player, battleObj.unit.pbUnit.slot, false).transform;
            vec = new Vector3(playerNode.forward.x, playerNode.forward.y, playerNode.forward.z);
            Vector3.Normalize(vec);
            if (goOut)//进场
            {
                newPos = BattleConst.distance * vec * -1.0f + playerNode.transform.position;
                moveTag = playerNode.position;
                battleGroup.PlayerFieldList[i].transform.localPosition = newPos;
            }
            else//离场
            {
                newPos = vec + playerNode.position;
                moveTag = BattleConst.distance * vec + playerNode.transform.position;
            }
            battleGroup.PlayerFieldList[i].transform.DOMove(moveTag, BattleConst.moveTime);
            //tw.SetEase(Ease.Linear);
            battleObj.TriggerEvent(BattleConst.unitExitandenter, Time.time, null);
        }
    }
    //---------------------------------------------------------------------------------------------
    public void PlayEntranceAnim()
    {
        float curTime = Time.time;
        for (int i = 0; i < battleGroup.EnemyFieldList.Count; ++i)
        {
            BattleObject bo = battleGroup.EnemyFieldList[i];
            if (bo != null && bo.unit.curLife > 0 && bo.unit.State != UnitState.Dead)
            {
                //TODO: use level time
                battleGroup.EnemyFieldList[i].TriggerEvent("chuchang", curTime, null);
            }
        }
    }
    //---------------------------------------------------------------------------------------------
    public void OnBattleOver(bool isSuccess)
    {
        mirrorState = MirrorState.CannotUse;
        battleSuccess = isSuccess;
        ItemDropManager.Instance.ClearDropItem();
        processStart = false;
        //Logger.LogWarning("Battle " + (isSuccess ? "Success" : "Failed"));
        AudioSystemMgr.Instance.StopMusic();
        string selfEvent = isSuccess ? "win" : "failed";
        string enemyEvent = isSuccess ? "failed" : "win";
        float curTime = Time.time;
        for (int i = 0; i < battleGroup.PlayerFieldList.Count; ++i)
        {
            BattleObject bo = battleGroup.PlayerFieldList[i];
            if (bo != null && bo.unit.curLife > 0 && bo.unit.State != UnitState.Dead)
            {
                battleGroup.PlayerFieldList[i].TriggerEvent(selfEvent, curTime, null);
            }
        }
        for (int i = 0; i < battleGroup.EnemyFieldList.Count; ++i)
        {
            BattleObject bo = battleGroup.EnemyFieldList[i];
            if (bo != null && bo.unit.curLife > 0 && bo.unit.State != UnitState.Dead)
            {
                battleGroup.EnemyFieldList[i].TriggerEvent(enemyEvent, curTime, null);
            }
        }
        MagicDazhaoController.Instance.ClearAll();
        PhyDazhaoController.Instance.ClearAll();
        process.HideFireFocus();

        PB.HSInstanceSettle instanceParam = new PB.HSInstanceSettle();
        instanceParam.deadMonsterCount = 0;
        if (mRevived == false)
        {
            List<BattleObject>boList = battleGroup.GetAllUnitList((int)UnitCamp.Player);
            for (int i = 0; i < boList.Count; ++i)
            {
                BattleObject bo = boList[i];
                if (bo != null)
                {
                    if (bo.unit.curLife <= 0 || bo.unit.State == UnitState.Dead)
                    {
                        instanceParam.deadMonsterCount++;
                    }
                }
            }
        }
        else
        {
            //3 means 1 star
            instanceParam.deadMonsterCount = 3;
        }

        instanceParam.passBattleCount = isSuccess ? maxProcessIndex : curProcessIndex;
        GameApp.Instance.netManager.SendMessage(PB.code.INSTANCE_SETTLE_C.GetHashCode(), instanceParam, false);
    } 
    //---------------------------------------------------------------------------------------------
    void OnInstanceSettleResult(ProtocolMessage msg)
    {
        UINetRequest.Close();
        if (msg.GetMessageType() == (int)PB.sys.ERROR_CODE)
        {
            Logger.LogError("instance settle result error");
            UnLoadBattleScene(ExitInstanceType.Exit_Instance_OK);
        }
        else
        {
            GameSpeedService.Instance.SetBattleSpeed(1.0f);
            if (mUIScore == null)
            {
                mUIScore = UIMgr.Instance.OpenUI_(UIScore.ViewName) as UIScore;
            }

            int starCount = 0;
            if(battleSuccess)
            {
                PB.HSInstanceSettleRet scoreInfo = msg.GetProtocolBody<PB.HSInstanceSettleRet>();
                GameEventMgr.Instance.FireEvent<int, string>(GameEventList.FinishedInstance, scoreInfo.starCount, instanceData.instanceProtoData.id);
                starCount = scoreInfo.starCount;
            }
            mUIScore.ShowScoreUI(battleSuccess, starCount);
            uiBattle.HideBattleUI();

            GameDataMgr.Instance.OnBattleOver(battleSuccess);
        }
    }
    //---------------------------------------------------------------------------------------------
    void OnScoreReward(ProtocolMessage msg)
    {
        PB.HSRewardInfo reward = msg.GetProtocolBody<PB.HSRewardInfo>();
        if (reward != null && reward.hsCode == PB.code.INSTANCE_SETTLE_C.GetHashCode())
        {
            if (mUIScore == null)
            {
                mUIScore = UIMgr.Instance.OpenUI_(UIScore.ViewName) as UIScore;
            }
            mUIScore.SetScoreInfo(reward);
        }
    }
    //---------------------------------------------------------------------------------------------
}
