using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace ScriptableData
{
  class MainClass
  {
    [STAThread]
    public static void Main(string[] args)
    {
      Dictionary<string, string> encodeTable = new Dictionary<string, string>();
      Dictionary<string, string> decodeTable = new Dictionary<string, string>();
      AddCrypto("skill", "@1", encodeTable, decodeTable);
      AddCrypto("section", "@2", encodeTable, decodeTable);
      AddCrypto("effect", "@3", encodeTable, decodeTable);
      AddCrypto("sound", "@4", encodeTable, decodeTable);
      AddCrypto("triger", "@5", encodeTable, decodeTable);
      AddCrypto("duration", "@6", encodeTable, decodeTable);
      AddCrypto("grap", "@7", encodeTable, decodeTable);
      AddCrypto("range", "@8", encodeTable, decodeTable);
      AddCrypto("animation", "@9", encodeTable, decodeTable);
      AddCrypto("impact", "@10", encodeTable, decodeTable);
      AddCrypto("hittarget", "@11", encodeTable, decodeTable);
      AddCrypto("throw", "@12", encodeTable, decodeTable);


      ScriptableDataFile file = new ScriptableDataFile();
      file.Load("test.txt");
#if FULL_VERSION
      file.Save("copy.txt");
      string code0 = file.GenerateObfuscatedCode(File.ReadAllText("test.txt"), encodeTable);
      File.WriteAllText("obfuscation.txt", code0);
#endif
      file.ScriptableDatas.Clear();
      string code = File.ReadAllText("obfuscation.txt");
      file.LoadObfuscatedCode(code, decodeTable);
#if FULL_VERSION
      file.Save("unobfuscation.txt");
#endif
    }

    private static void AddCrypto(string s, string d, Dictionary<string, string> encodeTable, Dictionary<string, string> decodeTable)
    {
      encodeTable.Add(s, d);
      decodeTable.Add(d, s);
    }
  }
}
