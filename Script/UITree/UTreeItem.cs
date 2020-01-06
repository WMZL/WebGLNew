using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UTreeItem : MonoBehaviour
{
    public delegate void ItemClick(UTreeData treeData);

    /// <summary>
    /// 图标
    /// </summary>
    public Image icon;

    /// <summary>
    /// 标题
    /// </summary>
    public Text text;

    /// <summary>
    /// 节点数据
    /// </summary>
    [HideInInspector]
    public UTreeData treeData;
    /// <summary>
    /// 当前层级
    /// </summary>
    private int level;

    /// <summary>
    /// 节点点击回调函数
    /// </summary>
    private UTreeItem.ItemClick itemClick;

    void Awake()
    {
        this.InitAwakeItem();
    }

    /// <summary>
    /// 树节点初始化
    /// </summary>
    protected virtual void InitAwakeItem()
    {
        icon.GetComponent<Button>().onClick.AddListener(() =>
        {
            if (this.itemClick != null)
            {
                this.itemClick(this.treeData);
            }
            if (this.treeData.expand)
            {
                icon.rectTransform.localRotation = Quaternion.Euler(0, 0, -90);
            }
            else
            {
                icon.rectTransform.localRotation = Quaternion.Euler(0, 0, 0);
            }
        });

        text.GetComponent<Button>().onClick.AddListener(() =>
        {
            Debug.Log("点击信息：" + this.treeData.name + " " + this.treeData.id);

            if (ClickItemManager.Instance.AllClickItem.ContainsKey(treeData.id))
            {
                ClickItemInfo clickite = ClickItemManager.Instance.AllClickItem[treeData.id];
                ///这里的方法已经不会被调用了，Unity中不存在UI树
                //clickite.MoveTo();
            }
            else
            {
                Debug.LogError("未配置");
            }
        });
    }

    /// <summary>
    /// 设置数据
    /// </summary>
    /// <param name="treeData">Tree data.</param>
    /// <param name="itemClick">Item click.</param>
    public void ChangeData(UTreeData treeData, UTreeItem.ItemClick itemClick)
    {
        this.itemClick = itemClick;
        this.treeData = treeData;
        this.text.text = treeData.name;

        level = treeData.level;
        this.TreeItemRender();
    }

    /// <summary>
    /// 树节点渲染，主要是设置位置偏移
    /// </summary>
    protected virtual void TreeItemRender()
    {
        float iconSizeX = icon.GetComponent<RectTransform>().sizeDelta.x;

        float offsetX = this.treeData.offset * this.treeData.level;

        this.icon.transform.localPosition = new Vector3(offsetX, this.icon.transform.localPosition.y, 0f);
        this.text.transform.localPosition = new Vector3(offsetX + iconSizeX * 2 + 40.0f, this.text.transform.localPosition.y, 0f);
    }
}
