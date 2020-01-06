using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class InterfaceInfo
{
    public const string InterfaceURL = "http://114.116.136.183:8888/appSearch/";
    //存储返回码
    private static Dictionary<HttpType, ErrorMsg> BackInfo;

    static InterfaceInfo()
    {
        InitData();
    }
    public static void InitData()
    {
        BackInfo = new Dictionary<HttpType, ErrorMsg>();
        BackInfo.Add(HttpType.queryUserBymobile, ErrorMsg.SUCCESS_CODE);
        BackInfo.Add(HttpType.insertMessageCode, ErrorMsg.FAILED_CODE);
        BackInfo.Add(HttpType.insertUser, ErrorMsg.SUCCESS_CODE);
        BackInfo.Add(HttpType.selectCheckCode, ErrorMsg.SUCCESS_CODE);
        BackInfo.Add(HttpType.updateUser, ErrorMsg.SUCCESS_CODE);
        BackInfo.Add(HttpType.selectPcaseList, ErrorMsg.SUCCESS_CODE);
    }
    /// <summary>
    /// 匹配返回码
    /// </summary>
    /// <param name="type"></param>
    /// <param name="InfoIndex"></param>
    /// <returns></returns>
    public static bool MsgCheck(HttpType type, string InfoIndex)
    {
        int index = int.Parse(InfoIndex);
        return (int)BackInfo[type] == index;
    }

    public enum HttpType
    {
        //用户登录
        queryUserBymobile,
        //获取场景信息
        selectDescription,
        //根据场景ID获取问题列表
        queryQuestionListBySceneId,
        //上传成绩
        savePracticeScore,
        //获取验证码
        insertMessageCode,
        //发送注册信息
        insertUser,
        //验证短信
        selectCheckCode,
        //修改密码
        updateUser,
        //修改用户信息
        updateUserInfo,
        //获取用户信息
        selectUserInfo,
        //获取案例信息
        selectPcaseList,
    }
    public enum ErrorMsg
    {
        //成功
        SUCCESS_CODE = 0,

        //用户不存在
        NO_USER_CODE = 1,

        //缺少参数
        LACK_PARAM_CODE = 2,

        //流程不存在
        LACK_FLOW_CODE = 3,

        //数据格式错误
        PARAM_PATTERN_CODE = 4,

        //缺少符合条件数据
        NO_DATA_CODE = 5,

        //验证码稍后就到
        FAILED_CODE = 6,

    }
}


