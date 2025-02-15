using UnityEngine;
using System.Collections;
using System.IO;
using Csv.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class StaticDataMgr : MonoBehaviour
{
    static StaticDataMgr mInst = null;
    public static StaticDataMgr Instance
    {
        get
        {
            if (mInst == null)
            {
                GameObject go = new GameObject("StaticDataMgr");
                mInst = go.AddComponent<StaticDataMgr>();
            }
            return mInst;
        }
    }

    Dictionary<string, WeakPointData> weakPointData = new Dictionary<string, WeakPointData>();
    Dictionary<string, UnitData> unitData = new Dictionary<string, UnitData>();
    Dictionary<int, UnitBaseData> unitBaseData = new Dictionary<int, UnitBaseData>();
    Dictionary<string, BuffPrototype> buffData = new Dictionary<string, BuffPrototype>();
    Dictionary<string, EffectPrototype> effectData = new Dictionary<string, EffectPrototype>();
    Dictionary<string, SpellProtoType> spellData = new Dictionary<string, SpellProtoType>();
    Dictionary<int, int> spellUpLevelData = new Dictionary<int, int>();
    Dictionary<string, InstanceData> instanceData = new Dictionary<string, InstanceData>();
    Dictionary<string, BattleLevelData> battleLevelData = new Dictionary<string, BattleLevelData>();
	Dictionary<int,LazyData>	lazyData = new Dictionary<int, LazyData>();
	Dictionary<int,CharacterData> characterData = new Dictionary<int, CharacterData>();
	Dictionary<int,Chapter>	chapterData = new Dictionary<int, Chapter>();
	Dictionary<string,ItemStaticData> itemData = new Dictionary<string, ItemStaticData>();
    Dictionary<int, UnitStageData> unitStageData = new Dictionary<int, UnitStageData>();

    Dictionary<string, string> assetMapping = new Dictionary<string, string>();
    Dictionary<string, string> audioMapping = new Dictionary<string, string>();
    Dictionary<string, Dictionary<int, EquipProtoData>> equipData = new Dictionary<string, Dictionary<int, EquipProtoData>>();
    Dictionary<string, EquipForgeData> equipForge = new Dictionary<string, EquipForgeData>();
    Dictionary<int, EquipLevelData> baseAttrData = new Dictionary<int, EquipLevelData>();
    
    Dictionary<string, FunctionData> functionDic = new Dictionary<string, FunctionData>();
    Dictionary<int, List<FunctionData>> functionLevelDic = new Dictionary<int, List<FunctionData>>();
    Dictionary<string, PlayerBehavior> PlayerBehaviorData = new Dictionary<string, PlayerBehavior>();
    public void Init()
    {
        DontDestroyOnLoad(gameObject);
        InitData();
    }

    public void InitData()
    {
        {
            var data = InitTable<UnitData>("unitData");
            foreach (var item in data)
                unitData.Add(item.index, item);
        }
        {
            var data = InitTable<UnitBaseData>("unitBaseData");
            foreach (var item in data)
                unitBaseData.Add(item.level, item);
        }
        {
            var data = InitTable<SpellProtoType>("spell");
            foreach (var item in data)
            {
                //Debug.Log(item.id);
                spellData.Add(item.id, item);
            }
        }
        {
            var data = InitTable<SpellUpLevelPrice>("skillUpPrice");
            foreach (var item in data)
                spellUpLevelData.Add(item.level, item.coin);
        }
        {
            var data = InitTable<BuffPrototype>("buff");
            foreach (var item in data)
            {
                try
                {
                    buffData.Add(item.id, item);
                }
                catch
                {
                    Logger.LogError("repeat key :" + item.id);
                }
            }
        }
        {
            //effectData = new Dictionary<string, EffectPrototype>();
            var data = InitTable<EffectWholeData>("effect");
            EffectPrototype effectPt = null;
            foreach (EffectWholeData wholeData in data)
            {
                EffectType et = (EffectType)(wholeData.effectType);
                switch (et)
                {
                    case EffectType.Effect_Type_Set:
                        {
                            effectPt = new EffectSetPrototype();
                            EffectSetPrototype setPt = effectPt as EffectSetPrototype;
                            //string[] effects = wholeData.effectList.Split(';');
                            ArrayList effectArrayList = MiniJsonExtensions.arrayListFromJson(wholeData.effectList);
                            for (int i = 0; i < effectArrayList.Count; ++i)
                            {
                                setPt.effectList.Add(effectArrayList[i] as string);
                            }
                        }
                        break;
                    case EffectType.Effect_Type_Search:
                        {
                            effectPt = new EffectSearchPrototype();
                            EffectSearchPrototype searchPt = effectPt as EffectSearchPrototype;

                            searchPt.count = wholeData.count;
                            searchPt.camp = wholeData.camp;
                            searchPt.effectID = wholeData.searchEffect;
                        }
                        break;
                    case EffectType.Effect_Type_Persistent:
                        {
                            effectPt = new EffectPersistentProtoType();
                            EffectPersistentProtoType persistPt = effectPt as EffectPersistentProtoType;

                            persistPt.effectStartID = wholeData.effectStartID;
                            persistPt.startDelayTime = wholeData.startDelayTime;
                            if (wholeData.periodEffectList != null)
                            {
                                string[] effectList = wholeData.periodEffectList.Split(';');
                                ///ArrayList effectArrayList = MiniJsonExtensions.arrayListFromJson (wholeData.periodEffectList);
                                for (int i = 0; i < effectList.Length; ++i)
                                {
                                    string[] effectKV = effectList[i].Split('|');
                                    if (effectKV.Length != 2)
                                        continue;
                                    persistPt.effectList.Add(new KeyValuePair<float, string>(float.Parse(effectKV[0]), effectKV[1]));
                                }
                            }
                        }
                        break;
                    case EffectType.Effect_Type_Damage:
                        {
                            effectPt = new EffectDamageProtoType();
                            EffectDamageProtoType damagePt = effectPt as EffectDamageProtoType;

                            damagePt.damageType = wholeData.damageType;
                            damagePt.attackFactor = wholeData.attackFactor;
                            damagePt.isHeal = wholeData.isHeal > 0;
                            damagePt.damageProperty = wholeData.damageProperty;

                            damagePt.fixLifeRatio = wholeData.fixLifeRatio;
                        }
                        break;
                    case EffectType.Effect_Type_Buff:
                        {
                            effectPt = new EffectApplyBuffPrototype();
                            EffectApplyBuffPrototype buffPt = effectPt as EffectApplyBuffPrototype;

                            buffPt.buffID = wholeData.buffID;
                        }
                        break;
                    case EffectType.Effect_Type_Switch:
                        {
                            effectPt = new EffectSwitchPrototype();
                            EffectSwitchPrototype switchPt = effectPt as EffectSwitchPrototype;

                            string[] effectList = wholeData.periodEffectList.Split(';');
                            ///ArrayList effectArrayList = MiniJsonExtensions.arrayListFromJson (wholeData.periodEffectList);
                            for (int i = 0; i < effectList.Length; ++i)
                            {
                                string[] effectKV = effectList[i].Split('|');
                                if (effectKV.Length != 2)
                                    continue;

                                switchPt.effectList.Add(new KeyValuePair<string, string>(effectKV[0], effectKV[1]));
                            }
                        }
                        break;
                    case EffectType.Effect_Type_Dispel:
                        {
                            effectPt = new EffectDispelProtoType();
                            EffectDispelProtoType dispelPt = effectPt as EffectDispelProtoType;
                            dispelPt.dispelCategory = wholeData.dispelCategory;
                        }
                        break;
                }
                effectPt.id = wholeData.id;
                effectPt.effectType = et;
                effectPt.targetType = wholeData.targetType;
                effectPt.casterType = wholeData.casterType;
                effectPt.linkEffect = wholeData.linkEffect;
                effectPt.chance = wholeData.chance;

                //Debug.Log(effectPt.id);
                try
                {
                    effectData.Add(effectPt.id, effectPt);
                }
                catch
                {
                    Logger.LogError("effect csv , key: " + effectPt.id);
                }
            }
        }
        #region instance data
        {
            var data = InitTable<InstanceProtoData>("instance");
            foreach (var item in data)
            {
                InstanceData curInstance = new InstanceData();
                curInstance.instanceProtoData = item;

                ArrayList battleArrayList = MiniJsonExtensions.arrayListFromJson(item.battleLevelList);
                for (int i = 0; i < battleArrayList.Count; ++i)
                {
                    curInstance.battleLevelList.Add(battleArrayList[i] as string);
                }

                instanceData.Add(curInstance.instanceProtoData.id, curInstance);
            }
        }
        #endregion
        #region battleLevel data
        {
            var data = InitTable<BattleLevelProtoData>("battleLevel");
            foreach (var item in data)
            {
                BattleLevelData blData = new BattleLevelData();
                blData.battleProtoData = item;
                if (string.IsNullOrEmpty(item.winFunc) == false)
                {
                    var cls = typeof(NormalScript);
                    blData.winFunc = cls.GetMethod(item.winFunc);
                }
                if (string.IsNullOrEmpty(item.loseFunc) == false)
                {
                    var cls = typeof(NormalScript);
                    blData.loseFunc = cls.GetMethod(item.loseFunc);
                }

                Hashtable monsterTable = MiniJsonExtensions.hashtableFromJson(item.monsterList);
                if (monsterTable != null)
                {
                    foreach (DictionaryEntry de in monsterTable)
                    {
                        blData.monsterList.Add(de.Key.ToString(), int.Parse(de.Value.ToString()));
                    }
                }

                battleLevelData.Add(blData.battleProtoData.id, blData);
            }
        }
        #endregion
        {
            var data = InitTable<WeakPointData>("weakPointData");
            foreach (var item in data)
            {
                item.AdaptData();
                weakPointData.Add(item.id, item);
            }
        }

        {
            var data = InitTable<LazyData>("lazy");
            foreach (var item in data)
                lazyData.Add(item.index, item);
        }

        {
            var data = InitTable<CharacterData>("character");
            foreach (var item in data)
                characterData.Add(item.index, item);
        }

        {
            var data = InitTable<Chapter>("instanceChapter");
            foreach (var item in data)
                chapterData.Add(item.chapter, item);
        }

        //{
        //    var data = InitTable<InstanceEntry>("instanceEntry");
        //    foreach (var item in data)
        //    {
        //        item.AdaptData();

        //        instanceEntryData.Add(item.id, item);
        //    }
        //}

        {
            var data = InitTable<ItemStaticData>("item");
            foreach (var item in data)
            {
                itemData.Add(item.id, item);
            }
        }
        {
            #region unitStage
            var data = InitTable<UnitStageData>("unitStage");
            foreach (var item in data)
            {
                item.demandItemList = ItemInfo.getItemInfoList(item.demandItem, ItemParseType.DemandItemType);
                item.demandMonsterList = ItemInfo.getItemInfoList(item.demandMonster, ItemParseType.DemandMonsterType);
                unitStageData.Add(item.stage, item);
                //Logger.Log(item.id + "\t" + item.english);
            }
            #endregion
        }
        {
            #region equip
            var data = InitTable<EquipProtoData>("equipAttr");
            Dictionary<int, EquipProtoData> equipData1 = null;
            foreach (var item in data)
            {
                if (!equipData.TryGetValue(item.id, out equipData1))
                {
                    equipData1 = new Dictionary<int, EquipProtoData>();
                    equipData.Add(item.id, equipData1);
                }
                if (equipData1.ContainsKey(item.stage))
                {
                    Logger.LogError("error: Enhanced level duplication");
                }
                else
                {
                    equipData1.Add(item.stage, item);
                }
            }
            #endregion
        }
        {
            #region equipForge
            var data = InitTable<EquipForgeData>("equipForge");
            foreach (var item in data)
            {
                equipForge.Add(item.stageLevel, item);
            }
            #endregion
        }
        {
            #region baseAttr
            var data = InitTable<EquipLevelData>("baseAttr");
            foreach (var item in data)
            {
                baseAttrData.Add(item.id, item);
            }
            #endregion
        }
        {
            var data = InitTable<FunctionData>("function");
            foreach(var item in data)
            {
                functionDic.Add(item.name, item);

                if(functionLevelDic.ContainsKey(item.needlevel))
                {
                    functionLevelDic[item.needlevel].Add(item);
                }
                else
                {
                    List<FunctionData> listItem = new List<FunctionData>();
                    listItem.Add(item);
                    functionLevelDic.Add(item.needlevel, listItem);
                }
            }
        }
        {
            var data = InitTable<PlayerBehavior>("playerBehavior");
            foreach (var item in data)
                PlayerBehaviorData.Add(item.id, item);
        }
    }

    List<T> InitTable<T>(string filename) where T : new()
    {
        // Deserialization
        List<string> rowData = new List<string>();
        List<List<string>> rows = new List<List<string>>();
        using (var reader = new CsvFileReader(Path.Combine(Util.StaticDataPath, filename + ".csv"), Encoding.UTF8))
        {
            while (reader.ReadRow(rowData))
            {
                rows.Add(rowData);
                rowData = new List<string>();
            }
            var serializer = new CsvSerializer<T>();
            var data = serializer.Deserialize(rows[1], rows.GetRange(3, rows.Count - 3));
            return data;
        }
    }
     
    #region Get Data

    public EquipLevelData GetEquipLevelData(int id)
    {
        EquipLevelData item = null;
        baseAttrData.TryGetValue(id, out item);
        return item;
    }
    public OperationData GetPlayerBehaviorData(string behaviorId)
    {
        if (PlayerBehaviorData.ContainsKey(behaviorId))
        {
            PlayerBehavior behaviorData = PlayerBehaviorData[behaviorId];
            OperationData operationData = new OperationData();
            string[] dazhaoTime = behaviorData.dazhao.Split(';');
            operationData.xgName = new string[dazhaoTime.Length];
            operationData.duijuNum = new int[dazhaoTime.Length];
            operationData.roundNum = new int[dazhaoTime.Length];
            string[] content;
            for (int i = 0; i < dazhaoTime.Length; i++)
            {
                content = dazhaoTime[i].Split('_');
                operationData.xgName[i] = content[0];
                operationData.duijuNum[i] = int.Parse(content[1]);
                operationData.roundNum[i] = int.Parse(content[2]);
            }
            string[] jihuoOperation = behaviorData.jihuo.Split(';');
            operationData.weaknessName = new string[jihuoOperation.Length];
            operationData.IntervalRoundNum = new int[jihuoOperation.Length];
            for (int i = 0; i < jihuoOperation.Length; i++)
            {
                content = jihuoOperation[i].Split('_');
                operationData.weaknessName[i] = content[0];
                operationData.IntervalRoundNum[i] = int.Parse(content[1]);
            }
            BattleToolMain.Instance.roundNum = operationData.IntervalRoundNum[0];
            return operationData;
        }           
        return null;
    }
    public UnitBaseData GetUnitBaseRowData(int level)
    {
        return unitBaseData[level];
    }
    public UnitData GetUnitRowData(string unitID)
    {
        if (unitData.ContainsKey(unitID))
            return unitData[unitID];
        return null;
    }
    public SpellProtoType GetSpellProtoData(string id)
    {
        if (spellData.ContainsKey(id))
            return spellData[id];
        return null;
    }
    public int GetSPellLevelPrice(int nextLevel)
    {
        if (spellUpLevelData.ContainsKey(nextLevel))
            return spellUpLevelData[nextLevel];
       
        return 0;
    }
    public EffectPrototype GetEffectProtoData(string id)
    {
        if (effectData.ContainsKey(id))
            return effectData[id];

        return null;
    }
    public BuffPrototype GetBuffProtoData(string id)
    {
        if (buffData.ContainsKey(id))
            return buffData[id];

        return null;
    }

    public WeakPointData GetWeakPointData(string id)
    {
        if (id == null)
            return null;

        WeakPointData wpData = null;
        weakPointData.TryGetValue(id, out wpData);
        return wpData;
    }

    public InstanceData GetInstanceData(string id)
    {
        InstanceData data;
        if (instanceData.TryGetValue(id, out data))
        {
            return data;
        }

        return null;
    }

    public BattleLevelData GetBattleLevelData(string id)
    {
        BattleLevelData blData;
        if (battleLevelData.TryGetValue(id, out blData))
        {
            return blData;
        }

        return null;
    }

	public LazyData GetLazyData(int index)
	{
		LazyData ldata = null;
		lazyData.TryGetValue (index, out ldata);
		return ldata;
	}

	public CharacterData GetCharacterData(int index)
	{
		CharacterData cData = null;
		characterData.TryGetValue (index, out cData);
		return cData;
	}

	public Chapter GetChapterData(int chapterIndex)
	{
		Chapter chapter = null;
		chapterData.TryGetValue (chapterIndex, out chapter);
		return chapter;
	}

	public ItemStaticData GetItemData(string id)
	{
		ItemStaticData item = null;
		itemData.TryGetValue (id, out item);
		return item;
	}

    public EquipProtoData GetEquipProtoData(string id, int state)
    {
        EquipProtoData item = null;
        Dictionary<int, EquipProtoData> item2 = null;
        if (id != string.Empty)
        {
            equipData.TryGetValue(id, out item2);
        }        
        if (item2 != null && item2.Count > 0)
        {
            item2.TryGetValue(state, out item);            
        }
        return item;
    }
    public EquipForgeData GetEquipForgeData(int stage, int level)
    {
        string forgeId = string.Format("{0}_{1}", stage, level);
        EquipForgeData forge = null;
        equipForge.TryGetValue(forgeId, out forge);
        return forge;
    }

    public UnitStageData getUnitStageData(int stage)
    {
        UnitStageData item = null;
        unitStageData.TryGetValue(stage, out item);
        return item;
    }

    public  FunctionData    GetFunctionStaticData(string name)
    {
        FunctionData item = null;
        functionDic.TryGetValue(name, out item);
        return item;
    }

    public  List<FunctionData> GetFunctionListEqualLevel(int level)
    {
        List<FunctionData> itemlist = null;
        functionLevelDic.TryGetValue(level, out itemlist);
        return itemlist;
    }

    public  List<FunctionData> GetFunctionNoSmallerLevel(int level)
    {
        List<FunctionData> listitem = new List<FunctionData>();

        foreach(var subItem in functionLevelDic)
        {
            if(subItem.Key  >= level)
            {
                listitem.AddRange(subItem.Value);
            }
        }

        return listitem;
    }

    public string GetBundleName(string asset)
    {
        string bundle = "";
        if (string.IsNullOrEmpty(asset)) return bundle;
        assetMapping.TryGetValue(asset,out bundle);
        return bundle;
    }

    public string GetAudioByID(string id)
    {
        string audio = "";
        if (string.IsNullOrEmpty(id)) return audio;
        audioMapping.TryGetValue(id, out audio);
        return audio;
    }


    #endregion
}