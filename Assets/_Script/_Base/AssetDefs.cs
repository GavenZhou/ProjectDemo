
using System.ComponentModel;


///////////////////////////////////////////////////////////////////////////////
// motion ID 
///////////////////////////////////////////////////////////////////////////////

// use format string: "{0:D3}" to string.
public enum MotionID {

    // 休闲待机	001
    MOTION_IDLE = 1,
    // 跑步	011
    MOTION_RUN = 11,
    // 走路	021
    MOTION_WALK = 21,
    // 翻滚	031
    MOTION_ROLL = 31,
    // 出生 041
    MOTION_BORN = 41,

    // 
    // 普通攻击1	101
    MOTION_ATTACK1 = 101,
    // 普通攻击2	102
    MOTION_ATTACK2 = 102,
    // 普通攻击3	103
    MOTION_ATTACK3 = 103,
    // 普通攻击4	104
    MOTION_ATTACK4 = 104,

    // 
    // 受击站立	501
    MOTION_HURT = 501,
    // 倒地	502
    MOTION_KNOCKDOWN = 502,
    // 击飞 503
    MOTION_KNOCKOFF = 503,
    // 死亡	504
    MOTION_DIE = 504,

    //
    // 更换武器	601
    MOTION_SWITCHWEAPON = 601,

    // 	
    // 减速	701
    MOTION_SLOWLY = 701,
    // 定身	702
    MOTION_IMMOBILIZED = 702,
    // 眩晕	703
    MOTION_DIZZY = 703,
    // 沉默	704
    MOTION_SILENT = 704,
    // 睡眠	705
    MOTION_SLEEP = 705,

    // 	
    // 跳跃攻击	1001
    SLUMP_ATTACK = 1001,
    // 蓄力攻击	1002
    POWER_ATTACK = 1002,
    // 抓取突进	1101
    CATCH_RUSH = 1101,
    // 抓取攻击	1102
    CATCH_ATTACK = 1102,
    // 抓取松手	1103
    CATCH_RELEASE = 1103,

    //
    // 技能释放准备1	2001
    SKILL_CAST1 = 2001,
    // 技能释放准备2	2002
    SKILL_CAST2 = 2002,

    // 	
    // 技能攻击1	2101
    SKILL_ATTACK1 = 2101,
    // 技能攻击2	2102
    SKILL_ATTACK2 = 2102,
    // 技能攻击3	2103
    SKILL_ATTACK3 = 2103,
    // 技能攻击4	2104
    SKILL_ATTACK4 = 2104,
}


public enum WeaponType {

    // 玄铁重剑
    DUELLING_SWORD = 1,
    // 七杀刀
    THE_BLADE = 2,
    // 唐门弩炮
    BALLISTA = 3,
    // 霸王枪
    SPEAR = 4,
}


