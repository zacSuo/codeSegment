#region ����
/// <summary>
/// �ص���������
/// </summary>
/// <param name="newCursor">����1</param>
/// <param name="clickedCompo">����2</param>
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
/// ����
/// </summary>
UcComponent compo = new UcComponent(item, ChangeCursor);
#endregion

#region ����

private ElementInfo componentInfo = null;
private Action<Cursor, string> UpdateFatherCursor = null;

/// <summary>
/// ��������
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