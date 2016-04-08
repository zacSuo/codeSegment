using System;
using System.IO;
//需要先引用ICSharpCode.SharpZipLib.DLL或ICSharpCode.SharpZipLib项目文件
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Checksums;

namespace ALDI.ZIP
{
    class UseCase
    {
        static void Main(string[] args)
        {
            //Zip1();
            //Zip2();
            //Zip();
            //UnZip();  
            //UnZip1();                     
        }

        static void Zip()
        {
            ZipFile zipFile = ZipFile.Create("E:\\测试.ZIP");
            zipFile.BeginUpdate();
            zipFile.Add("E:\\file1.xlsx");
            zipFile.Add("E:\\file2.txt");
            zipFile.CommitUpdate();
        }

        static void ZipFolder()
        {
            FastZip zipFile = new FastZip();
            zipFile.CreateZip("E:\\测试.ZIP", "E:\\test\\", true, "");

        }

        static void UnZip()
        {
            FastZip zipFile = new FastZip();
            zipFile.ExtractZip("E:\\测试.ZIP", "E:\\test\\", "");
        }

        static void Zip1()
        {
            int size = 512;
            FileStream fs = File.Create("E:\\test1.ZIP");
            ZipOutputStream zipStream = new ZipOutputStream(fs);

            string[] fileList = { "E:\\file1.xlsx", "E:\\file2.txt" };
            foreach (string fileName in fileList)
            {
                FileStream source = File.OpenRead(fileName);
                ZipEntry zip = new ZipEntry(source.Name);
                zip.DateTime = DateTime.Now;
                zipStream.PutNextEntry(zip);

                byte[] buffer = new byte[size];
                int readIdx = 0, tmpRead;
                while (readIdx < source.Length)
                {
                    tmpRead = source.Read(buffer, 0, size);
                    zipStream.Write(buffer, 0, tmpRead);
                    readIdx += tmpRead;
                }
                source.Close();
            }
            zipStream.Finish();
            zipStream.Close();
        }


        static void UnZip1()
        {
            int size = 512;
            string folder = "E:\\test\\";
            Directory.CreateDirectory(folder);

            FileStream fs = File.OpenRead("E:\\test1.ZIP");
            ZipInputStream zipStream = new ZipInputStream(fs);

            ZipEntry zip = null;
            while ((zip = zipStream.GetNextEntry()) != null)
            {
                string fileName = Path.GetFileName(zip.Name);
                if (fileName.Equals(string.Empty))
                {
                    Directory.CreateDirectory(folder + zip.Name);
                    continue;
                }
                FileStream fsFile = File.Create(folder + zip.Name);

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
        /// 加参数
        /// </summary>
        static void Zip2()
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
    }
}
