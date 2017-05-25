using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.IO;
using System.Collections.Generic;


namespace NetControl
{
    class JSON
    {
        public static T parse<T>(string jsonString)
        {
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonString)))
            {
                return (T)new DataContractJsonSerializer(typeof(T)).ReadObject(ms);
            }
        }

        public static string stringify(object jsonObject)
        {
            using (var ms = new MemoryStream())
            {
                new DataContractJsonSerializer(jsonObject.GetType()).WriteObject(ms, jsonObject);
                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }
    }

    [DataContract]
    public class MachineInfo
    {
        [DataMember(Order = 0, IsRequired = true)]
        public string POWER
        {
            get;
            set;
        }


        [DataMember(Order = 1, IsRequired = true)]
        public string GONGLI1
        {
            get;
            set;
        }


        [DataMember(Order = 2, IsRequired = true)]
        public string GONGLI2
        {
            get;
            set;
        }


        [DataMember(Order = 3, IsRequired = false)]
        public string LIANXU1
        {
            get;
            set;
        }


        [DataMember(Order = 4, IsRequired = false)]
        public string LIANXU2
        {
            get;
            set;
        }


        [DataMember(Order = 5, IsRequired = false)]
        public string MONTH
        {
            get;
            set;
        }

        [DataMember(Order = 6, IsRequired = false)]
        public string YEAR
        {
            get;
            set;
        }


        [DataMember(Order = 7, IsRequired = false)]
        public string ALL
        {
            get;
            set;
        }
    }


    public class Test
    {
        public Test()
        {
            //单个
            MachineInfo itemOne = new MachineInfo() { POWER="k", GONGLI1="1111", GONGLI2="2222", LIANXU1 = "333", LIANXU2="444" };
            string strOne = JSON.stringify(itemOne);
            MachineInfo infoOne = JSON.parse<MachineInfo>(strOne);

            //多个
            MachineInfo[] itemMore = new MachineInfo[]{
                new MachineInfo(){ POWER = "k", GONGLI1 = "1111", GONGLI2 = "2222", LIANXU1 = "333", LIANXU2 = "444" }
                new MachineInfo(){ POWER = "l", GONGLI1 = "5555", GONGLI2 = "6666", LIANXU1 = "777", LIANXU2 = "888" }
            };
            string strMore = JSON.stringify(itemMore);
            List<MachineInfo> infoMore = JSON.parse<List<MachineInfo>>(strMore);
        }
    }
}
