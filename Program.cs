﻿using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MHFQuestToMH2Dos
{
    internal class Program
    {
        static string loc = Path.GetDirectoryName(AppContext.BaseDirectory) + "\\";

        const string InDir = "Input";
        const string OutDir = "Out";
        const string PosFile2DosDir = "PosFile2Dos";
        const string Ver = "0.3.2";

        static void Main(string[] args)
        {
            string title = $"MHFQuestToMH2Dos Ver.{Ver} By 皓月云 axibug.com";
            Console.Title = title;
            Console.WriteLine(title);


            if (!Directory.Exists(loc + InDir))
            {
                Console.WriteLine("Input文件不存在");
                Console.ReadLine();
                return;
            }

            if (!Directory.Exists(loc + OutDir))
            {
                Console.WriteLine("Out文件不存在");
                Console.ReadLine();
                return;
            }

            if (!Directory.Exists(loc + PosFile2DosDir))
            {
                Console.WriteLine("Templete文件不存在");
                Console.ReadLine();
                return;
            }

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            string[] tempfiles = FileHelper.GetDirFile(loc + PosFile2DosDir);
            int index_temp = 0;
            int errcount_temp = 0;
            for (int i = 0; i < tempfiles.Length; i++)
            {
                string FileName = tempfiles[i].Substring(tempfiles[i].LastIndexOf("\\"));

                if (!FileName.ToLower().Contains(".mib") && !FileName.ToLower().Contains(".bin"))
                {
                    continue;
                }
                index_temp++;

                //Console.WriteLine($">>>>>>>>>>>>>>读取 第{index_temp}个模板文件  {FileName}<<<<<<<<<<<<<<<<<<<");
                FileHelper.LoadFile(tempfiles[i], out byte[] data);
                if (LoadToSaveTemplate.LoadMapTemplateAreaData(data, FileName, tempfiles[i]))
                {
                    //Console.WriteLine($">>>>>>>>>>>>>>成功读取 第{index_temp}个,"+ FileName);
                }
                else
                {
                    errcount_temp++;
                    //Console.WriteLine($">>>>>>>>>>>>>>成功失败 第{index_temp}个");
                }


                //LoadToSaveTemplate.LoadMaxGuti(data);

            }

            //int[] gutikeys = LoadToSaveTemplate.DictGutiName.Keys.ToArray();
            //gutikeys = gutikeys.OrderBy(w => w).ToArray();
            //foreach (var k in gutikeys)
            //{
            //    Log.HexInfo(k, "任务" + LoadToSaveTemplate.DictGutiName[k] + ",固体值{0}", k);
            //}

            //int[] gutikeys = LoadToSaveTemplate.DictStarName.Keys.ToArray();
            //gutikeys = gutikeys.OrderBy(w => w).ToArray();
            //foreach (var k in gutikeys)
            //{
            //    Log.HexInfo(k, "任务" + LoadToSaveTemplate.DictStarName[k] + ",星{0}", k);
            //}


            Console.WriteLine($"-----------原数据读取完毕-----------");

            string[] files = FileHelper.GetDirFile(loc + InDir);
            Console.WriteLine($"共{files.Length}个文件，是否处理? (y/n)");

            string yn = Console.ReadLine();
            if (yn.ToLower() != "y")
                return;

            int index= 0;
            int errcount = 0;
            for(int i = 0;i < files.Length;i++) 
            {
                string FileName = files[i].Substring(files[i].LastIndexOf("\\"));

                if (!FileName.ToLower().Contains(".mib") && !FileName.ToLower().Contains(".bin"))
                {
                    continue;
                }
                index++;

                Console.WriteLine($">>>>>>>>>>>>>>开始处理 第{index}个文件  {FileName}<<<<<<<<<<<<<<<<<<<");
                FileHelper.LoadFile(files[i], out byte[] data);
                if (ModifyQuest.ModifyQuset(data, out byte[] targetdata, out int targetQuestID))
                {
                    string newfileName = FileName + "_fix_toid_"+ targetQuestID;
                    string outstring = loc + OutDir + "\\" + newfileName;

                    //LoadToSaveTemplate.GetModeType(targetdata, FileName);

                    FileHelper.SaveFile(outstring, targetdata);
                    Console.WriteLine($">>>>>>>>>>>>>>成功处理 第{index}个:{outstring}");
                }
                else
                {
                    errcount++;
                    Console.WriteLine($">>>>>>>>>>>>>>处理失败 第{index}个");
                }
            }

            Console.WriteLine($"已处理{files.Length}个文件，其中{errcount}个失败");


            string[] tempkeys = LoadToSaveTemplate.DictTimeTypeCount.Keys.OrderBy(w => w).ToArray();

            foreach (var r in tempkeys)
            {
                Console.WriteLine(r + ":" + LoadToSaveTemplate.DictTimeTypeCount[r]);
            }

            Console.ReadLine();
        }
    }
}