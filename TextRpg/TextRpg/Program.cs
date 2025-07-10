using System;


namespace TxtRpg
{
    enum ItemType
    {
        Weapon,
        Armor
    }

    struct Item
    {
        public string Name;
        public string Description;
        public int AttackBonus;
        public int DefenseBonus;
        public int Price;
        public bool IsEquipped;
        public ItemType Type;

        public Item(string name, int atk, int def, string desc, int price, ItemType type)
        {
            Name = name;
            AttackBonus = atk;
            DefenseBonus = def;
            Description = desc;
            Price = price;
            IsEquipped = false;
            Type = type;
        }

        public void ShowItem(bool owned)
        {
            string equipMark = IsEquipped ? "[E]" : "   ";
            string priceText = owned ? "구매완료" : Price + " G";
            Console.WriteLine($"{equipMark} {Name.PadRight(10)} | 공격력 +{AttackBonus} | 방어력 +{DefenseBonus} | {Description.PadRight(30)} | {priceText}");
        }
    }

    class Character
    {
        public int Level = 1;
        public string Name;
        public string Job;

        public float BaseAttack = 10;
        public float BaseDefense = 5;
        public int HP = 100;
        public int Gold = 800;

        public int DungeonClearCount = 0;

        public Item[] Inventory = new Item[100];
        public int InventoryCount = 0;

        public int GetTotalAttackBonus()
        {
            int total = 0;
            for (int i = 0; i < InventoryCount; i++)
            {
                if (Inventory[i].IsEquipped)
                {
                    total += Inventory[i].AttackBonus;
                }
            }
            return total;
        }

        public int GetTotalDefenseBonus()
        {
            int total = 0;
            for (int i = 0; i < InventoryCount; i++)
            {
                if (Inventory[i].IsEquipped)
                {
                    total += Inventory[i].DefenseBonus;
                }
            }
            return total;
        }

        public float GetAttack()
        {
            return BaseAttack + GetTotalAttackBonus();
        }

        public float GetDefense()
        {
            return BaseDefense + GetTotalDefenseBonus();
        }

        public void IncreaseDungeonClearCount()
        {
            //던전 클리어 횟수가 현재 레벨보다 같거나 많으면 레벨업 시켜줌 
            DungeonClearCount++;
            if (DungeonClearCount >= Level)
            {
                Level++;
                DungeonClearCount = 0;
                BaseAttack += 0.5f;
                BaseDefense += 1.0f;
                Console.WriteLine($"레벨업! Lv.{Level}이 되었습니다!");
                Console.WriteLine("기본 공격력 +0.5, 기본 방어력 +1 증가!");
            }
        }
        public void SellItem(int index)
        {
            if (index < 0 || index >= InventoryCount)
            {
                Console.WriteLine("잘못된 아이템 번호입니다.");
                return;
            }

            Item itemToSell = Inventory[index];

            if (itemToSell.IsEquipped)
            {
                itemToSell.IsEquipped = false;
                Inventory[index] = itemToSell;  // 구조체라 다시 넣어줘야 변경 내용이 반영됨
            }

            int sellPrice = (int)(itemToSell.Price * 0.85);
            Gold += sellPrice;

            for (int i = index; i < InventoryCount - 1; i++)
            {
                Inventory[i] = Inventory[i + 1];
            }
            InventoryCount--;

            Console.WriteLine($"{itemToSell.Name} 을(를) {sellPrice} G에 판매했습니다.");
        }

        public void Rest()
        {
            const int restCost = 500;
            if (Gold >= restCost)
            {
                Gold -= restCost;
                HP = 100;
                Console.WriteLine($"휴식을 완료했습니다. (골드 -{restCost}G)");
            }
            else
            {
                Console.WriteLine("Gold가 부족합니다.");
            }
        }

        public void ShowStatus()
        {
            Console.WriteLine("Lv. " + Level.ToString("00")); //두자리 숫자로 표현
            Console.WriteLine(Name + " ( " + Job + " )"); // 이름과 직업을 한 줄로 보기 편하게

            int attackBonus = GetTotalAttackBonus();
            if (attackBonus > 0)
                Console.WriteLine("공격력 : " + GetAttack().ToString("0.0") + " (" + BaseAttack.ToString("0.0") + " + " + attackBonus + ")");
            else
                Console.WriteLine("공격력 : " + GetAttack().ToString("0.0"));

            int defenseBonus = GetTotalDefenseBonus();
            if (defenseBonus > 0)
                Console.WriteLine("방어력 : " + GetDefense().ToString("0.0") + " (" + BaseDefense.ToString("0.0") + " + " + defenseBonus + ")");
            else
                Console.WriteLine("방어력 : " + GetDefense().ToString("0.0"));

            Console.WriteLine("체 력 : " + HP);
            Console.WriteLine("Gold : " + Gold + " G\n");
        }

        public void ShowInventory()
        {
            Console.WriteLine("인벤토리");
            Console.WriteLine("보유 중인 아이템을 관리할 수 있습니다.\n");

            Console.WriteLine("[아이템 목록]");

            if (InventoryCount == 0)
            {
                Console.WriteLine("아이템이 없습니다.");
            }
            else
            {
                for (int i = 0; i < InventoryCount; i++)
                {
                    Console.Write((i + 1) + ". ");
                    Inventory[i].ShowItem(true);
                }
            }

            Console.WriteLine("\n1. 장착 관리");
            Console.WriteLine("0. 나가기");
            Console.Write("원하시는 행동을 입력해주세요.\n>> ");
        }

        public void ManageEquip()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("장착 관리\n");
                Console.WriteLine("[아이템 목록]");

                if (InventoryCount == 0)
                {
                    Console.WriteLine("아이템이 없습니다.");
                    Console.WriteLine("0. 나가기");
                    Console.Write(">> ");
                    string input = Console.ReadLine();
                    if (input == "0")
                        break;
                    else
                        continue;
                }

                for (int i = 0; i < InventoryCount; i++)
                {
                    Console.Write((i + 1) + ". ");
                    Inventory[i].ShowItem(true);
                }

                Console.WriteLine("0. 나가기");
                Console.Write("장착/해제할 아이템 번호를 입력하세요.\n>> ");
                string choice = Console.ReadLine();

                if (choice == "0")
                    break;

                int selectedIndex;
                bool isNumber = int.TryParse(choice, out selectedIndex);

                if (!isNumber)
                {
                    Console.WriteLine("숫자를 입력해주세요.");
                    Console.ReadLine();
                    continue;
                }

                if (selectedIndex < 1 || selectedIndex > InventoryCount)
                {
                    Console.WriteLine("잘못된 번호입니다.");
                    Console.ReadLine();
                    continue;
                }

                int index = selectedIndex - 1;  //배열은 0번 부터 시작해서 -1
                Item selectedItem = Inventory[index];

                if (selectedItem.IsEquipped)
                {

                    selectedItem.IsEquipped = false;
                    Inventory[index] = selectedItem;
                    Console.WriteLine($"{selectedItem.Name} 장착 해제됨.");
                }
                else
                {
                    // 같은 타입의 장착 중인 아이템이 있는지 확인하고 해제
                    for (int i = 0; i < InventoryCount; i++)
                    {
                        if (Inventory[i].IsEquipped && Inventory[i].Type == selectedItem.Type)
                        {
                            Item equippedItem = Inventory[i];
                            equippedItem.IsEquipped = false;
                            Inventory[i] = equippedItem;
                            Console.WriteLine($"기존 {equippedItem.Type} 아이템 [{equippedItem.Name}] 장착 해제됨.");
                        }
                    }


                    selectedItem.IsEquipped = true;
                    Inventory[index] = selectedItem;
                    Console.WriteLine($"{selectedItem.Name} 장착 완료.");
                }

                Console.WriteLine("계속하려면 엔터...");
                Console.ReadLine();
            }
        }

        public bool HasItem(string itemName)
        {
            for (int i = 0; i < InventoryCount; i++)
            {
                if (Inventory[i].Name == itemName)
                    return true;
            }
            return false;
        }

        // 인벤토리에 특정 이름의 아이템이 있는지 확인하는 메서드
        public void BuyItem(Item item)
        {
            if (Gold >= item.Price)
            {
                Gold -= item.Price;
                if (InventoryCount < Inventory.Length)
                {
                    Inventory[InventoryCount++] = item;
                    Console.WriteLine(item.Name + " 구매 완료!");
                }
                else
                {
                    Console.WriteLine("인벤토리가 가득 찼습니다.");
                }
            }
            else
            {
                Console.WriteLine("Gold가 부족합니다.");
            }
        }
    }

    class Program
    {
        static void EnterDungeon(Character player)
        {
            Console.Clear();
            Console.WriteLine("입장할 던전을 선택하세요:\n");
            Console.WriteLine("1. 쉬움 던전 (권장 방어력: 5이상)");
            Console.WriteLine("2. 일반 던전 (권장 방어력: 11이상)");
            Console.WriteLine("3. 어려운 던전 (권장 방어력: 17이상)");
            Console.WriteLine("0. 나가기");
            Console.Write(">> ");
            string input = Console.ReadLine();

            int requiredDefense = 0;
            int baseReward = 0;
            string dungeonName = "";

            switch (input)
            {
                case "1":
                    requiredDefense = 5;
                    baseReward = 1000;
                    dungeonName = "쉬움 던전";
                    break;
                case "2":
                    requiredDefense = 11;
                    baseReward = 1700;
                    dungeonName = "일반 던전";
                    break;
                case "3":
                    requiredDefense = 17;
                    baseReward = 2500;
                    dungeonName = "어려운 던전";
                    break;
                case "0":
                    return;
                default:
                    Console.WriteLine("잘못된 입력입니다.");
                    Console.ReadLine();
                    return;
            }

            Console.Clear();
            Console.WriteLine($"{dungeonName}에 입장합니다!\n");

            int playerDefense = (int)player.GetDefense(); //GetDefense가 float 반환형이기 때문에 
            int playerAttack = (int)player.GetAttack();  //(int)를 써서 float를 int로 강제 변환(소수점내림)
            Random rand = new Random();                    // 확률 계산에 편리해서 정수형으로 변환

            if (playerDefense < requiredDefense)
            {

                if (rand.Next(100) < 40) // 0~99 랜덤한 정수 중 값이 40 미만이면 실패 즉 40프로 확률
                {
                    int lostHP = player.HP / 2;
                    player.HP -= lostHP;
                    Console.WriteLine("던전 실패! 체력이 절반 감소했습니다.");
                    Console.WriteLine($"체력 -{lostHP}");
                    Console.ReadLine();
                    return;
                }
            }

            // 성공 처리
            int diff = requiredDefense - playerDefense;
            int hpLossMin = 20 + Math.Max(diff, 0);
            int hpLossMax = 35 + Math.Max(diff, 0);
            int hpLoss = rand.Next(hpLossMin, hpLossMax + 1);

            player.HP -= hpLoss;
            if (player.HP < 0) player.HP = 0;


            int bonusPercent = rand.Next(playerAttack, playerAttack * 2 + 1); //공격력이 15라면 15%이상 31%미만
            int bonusGold = baseReward * bonusPercent / 100;
            int totalGold = baseReward + bonusGold;

            player.Gold += totalGold;

            Console.WriteLine("던전 클리어!");
            Console.WriteLine($"체력 {hpLoss} 감소");
            Console.WriteLine($"기본 보상: {baseReward} G");
            Console.WriteLine($"추가 보상: {bonusGold} G (공격력 기반)");
            Console.WriteLine($"총 보상: {totalGold} G");

            player.IncreaseDungeonClearCount();

            Console.ReadLine();
        }


        static Item[] ShopItems = new Item[]
        {
            new Item("수련자 갑옷", 0, 5, "수련에 도움을 주는 갑옷입니다.", 1000, ItemType.Armor),
            new Item("무쇠갑옷", 0, 9, "무쇠로 만들어져 튼튼한 갑옷입니다.", 2000, ItemType.Armor),
            new Item("스파르타의 갑옷", 0, 15, "스파르타의 전사들이 사용했다는 전설의 갑옷입니다.", 3500, ItemType.Armor),
            new Item("낡은 검", 2, 0, "쉽게 볼 수 있는 낡은 검 입니다.", 600, ItemType.Weapon),
            new Item("청동 도끼", 5, 0, "어디선가 사용됐던거 같은 도끼입니다.", 1500, ItemType.Weapon),
            new Item("스파르타의 창", 7, 0, "스파르타의 전사들이 사용했다는 전설의 창입니다.", 2500, ItemType.Weapon),
            new Item("청룡 언월도", 50, 0, "용의 힘과 위엄을 상징하는 강력한 장병기입니다", 45000, ItemType.Weapon),
            new Item("청룡의 갑옷", 0, 50, "관우가 착용했다 전해지는 전설의 갑옷입니다", 45000, ItemType.Armor),
            new Item("백호신갑", 0, 50, "조운이 착용했다 전해지는 전설의 갑옷입니다", 45000, ItemType.Armor)
        };

        static void ShowSellMenu(Character player)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("아이템 판매\n");

                if (player.InventoryCount == 0)
                {
                    Console.WriteLine("판매할 아이템이 없습니다.");
                    Console.WriteLine("0. 나가기");
                    Console.Write(">> ");
                    string input = Console.ReadLine();
                    if (input == "0")
                        break;
                    else
                        continue;
                }

                Console.WriteLine("[판매 가능한 아이템 목록]");
                for (int i = 0; i < player.InventoryCount; i++)
                {
                    Console.Write((i + 1) + ". ");
                    player.Inventory[i].ShowItem(true);
                }
                Console.WriteLine("0. 나가기");
                Console.Write("판매할 아이템 번호를 입력하세요.\n>> ");

                string choice = Console.ReadLine();
                if (choice == "0")
                    break;

                if (!int.TryParse(choice, out int index))
                {
                    Console.WriteLine("숫자를 입력해주세요.");
                    Console.ReadLine();
                    continue;
                }

                if (index < 1 || index > player.InventoryCount)
                {
                    Console.WriteLine("잘못된 번호입니다.");
                    Console.ReadLine();
                    continue;
                }

                player.SellItem(index - 1);
                Console.WriteLine("계속하려면 엔터...");
                Console.ReadLine();
            }
        }

        static void ShowShop(Character player)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("상점");
                Console.WriteLine("필요한 아이템을 얻을 수 있는 상점입니다.\n");
                Console.WriteLine("[보유 골드]");
                Console.WriteLine(player.Gold + " G\n");

                Console.WriteLine("[아이템 목록]");

                for (int i = 0; i < ShopItems.Length; i++)
                {
                    Item shopItem = ShopItems[i];
                    bool owned = player.HasItem(shopItem.Name);

                    Console.Write((i + 1) + ". ");
                    shopItem.ShowItem(owned);
                }

                Console.WriteLine("\n1. 아이템 구매");
                Console.WriteLine("2. 아이템 판매");
                Console.WriteLine("0. 나가기");
                Console.Write("원하시는 행동을 입력해주세요.\n>> ");
                string input = Console.ReadLine();

                if (input == "0")
                {
                    break;
                }
                else if (input == "1")
                {
                    Console.Write("구매할 아이템 번호를 입력하세요.\n>> ");
                    string choice = Console.ReadLine();
                    if (!int.TryParse(choice, out int index))
                    {
                        Console.WriteLine("숫자를 입력해주세요.");
                        Console.ReadLine();
                        continue;
                    }

                    if (index < 1 || index > ShopItems.Length)
                    {
                        Console.WriteLine("잘못된 번호입니다.");
                        Console.ReadLine();
                        continue;
                    }

                    Item selectedItem = ShopItems[index - 1];
                    if (player.HasItem(selectedItem.Name))
                    {
                        Console.WriteLine("이미 구매한 아이템입니다.");
                    }
                    else
                    {
                        player.BuyItem(selectedItem);
                    }
                    Console.WriteLine("계속하려면 엔터...");
                    Console.ReadLine();
                }
                else if (input == "2")
                {
                    ShowSellMenu(player);
                }
                else
                {
                    Console.WriteLine("잘못된 입력입니다.");
                    Console.ReadLine();
                }
            }
        }

        static void Main(string[] args)
        {
            Console.WriteLine("스파르타 마을에 오신 여러분 환영합니다.");
            Console.WriteLine("이곳에서 던전으로 들어가기전 활동을 할 수 있습니다.\n");

            Console.Write("원하시는 이름을 설정해주세요: ");
            string playerName = Console.ReadLine();
            Console.WriteLine("입력하신 이름은 " + playerName + " 입니다.\n");

            string job = "";

            while (true)
            {
                Console.WriteLine("직업을 선택하세요:");
                Console.WriteLine("1. 전사");
                Console.WriteLine("2. 마법사");
                Console.WriteLine("3. 궁수");
                Console.WriteLine("4. 도적");
                Console.Write("번호를 입력하세요: ");

                string jobInput = Console.ReadLine();

                switch (jobInput)
                {
                    case "1":
                        job = "전사";
                        Console.WriteLine("전사를 선택하셨습니다.");
                        break;
                    case "2":
                        job = "마법사";
                        Console.WriteLine("마법사를 선택하셨습니다.");
                        break;
                    case "3":
                        job = "궁수";
                        Console.WriteLine("궁수를 선택하셨습니다.");
                        break;
                    case "4":
                        job = "도적";
                        Console.WriteLine("도적을 선택하셨습니다.");
                        break;
                    default:
                        Console.WriteLine("잘못된 입력입니다. 다시 선택해주세요.\n");
                        continue;
                }
                break;
            }

            Character player = new Character();
            player.Name = playerName;
            player.Job = job;

            //기본 아이템
            player.Inventory[player.InventoryCount++] = new Item("무쇠갑옷", 0, 5, "무쇠로 만들어져 튼튼한 갑옷입니다.", 2000, ItemType.Armor);
            player.Inventory[player.InventoryCount++] = new Item("스파르타의 창", 7, 0, "스파르타의 전사들이 사용했다는 전설의 창입니다.", 2500, ItemType.Weapon);
            player.Inventory[player.InventoryCount++] = new Item("낡은 검", 2, 0, "쉽게 볼 수 있는 낡은 검 입니다.", 600, ItemType.Weapon);

            while (true)
            {
                Console.WriteLine("\n1. 상태 보기");
                Console.WriteLine("2. 인벤토리");
                Console.WriteLine("3. 상점");
                Console.WriteLine("4. 던전입장");
                Console.WriteLine("5. 휴식하기");
                Console.WriteLine("0. 종료");
                Console.Write("원하시는 행동을 입력해주세요: ");

                string input = Console.ReadLine();

                if (input == "1")
                {
                    Console.Clear();
                    player.ShowStatus();
                }
                else if (input == "2")
                {
                    Console.Clear();
                    player.ShowInventory();
                    string invInput = Console.ReadLine();

                    if (invInput == "1")
                    {
                        player.ManageEquip();
                    }
                }
                else if (input == "3")
                {
                    ShowShop(player);
                }
                else if (input == "4")
                {
                    Console.Clear();
                    EnterDungeon(player);
                }
                else if (input == "5")
                {
                    Console.Clear();
                    Console.WriteLine("휴식하기");
                    Console.WriteLine($"500 G를 내면 체력을 회복할 수 있습니다. (보유 골드 : {player.Gold} G)\n");
                    Console.WriteLine("1. 휴식하기");
                    Console.WriteLine("0. 나가기");
                    Console.Write("원하시는 행동을 입력해주세요.\n>> ");

                    string restInput = Console.ReadLine();

                    if (restInput == "1")
                    {
                        player.Rest();
                        Console.WriteLine("계속하려면 엔터...");
                        Console.ReadLine();
                    }
                }
                else if (input == "0")
                {
                    Console.WriteLine("게임을 종료합니다.");
                    break;
                }
                else
                {
                    Console.WriteLine("잘못된 입력입니다.");
                }
            }
        }
    }
}
