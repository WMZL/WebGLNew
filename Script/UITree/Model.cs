using System.Collections;
using System.Collections.Generic;

public class BllTreeNodeInfo
{
    public string NodeName { get; set; }

    private List<BllTreeNodeInfo> children = new List<BllTreeNodeInfo>();
    public List<BllTreeNodeInfo> Children { get { return children; } }

    public string TreeID { get; set; }
    public string TreeParentID { get; set; }
}
