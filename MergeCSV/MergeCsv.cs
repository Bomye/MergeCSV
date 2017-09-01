using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MergeCSV
{
    class MergeCsv
    {
        public static void Merging(string entrance)
        {
            var folders = getFolders(entrance);
            //得到文件名称字典
            List<Folder_File_Path> folder_file_path = new List<Folder_File_Path>();
            for (int i = 0; i < folders.Count; i++)
			{
                string folderPath = entrance + "\\" + folders[i].Name;
                Folder_File_Path ffp = new Folder_File_Path();
                ffp.folderName = folders[i].Name;
                ffp.dic = getFileDic(folderPath);
                folder_file_path.Add(ffp);
			}

            if (!Directory.Exists("result")) Directory.CreateDirectory("result");
            else
            {
                Directory.Delete("result",true);
                Directory.CreateDirectory("result");
            }
                
            //遍历每个文件夹和文件夹下的文件匹配相同名字的文件
            for (int i = 0; i < folder_file_path.Count; i++)
            {
                foreach (var item in folder_file_path[i].dic.Keys)
                {
                    if (!File.Exists("result\\"+item))
                    {
                        using (StreamWriter sw = new StreamWriter(@"result\" + item))
                        {
                            var lines = ReadToList(folder_file_path[i].dic[item]);
                            foreach (var li in lines)
                            {
                                sw.WriteLine(li);
                            }

                            for (int j = i + 1; j < folder_file_path.Count - 1; j++)
                            {
                                //判断有没有相同的名字 
                                if (folder_file_path[j].dic.ContainsKey(item))
                                {
                                    lines = ReadToList(folder_file_path[j].dic[item]);
                                    for (int l = 1; l < lines.Count; l++)
                                    {
                                        sw.WriteLine(lines[l]);
                                    }
                                }
                            }
                            sw.Close();
                        }
                    }
                }   
            }
        }

        /// <summary>
        /// 返回主文件夹下子文件夹名称
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        static List<DirectoryInfo> getFolders(string path)
        {
            List<DirectoryInfo> folderList = new List<DirectoryInfo>();
            DirectoryInfo di = new DirectoryInfo(path);
            var folders = di.GetDirectories();
            foreach (var item in folders)
            {
                folderList.Add(item);
            }
            return folderList;
        }

        /// <summary>
        /// 获取文件夹下的所有文件，返回文件名，文件路径的字典
        /// </summary>
        /// <param name="folderPath"></param>
        /// <returns></returns>
        static Dictionary<string, string> getFileDic(string folderPath)
        {
            var dic = new Dictionary<string, string>();
            string[] files = Directory.GetFiles(folderPath);
            foreach (var item in files)
            {
                string name = Regex.Match(item, @"([^<>/\\\|:""\*\?]+)\.\w+$").Groups[0].Value;
                dic.Add(name, item);
            }
            return dic;
        }
        static List<string> ReadToList(string filePath)
        {
            var csvLines = new List<string>();
            using (System.IO.StreamReader sr = new System.IO.StreamReader(filePath))
            {
                while (!sr.EndOfStream)
                {
                    csvLines.Add(sr.ReadLine());       
                }
                sr.Close();
            }
            return csvLines;
        }
    }
    class Folder_File_Path
    {
        public string folderName;
        public Dictionary<string, string> dic;
    }
}
