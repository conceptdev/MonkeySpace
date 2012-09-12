using System;
using System.Xml.Serialization;
using System.Runtime.Serialization;
/// <summary>
/// Store objects at absolute zero, and thaw for use as required
/// </summary>
/// <remarks>
/// Kelvin can serialize and deserialize any type of object 
/// (well, it has to be serializable to start with - I just mean it uses Generics)
/// Use Xml (String) or Binary (Byte[]) datastructures or read/write files of both types.
/// </remarks>
/// <typeparam name="T">Type/Class that you wish to serialize/deserialize/</typeparam>
/// <example>
/// Kelvin is a *static* class to provide simple, single-line, typed access to Serialization:
/// <code><![CDATA[
/// Catalog currentCatalog = new Catalog ();
/// // ... currentCatalog population ...
/// // Save to disk
/// Kelvin<Catalog>.FreezeToBinaryFile(currentCatalog, @"C:\Temp\Catalog.dat");
/// // Load from disk
/// Catalog loadedCatalog = Kelvin<Catalog>.ThawFromBinaryFile(@"C:\Temp\Catalog.dat");
/// 
/// string[] words = new string[] {"Sample", "Array", "of", "Words"};
/// Kelvin<string[]>.FreezeToXmlFile(words, @"C:\Temp\Words.xml.");
/// string[] loadedWords = Kelvin<string[]>.ThawFromXmlFile(@"C:\Temp\Words.xml");
/// ]]></code>
/// </example>
namespace ConceptDevelopment
{
   public static class Kelvin<T>
   {
      #region Static Constructor (empty)
      /// <summary>
      /// Kelvin cannot be instantiated. All methods are static. 
      /// Nested classes are not static, but are only used internally.
      /// </summary>
      static Kelvin() { }
      #endregion

      #region To/From a File
      /// <summary>
      /// Serialize object to an XML file on disk
      /// </summary>
      /// <param name="cryo">T instance to serialize</param>
      /// <param name="fileName">Full file path, including name and extension, eg @"C:\Temp\NewFile.xml"</param>
      /// <returns>true if save was successful, false if an error occured</returns>
      public static bool ToXmlFile(T cryo, string fileName)
      {
         try
         {
            XmlSerializer serializerXml = new XmlSerializer(typeof(T));
            System.IO.TextWriter writer = new System.IO.StreamWriter(fileName);
            serializerXml.Serialize(writer, cryo);
            writer.Close();
            return true;
         }
         catch (System.IO.DirectoryNotFoundException)
         {
            return false;
         }
      }
      /// <summary>
      /// Deserialize an Xml File to T object
      /// </summary>
      /// <param name="frozenObjectFileName">Full file path, including name and extension, eg @"C:\Temp\NewFile.xml"</param>
      /// <returns>T instance or default(T)</returns>
      public static T FromXmlFile(string frozenObjectFileName)
      {
         XmlSerializer serializerXml = new XmlSerializer(typeof(T));
         if (System.IO.File.Exists(frozenObjectFileName))
         {
            System.IO.Stream stream = new System.IO.FileStream(frozenObjectFileName, System.IO.FileMode.Open);
            object o = serializerXml.Deserialize(stream);
            stream.Close();
            return (T)o;
         }
         else
         {
            throw new System.IO.FileNotFoundException(frozenObjectFileName + " was not found.");
         }
      }
      #endregion

      #region To/From a String
      /// <summary>
      /// Serialize object to an Xml String for use in your code
      /// </summary>
      /// <param name="cryo">T instance to serialize</param>
      /// <returns><see cref="System.String"/> representation of T object</returns>
      public static string ToXmlString(T cryo)
      {
         XmlSerializer serializer = new XmlSerializer(typeof(T));
         System.IO.TextWriter writer = new System.IO.StringWriter();
         try
         {
            serializer.Serialize(writer, cryo);
         }
         finally
         {
            writer.Flush();
            writer.Close();
         }

         return writer.ToString();
      }
      /// <summary>
      /// Deserialize a String containing Xml to T object
      /// </summary>
      /// <param name="frozen"></param>
      /// <returns>T instance or default(T)</returns>
      public static T FromXml(string frozen)
      {
         if (frozen.Length <= 0) throw new ArgumentOutOfRangeException("frozenObject", "Cannot thaw a zero-length string");

         XmlSerializer serializer = new XmlSerializer(typeof(T));
         System.IO.TextReader reader = new System.IO.StringReader(frozen);
         object @object = default(T);    // default return value
         try
         {
            @object = serializer.Deserialize(reader);
         }
         finally
         {
            reader.Close();
         }
         return (T)@object;
      }
      #endregion
   }
}