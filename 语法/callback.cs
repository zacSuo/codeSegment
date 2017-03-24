#region 父类
/// <summary>
/// 回调函数定义
/// </summary>
/// <param name="newCursor">参数1</param>
/// <param name="clickedCompo">参数2</param>
private void ChangeCursor(Cursor newCursor, string clickedCompo)
{
    if (this.InvokeRequired)
    {
        Action<Cursor, string> delegateChangeCursor = new Action<Cursor, string>(ChangeCursor);
        this.Invoke(delegateChangeCursor, new object[] { newCursor,clickedCompo });
        return;
    } 
            
    this.Cursor = newCursor;
    this.currentSelectedComponent = clickedCompo;
}

/// <summary>
/// 调用
/// </summary>
UcComponent compo = new UcComponent(item, ChangeCursor);
#endregion

#region 子类

private ElementInfo componentInfo = null;
private Action<Cursor, string> UpdateFatherCursor = null;

/// <summary>
/// 参数传入
/// </summary>
/// <param name="compoInfo"></param>
/// <param name="delegateCursor"></param>
public UcComponent(ElementInfo compoInfo, Action<Cursor, string> delegateCursor)
{
    this.componentInfo = compoInfo;
    this.UpdateFatherCursor = delegateCursor;

}


private void sthHandler(){
    //sth
    this.UpdateFatherCursor(this.Cursor, this.componentType);
}
#endregion