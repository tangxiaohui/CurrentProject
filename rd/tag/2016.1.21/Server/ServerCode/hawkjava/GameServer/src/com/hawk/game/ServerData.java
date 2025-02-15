package com.hawk.game;

import java.util.Calendar;
import java.util.Collections;
import java.util.HashMap;
import java.util.HashSet;
import java.util.List;
import java.util.Map;
import java.util.Set;
import java.util.concurrent.ConcurrentHashMap;
import java.util.concurrent.atomic.AtomicInteger;

import org.hawk.config.HawkConfigManager;
import org.hawk.db.HawkDBManager;
import org.hawk.log.HawkLog;
import org.hawk.os.HawkException;
import org.hawk.os.HawkTime;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import com.hawk.game.config.HoleCfg;

/**
 * 服务器数据
 * 
 * @author hawk
 */
public class ServerData {
	/**
	 * 日志记录器
	 */
	private static final Logger logger = LoggerFactory.getLogger("Server");
	/**
	 * 注册玩家数
	 */
	private AtomicInteger registerPlayer;
	/**
	 * 在线玩家数
	 */
	private AtomicInteger onlinePlayer;
	/**
	 * puid和玩家id的映射表
	 */
	protected ConcurrentHashMap<String, Integer> puidMap;
	/**
	 * 玩家id和puid的映射表
	 */
	protected ConcurrentHashMap<Integer, String> idMap;
	/**
	 * 玩家名和玩家id的映射表
	 */
	protected ConcurrentHashMap<String, Integer> nameMap;
	/**
	 * 玩家id和语言映射表
	 */
	protected ConcurrentHashMap<Integer, String> langMap;
	/**
	 * 在线玩家列表
	 */
	protected ConcurrentHashMap<Integer, Integer> onlineMap;
	/**
	 * 无效设备
	 */
	protected ConcurrentHashMap<String, String> disablePhoneMap;
	/**
	 * 系统刷新时间的映射表
	 * 只有主线程使用
	 */
	protected HashMap<Integer, Calendar> refreshTimeMap;
	/**
	 * 已付费订单列表
	 */
	protected Set<String> rechargeList;
	/**
	 * 洞开启状态的映射表
	 */
	protected ConcurrentHashMap<Integer, Boolean> holeStateMap;
	/**
	 * 上次信息显示时间
	 */
	protected int lastShowTime = 0;

	/**
	 * 全局对象实例
	 */
	private static ServerData instance = null;

	/**
	 * 获取全局实例对象
	 * 
	 * @return
	 */
	public static ServerData getInstance() {
		if (instance == null) {
			instance = new ServerData();
		}
		return instance;
	}

	/**
	 * 构造
	 */
	private ServerData() {
		registerPlayer = new AtomicInteger();
		onlinePlayer = new AtomicInteger();
		puidMap = new ConcurrentHashMap<String, Integer>();
		idMap = new ConcurrentHashMap<Integer, String>();
		nameMap = new ConcurrentHashMap<String, Integer>();
		langMap = new ConcurrentHashMap<Integer, String>();
		onlineMap = new ConcurrentHashMap<Integer, Integer>();
		disablePhoneMap = new ConcurrentHashMap<String, String>();
		refreshTimeMap = new HashMap<Integer, Calendar>();
		rechargeList = Collections.synchronizedSet(new HashSet<String>());
		holeStateMap = new ConcurrentHashMap<Integer, Boolean>();
		lastShowTime = HawkTime.getSeconds();

		// 洞初始状态为关闭，启动后refresh
		Map<Object, HoleCfg> holeCfgMap = HawkConfigManager.getInstance().getConfigMap(HoleCfg.class);
		for (HoleCfg hole : holeCfgMap.values()) {
			holeStateMap.put(hole.getId(), false);
		}
	}

	/**
	 * 初始化服务器数据
	 */
	public boolean init() {
		// 从db拉取玩家个数
		try {
			HawkLog.logPrintln("load player count from db......");
			long count = HawkDBManager.getInstance().count("select count(*) from PlayerEntity");
			registerPlayer.set((int) count);
		} catch (Exception e) {
			HawkException.catchException(e);
			return false;
		}

		// 从db拉取玩家puid和id的映射表
		try {
			HawkLog.logPrintln("load puid and playerId from db......");
			List<Object> rowInfos = HawkDBManager.getInstance().executeQuery("select puid, id from player");
			for (Object rowInfo : rowInfos) {
				Object[] colInfos = (Object[]) rowInfo;
				addPuidAndPlayerId((String) colInfos[0], (Integer) colInfos[1]);
			}
		} catch (Exception e) {
			HawkException.catchException(e);
			return false;
		}

		//
		try {
			HawkLog.logPrintln("load orderSerial from db......");
			List<Object> rowInfos = HawkDBManager.getInstance().executeQuery("select orderSerial from recharge");
			for (Object rowInfo : rowInfos) {
				addOrderSerial((String)rowInfo);
			}
		} catch (Exception e) {
			HawkException.catchException(e);
			return false;
		}

		// 从db拉取玩家name和id的映射表
		try {
			HawkLog.logPrintln("load nickname and playerId from db......");
			List<Object> rowInfos = HawkDBManager.getInstance().executeQuery("select nickname, id from player");
			for (Object rowInfo : rowInfos) {
				Object[] colInfos = (Object[]) rowInfo;
				addNameAndPlayerId((String) colInfos[0], (Integer) colInfos[1]);
			}
		} catch (Exception e) {
			HawkException.catchException(e);
			return false;
		}

		// 从db拉取玩家id和language的映射表
		try {
			HawkLog.logPrintln("load playerId and language from db......");
			List<Object> rowInfos = HawkDBManager.getInstance().executeQuery("select id, language from player");
			for (Object rowInfo : rowInfos) {
				Object[] colInfos = (Object[]) rowInfo;
				addPlayerIdAndLang((Integer) colInfos[0], (String) colInfos[1]);
			}
		} catch (Exception e) {
			HawkException.catchException(e);
			return false;
		}

		return true;
	}

	/**
	 * 增加注册玩家数
	 */
	public int addRegisterPlayer() {
		return registerPlayer.addAndGet(1);
	}

	/**
	 * 获取注册玩家数
	 */
	public int getRegisterPlayer() {
		return registerPlayer.get();
	}

	/**
	 * 增加在线玩家数
	 */
	public int addOnlinePlayer() {
		return onlinePlayer.addAndGet(1);
	}

	/**
	 * 获取在线玩家数
	 */
	public int getOnlinePlayer() {
		return onlinePlayer.get();
	}

	/**
	 * 通过puid获取玩家id
	 */
	public int getPlayerIdByPuid(String puid) {
		Integer id = puidMap.get(puid);
		if (id != null) {
			return id;
		}
		return 0;
	}

	/**
	 * 通过玩家id获取puid
	 */
	public String getPuidByPlayerId(int playerId) {
		return idMap.get(playerId);
	}

	/**
	 * 增加puid和玩家id的映射
	 */
	public void addPuidAndPlayerId(String puid, int playerId) {
		puidMap.put(puid, playerId);
		idMap.put(playerId, puid);
	}

	/**
	 * 增加name和玩家id的映射
	 */
	public void addNameAndPlayerId(String name, int playerId) {
		nameMap.put(name.toLowerCase(), playerId);
	}

	/**
	 * 增加name和玩家id的映射
	 */
	public void addPlayerIdAndLang(int playerId, String lang) {
		langMap.put(playerId, lang);
	}

	/**
	 * 是否存在玩家
	 */
	public boolean isExistPlayer(int playerId) {
		return idMap.containsKey(playerId);
	}

	/**
	 * 获取所有玩家Id
	 */
	public Set<Integer> getAllPlayerIdSet() {
		return idMap.keySet();
	}

	/**
	 * 获取玩家语言
	 */
	public String getPlayerLang(int playerId) {
		return langMap.get(playerId);
	}

	/**
	 * 增加order
	 */
	public void addOrderSerial(String orderSerial) {
		rechargeList.add(orderSerial);
	}

	/**
	 * 是否order
	 */
	public boolean isExistOrder(String orderSerial) {
		return rechargeList.contains(orderSerial);
	}

	/**
	 * 是否存在名字
	 */
	public boolean isExistName(String name) {
		return nameMap.containsKey(name.toLowerCase());
	}

	/**
	 * 添加在线id
	 */
	public void addOnlinePlayerId(int playerId) {
		onlineMap.put(playerId, playerId);
	}

	/**
	 * 移除在线id
	 */
	public void removeOnlinePlayerId(int playerId) {
		try {
			onlineMap.remove(playerId);
		} catch (Exception e) {
			HawkException.catchException(e);
		}
	}

	/**
	 * 玩家在线判断
	 */
	public boolean isPlayerOnline(int playerId) {
		return onlineMap.containsKey(playerId);
	}

	/**
	 * 添加禁用设备
	 */
	public void addDisablePhone(String phoneInfo) {
		disablePhoneMap.put(phoneInfo, phoneInfo);
	}

	/**
	 * 是否为禁用设备
	 */
	public boolean isDisablePhone(String phoneInfo) {
		return disablePhoneMap.containsKey(phoneInfo);
	}

	/**
	 * 清空无效设备信息
	 */
	public void clearDisablePhone() {
		disablePhoneMap.clear();
	}

	public Calendar getLastRefreshTime(int timeCfgId) {
		return refreshTimeMap.get(timeCfgId);
	}

	public void setRefreshTime(int timeCfgId, Calendar time) {
		this.refreshTimeMap.put(timeCfgId, time);
	}

	public Map<Integer, Boolean> getHoleStateMap() {
		return Collections.unmodifiableMap(holeStateMap);
	}

	public boolean isHoleOpen(int holeId) {
		return holeStateMap.get(holeId);
	}

	public void setHoleOpen(int holeId, boolean isOpen) {
		this.holeStateMap.put(holeId, isOpen);
	}

	/**
	 * 打印服务器状态信息
	 */
	public void showServerInfo() {
		// 每分钟显示一个服务器信息
		if (HawkTime.getSeconds() - lastShowTime >= 60) {
			lastShowTime = HawkTime.getSeconds();
			// 记录信息
			logger.info("online user: {}", onlineMap.size());
		}
	}
}
