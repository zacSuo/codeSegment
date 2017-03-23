using System;
using MSWord = Microsoft.Office.Interop.Word;

namespace Core
{
    public class WordInsert
    {
        MSWord.Application wordApp = new MSWord.Application();

        /// <summary>
        /// 打开指定文件
        /// </summary>
        /// <param name="fileName">绝对路径</param>
        /// <returns></returns>
        public MSWord.Document OpenFile(string fileName)
        {
            object fileTarget = null;
            //Word COM            
            object Nothing = Missing.Value;                       //COM调用时用于占位
            object format = MSWord.WdSaveFormat.wdFormatDocumentDefault; //Word文档的保存格式          

            FileInfo docItem = new FileInfo(fileName);
            if ((docItem.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden || docItem.Extension != ".doc" && docItem.Extension != ".docx")
                return;

            return wordApp.Documents.Open(docItem.FullName);

        }

        /// <summary>
        /// 关闭文档
        /// </summary>
        /// <param name="worddoc"></param>
        public void CloseFile(MSWord.Document worddoc)
        {
            worddoc.Close();
            docItem = null;
        }

        /// <summary>
        /// 保存文档
        /// </summary>
        /// <param name="worddoc"></param>
        /// <param name="fileName"></param>
        public void SaveFile(MSWord.Document worddoc, string fileName)
        {
            worddoc.SaveAs2(fileName);
        }

        /// <summary>
        /// 文档处理
        /// </summary>
        /// <param name="doc">文档内容</param>
        private void HandlerContents(MSWord.Document doc)
        {
            int senTotal = 0, idx = 0, idxTmp = 0;
            MSWord.Range objRange = null;
            string strTmp = string.Empty;
            MSWord.Sentences content = null;

            content = doc.Sentences;
            senTotal = content.Count;
            for (int i = 1; i <= senTotal; i++)
            {
                objRange = content[i].Words.First;
                idxTmp = content[i].Words.Count;
                objRange.InsertAfter("\r");
                idx = 1;
                objRange = content[i].Words[idx];
                content[i].Words[idx].Font.Fill.Transparency = 1F;
                content[i].Words[idx].Font.Spacing = -30;
            }

            doc.Content.ParagraphFormat.AddSpaceBetweenFarEastAndAlpha = 0;
            doc.Content.ParagraphFormat.AddSpaceBetweenFarEastAndDigit = 0;
        }
    }
}