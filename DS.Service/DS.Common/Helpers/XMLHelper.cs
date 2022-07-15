using System.IO;
using System.Reflection;
using System.Xml.Serialization;

namespace DS.Common.Helpers
{
    public static class XMLHelper
    {
        public static T DeserializationXml<T>(T objectInstance, string inputFileXml) where T : class
        {
            try
            {
                XmlSerializer mySerializer = new XmlSerializer(typeof(T));
                using (FileStream myFileStream = new FileStream(inputFileXml, FileMode.Open))
                {
                    objectInstance = (T)mySerializer.Deserialize(myFileStream);
                    return objectInstance;
                }
            }
            catch (System.Exception ex)
            {
                ConfigHelper.Instance.WriteLog("Wrong at DeserializationXml", ex, MethodBase.GetCurrentMethod().Name, "DeserializationXml");
                return null;
            }
        }

        public static string SerializationXml<T>(T objectInstance, string outputFileXml)
        {
            try
            {
                XmlSerializer serialInstance = new XmlSerializer(typeof(T));

                using (StreamWriter sw = new StreamWriter(outputFileXml))
                {

                    serialInstance.Serialize(sw, objectInstance);

                    sw.Close();

                    return sw.ToString();
                }
            }
            catch (System.Exception ex)
            {
                ConfigHelper.Instance.WriteLog("Wrong at SerializationXml 1", ex, MethodBase.GetCurrentMethod().Name, "SerializationXml");
                return null;
            }
        }

        public static string SerializationXmlWithPrefix<T>(T objectInstance, string outputFileXml, XmlSerializerNamespaces ns)
        {
            try
            {
                XmlSerializer serialInstance = new XmlSerializer(typeof(T));

                using (StreamWriter sw = new StreamWriter(outputFileXml))
                {

                    serialInstance.Serialize(sw, objectInstance, ns);

                    sw.Close();

                    return sw.ToString();
                }
            }
            catch (System.Exception ex)
            {
                ConfigHelper.Instance.WriteLog("Wrong at SerializationXmlWithPrefix", ex, MethodBase.GetCurrentMethod().Name, "GetUserInfo");
                return null;
            }
        }
    }
}
