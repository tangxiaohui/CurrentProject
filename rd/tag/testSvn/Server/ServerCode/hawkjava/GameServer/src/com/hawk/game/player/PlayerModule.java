package com.hawk.game.player;

import java.util.List;

import org.hawk.app.HawkObjModule;
import org.hawk.msg.HawkMsg;
import org.hawk.net.protocol.HawkProtocol;

import com.google.protobuf.ProtocolMessageEnum;

/**
 * 玩家模块基类
 * 
 * @author hawk
 */
public class PlayerModule extends HawkObjModule {
	/**
	 * 玩家对象
	 */
	protected Player player = null;

	/**
	 * 构造函数
	 * 
	 * @param player
	 */
	public PlayerModule(Player player) {
		super(player);
		this.player = player;
	}

	/**
	 * 玩家上线处理
	 * 
	 * @return
	 */
	protected boolean onPlayerReconnect(HawkMsg msg) {
		return true;
	}

	/**
	 * 玩家组装完成, 主要用来后期数据同步
	 * 
	 * @return
	 */
	protected boolean onPlayerAssemble() {
		return true;
	}
	
	/**
	 * 玩家重新连接
	 * 
	 * @return
	 */
	protected boolean onPlayerLogin() {
		return true;
	}
	
	/**
	 * 玩家下线处理
	 * 
	 * @return
	 */
	protected boolean onPlayerLogout() {
		return true;
	}

	/**
	 * 玩家数据刷新
	 * 
	 * @return
	 */
	public boolean onPlayerRefresh(List<Integer> refreshIndexList, boolean onLogin) {
		return true;
	}

	/**
	 * 发送协议
	 * 
	 * @param protocol
	 * @return
	 */
	public boolean sendProtocol(HawkProtocol protocol) {
		if (player != null) {
			return player.sendProtocol(protocol);
		}
		return false;
	}

	/**
	 * 通知错误码
	 * 
	 * @param errCode
	 */
	public void sendError(int hsCode, int errCode) {
		player.sendError(hsCode, errCode);
	}

	/**
	 * 通知错误码
	 * 
	 * @param errCode
	 */
	public void sendError(int hsCode, int errCode, int errFlag) {
		player.sendError(hsCode, errCode, errFlag);
	}

	/**
	 * 通知错误码
	 * 
	 * @param errCode
	 */
	public void sendError(int hsCode, ProtocolMessageEnum errCode) {
		player.sendError(hsCode, errCode.getNumber());
	}
}
