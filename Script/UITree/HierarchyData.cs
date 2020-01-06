/*
 * 生成层级对象时的数据
 */

using UnityEngine;

public class HierarchyData
{
    public string CurID; // 当前自身ID
    public int childDeep; // 数据所属的层级ID
    public string CurName;/// 当前自身的名字
    public string ParentID; // 数据所属游戏对象
}
