using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public static class DataLogger
{
    public static void WriteString(string path, string data)
    {      
        //Write some text to the test.txt file
        StreamWriter writer = new StreamWriter(path, true);
        writer.WriteLine(data);
        writer.Close();        
    } 
}
