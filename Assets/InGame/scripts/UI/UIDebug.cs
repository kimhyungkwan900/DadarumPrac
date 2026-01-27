using Gpm.Ui;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class UIDebug : MonoBehaviour
{
    public InfiniteScroll itemInvList = null;
    public Text logText = null;
    public Text dataCount = null;
    public ScrollRect logScrollRect = null;
    public Dropdown moveDataSelect = null;
    public Dropdown moveDataTypeSelect = null;
    public InputField moveDataTime = null;
    public Dropdown moveDataCurveTypeSelect = null;
    public InputField moveItemSelect = null;
    public Text itemCount = null;

    private int index = 0;
    private int insertCount = 0;
    private int removceCount = 0;

    private List<TestInvData> dataList = new List<TestInvData>();
    private StringBuilder log = new StringBuilder();
    
    void Start(){
        itemInvList.AddSelectCallback((data) => {
            AddLog(string.Format("Inv Select data: {0}", ((TestInvData)data).index.ToString()));
        });

        moveDataSelect.onValueChanged.AddListener((option) =>
        {
            MoveToDataIndex(option);
        });

        moveItemSelect.onValueChanged.AddListener((text) =>
        {
            int index = 0;
            if (int.TryParse(text, out index) == false)
            {
                AddLog("Time is not Number");
            }

            MoveToItemIndex(index);
        });
    }
    
    public void InsertData(){
        TestInvData data = new TestInvData();
        data.index = index++;
        dataList.Add(data);

        itemInvList.InsertData(data);

        var options = new List<Dropdown.OptionData>() {new Dropdown.OptionData(data.index.ToString())};
        moveDataSelect.AddOptions(options);

        ++insertCount;
        UpdateDataCount();

        AddLog(string.Format("Insert Data : {0}", index-1));

    }

    private void MoveToItemIndex(int itemIndex){
        float time = 0;
        if(float.TryParse(moveDataTime.text, out time) == false){
            AddLog("Time is not Number");
        }

        var curveType = (InfiniteScroll.CurveType)moveDataCurveTypeSelect.value;

        itemInvList.MoveTo(itemIndex, (InfiniteScroll.MoveToType)moveDataTypeSelect.value, new InfiniteScroll.CurveFromType(curveType, time));

        AddLog(string.Format("Move to Item Index : {0}", itemIndex));
    }

    private void MoveToDataIndex(int dataIndex){
        float time = 0;
        if(float.TryParse(moveDataTime.text, out time) == false){
            AddLog("Time is not Number");
        }

        var curveType = (InfiniteScroll.CurveType)moveDataCurveTypeSelect.value;
        if(UnityEngine.Random.Range(0,2) == 0){
            itemInvList.MoveToFromDataIndex(dataIndex, (InfiniteScroll.MoveToType)moveDataTypeSelect.value, new InfiniteScroll.CurveFromType(curveType, time));
        }
        else{
            TestInvData data = dataList[dataIndex];

            itemInvList.MoveTo(data, (InfiniteScroll.MoveToType)moveDataTypeSelect.value, new InfiniteScroll.CurveFromType(curveType, time));
        }

        AddLog(string.Format("Move to Data Index : {0}", dataIndex));
    }

    private void UpdateDataCount(){
        dataCount.text = string.Format("Data Count : {0} (Insert: {1}, Remove: {2})", dataList.Count, insertCount, removceCount);
    }

    private void AddLog(string text){
        log.AppendLine(text);
        logText.text = log.ToString();
        logScrollRect.verticalNormalizedPosition = 0.0f;
    }
}
