using SnifferMC.Resourse;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace SnifferMC
{
    /// <summary>
    /// This class is processing information to load/write from/in file By using XML or Binary serialization
    /// </summary>
    public sealed class FileWorker
    {
        public FileWorker()
        { }
        /// <summary>Binary serializer</summary>
        /// <param name="lsitIN"></param>
        /// <param name="path"></param>
        ///  <exception cref="SerializationException"></exception>
        public static void binarySerializer(List<IPAddreData> lsitIN, string path)
        {   //It cane be SoapFormatter soap = new SoapFormatter();
            /*В отличие от BinaryFormatter, платформа и операционная система 
             * не влияют на успешное восстановление данных, сериализированных с помощью SoapFormatter.
             */
            try
            {
                IFormatter formatter = new BinaryFormatter();
                SerializeItem(path, formatter, lsitIN);
            }
            catch (SerializationException e){throw new SerializationException(e.Message);}
        }
        /// <summary>Binary deserializer</summary>
        /// <param name="path"></param>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <returns></returns>
        public static List<IPAddreData> binaryDeserializer(string path)
        {
            try
            {
                IFormatter formatter = new BinaryFormatter();
                return DeserializeItem(path, formatter);
            }
            catch (NotSupportedException e) { throw new NotSupportedException(e.Message); }
            catch (DirectoryNotFoundException e) { throw new DirectoryNotFoundException(e.Message); }
            catch (FileNotFoundException e) { throw new FileNotFoundException(e.Message); }
            catch (ArgumentNullException e) { throw new ArgumentNullException(e.Message); }
            catch (InvalidOperationException e) { throw new InvalidOperationException(e.Message); }
        }
        /// <summary> XML Serializer of information</summary>
        /// <param name="lsitIN"></param>
        /// <param name="path"></param>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        public static void xmlSerializer(List<IPAddreData> lsitIN, string path)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<IPAddreData>));
                using (TextWriter writer = new StreamWriter(path))
                {
                    serializer.Serialize(writer, lsitIN);
                }
            }
            catch (NotSupportedException e) { throw new NotSupportedException(e.Message); }
            catch (DirectoryNotFoundException e) { throw new DirectoryNotFoundException(e.Message); }


        }
        /// <summary>Load informatio from XML file</summary>
        /// <param name="path"></param>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        ///  <returns></returns>
        public static List<IPAddreData> xmlDeserializer(string path) 
        {
            try
            {
                List<IPAddreData> listOUT = new List<IPAddreData>();
                XmlSerializer serializer = new XmlSerializer(typeof(List<IPAddreData>));
                using (FileStream fs = new FileStream(path, FileMode.Open))
                {
                    return listOUT = (List<IPAddreData>)serializer.Deserialize(fs);

                }
            }
            catch (NotSupportedException e) { throw new NotSupportedException(e.Message); }
            catch (DirectoryNotFoundException e) { throw new DirectoryNotFoundException(e.Message); }
            catch (FileNotFoundException e) { throw new FileNotFoundException(e.Message); }
            catch (ArgumentNullException e) { throw new ArgumentNullException(e.Message); }
            catch (InvalidOperationException e) { throw new InvalidOperationException(e.Message); }

        }

        /*
         *Method For binary method*s 
         */
        /// <summary> Serialize Item</summary>
        /// <param name="fileName"></param>
        /// <param name="formatter"></param>
        /// <param name="lsitIN"></param>
        ///  /// <exception cref="SerializationException"></exception>
        private static void SerializeItem(string fileName, IFormatter formatter, List<IPAddreData> lsitIN)
        {
            List<IPAddreData> arr = lsitIN;
            FileStream save = null;
            try
            {
                save = new FileStream(fileName, FileMode.Create);
                formatter.Serialize(save, lsitIN);
            }
            catch (SerializationException e)
            {
                throw new SerializationException(e.Message);
            }
            finally { if (save != null) save.Close(); }


        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="formatter"></param>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <returns></returns>
        private static List<IPAddreData> DeserializeItem(string fileName, IFormatter formatter)
        {
            try
            {
                List<IPAddreData> listOUT = new List<IPAddreData>();
                using (FileStream read = new FileStream(fileName, FileMode.Open))
                {
                    listOUT = (List<IPAddreData>)formatter.Deserialize(read);
                }

                return listOUT;
            }
            catch (NotSupportedException e) { throw new NotSupportedException(e.Message); }
            catch (DirectoryNotFoundException e) { throw new DirectoryNotFoundException(e.Message); }
            catch (FileNotFoundException e) { throw new FileNotFoundException(e.Message); }
            catch (ArgumentNullException e) { throw new ArgumentNullException(e.Message); }
            catch (InvalidOperationException e) { throw new InvalidOperationException(e.Message); }
        }

    }
}
    

