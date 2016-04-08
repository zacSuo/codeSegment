using System;
using System.IO;
//需要先引用ICSharpCode.SharpZipLib.DLL或ICSharpCode.SharpZipLib项目文件
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Checksums;

namespace ALDI
{
    /// <summary>
    /// 上传下载FTP文件
    /// copyright:  Zac (suoxd123@126.com)
    /// </summary>
    public class ZIP
    {

        /// <summary>
        /// 使用示例说明
        /// </summary>
        private void UserCase()
        {
            //Zip1();
            //Zip2();
            //Zip();
            //UnZip();  
            //UnZip1();                     
        }

        #region 快速实现方法

        /// <summary>
        /// 压缩单个文件
        /// </summary>
        public void Zip()
        {
            ZipFile zipFile = ZipFile.Create("E:\\测试.ZIP");
            zipFile.BeginUpdate();
            zipFile.Add("E:\\file1.xlsx");
            zipFile.Add("E:\\file2.txt");
            zipFile.CommitUpdate();
        }

        /// <summary>
        /// 压缩文件夹
        /// </summary>
        /// <param name="filterFile">排除文件</param>
        public void ZipFolder(string filterFile)
        {
            FastZip zipFile = new FastZip();
            zipFile.CreateZip("E:\\测试.ZIP", "E:\\test\\", true, filterFile);

        }

        /// <summary>
        /// 解压文件到指定目录
        /// </summary>
        public void UnZip()
        {
            FastZip zipFile = new FastZip();
            zipFile.ExtractZip("E:\\测试.ZIP", "E:\\test\\", "");
        }
        #endregion 

        #region 

        /// <summary>
        /// 自己操作文件流的压缩方法
        /// </summary>
        public void Zip1()
        {
            //写入的单个块大小
            int size = 512;

            //生成压缩文件
            FileStream fs = File.Create("E:\\test1.ZIP");
            ZipOutputStream zipStream = new ZipOutputStream(fs);

            string[] fileList = { "E:\\file1.xlsx", "E:\\file2.txt" };
            foreach (string fileName in fileList)
            {
                //依次将待压缩加入缓存
                FileStream source = File.OpenRead(fileName);
                ZipEntry zip = new ZipEntry(source.Name);
                zip.DateTime = DateTime.Now;
                zipStream.PutNextEntry(zip);

                byte[] buffer = new byte[size];
                int readIdx = 0, tmpRead;
                while (readIdx < source.Length)
                {//将单个文件按块加入压缩文件缓存
                    tmpRead = source.Read(buffer, 0, size);
                    zipStream.Write(buffer, 0, tmpRead);
                    readIdx += tmpRead;
                }
                source.Close();
            }
            zipStream.Finish();
            zipStream.Close();
        }

        /// <summary>
        /// 自己操作文件流的解压方法
        /// </summary>
        public void UnZip1()
        {
            //读取的单个流大小（字节）
            int size = 512;

            //创建解压目录
            string folder = "E:\\test\\";
            Directory.CreateDirectory(folder);

            //读取待解压文件
            FileStream fs = File.OpenRead("E:\\test1.ZIP");
            ZipInputStream zipStream = new ZipInputStream(fs);

            ZipEntry zip = null;
            while ((zip = zipStream.GetNextEntry()) != null)
            {//依次解压文件
                string fileName = Path.GetFileName(zip.Name);
                if (fileName.Equals(string.Empty))
                {//对于目录则在本地创建目录
                    Directory.CreateDirectory(folder + zip.Name);
                    continue;
                }
                //对于文件则生成文件流
                FileStream fsFile = File.Create(folder + zip.Name);

                //将解压文件流按块写入文件
                byte[] buffer = new byte[size];
                int readIdx = 0, tmpRead;
                while (readIdx < zip.Size)
                {
                    tmpRead = zipStream.Read(buffer, 0, size);
                    fsFile.Write(buffer, 0, tmpRead);
                    readIdx += tmpRead;
                }
                fsFile.Close();
            }
            zipStream.Close();
        }

        /// <summary>
        /// 自己操作文件流，同时增加参数设置的方法
        /// </summary>
        public void Zip2()
        {
            FileStream fs = File.Create("E:\\test2.ZIP");
            ZipOutputStream zipStream = new ZipOutputStream(fs);
            zipStream.SetLevel(9);//压缩比例（0最低-9最高）
            zipStream.Password = "passwordTest";//解压密码
            zipStream.UseZip64 = UseZip64.Dynamic; //64位压缩
            zipStream.SetComment("Just for Test");//备注

            string[] fileList = { "E:\\file1.xlsx", "E:\\file2.txt" };
            foreach (string fileName in fileList)
            {
                FileStream source = File.OpenRead(fileName);
                ZipEntry zip = new ZipEntry(source.Name);

                byte[] buffer = new byte[source.Length];
                source.Read(buffer, 0, buffer.Length);
                zip.Size = buffer.Length;
                zip.DateTime = DateTime.Now;//时间

                source.Close();
                Crc32 crc = new Crc32();
                crc.Update(buffer);
                zip.Crc = crc.Value;//CRC

                zipStream.PutNextEntry(zip);
                zipStream.Write(buffer, 0, buffer.Length);
            }
            zipStream.Finish();
            zipStream.Close();
        }
        #endregion 
    }
}
