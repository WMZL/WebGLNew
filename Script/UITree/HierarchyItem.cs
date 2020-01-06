using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

/// <summary>
/// 对象资源树数据
/// </summary>
public class HierarchyItem : MonoBehaviour
{
    public GameObject unfoladButton;
    //public UISprite mondleMarker;
    //public UILabel hierarchyText;
    public GameObject hierarchyButton;
    public GameObject checkIdentical;
    public GameObject selectionMarker;

    /// <summary>
    /// 纵向间距
    /// </summary>
    public int MarkerSpace { get { return 30; } }

    #region Variables
    /// <summary>
    /// 层级对象名字
    /// </summary>
    private string _nameID;
    public string NameID
    {
        get { return _nameID; }
        set { _nameID = value; }
    }

    /// <summary>
    /// 层级对象对应游戏对象
    /// </summary>
    private GameObject _gameObjID;
    public GameObject GameObjID
    {
        get { return _gameObjID; }
        set { _gameObjID = value; }
    }

    /// <summary>
    /// 当前元素在资源树中所属的层级
    /// </summary>
    public int _childDeep = 0;
    public int ChildDeep
    {
        get { return _childDeep; }
        set { _childDeep = value; }
    }

    /// <summary>
    /// 当前元素在资源树中所属的层级
    /// </summary>
    public int _hierarchy = 0;
    public int Hierarchy
    {
        get { return _hierarchy; }
        set { _hierarchy = value; }
    }

    /// <summary>
    /// 当前元素在树形图中指向的父元素
    /// </summary>
    private HierarchyItem _hierarchyParent;
    public HierarchyItem HierarchyParent
    {
        get { return _hierarchyParent; }
        set { _hierarchyParent = value; }
    }
    
    /// <summary>
    /// 当前元素的子元素是否展开（展开时可见）
    /// </summary>
    private bool isExpanding = true;
    public bool IsExpanding
    {
        get { return isExpanding; }
        set { isExpanding = value; }
    }

    /// <summary>
    /// 当前元素下一层级的所有子元素
    /// </summary>
    public List<HierarchyItem> _treeChildren = new List<HierarchyItem>();
    #endregion                          

    #region 属性访问
   
    /// <summary>
    /// 获取在子物体中的位置
    /// </summary>
    public int GetChildIndex()
    {
        if (_hierarchyParent)
            return _hierarchyParent._treeChildren.IndexOf(this);
        return -1;
    }

    /// <summary>
    ///  给资源树添加子物体
    /// </summary>
    public void AddChildren(HierarchyItem children)
    {
        if (_treeChildren == null)
            _treeChildren = new List<HierarchyItem>();

        _treeChildren.Add(children);
    }

    /// <summary>
    ///  给资源树插入子物体
    /// </summary>
    public void InsertChildren(HierarchyItem indexOfItem, HierarchyItem insertItem)
    {
        if (_treeChildren == null)
        {
            _treeChildren = new List<HierarchyItem>();
            _treeChildren.Add(insertItem);
            return;
        }
        _treeChildren.Insert(_treeChildren.IndexOf(indexOfItem), insertItem);
    }

    /// <summary>
    ///  给资源树插入子物体
    /// </summary>
    public void InsertChildren(int index, HierarchyItem insertItem)
    {
        if (_treeChildren == null)
        {
            _treeChildren = new List<HierarchyItem>();
            _treeChildren.Add(insertItem);
            return;
        }
        if (index <= _treeChildren.Count)
            _treeChildren.Insert(index, insertItem);
        else
            _treeChildren.Add(insertItem);
    }

    /// <summary>
    /// 移除资源树子物体
    /// </summary>
    public void RemoveChildren(HierarchyItem children)
    {
        if (_treeChildren == null)
        {
            return;
        }
        _treeChildren.Remove(children);
    }

    /// <summary>
    ///  移除资源树子物体
    /// </summary>
    public void RemoveChildren(int index)
    {
        if (_treeChildren == null || index < 0 || index >= _treeChildren.Count)
        {
            return;
        }
        _treeChildren.RemoveAt(index);
    }

    /// <summary>
    /// 获取资源树子物体个数
    /// </summary>
    public int GetChildrenCount()
    {
        return _treeChildren.Count;
    }

    /// <summary>
    /// 获取资源树子物体
    /// </summary>
    public HierarchyItem GetChildrenByIndex(int index)
    {
        if (index >= _treeChildren.Count)
        {
            return null;
        }
        return _treeChildren[index];
    }

    /// <summary>
    /// 获取下一层级所有子物体
    /// </summary>
    /// <param name="isIncludeDelete"> 是否包括已经删除的 </param>
    /// <returns></returns>
    public List<HierarchyItem> GetAllChildItem(bool isIncludeDelete)
    {
        if (!isIncludeDelete)
        {
            List<HierarchyItem> tempList = new List<HierarchyItem>();

            foreach (HierarchyItem tvi in _treeChildren)
            {
                tempList.Add(tvi);
            }
            return tempList;
        }
        else
        {
            return _treeChildren;
        }
    }

    private List<HierarchyItem> allDethChildItem = new List<HierarchyItem>();
    /// <summary>
    /// 获取资源树所有深度的子物体
    /// </summary>
    /// <param name="isIncludeDelete"> 是否包括已经删除的 </param>
    /// <returns></returns>
    public List<HierarchyItem> GetAllDethChildItem(bool isIncludeDelete)
    {
        allDethChildItem.Clear();

        if (isIncludeDelete)
        {
            foreach (HierarchyItem tvi in _treeChildren)
            {
                allDethChildItem.Add(tvi);

                if (tvi.GetChildrenCount() > 0) AddChildItem(tvi, true);
            }
        }
        else
        {
            foreach (HierarchyItem tvi in _treeChildren)
            {
                allDethChildItem.Add(tvi);

                if (tvi.GetChildrenCount() > 0) AddChildItem(tvi, false);
            }
        }
      
        return allDethChildItem;
    }

    private void AddChildItem(HierarchyItem item, bool isIncludeDelete)
    {
        if (isIncludeDelete)
        {
            foreach (HierarchyItem tvi in item.GetAllChildItem(true))
            {
                allDethChildItem.Add(tvi);

                if (tvi.GetChildrenCount() > 0) AddChildItem(tvi, true); 
            }
        }
        else
        {
            foreach (HierarchyItem tvi in item.GetAllChildItem(false))
            {
                allDethChildItem.Add(tvi);

                if (tvi.GetChildrenCount() > 0) AddChildItem(tvi, false);
            }
        }
    }
    #endregion
}
