using MapleLib.WzLib;
using System.Text.RegularExpressions;

namespace HarepackerCLI
{
    public enum QuestState : byte
    {
        Unavailable = 0xFF,
        Available = 0,
        InProgress = 1,
        Completed = 2
    }
    public class WzQuest
    {
        public WzQuestInfo QuestInfo { get; private set; }
        public short QuestID { get; private set; }
        public string ScriptQuestID => "101" + QuestID;
        public string ScriptSavedDateQuestID => "102" + QuestID;
        public IDictionary<QuestStage, WzQuestStage> Stages { get; private set; } = new Dictionary<QuestStage, WzQuestStage>();
        public WzQuest(WzFile questWz, short questID)
        {
            var checkNode = (WzImageProperty)questWz["Check.img"][questID.ToString()];
            var questInfoNode = (WzImageProperty)questWz["QuestInfo.img"][questID.ToString()];
            QuestInfo = new WzQuestInfo(this, questInfoNode);
            foreach (var checkStage in checkNode.WzProperties)
            {
                var actStage = (WzImageProperty)questWz["Act.img"][questID.ToString()][checkStage.Name];
                var sayStage = (WzImageProperty)questWz["Say.img"][questID.ToString()][checkStage.Name];
                WzQuestStage stage = new WzQuestStage(questWz, this, checkStage, actStage, sayStage);
                Stages.Add(stage.Stage, stage);
            }
            QuestID = questID;
        }
    }
    public class WzQuestState
    {
        public short QuestID { get; private set; }
        public string ScriptQuestID => "101" + QuestID;
        public QuestState State { get; private set; }
        public WzQuestState(WzImageProperty node)
        {
            State = (QuestState)node["state"].GetInt();
            QuestID = node["id"].GetShort();
        }
    }
    public class WzSayInfo
    {
        public string Dialogue { get; private set; }
        public bool IsMenu => Menu?.Options.Count > 0;
        public WzSayMenu Menu { get; private set; }

        public WzSayInfo(string dialogue)
        {
            if (WzSayMenu.TryParse(ref dialogue, out var menu))
            {
                Menu = menu;
            }
            Dialogue = dialogue;
        }
    }
    public class WzSayMenu
    {
        public IList<string> Options { get; private set; }
        public IList<string> StopSay { get; private set; }
        public int Answer { get; private set; }

        private WzSayMenu(List<string> options)
        {
            Options = options;
            StopSay = new List<string>();
        }

        public static bool TryParse(ref string dialogue, out WzSayMenu menu)
        {
            var matches = Regex.Matches(dialogue, @"(\\r\\n)?#L\d+#(?'text'.+?)(#l)?(\\r\\n|$)", RegexOptions.Singleline);
            menu = null;
            if (matches.Count > 0)
            {
                var options = matches.Select(m => m.Groups[4].Value).ToList();
                var firstMatch = matches[0];
                var lastMatch = matches[matches.Count - 1];
                dialogue = dialogue.Remove(firstMatch.Index, (lastMatch.Index + lastMatch.Length) - firstMatch.Index);
                menu = new WzSayMenu(options);
            }
            return matches.Count > 0;
        }

        public void SetStopSay(WzImageProperty stopProp)
        {
            foreach (var prop in stopProp.WzProperties.OrderBy(p => p.Name))
            {
                if (int.TryParse(prop.Name, out int idx))
                {
                    StopSay.Add(prop.GetString());
                }
                else
                {
                    switch (prop.Name)
                    {
                        case "answer":
                            Answer = prop.GetInt() - 1;
                            break;
                        default:
                            Console.WriteLine("Unknown stop say prop with name \"" + prop.Name + "\"!");
                            break;
                    }
                }
            }
        }
    }
    public class WzQuestSay
    {
        public IDictionary<int, WzSayInfo> Dialogue { get; private set; }
        public IList<string> Yes { get; private set; }
        public IList<string> No { get; private set; }

        public IDictionary<string, IList<string>> Stop { get; private set; }
        public IDictionary<string, IList<string>> Lost { get; private set; }

        public bool IsAsk { get; private set; }
        public WzQuestStage Stage { get; }
        public WzQuestSay(WzQuestStage stage, WzImageProperty node)
        {
            Dialogue = new Dictionary<int, WzSayInfo>();
            Yes = new List<string>();
            No = new List<string>();
            Lost = new Dictionary<string, IList<string>>();
            Stop = new Dictionary<string, IList<string>>();
            Stage = stage;
            foreach (var sayProp in node.WzProperties.OrderBy(n => n.Name))
            {
                if (int.TryParse(sayProp.Name, out int sayIdx))
                {
                    Dialogue.Add(sayIdx, new WzSayInfo(sayProp.GetString()));
                }
                else
                {
                    switch (sayProp.Name)
                    {
                        case "yes":
                            Yes = GetDialogue(sayProp.WzProperties);
                            break;
                        case "no":
                            No = GetDialogue(sayProp.WzProperties);
                            break;
                        case "stop":
                            foreach (var stopProp in sayProp.WzProperties)
                            {
                                if (int.TryParse(stopProp.Name, out sayIdx))
                                {
                                    if (Dialogue.TryGetValue(sayIdx, out var sayInfo))
                                    {
                                        if (sayInfo.IsMenu)
                                        {
                                            sayInfo.Menu.SetStopSay(stopProp);
                                        }
                                        else
                                        {
                                            Console.WriteLine($"Saw stop/{sayIdx} but say is not a menu! Check {ToString()}/{sayIdx}");
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine($"Unknown say with idx \"{sayIdx}\" at stop! Check {ToString()}/{sayIdx}");
                                    }
                                }
                                else
                                {
                                    switch (stopProp.Name)
                                    {
                                        case "npc":
                                        case "mob":
                                        case "item":
                                        case "quest":
                                            Stop.Add(stopProp.Name, GetDialogue(stopProp.WzProperties));
                                            break;
                                        default:
                                            Console.WriteLine($"Unknown stop prop \"{stopProp.Name}\"! Check {ToString()}/stop/{stopProp.Name}");
                                            break;
                                    }
                                }
                            }
                            break;
                        case "lost":
                            Lost.Add("say", GetDialogue(sayProp.WzProperties));
                            foreach (var lostProp in sayProp.WzProperties)
                            {
                                switch (lostProp.Name)
                                {
                                    case "yes":
                                        Lost.Add(lostProp.Name, GetDialogue(lostProp.WzProperties));
                                        break;
                                    default:
                                        if (!int.TryParse(lostProp.Name, out int _))
                                            Console.WriteLine($"Unknown lost prop \"{lostProp.Name}\"! Check {ToString()}/lost/{lostProp.Name}");
                                        break;
                                }
                            }
                            break;
                        case "ask":
                            IsAsk = sayProp.GetInt() == 1;
                            break;
                        default:
                            Console.WriteLine($"Unknown say prop \"{sayProp.Name}\"! Check {ToString()}/{sayProp.Name}");
                            break;
                    }
                }
            }
        }

        private IList<string> GetDialogue(WzPropertyCollection props)
        {
            var dialogue = new List<string>();
            foreach (var prop in props.OrderBy(p => p.Name))
            {
                if (int.TryParse(prop.Name, out int lostIdx))
                {
                    dialogue.Add(prop.GetString());
                }
            }
            return dialogue;
        }

        public override string ToString()
        {
            return "Say/" + Stage.Quest.QuestID + "/" + (byte)Stage.Stage;
        }
    }
    public class WzQuestAct
    {
        public WzQuestStage Stage { get; }
        public WzQuest NextQuest { get; }
        public int Exp { get; }
        public int Mesos { get; }
        public short Fame { get; }
        /// <summary>
        /// Important that it's a list rather than a dictionary. Needs to be indexed correctly for item selection.
        /// </summary>
        public List<QuestItem> Items { get; }

        public WzQuestAct(WzFile questWz, WzQuestStage stage, WzImageProperty node)
        {
            Items = new List<QuestItem>();
            Stage = stage;
            foreach (var subNode in node.WzProperties)
            {
                switch (subNode.Name)
                {
                    case "money":
                        Mesos = subNode.GetInt();
                        break;
                    case "exp":
                        Exp = subNode.GetInt();
                        break;
                    case "item":
                        foreach (var itemNode in subNode.WzProperties)
                        {
                            var item = new QuestItem(stage, itemNode);
                            Items.Add(item);
                        }
                        break;
                    case "pop":
                        Fame = subNode.GetShort();
                        break;
                    case "nextQuest":
                        NextQuest = new WzQuest(questWz, subNode.GetShort());
                        break;
                    default:
                        break;
                }
            }
        }
    }
    public class WzQuestCheck
    {
        public WzQuestStage Stage { get; private set; }
        public List<WzQuestState> Quests { get; private set; } = new List<WzQuestState>();
        public IDictionary<int, QuestMob> Mobs { get; private set; } = new Dictionary<int, QuestMob>();
        public IDictionary<int, QuestItem> Items { get; private set; } = new Dictionary<int, QuestItem>();
        public int NpcID { get; private set; }
        public List<short> Jobs { get; private set; } = new List<short>();
        public int Mesos { get; private set; }
        public int LvMin { get; private set; }
        public int LvMax { get; private set; }
        public string End { get; private set; }
        public short Fame { get; private set; }
        public int IntervalMins { get; private set; }
        public WzQuestCheck(WzQuestStage stage, WzImageProperty node)
        {
            Stage = stage;
            foreach (var subNode in node.WzProperties)
            {
                switch (subNode.Name)
                {
                    case "mob":
                        foreach (var mobNode in subNode.WzProperties)
                        {
                            var questMob = new QuestMob(mobNode);
                            Mobs.Add(questMob.MobID, questMob);
                        }
                        break;
                    case "quest":
                        foreach (var qtNode in subNode.WzProperties)
                        {
                            var trigger = new WzQuestState(qtNode);
                            Quests.Add(trigger);
                        }
                        break;
                    case "item":
                        foreach (var itemNode in subNode.WzProperties)
                        {
                            var item = new QuestItem(stage, itemNode);
                            Items.Add(item.ItemID, item);
                        }
                        break;
                    case "job":
                        foreach (var itemNode in subNode.WzProperties)
                        {
                            Jobs.Add(itemNode.GetShort());
                        }
                        break;
                    case "npc":
                        NpcID = subNode.GetInt();
                        break;
                    case "lvmin":
                        LvMin = subNode.GetInt();
                        break;
                    case "lvmax":
                        LvMax = subNode.GetInt();
                        break;
                    case "money":
                        Mesos = subNode.GetInt();
                        break;
                    case "end":
                        End = subNode.GetString();
                        break;
                    case "pop":
                        Fame = subNode.GetShort();
                        break;
                    case "interval":
                        IntervalMins = subNode.GetInt();
                        break;
                    default:
                        break;
                }
            }
        }
    }
    public class WzQuestInfo
    {
        public WzQuest Quest { get; private set; }
        public IDictionary<byte, string> Stages { get; private set; } = new Dictionary<byte, string>();
        public int Area { get; private set; }
        public string Name { get; private set; }
        public int Order { get; private set; }
        public string Parent { get; private set; }
        public WzQuestInfo(WzQuest quest, WzImageProperty node)
        {
            Quest = quest;
            foreach (var subnode in node.WzProperties)
            {
                switch (subnode.Name)
                {
                    case "area":
                        Area = subnode.GetInt();
                        break;
                    case "name":
                        Name = subnode.GetString();
                        break;
                    case "order":
                        Order = subnode.GetInt();
                        break;
                    case "parent":
                        Parent = subnode.GetString();
                        break;
                    default:
                        if (byte.TryParse(subnode.Name, out byte stage))
                        {
                            Stages[stage] = subnode.GetString();
                        }
                        break;
                }
            }
        }
    }
    public enum QuestStage : byte
    {
        Start = 0,
        Complete = 1
    }
    public class WzQuestStage
    {
        public QuestStage Stage { get; private set; }
        public WzQuest Quest { get; private set; }
        public WzQuestCheck Check { get; private set; }
        public WzQuestAct Act { get; private set; }
        public WzQuestSay Say { get; private set; }
        public WzQuestStage(WzFile questWz, WzQuest quest, WzImageProperty checkNode, WzImageProperty actNode, WzImageProperty sayNode)
        {
            Stage = (QuestStage)byte.Parse(checkNode.Name);
            Quest = quest;
            Check = new WzQuestCheck(this, checkNode);
            Act = new WzQuestAct(questWz, this, actNode);
            Say = new WzQuestSay(this, sayNode);
        }
    }
    [Flags]
    public enum QuestJob : byte
    {
        None = 0,
        Beginner = 1,
        Warrior = 2,
        Magician = 4,
        Bowman = 8,
        Thief = 16,
        GM = 0xFF
    }
    public enum PlayerGender : byte
    {
        NotApplicable = 0xFF,
        Male = 0,
        Female = 1,
        Unisex = 2,
        Unset = 10
    }
    public class QuestItem
    {
        public int ItemID { get; }
        public short Amount { get; }
        /// <summary>
        /// The chance to get the item, -1 means it is a selectable item in the dialog menu.
        /// </summary>
        public int Prop { get; }
        public PlayerGender Gender { get; } = PlayerGender.NotApplicable;
        public QuestJob Job { get; }
        public WzQuestStage Stage { get; set; }
        public QuestItem(WzQuestStage stage, WzImageProperty node)
        {
            Stage = stage;
            foreach (var subNode in node.WzProperties)
            {
                switch (subNode.Name)
                {
                    case "count":
                        Amount = subNode.GetShort();
                        break;
                    case "id":
                        ItemID = subNode.GetInt();
                        break;
                    case "prop":
                        Prop = subNode.GetInt();
                        break;
                    case "gender":
                        Gender = (PlayerGender)subNode.GetInt();
                        break;
                    case "job":
                        Job = (QuestJob)subNode.GetInt();
                        break;
                }
            }
        }
    }
    public class QuestMob
    {
        public int ID { get; }
        public int MobID { get; }
        public int Count { get; }
        public QuestMob(WzImageProperty node)
        {
            ID = int.Parse(node.Name);
            MobID = node["id"].GetInt();
            Count = node["count"].GetInt();
        }
    }
}
