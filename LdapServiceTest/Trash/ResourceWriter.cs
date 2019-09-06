
namespace LdapServiceTest.Trash
{


    class ResourceWriter
    {


        // LdapServiceTest.Trash.ResourceWriter.Test();
        public static void Test()
        {
            System.Collections.Generic.Dictionary<string, object> dict = 
                new System.Collections.Generic.Dictionary<string, object>(System.StringComparer.OrdinalIgnoreCase);


            dict.Add("SomeString", "Test 123");
            dict.Add("SomeOtherString", "Test 456");

            WriteResourceFile(@"d:\ResourceThatWorksWithGenerator.resx.xml", dict);


            dict.Add("SomeNumber", 123);
            dict.Add("SomeBytes", System.Text.Encoding.UTF8.GetBytes("This is a test !"));

            WriteResourceFile(@"d:\ResourceThatDoesNotWorkWithGenerator.resx1.xml", dict);
            WriteBinaryResourceFile(@"d:\BinaryResource.resx.bin.txt", dict);
        } // End Sub Test 


        public static void WriteAnyResourceFile<T>(System.Collections.Generic.Dictionary<string, object> dictionary, params object[] args)
            where T: System.Resources.IResourceWriter 
        {
            // new System.Resources.ResXResourceWriter()
            // new System.Resources.ResourceWriter()
            using (System.Resources.IResourceWriter writer = (System.Resources.IResourceWriter)System.Activator.CreateInstance(typeof(T), args))
            {
                foreach (System.Collections.Generic.KeyValuePair<string, object> item in dictionary)
                {
                    writer.AddResource(item.Key, item.Value);
                } // Next item 

                writer.Generate();
                writer.Close();
            } // End Using writer 

        } // End Sub WriteAnyResourceFile 


        // libResXResourceReader
        public static void WriteResourceFile(string file, System.Collections.Generic.Dictionary<string, object> dictionary)
        {
            WriteAnyResourceFile<System.Resources.ResXResourceWriter>(dictionary, file);
        } // End Sub WriteResourceFile 


        // Assembly System.Resources.Writer
        public static void WriteBinaryResourceFile(string file, System.Collections.Generic.Dictionary<string, object> dictionary)
        {
            WriteAnyResourceFile<System.Resources.ResourceWriter>(dictionary, file);
        } // End Sub WriteBinaryResourceFile 


    }


}
