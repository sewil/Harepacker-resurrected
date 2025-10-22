using MapleLib.WzLib;

namespace HarepackerCLI
{
    internal class WzUtils
    {
        public enum ItemTypes
        {
            ArmorHelm = 100,
            AccessoryFace = 101,
            AccessoryEye = 102,
            AccessoryEarring = 103,
            ArmorTop = 104,
            ArmorOverall = 105,
            ArmorBottom = 106,
            ArmorShoe = 107,
            ArmorGlove = 108,
            ArmorShield = 109,
            ArmorCape = 110,
            ArmorRing = 111,
            ArmorPendant = 112,
            Medal = 114,
            Weapon1hSword = 130,
            Weapon1hAxe = 131,
            Weapon1hMace = 132,
            WeaponDagger = 133,
            WeaponWand = 137,
            WeaponStaff = 138,
            Weapon2hSword = 140,
            Weapon2hAxe = 141,
            Weapon2hMace = 142,
            WeaponSpear = 143,
            WeaponPolearm = 144,
            WeaponBow = 145,
            WeaponCrossbow = 146,
            WeaponClaw = 147,
            WeaponSkillFX = 160,
            WeaponCash = 170,
            PetEquip = 180,
            PetSkills = 181,

            ItemPotion = 200,
            ItemSpecialPotion = 201, // Like drakes blood and poisonous mushroom
            ItemFood = 202,
            ItemReturnScroll = 203,
            ItemScroll = 204,
            ItemCure = 205,
            ItemArrow = 206,
            ItemStar = 207,
            ItemMegaPhone = 208,
            ItemWeather = 209,
            ItemSummonBag = 210,
            ItemPetTag = 211,
            ItemPetFood = 212,
            ItemMessageBox = 213,
            ItemMesoSack = 214,
            ItemJukebox = 215,
            ItemNote = 216,
            ItemTeleportRock = 217,
            ItemAPSPReset = 218,
            ItemMonsterBook = 238,

            Pet = 500,
            SetupEventItem = 399,
            EtcMetal = 401,
            EtcMineral = 402,
            EtcEmote = 404,
            EtcCoupon = 405,
            EtcStorePermit = 406,
            EtcWaterOfLife = 407,
            EtcOmokSet = 408,
            EtcChocolate = 409,
            EtcEXPCoupon = 410,
            EtcGachaponTicket = 411,
            EtcSafetyCharm = 412,
            EtcForging = 413,
        }
        public static byte GetInventory(int itemid) => (byte)(itemid / 1000000);
        public static ItemTypes GetItemType(int itemid) => (ItemTypes)(itemid / 10000);
        public static WzObject GetItem(WzFile characterWz, WzFile itemWz, int itemID)
        {
            string path = "";
            WzObject obj;
            switch (GetInventory(itemID))
            {
                case 1:
                    switch (GetItemType(itemID))
                    {
                        case ItemTypes.ArmorHelm:
                            path = "Cap";
                            break;
                        case ItemTypes.AccessoryFace:
                        case ItemTypes.AccessoryEye:
                        case ItemTypes.AccessoryEarring:
                            path = "Accessory";
                            break;
                        case ItemTypes.ArmorTop:
                            path = "Coat";
                            break;
                        case ItemTypes.ArmorOverall:
                            path = "Longcoat";
                            break;
                        case ItemTypes.ArmorBottom:
                            path = "Pants";
                            break;
                        case ItemTypes.ArmorShoe:
                            path = "Shoes";
                            break;
                        case ItemTypes.ArmorGlove:
                            path = "Glove";
                            break;
                        case ItemTypes.ArmorShield:
                            path = "Shield";
                            break;
                        case ItemTypes.ArmorCape:
                            path = "Cape";
                            break;
                        case ItemTypes.ArmorRing:
                            path = "Ring";
                            break;
                        case ItemTypes.Weapon1hSword:
                        case ItemTypes.Weapon1hAxe:
                        case ItemTypes.Weapon1hMace:
                        case ItemTypes.WeaponDagger:
                        case ItemTypes.WeaponWand:
                        case ItemTypes.WeaponStaff:
                        case ItemTypes.Weapon2hSword:
                        case ItemTypes.Weapon2hAxe:
                        case ItemTypes.Weapon2hMace:
                        case ItemTypes.WeaponSpear:
                        case ItemTypes.WeaponPolearm:
                        case ItemTypes.WeaponBow:
                        case ItemTypes.WeaponCrossbow:
                        case ItemTypes.WeaponClaw:
                        case ItemTypes.WeaponSkillFX:
                        case ItemTypes.WeaponCash:
                            path = "Weapon";
                            break;
                        case ItemTypes.PetEquip:
                            path = "PetEquip";
                            break;
                        default:
                            break;
                    }
                    return characterWz[path][$"{itemID.ToString().PadLeft(8, '0')}.img"];
                case 2:
                    path = $"Consume/{itemID.ToString().Substring(0, 3).PadLeft(4, '0')}.img/{itemID.ToString().PadLeft(8, '0')}";
                    break;
                case 3:
                    path = $"Install/{itemID.ToString().Substring(0, 3).PadLeft(4, '0')}.img/{itemID.ToString().PadLeft(8, '0')}";
                    break;
                case 4:
                    path = $"Etc/{itemID.ToString().Substring(0, 3).PadLeft(4, '0')}.img/{itemID.ToString().PadLeft(8, '0')}";
                    break;
                case 5:
                    path = $"Pet/{itemID}.img";
                    break;
                default:
                    break;
            }
            obj = itemWz;
            foreach (var item in path.Split("/"))
            {
                obj = obj[item];
            }
            return obj;
        }
        public static void ParseWzFile(WzFile wzFile)
        {
            WzFileParseStatus parseStatus = wzFile.ParseWzFile();
            if (parseStatus != WzFileParseStatus.Success)
            {
                Console.WriteLine("Wz file parse error \"" + parseStatus.GetErrorDescription() + "\" at " + wzFile.FilePath);
                Environment.Exit(1);
            }
        }
    }
}
