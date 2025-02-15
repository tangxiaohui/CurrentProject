package com.hawk.game.util;

import java.util.ArrayList;
import java.util.Collections;
import java.util.Comparator;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import com.hawk.game.config.InstanceEntryCfg;
import com.hawk.game.protocol.Const;

public class InstanceUtil {

	public static class InstanceChapter {
		public int chapterId;
		public List<InstanceEntryCfg> normalList = new ArrayList<>();
		public List<InstanceEntryCfg> hardList = new ArrayList<>();
	}

	/**
	 * @key chapterId
	 */
	private static Map<Integer, InstanceChapter> chapterMap = new HashMap<>();

	/**
	 * @key instanceId
	 */
	private static Map<String, InstanceEntryCfg> storyInstanceMap = new HashMap<>();

	// 构造阶段---------------------------------------------------------------------

	/**
	 * 添加副本
	 */
	public static void addInstance(InstanceEntryCfg entryCfg) {
		// 非故事副本不记录
		if (entryCfg.getType() != Const.InstanceType.INSTANCE_STORY_VALUE) {
			return;
		}

		storyInstanceMap.put(entryCfg.getInstanceId(), entryCfg);

		InstanceChapter chapter = chapterMap.get(entryCfg.getChapter());
		if (chapter == null) {
			chapter = new InstanceChapter();
			chapter.chapterId = (entryCfg.getChapter());
			chapterMap.put(chapter.chapterId, chapter);
		}

		if (entryCfg.getDifficult() == GsConst.Instance.NORMAL) {
			chapter.normalList.add(entryCfg);
		} else if (entryCfg.getDifficult() == GsConst.Instance.HARD) {
			chapter.hardList.add(entryCfg);
		}

		Collections.sort(chapter.normalList, new Comparator<InstanceEntryCfg>() {
			@Override
			public int compare(InstanceEntryCfg o1, InstanceEntryCfg o2) {
				return Integer.valueOf(o1.getIndex()).compareTo(Integer.valueOf(o2.getIndex()));
			}
		});
	}

	// 使用阶段----------------------------------------------------------------------

	public static Map<Integer, InstanceChapter> getInstanceChapterMap() {
		return Collections.unmodifiableMap(chapterMap);
	}

	public static InstanceChapter getInstanceChapter(int chapterId) {
		return chapterMap.get(chapterId);
	}

	public static Map<String, InstanceEntryCfg> getStoryInstanceMap() {
		return Collections.unmodifiableMap(storyInstanceMap);
	}

}