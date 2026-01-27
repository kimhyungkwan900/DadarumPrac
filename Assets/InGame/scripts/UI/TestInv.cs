using Gpm.Ui;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
public class TestInvData : InfiniteScrollData
{
    public int index = 0;
    public string description = string.Empty;
}
public class TestInv : InfiniteScrollItem
{
    public Text text = null;
    public bool isVertical = true;

    public override void UpdateData(InfiniteScrollData scrollData)
    {
        base.UpdateData(scrollData);

        TestInvData invData = (TestInvData) scrollData;
        StringBuilder sb = new StringBuilder();
        sb.Append(string.Format("Item : {0} ", invData.index));

        if (invData.index % 2 == 0)
        {
            sb.Append("(Even) ");
        }
        else
        {
            sb.Append("(Odd) ");
        }

        sb.Append(invData.description);
        text.text = sb.ToString();
    }

    public void OnClick()
    {
        OnSelect();
    }

    public void OnChangeSizeClick()
    {
        float size = Random.Range(30, 400);

        SetSize(size);
    }
}