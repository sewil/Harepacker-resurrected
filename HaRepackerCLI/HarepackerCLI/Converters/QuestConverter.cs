using MapleLib;
using MapleLib.WzLib;
using MapleLib.WzLib.Serializer;
using MapleLib.WzLib.WzProperties;
using System.CodeDom.Compiler;
using System.Text.RegularExpressions;

namespace HarepackerCLI.Converters
{
    class QuestConverter
    {
        private readonly WzFile _questWz;
        private readonly WzFile _stringWz;
        private readonly WzFile _toDataWz;
        private readonly WzFile _toServerWz;
        private readonly List<WzQuest> _quests;
        private readonly int _npcID;
        private readonly string _npcName;
        private readonly StringWriter _sw;
        private readonly IndentedTextWriter _iw;
        private readonly IDictionary<int, List<int>> _questItems;

        public QuestConverter(WzFile questWz, WzFile stringWz, WzFile itemWz, WzFile characterWz, WzFile toDataWz, WzFile toServerWz, List<WzQuest> quests, int npcID)
        {
            _sw = new StringWriter();
            _iw = new IndentedTextWriter(_sw, "    ");
            _stringWz = stringWz;
            _questWz = questWz;
            _npcID = npcID;
            _npcName = _stringWz["Npc.img"][_npcID.ToString()]["name"].GetString();
            _toDataWz = toDataWz;
            _toServerWz = toServerWz;
            _quests = quests;
            _questItems = new Dictionary<int, List<int>>();
            foreach (var qi in _quests.SelectMany(i => i.Stages[QuestStage.Start].Act.Items).DistinctBy(i => i.ItemID))
            {
                var o = WzUtils.GetItem(characterWz, itemWz, qi.ItemID);
                var isQuest = o["info"]["quest"];
                if (isQuest != null && isQuest.GetInt() == 1)
                {
                    var key = qi.Stage.Quest.QuestID;
                    if (!_questItems.ContainsKey(key))
                    {
                        _questItems.Add(key, new List<int>());
                    }
                    _questItems[key].Add(qi.ItemID);
                }
            }
        }

        public static void Convert(string[] args)
        {
            if (args.Length < 7)
            {
                Console.WriteLine("Usage: HarepackerCLI.exe convert quest <data-directory> <game-version> <script-directory> <from-data-directory> <npc-id>");
                Environment.Exit(1);
            }
            else
            {
                string toDataDirectory = args[2];
                string gameVersion = args[3];
                string scriptDirectory = args[4];
                string fromDataDirectory = args[5];
                int npcID = int.Parse(args[6]);
                WzFile questWz = new WzFile(Path.Join(fromDataDirectory, "Quest.wz"), null, WzMapleVersion.CLASSIC);
                WzUtils.ParseWzFile(questWz);
                WzFile stringWz = new WzFile(Path.Join(fromDataDirectory, "String.wz"), null, WzMapleVersion.CLASSIC);
                WzUtils.ParseWzFile(stringWz);
                WzFile itemWz = new WzFile(Path.Join(fromDataDirectory, "Item.wz"), null, WzMapleVersion.CLASSIC);
                WzUtils.ParseWzFile(itemWz);
                WzFile characterWz = new WzFile(Path.Join(fromDataDirectory, "Character.wz"), null, WzMapleVersion.CLASSIC);
                WzUtils.ParseWzFile(characterWz);
                WzFile toDataWz = new WzFile(Path.Join(toDataDirectory, "Data.wz"), gameVersion, WzMapleVersion.CLASSIC);
                WzUtils.ParseWzFile(toDataWz);
                WzFile toServerWz = new WzFile(Path.Join(toDataDirectory, "Server.wz"), gameVersion, WzMapleVersion.CLASSIC);
                WzUtils.ParseWzFile(toServerWz);

                WzFileManager.fileManager = new WzFileManager(fromDataDirectory, false);

                var quests = new List<WzQuest>();
                var checkImg = questWz.WzDirectory.GetImageByName("Check.img");
                foreach (WzSubProperty prop in checkImg.WzProperties)
                {
                    if (prop["0"]["npc"]?.GetInt() == npcID || prop["1"]["npc"]?.GetInt() == npcID)
                    {
                        short questID = short.Parse(prop.Name);
                        var quest = new WzQuest(questWz, questID);
                        quests.Add(quest);
                    }
                }

                if (quests.Count == 0)
                {
                    Console.WriteLine("No quests found for npc " + npcID + "!");
                    Environment.Exit(0);
                }

                var con = new QuestConverter(questWz, stringWz, itemWz, characterWz, toDataWz, toServerWz, quests, npcID);
                con.Convert(scriptDirectory);
            }
        }

        public void Convert(string scriptDirectory)
        {
            ConvertScript(scriptDirectory, out string scriptName);
            ConvertQuestInfo();
            UpdateQuestDemand();
            UpdateNpc(scriptName);
        }

        private void UpdateNpc(string scriptName)
        {
            // Find npc, otherwise give error
            string npcImgName = (_npcID + ".img").PadLeft(7, '0');
            var deserializer = new WzImgDeserializer(true);
            string npcImgFilePath = Path.Join(Path.GetDirectoryName(_toDataWz.FilePath), "Npc", npcImgName);
            var npcImg = deserializer.WzImageFromIMGFile(npcImgFilePath, _toDataWz.WzIv, npcImgName, out bool success);
            _toDataWz.WzDirectory.AddImage(npcImg);
            if (!success || npcImg == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Couldn't parse file " + npcImgFilePath + "!");
                Console.ForegroundColor = ConsoleColor.Gray;
            }
            else
            {
                // Set npc script, check if already set also
                var info = npcImg["info"];
                if (info["quest"] != null)
                {
                    Console.WriteLine("Quest name already set for NPC " + _npcID + " to " + info["quest"].GetString() + "!");
                }
                else
                {
                    info.WzProperties.Add(new WzStringProperty("quest", scriptName));
                    Console.WriteLine("Set npc quest prop to " + scriptName);
                    npcImg.Changed = true;
                    var serializer = new WzImgSerializer();
                    File.Copy(npcImgFilePath, npcImgFilePath + ".bak", true);
                    serializer.SerializeImage(npcImg, npcImgFilePath);
                    Console.WriteLine("Updated file " + npcImgFilePath);
                }
            }
        }

        private void ConvertQuestInfo()
        {
            var deserializer = new WzImgDeserializer(true);
            string imgFilePath = Path.Join(Path.GetDirectoryName(_toDataWz.FilePath), "Etc", "QuestInfo.img");
            var img = deserializer.WzImageFromIMGFile(imgFilePath, _toDataWz.WzIv, "QuestInfo.img", out bool success);
            _toDataWz.WzDirectory.AddImage(img);

            if (!success || img == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Failed parsing file " + imgFilePath + "!");
                Console.ForegroundColor = ConsoleColor.Gray;
                return;
            }

            foreach (var q in _quests)
            {
                var questProp = img.WzProperties.FirstOrDefault(p => p.Name == q.ScriptQuestID);
                if (questProp != null)
                {
                    Console.WriteLine($"Quest {q.ScriptQuestID} already exists in Etc/QuestInfo.img!");
                    continue;
                }
                else
                {
                    questProp = new WzSubProperty(q.ScriptQuestID);

                    // Info
                    var infoProp = new WzSubProperty("info");
                    infoProp.AddProperty(new WzStringProperty("subject", q.QuestInfo.Name));

                    var reqs = new List<string>();
                    var startCheck = q.Stages[QuestStage.Start].Check;
                    var endCheck = q.Stages[QuestStage.Complete].Check;
                    if (startCheck.LvMin > 0) reqs.Add("Level " + startCheck.LvMin + (startCheck.LvMax > 0 ? "-" + startCheck.LvMax : ""));
                    else if (startCheck.LvMax > 0) reqs.Add("Under Level " + startCheck.LvMax);
                    if (startCheck.Fame > 0) reqs.Add("Fame " + startCheck.Fame);
                    if (startCheck.Jobs.Count > 0)
                    {
                        var jobs = startCheck.Jobs.Select(GetJobName).Distinct().ToList();
                        string jr;
                        if (jobs.Count == 4 && !jobs.Contains("Beginner"))
                        {
                            jr = "Beginners not allowed";
                        }
                        else if (jobs.Count > 2)
                        {
                            jr = string.Join(" and ", [
                                string.Join(", ", jobs.Take(jobs.Count - 2)),
                                jobs.Last()
                            ]);
                        }
                        else jr = string.Join(" and ", jobs);
                        reqs.Add(jr);
                    }
                    if (reqs.Count == 0) reqs.Add("None");
                    infoProp.AddProperty(new WzStringProperty("req", string.Join(", ", reqs)));

                    int island;
                    if (q.QuestInfo.Area <= 20) island = 0;
                    else if (q.QuestInfo.Area <= 30) island = 1;
                    else island = 2;
                    infoProp.AddProperty(new WzIntProperty("island", island));

                    questProp.WzProperties.Add(infoProp);

                    // Descs
                    var infoFrom = (WzImageProperty)_questWz["QuestInfo.img"][q.QuestID.ToString()];

                    string defaultDesc = infoFrom["1"].GetString();

                    if (endCheck.Mobs.Count > 0)
                    {
                        string mobDesc = "";
                        for (int i = 0; i < endCheck.Mobs.Count; i++)
                        {
                            var mob = endCheck.Mobs.ElementAt(i).Value;
                            string mobName = _stringWz["Mob.img"][mob.MobID.ToString()]["name"].GetString();
                            mobDesc += @$"\n{mobName}s to kill : #r#a{q.ScriptQuestID}{i + 1}##k";
                        }
                        defaultDesc += @"\n" + mobDesc;
                        var mobInfo = GetMobEndInfo(endCheck);
                        var mobProp = new WzSubProperty(mobInfo);

                        mobProp.AddProperty(new WzStringProperty("desc", defaultDesc));
                        questProp.WzProperties.Add(mobProp);
                    }

                    var defaultProp = new WzSubProperty("default");
                    defaultProp.AddProperty(new WzStringProperty("desc", defaultDesc));
                    questProp.WzProperties.Add(defaultProp);

                    string endDesc = infoFrom["2"].GetString();
                    var endProp = new WzSubProperty("end");
                    endProp.AddProperty(new WzStringProperty("desc", endDesc));
                    questProp.WzProperties.Add(endProp);

                    img.AddProperty(questProp);
                    Console.WriteLine($"Added Etc/QuestInfo.img/{q.ScriptQuestID}");
                }
            }

            File.Copy(imgFilePath, imgFilePath + ".bak", true);
            var serializer = new WzImgSerializer();
            img.Changed = true;
            serializer.SerializeImage(img, imgFilePath);
            Console.WriteLine("Updated file " + imgFilePath);
        }

        private void UpdateQuestDemand()
        {
            var deserializer = new WzImgDeserializer(true);
            string imgFilePath = Path.Join(Path.GetDirectoryName(_toServerWz.FilePath), "Server", "QuestDemand.img");
            var img = deserializer.WzImageFromIMGFile(imgFilePath, _toServerWz.WzIv, "QuestDemand.img", out bool success);
            _toServerWz.WzDirectory.AddImage(img);

            if (!success || img == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Failed parsing file " + imgFilePath + "!");
                Console.ForegroundColor = ConsoleColor.Gray;
                return;
            }

            bool changed = false;

            foreach (var q in _quests)
            {
                var endCheck = q.Stages[QuestStage.Complete].Check;
                if (endCheck.Mobs.Count == 0) continue;
                changed = true;

                var demandProp = img.WzProperties.FirstOrDefault(p => p.Name == q.ScriptQuestID);
                if (demandProp != null)
                {
                    Console.WriteLine($"Quest {q.ScriptQuestID} already exists in Server/QuestDemand.img!");
                    continue;
                }

                demandProp = new WzSubProperty(q.ScriptQuestID);
                for (var i = 0; i < endCheck.Mobs.Count; i++)
                {
                    var mob = endCheck.Mobs.ElementAt(i).Value;
                    demandProp.WzProperties.Add(new WzIntProperty(i.ToString(), mob.MobID));
                }

                img.AddProperty(demandProp);
                Console.WriteLine($"Added Server/QuestDemand.img/{q.ScriptQuestID}");
            }

            if (changed)
            {
                File.Copy(imgFilePath, imgFilePath + ".bak", true);
                var serializer = new WzImgSerializer();
                img.Changed = true;
                serializer.SerializeImage(img, imgFilePath);
                Console.WriteLine("Updated file " + imgFilePath);
            }
        }

        private string GetJobName(short jobID)
        {
            if (jobID == 0) return "Beginner";
            else if (jobID.ToString().StartsWith("1")) return "Warrior";
            else if (jobID.ToString().StartsWith("2")) return "Magician";
            else if (jobID.ToString().StartsWith("3")) return "Bowman";
            else if (jobID.ToString().StartsWith("4")) return "Thief";
            return "";
        }

        public void ConvertScript(string toScriptDirectory, out string scriptName)
        {
            _iw.WriteLine("using System;");
            _iw.WriteLine("using System.Collections.Generic;");
            _iw.WriteLine("using System.Linq;");
            _iw.WriteLine("using WvsBeta.Game;");
            _iw.WriteLineNoTabs("");
            _iw.WriteLine($"// {_npcID} - {_npcName}");
            _iw.WriteLine("public class NpcScript : IScriptV2");
            _iw.WriteLine("{");
            _iw.Indent++;

            Check();
            foreach (var cq in _quests.GroupBy(q => q.QuestInfo.Parent ?? q.ScriptQuestID))
            {
                Act(cq.ToList());
            }
            Run();

            _iw.Indent--;
            _iw.WriteLine("}");

            string script = _sw.ToString();

            // Create file
            scriptName = Regex.Replace(_npcName, @"[^A-Za-z\d]", "").ToLower();
            string fileName = Path.Join(toScriptDirectory, scriptName + ".cs");
            if (File.Exists(fileName)) File.Copy(fileName, fileName + ".bak", true);
            File.WriteAllText(fileName, script);
            Console.WriteLine("Created script file " + fileName);
        }

        private void Check()
        {
            _iw.WriteLine("public string Check(int quest)");
            _iw.WriteLine("{");
            _iw.Indent++;
            _iw.WriteLine("string info = GetQuestData(quest);");
            foreach (var q in _quests)
            {
                var startCheck = q.Stages[QuestStage.Start].Check;
                var endCheck = q.Stages[QuestStage.Complete].Check;
                var completeStage = q.Stages[QuestStage.Complete];
                _iw.WriteLine("if (quest == " + q.ScriptQuestID + ")");
                _iw.WriteLine("{");
                _iw.Indent++;

                // Check end date
                if (!string.IsNullOrWhiteSpace(startCheck.End))
                {
                    _iw.WriteLine($"var endDate = DateTime.ParseExact(\"{startCheck.End}\", \"yyyyMMddHH\", System.Globalization.CultureInfo.InvariantCulture);");
                    _iw.WriteLine("if (DateTime.UtcNow > endDate)");
                    _iw.Indent++;
                    _iw.WriteLine("return null;");
                    _iw.Indent--;

                    _iw.WriteLineNoTabs("");
                }

                // Check repeatable
                if (startCheck.IntervalMins > 0)
                {
                    _iw.WriteLine("var now = DateTime.UtcNow;");
                    _iw.WriteLine($"string lastSaved = GetQuestData({q.ScriptSavedDateQuestID});");
                    _iw.WriteLine("if (lastSaved != \"\")");
                    _iw.WriteLine("{");
                    _iw.Indent++;
                    _iw.WriteLine("var lastDate = DateTime.ParseExact(lastSaved, \"yyyyMMddHHmm\", System.Globalization.CultureInfo.InvariantCulture);");
                    _iw.WriteLine($"if ((now - lastDate).TotalMinutes < {startCheck.IntervalMins})");
                    _iw.Indent++;
                    _iw.WriteLine("return null;");
                    _iw.Indent--;
                    _iw.WriteLine("}");
                    _iw.Indent--;

                    _iw.WriteLineNoTabs("");
                }

                // Check prerequisite quests are done
                foreach (var sq in startCheck.Quests)
                {
                    _iw.WriteLine($"string q{sq.ScriptQuestID} = GetQuestData({sq.ScriptQuestID});");
                }

                _iw.Write($"if (info != \"{GetInfo(q, QuestState.Completed)}\"");
                foreach (var sq in startCheck.Quests)
                {
                    _iw.Write($" && q{sq.ScriptQuestID} == \"{GetInfo(q, QuestState.Completed)}\"");
                }
                if (startCheck.NpcID != _npcID) _iw.Write(" && info != \"\""); // Quest is not unstarted if NPC is not the starter
                if (startCheck.LvMin > 0) _iw.Write(" && Level >= " + startCheck.LvMin);
                if (startCheck.LvMax > 0) _iw.Write(" && Level <= " + startCheck.LvMax);
                if (startCheck.Fame > 0) _iw.Write(" && Fame >= " + startCheck.Fame);
                if (startCheck.Mesos > 0) _iw.Write(" && Mesos >= " + startCheck.Mesos);
                if (startCheck.Jobs.Count > 0)
                {
                    _iw.Write(" && (" + string.Join(" || ", startCheck.Jobs.Select(j => "Job ==" + j)) + ")");
                }
                _iw.WriteLine(")");
                _iw.Indent++;
                _iw.WriteLine($"return \" {q.QuestInfo.Name}\";");
                _iw.Indent--;

                _iw.Indent--;
                _iw.WriteLine("}");
            }

            _iw.WriteLine("return null;");

            _iw.Indent--;
            _iw.WriteLine("}");
        }
        private void Act(IList<WzQuest> chainQuests)
        {
            if (chainQuests.Count == 0) return;
            string parentFuncName = GetActFuncName(chainQuests[0].QuestInfo);
            bool haveParent = !string.IsNullOrWhiteSpace(chainQuests[0].QuestInfo.Parent);
            _iw.Write($"private void {parentFuncName}(string quest");
            if (haveParent)
            {
                _iw.Write($", int order");
            }
            _iw.WriteLine($")");
            _iw.WriteLine("{");
            _iw.Indent++;

            foreach (var q in chainQuests)
            {
                if (haveParent)
                {
                    _iw.WriteLine($"if (order == {q.QuestInfo.Order})");
                    _iw.WriteLine("{");
                    _iw.Indent++;
                }

                var startCheck = q.Stages[QuestStage.Start].Check;

                var endCheck = q.Stages[QuestStage.Complete].Check;

                // Start quest
                if (startCheck.NpcID == _npcID)
                {
                    ActStartQuest(q);
                }

                // Complete quest
                if (endCheck.NpcID == _npcID)
                {
                    ActCompleteQuest(q);
                }

                if (haveParent)
                {
                    _iw.Indent--;
                    _iw.WriteLine("}");
                }
            }

            _iw.Indent--;
            _iw.WriteLine("}");
        }
        private void Run()
        {
            _iw.WriteLine("public override void Run()");
            _iw.WriteLine("{");
            _iw.Indent++;

            _iw.WriteLine("var options = new List<(int Index, string Name)>();");
            _iw.WriteLine("int[] quests = {" + string.Join(",", _quests.Select(q => q.ScriptQuestID)) + "};");
            _iw.WriteLine("for (int i = 0; i < quests.Length; i++)");
            _iw.WriteLine("{");
            _iw.Indent++;

            _iw.WriteLine("var quest = quests[i];");
            _iw.WriteLine("string name = Check(quest);");
            _iw.WriteLine("if (name != null)");
            _iw.Indent++;
            _iw.WriteLine("options.Add((i, name));");
            _iw.Indent--;

            _iw.Indent--;
            _iw.WriteLine("}");

            _iw.WriteLineNoTabs("");

            var lastQuest = _quests.Last();
            string d0 = _stringWz["Npc.img"][_npcID.ToString()]["d0"].GetString();
            string d1 = _stringWz["Npc.img"][_npcID.ToString()]["d1"].GetString();
            _iw.WriteLine($"string dialogue = \"{EscapeStr(d0)}\";");
            _iw.WriteLine($"if (GetQuestData({lastQuest.ScriptQuestID}) == \"{GetInfo(lastQuest, QuestState.Completed)}\")");
            _iw.Indent++;
            _iw.WriteLine($"dialogue = \"{EscapeStr(d1)}\";");
            _iw.Indent--;
            _iw.WriteLine("if (options.Count == 0)");
            _iw.WriteLine("{");
            _iw.Indent++;
            _iw.WriteLine("self.say(dialogue);");
            _iw.WriteLine("return;");
            _iw.Indent--;
            _iw.WriteLine("}");
            _iw.WriteLine("int choice;");
            _iw.WriteLine("if (options.Count == 1)");
            _iw.Indent++;
            _iw.WriteLine("choice = options[0].Index;");
            _iw.Indent--;
            _iw.WriteLine("else");
            _iw.Indent++;
            _iw.WriteLine("choice = AskMenu($\"{dialogue}#b\", options.ToArray());");
            _iw.Indent--;

            _iw.WriteLineNoTabs("");

            _iw.WriteLine("switch (choice)");
            _iw.WriteLine("{");
            _iw.Indent++;
            for (int i = 0; i < _quests.Count; i++)
            {
                var q = _quests[i];
                string actFuncName = GetActFuncName(q.QuestInfo);
                _iw.Write($"case {i}: {actFuncName}(GetQuestData({q.ScriptQuestID})");
                if (!string.IsNullOrWhiteSpace(q.QuestInfo.Parent))
                {
                    _iw.Write($", {q.QuestInfo.Order}");
                }
                _iw.WriteLine("); break;");
            }
            _iw.Indent--;
            _iw.WriteLine("}");

            _iw.Indent--;
            _iw.WriteLine("}");
        }

        private List<string> GetJobConditions(QuestJob job)
        {
            var conditions = new List<string>();
            if (job.HasFlag(QuestJob.Beginner)) conditions.Add("Job == 0");
            if (job.HasFlag(QuestJob.Warrior)) conditions.Add("Job.ToString().StartsWith(\"1\")");
            if (job.HasFlag(QuestJob.Magician)) conditions.Add("Job.ToString().StartsWith(\"2\")");
            if (job.HasFlag(QuestJob.Bowman)) conditions.Add("Job.ToString().StartsWith(\"3\")");
            if (job.HasFlag(QuestJob.Thief)) conditions.Add("Job.ToString().StartsWith(\"4\")");
            if (job.HasFlag(QuestJob.GM)) conditions.Add("Job.ToString().StartsWith(\"5\")");
            return conditions;
        }

        private string GetInfo(WzQuest quest, QuestState state)
        {
            if (state == QuestState.InProgress)
            {
                var checkEnd = quest.Stages[QuestStage.Complete].Check;
                if (checkEnd.Mobs.Count > 0)
                {
                    string mobAct = string.Join("", checkEnd.Mobs.Select(i => i.Value.Count.ToString().PadLeft(3, '0')));
                    return mobAct;
                }
                else
                {
                    return "s";
                }
            }
            else if (state == QuestState.Completed)
            {
                return "end";
            }
            else return "";
        }

        private void WriteStop(WzQuestSay say, string key)
        {
            if (say.Stop.ContainsKey(key))
            {
                foreach (var stopLine in say.Stop[key])
                {
                    Say(stopLine);
                }
            }
            else
            {
                Console.WriteLine($"Missing stop \"{key}\" dialogue for quest!");
                _iw.WriteLine($"// Missing stop dialogue for \"{key}\"!");
            }
        }

        private string EscapeStr(string say)
        {
            return say.Replace("\"", "\\\"");
        }

        private void Say(string say)
        {
            _iw.WriteLine($"self.say(\"{EscapeStr(say)}\");");
        }

        private void Say(WzSayInfo say)
        {
            if (say.IsMenu)
            {
                bool multi = say.Menu.Options.Count > 1;
                if (multi)
                {
                    _iw.WriteLine("{");
                    _iw.Indent++;
                }
                var options = string.Join(", ", say.Menu.Options.Select(o => $"\"{EscapeStr(o)}\""));
                if (multi) _iw.Write("var answer = ");
                _iw.WriteLine($"AskMenu(\"{EscapeStr(say.Dialogue)}\", {options});");
                if (multi)
                {
                    var stopSay = string.Join(", ", say.Menu.StopSay.Select(s => $"\"{EscapeStr(s)}\""));
                    _iw.WriteLine("var stopSay = new string[] { " + stopSay + " };");
                    _iw.WriteLine("if (answer != " + say.Menu.Answer + ")");
                    _iw.WriteLine("{");
                    _iw.Indent++;
                    _iw.WriteLine("self.say(stopSay[answer]);");
                    _iw.WriteLine("return;");
                    _iw.Indent--;
                    _iw.WriteLine("}");

                    _iw.Indent--;
                    _iw.WriteLine("}");
                }
            }
            else
            {
                Say(say.Dialogue);
            }
        }

        private void ActStartQuest(WzQuest quest)
        {
            var start = quest.Stages[QuestStage.Start];
            var end = quest.Stages[QuestStage.Complete];
            var startCheck = start.Check;
            var startSay = start.Say;
            var startAct = start.Act;
            var endSay = end.Say;
            var endCheck = end.Check;

            _iw.Write("if (quest == \"\"");
            if (startCheck.IntervalMins > 0) _iw.Write($" || quest == \"{GetInfo(quest, QuestState.Completed)}\"");
            _iw.WriteLine(")");
            _iw.WriteLine("{");
            _iw.Indent++;

            bool isAsk = startSay.Yes.Count > 0 || startSay.No.Count > 0;
            for (int i = 0; i < startSay.Dialogue.Count; i++)
            {
                var line = startSay.Dialogue[i];
                if (isAsk && i == startSay.Dialogue.Count - 1) // Last line
                {
                    _iw.WriteLine($"bool askStart = AskYesNo(\"{EscapeStr(line.Dialogue)}\");");
                    _iw.WriteLine("if (!askStart)");
                    _iw.WriteLine("{");
                    _iw.Indent++;
                    foreach (var noLine in startSay.No)
                    {
                        Say(noLine);
                    }
                    _iw.WriteLine("return;");
                    _iw.Indent--;
                    _iw.WriteLine("}");
                }
                else
                {
                    Say(line);
                }
            }

            if (startAct.Mesos != 0 || startAct.Items.Count > 0)
            {
                _iw.WriteLineNoTabs("");
                _iw.WriteLine("var itemExchange = new List<(int ItemID, int Amount)>();");
                if (startAct.Items.Count > 0)
                {
                    foreach (var item in startAct.Items)
                    {
                        var ifConditions = new List<string>();
                        if (item.Job > 0)
                        {
                            ifConditions.Add("(" + string.Join(" || ", GetJobConditions(item.Job)) + ")");
                        }
                        if (item.Gender == PlayerGender.Female || item.Gender == PlayerGender.Male)
                        {
                            ifConditions.Add("chr.Gender == " + item.Gender);
                        }
                        if (ifConditions.Count > 0)
                        {
                            _iw.Write("if (");
                            _iw.Write(string.Join(" && ", ifConditions));
                            _iw.WriteLine(")");
                            _iw.WriteLine("{");
                            _iw.Indent++;
                            _iw.WriteLine($"itemExchange.Add(({item.ItemID}, {item.Amount}));");
                            _iw.Indent--;
                            _iw.WriteLine("}");
                        }
                        else
                        {
                            _iw.WriteLine($"itemExchange.Add(({item.ItemID}, {item.Amount}));");
                        }
                    }
                }
                _iw.WriteLine("if (!Exchange(" + startAct.Mesos + ", itemExchange.SelectMany(i => new[] { i.ItemID, i.Amount }).ToArray()))");
                _iw.WriteLine("{");
                _iw.Indent++;
                Say("Please make sure you have enough space in your inventory!");
                _iw.WriteLine("return;");
                _iw.Indent--;
                _iw.WriteLine("}");
            }

            _iw.WriteLineNoTabs("");

            // Start quest
            _iw.WriteLine($"SetQuestData({quest.ScriptQuestID}, \"{GetInfo(quest, QuestState.InProgress)}\");");

            // Quest started dialogue
            foreach (var yesLine in startSay.Yes)
            {
                Say(yesLine);
            }

            _iw.WriteLine("return;");
            _iw.Indent--;
            _iw.WriteLine("}");

            // Talking to wrong npc to complete quest
            if (endSay.Stop.ContainsKey("npc") && endCheck.NpcID != _npcID)
            {
                _iw.WriteLine("else");
                _iw.WriteLine("{");
                _iw.Indent++;
                WriteStop(endSay, "npc");
                _iw.WriteLine("return;");
                _iw.Indent--;
                _iw.WriteLine("}");
            }
        }

        private void ActCompleteQuest(WzQuest quest)
        {
            var start = quest.Stages[QuestStage.Start];
            var end = quest.Stages[QuestStage.Complete];
            var startCheck = start.Check;
            var startSay = start.Say;
            var startAct = start.Act;
            var endSay = end.Say;
            var endAct = end.Act;
            var endCheck = end.Check;

            // Quest started
            _iw.WriteLine($"if (quest != \"\")");
            _iw.WriteLine("{");
            _iw.Indent++;

            // Check lost item
            if (endSay.Lost.Count > 0)
            {
                if (!_questItems.TryGetValue(quest.QuestID, out var qItems))
                {
                    Console.WriteLine($"Quest {quest.ScriptQuestID} has \"lost\" prop but no quest items recorded!");
                }
                else
                {
                    var questItems = endCheck.Items.Where(endItem => qItems.Contains(endItem.Key));
                    _iw.WriteLine("var lostExchange = new List<int>();");
                    foreach (var qi in questItems.Select(i => i.Value))
                    {
                        _iw.WriteLine($"if (ItemCount({qi.ItemID}) < {qi.Amount})");
                        _iw.WriteLine("{");
                        _iw.Indent++;
                        _iw.WriteLine($"lostExchange.Add({qi.ItemID});");
                        _iw.WriteLine($"lostExchange.Add({qi.Amount});");
                        _iw.Indent--;
                        _iw.WriteLine("}");
                    }

                    _iw.WriteLine("if (lostExchange.Count > 0)");
                    _iw.WriteLine("{");
                    _iw.Indent++;

                    if (endSay.Lost.TryGetValue("say", out var lostSays))
                    {
                        foreach (var lostSay in lostSays)
                        {
                            Say(lostSay);
                        }
                    }

                    _iw.WriteLine("if (!Exchange(0, lostExchange.ToArray()))");
                    _iw.WriteLine("{");
                    _iw.Indent++;
                    Say("Please make sure you have enough space in your inventory!");
                    _iw.WriteLine("return;");
                    _iw.Indent--;
                    _iw.WriteLine("}");

                    if (endSay.Lost.TryGetValue("yes", out var lostYeses))
                    {
                        foreach (var lostYes in lostYeses)
                        {
                            Say(lostYes);
                        }
                    }

                    _iw.WriteLine("return;");

                    _iw.Indent--;
                    _iw.WriteLine("}");
                }
            }

            // Check quest is complete
            string mobEndInfo = GetMobEndInfo(endCheck);
            if (!string.IsNullOrWhiteSpace(mobEndInfo))
            {
                _iw.WriteLine($"if (quest != \"{mobEndInfo}\")");
                _iw.WriteLine("{");
                _iw.Indent++;
                WriteStop(endSay, "mob");
                _iw.WriteLine("return;");

                _iw.Indent--;
                _iw.WriteLine("}");
            }

            // Check has items
            if (endCheck.Items.Count > 0)
            {
                _iw.Write("if (");
                _iw.Write(string.Join(" || ", endCheck.Items.Select(i => $"ItemCount({i.Value.ItemID}) < {i.Value.Amount}")));
                _iw.WriteLine(")");
                _iw.WriteLine("{");
                _iw.Indent++;
                    WriteStop(endSay, "item");
                    _iw.WriteLine("return;");
                _iw.Indent--;
                _iw.WriteLine("}");
            }

            // Check if prerequisites are completed
            if (endCheck.Quests.Count > 0)
            {
                _iw.Write("if (");
                var qs = new List<string>();
                foreach (var sq in startCheck.Quests)
                {
                    qs.Add($"GetQuestData({sq.ScriptQuestID}) != \"{GetInfo(quest, QuestState.Completed)}\"");
                }
                _iw.Write(string.Join(" || ", qs));
                _iw.WriteLine(")");
                _iw.WriteLine("{");
                _iw.Indent++;
                WriteStop(endSay, "quest");
                _iw.WriteLine("return;");
                _iw.Indent--;
                _iw.WriteLine("}");
            }

            // Item exchange
            bool isSelect = endAct.Items.Any(i => i.Prop == -1);
            bool hasProp = endAct.Items.Any(i => i.Prop > 0);
            if (endAct.Items.Count > 0)
            {
                _iw.WriteLine("var itemExchange = new List<(int ItemID, int Amount)>();");
                if (hasProp)
                {
                    _iw.WriteLine("var rndRewards = new List<(int ItemID, int Amount, int Prop)>();");
                }
                if (isSelect)
                {
                    _iw.WriteLine("var selectRewards = new List<(int ItemID, int Amount)>();");
                    _iw.WriteLine("var selectOptions = new List<string>();");
                }
                foreach (var item in endAct.Items)
                {
                    var ifConditions = new List<string>();
                    if (item.Job > 0)
                    {
                        ifConditions.Add("(" + string.Join(" || ", GetJobConditions(item.Job)) + ")");
                    }
                    if (item.Gender == PlayerGender.Female || item.Gender == PlayerGender.Male)
                    {
                        ifConditions.Add("chr.Gender == " + item.Gender);
                    }
                    if (ifConditions.Count > 0)
                    {
                        _iw.Write("if (");
                        _iw.Write(string.Join(" && ", ifConditions));
                        _iw.WriteLine(")");
                        _iw.WriteLine("{");
                        _iw.Indent++;
                    }

                    if (item.Prop > 0)
                    {
                        _iw.WriteLine($"rndRewards.Add(({item.ItemID},{item.Amount}, {item.Prop}));");
                    }
                    else if (item.Prop == -1)
                    {
                        _iw.WriteLine($"selectRewards.Add(({item.ItemID},{item.Amount}));");
                        _iw.WriteLine($"selectOptions.Add(\" #t{item.ItemID}#\");");
                    }
                    else
                    {
                        _iw.WriteLine($"itemExchange.Add(({item.ItemID}, {item.Amount}));");
                    }

                    if (ifConditions.Count > 0)
                    {
                        _iw.Indent--;
                        _iw.WriteLine("}");
                    }
                }

                _iw.WriteLineNoTabs("");
            }

            // Dialogue yes/no
            for (int i = 0; i < endSay.Dialogue.Count; i++)
            {
                var line = endSay.Dialogue[i];
                if ((endSay.IsAsk || isSelect) && i == endSay.Dialogue.Count - 1) // Last line
                {
                    _iw.WriteLineNoTabs("");

                    if (isSelect)
                    {

                        _iw.WriteLine("if (selectOptions.Count > 0)");
                        _iw.WriteLine("{");
                        _iw.Indent++;
                        _iw.WriteLine($"int askReward = AskMenu(\"{EscapeStr(line.Dialogue)}\", selectOptions.ToArray());");
                        _iw.WriteLine($"var selectedReward = selectRewards[askReward];");
                        _iw.WriteLine($"itemExchange.Add((selectedReward.ItemID, selectedReward.Amount));");
                        _iw.Indent--;
                        _iw.WriteLine("}");
                    }
                    else
                    {
                        _iw.WriteLine($"bool askComplete = AskYesNo(\"{EscapeStr(line.Dialogue)}\");");
                        _iw.WriteLine("if (!askComplete)");
                        _iw.WriteLine("{");
                        _iw.Indent++;
                        foreach (var noLine in endSay.No)
                        {
                            Say(noLine);
                        }
                        _iw.WriteLine("return;");
                        _iw.Indent--;
                        _iw.WriteLine("}");
                    }
                }
                else
                {
                    Say(line);
                }
            }

            // Exchange
            if (endAct.Mesos > 0 || endAct.Items.Count > 0)
            {
                _iw.WriteLineNoTabs("");

                if (hasProp)
                {
                    _iw.WriteLine("int propSum = rndRewards.Sum(r => r.Prop);");
                    _iw.WriteLine("int rand = Rand32.NextBetween(0, propSum);");
                    _iw.WriteLine("int from = 0;");
                    _iw.WriteLine("int to = 0;");
                    _iw.WriteLine("foreach (var reward in rndRewards)");
                    _iw.WriteLine("{");
                    _iw.Indent++;
                        _iw.WriteLine("to += reward.Prop;");
                        _iw.WriteLine("bool win = from <= rand && rand < to;");
                        _iw.WriteLine("from += reward.Prop;");
                        _iw.WriteLine("if (win)");
                        _iw.WriteLine("{");
                        _iw.Indent++;
                            _iw.WriteLine("itemExchange.Add((reward.ItemID, reward.Amount));");
                        _iw.Indent--;
                        _iw.WriteLine("}");
                    _iw.Indent--;
                    _iw.WriteLine("}");
                }

                _iw.Write("if (!Exchange(" + endAct.Mesos + ", itemExchange.SelectMany(i => new [] { i.ItemID, i.Amount }).ToArray()");
                _iw.WriteLine("))");
                _iw.WriteLine("{");
                _iw.Indent++;
                    Say("Please make sure you have enough space in your inventory!");
                    _iw.WriteLine("return;");
                _iw.Indent--;
                _iw.WriteLine("}");
            }

            _iw.WriteLineNoTabs("");

            // Give exp etc
            if (endAct.Exp != 0) _iw.WriteLine($"AddEXP({endAct.Exp});");
            if (endAct.Fame != 0) _iw.WriteLine($"AddFame({endAct.Fame});");

            // Complete quest
            _iw.WriteLine($"SetQuestData({quest.ScriptQuestID}, \"{GetInfo(quest, QuestState.Completed)}\");");

            // Set repeatable date if any
            if (startCheck.IntervalMins > 0)
            {
                _iw.WriteLine($"SetQuestData({quest.ScriptSavedDateQuestID}, DateTime.UtcNow.ToString(\"yyyyMMddHHmm\"));");
            }

            _iw.WriteLine("QuestEndEffect();");

            foreach (var yesLine in endSay.Yes)
            {
                Say(yesLine);
            }

            // Start next quest if any
            if (endAct.NextQuest != null)
            {
                ActStartQuest(endAct.NextQuest);
            }

            _iw.WriteLine("return;");
            _iw.Indent--;
            _iw.WriteLine("}");
        }

        private string GetMobEndInfo(WzQuestCheck check)
        {
            string mobCheck = string.Join("", check.Mobs.Select(i => "000"));
            return mobCheck;
        }

        private string GetActFuncName(WzQuestInfo info)
        {
            string san = "";
            string name = info.Parent ?? info.Name;
            if (!string.IsNullOrWhiteSpace(name))
            {
                san = Regex.Replace(name, @"[^ A-Za-z\d]", "").Replace("  ", " ");
            }
            else
            {
                san = info.Quest.ScriptQuestID;
            }
            return string.Join("", san.Split(' ').Select(i => i[0].ToString().ToUpper() + i.Substring(1)));
        }
    }
}
