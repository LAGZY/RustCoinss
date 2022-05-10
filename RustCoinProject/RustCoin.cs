using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Facepunch.Extend;
using Newtonsoft.Json;
using Oxide.Core;
using Oxide.Core.Libraries;
using Oxide.Core.Libraries.Covalence;
using Oxide.Core.Plugins;
using Oxide.Game.Rust.Cui;
using UnityEngine;
using UnityEngine.UI;

namespace Oxide.Plugins
{
    [Info("RustCoin", "LAGZYA feat fermens and megargan", "1.0.49")]
    public class RustCoin : RustPlugin
    {
        [PluginReference] Plugin ImageLibrary;
        
        
        
        #region Configuration
        private static Configuration _config = new Configuration();

        public class Configuration
        {
            [JsonProperty("Включить товары на сервере?")]
            public bool isShopWorking { get; set; } = false;

            [JsonProperty("Товары на сервере")] 
            public Dictionary<int, Shop> Tovars = new Dictionary<int,Shop>();

            public class Shop
            {
                [JsonProperty("Картинка")] public string Image;
                [JsonProperty("Название")] public string Name;
                [JsonProperty("Цена")] public double Cost;
                [JsonProperty("Исполняемая команда([STEAMID] будет заменено на ID купившего)")] public string Command;
            }

            public static Configuration GetNewConfiguration()
            {
                return new Configuration
                {
                    Tovars = new Dictionary<int, Shop>
                    {
                        {
                            1, new Shop
                            {
                                Image = "https://imgur.com/B846zP5.png",
                                Name = "VIP",
                                Cost = 4000.01,
                                Command = "say [STEAMID] красавчик"
                            }
                        },
                        {
                            2, new Shop
                            {
                                Image = "https://imgur.com/B846zP5.png",
                                Name = "PREM",
                                Cost = 80000.1,
                                Command = "say [STEAMID] мегакрасавчик"
                            }
                        }
                    }
                };
            }
        }

        protected override void LoadConfig()
        {
            base.LoadConfig();
            try
            {
                _config = Config.ReadObject<Configuration>();
                if (_config == null) LoadDefaultConfig();
            }
            catch
            {
                Puts("!!!!ОШИБКА КОНФИГУРАЦИИ!!!! Проверьте парамеры конфига!");
            }

            NextTick(SaveConfig);
        }

        protected override void LoadDefaultConfig() => _config = Configuration.GetNewConfiguration();
        protected override void SaveConfig() => Config.WriteObject(_config);

        #endregion
        
        
        #region Hooks

        private void OnPlayerConnected(BasePlayer player) 
        {
            GetInfos(player);
        }

        private void OnPlayerDisconnected(BasePlayer player)
        {
            DataPlayer info;
            if (!_players.TryGetValue(player, out info)) return;
            Update(player, info.coins, info.serverid, info.upgrades);
            _players.Remove(player);
        }

        private void MediumUnload()
        {
            foreach (var coroutine in _coroutines.ToList().Where(c => c != null))
            {
                ServerMgr.Instance.StopCoroutine(coroutine);
            }

            foreach (var basePlayer in BasePlayer.activePlayerList)
            {
                OnPlayerDisconnected(basePlayer);
            }
        }

        private void Unload()
        {
            UpdateMysql();
            Debug.LogWarning("Плагин выгружается.");
            test.Shutdown();
            if (start != null) ServerMgr.Instance.StopCoroutine(start);
            foreach (var coroutine in _coroutines.ToList().Where(c => c != null))
            {
                ServerMgr.Instance.StopCoroutine(coroutine);
            }
            Debug.LogWarning("Плагин успешно выгружен.");
        }

        public string main_back = "https://imgur.com/gxOM6f8.png";
        public string main_border = "https://imgur.com/WvsSWHB.png";
        public string main_tap = "https://imgur.com/mnFRlYZ.png";
        public string main_balance = "https://imgur.com/lqwgcN9.png";
        public string main_buttons = "https://imgur.com/EMHab4A.png";
        public string main_phone = "https://imgur.com/PDwzpbG.png";
        public string main_upgrades = "https://imgur.com/5jqO4BC.png";
        public string upgrade_backimage = "https://imgur.com/7MFgOuM.png";
        public string main_top = "https://imgur.com/CR0T5ZL.png";
        public string avatar_rc = "https://i.imgur.com/pd0rGiP.png";
        public string server_players = "https://imgur.com/szQgcre.png";
        public string global_players = "https://imgur.com/KXk2pQ6.png";
        public string global_servers = "https://imgur.com/Z7k6n5B.png";
        public string transfer_main = "https://imgur.com/oNn09N4.png";
        public string upperbuttons = "https://imgur.com/z8DkjBB.png";
        public string promo_main = "https://imgur.com/oZXxwL1.png";
        public string shop_main = "https://imgur.com/1uaFQMX.png";
        public string shop_slotimg = "https://imgur.com/m9bqNN8.png";

        private Coroutine start;
        private WebRequests test = new WebRequests();
        private void OnServerInitialized()
        {
            
            StatusCheck();
            start = ServerMgr.Instance.StartCoroutine(UpdateMysql());
            if (!ImageLibrary)
            {
                Debug.LogError("[RUST-COIN] ImageLibrary не установлена!!! Плагин работать не будет!!");
                Interface.Oxide.UnloadPlugin(Name);
            }

            if (_config.isShopWorking)
            {
                foreach (Configuration.Shop shp in _config.Tovars.Values)
                {
                    ImageLibrary.Call("AddImage", shp.Image, shp.Image);
                }
            }
           
            ImageLibrary.Call("AddImage", main_back, main_back); 
            ImageLibrary.Call("AddImage", main_border, main_border); 
            ImageLibrary.Call("AddImage", main_tap, main_tap); 
            ImageLibrary.Call("AddImage", main_balance, main_balance); 
            ImageLibrary.Call("AddImage", main_buttons, main_buttons); 
            ImageLibrary.Call("AddImage", main_phone, main_phone); 
            ImageLibrary.Call("AddImage", main_upgrades, main_upgrades); 
            ImageLibrary.Call("AddImage", upgrade_backimage, upgrade_backimage);
            ImageLibrary.Call("AddImage", main_top, main_top); 
            ImageLibrary.Call("AddImage", avatar_rc, avatar_rc); 
            ImageLibrary.Call("AddImage", server_players, server_players); 
            ImageLibrary.Call("AddImage", global_players, global_players); 
            ImageLibrary.Call("AddImage", global_servers, global_servers);
            ImageLibrary.Call("AddImage",transfer_main, transfer_main);
            ImageLibrary.Call("AddImage", upperbuttons, upperbuttons);
            ImageLibrary.Call("AddImage", promo_main, promo_main);
            ImageLibrary.Call("AddImage", shop_main, shop_main);
            ImageLibrary.Call("AddImage", shop_slotimg, shop_slotimg);
            
            
            AddCovalenceCommand("RCOIN_CONS", nameof(Commands));
          
        }

        #endregion

        #region Interface

        #region VAR

        private float all_fadein = 0f;

        private string main_balance_gui_json;
        private string main_json;
        private string upgrades_json;
        private string upgrade_plate_json;
        private string upgarde_slot_json;
        private string top_json_all;
        private string top_json_server;
        private string transfer_json;
        private string promocodes_json;
        private string shop_json;
        private string shop_slot_json;
        #endregion

        void Generate()
        {
            CuiElementContainer main = new CuiElementContainer();
            CuiElementContainer main_balance_gui = new CuiElementContainer();


            main.Add(new CuiButton
            {
                RectTransform =
                {
                    AnchorMax = "1 1",
                    AnchorMin = "0 0"
                },
                Button =
                {
                    Color = "0, 0, 0, 0",
                    Command = "RCOIN_CONS main_close",
                },
                Text = {Text = ""}
            }, "Overlay", "closebutton");

            main.Add(new CuiPanel
            {
                CursorEnabled = true,
                RectTransform =
                {
                    AnchorMin = "1 0.5",
                    AnchorMax = "1 0.5",
                    OffsetMin = "-300 -216",
                    OffsetMax = "-50 216"
                },
                Image =
                {
                    Color = "0, 0, 0, 0"
                }
            }, "closebutton", "main");
            main.Add(new CuiElement
            {
                Parent = "main",
                Components =
                {
                    new CuiRawImageComponent
                    {
                        FadeIn = all_fadein,
                        Png = ImageLibrary.Call<string>("GetImage", main_back)
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0 0",
                        AnchorMax = "1 1"
                    }
                }
            });
            main.Add(new CuiElement
            {
                Parent = "main",
                Components =
                {
                    new CuiRawImageComponent
                    {
                        FadeIn = all_fadein,
                        Png = "[MAIN_AVATAR]"
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0.5 1",
                        AnchorMax = "0.5 1",
                        OffsetMax = "-50 -29",
                        OffsetMin = "-83 -60"
                    }
                }
            });
            main.Add(new CuiElement
            {
                Parent = "main",
                Components =
                {
                    new CuiRawImageComponent
                    {
                        FadeIn = all_fadein,
                        Png = ImageLibrary.Call<string>("GetImage", main_border)
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0 0",
                        AnchorMax = "1 1"
                    }
                }
            });
            main.Add(new CuiLabel
            {
                RectTransform =
                {
                    AnchorMin = "0.5 1",
                    AnchorMax = "0.5 1",
                    OffsetMin = "-50 -45",
                    OffsetMax = "80 -30"
                },
                Text =
                {
                    FadeIn = all_fadein,
                    Text = "[NICKNAME]",
                    Align = TextAnchor.MiddleLeft,
                    FontSize = 12,
                    Color = "0.5, 0.5, 0.5, 1"
                }
            }, "main");
            main.Add(new CuiLabel
            {
                RectTransform =
                {
                    AnchorMin = "0.5 1",
                    AnchorMax = "0.5 1",
                    OffsetMin = "-50 -60",
                    OffsetMax = "80 -45"
                },
                Text =
                {
                    FadeIn = all_fadein,
                    Text = "ID:[ID]",
                    Align = TextAnchor.MiddleLeft,
                    FontSize = 12
                }
            }, "main");
            main.Add(new CuiLabel
            {
                RectTransform =
                {
                    AnchorMin = "0.5 1",
                    AnchorMax = "0.5 1",
                    OffsetMin = "0 -60",
                    OffsetMax = "80 -45"
                },
                Text =
                {
                    FadeIn = all_fadein,
                    Text = "TOP:[TOP_POSITION]",
                    Align = TextAnchor.MiddleLeft,
                    FontSize = 12
                }
            }, "main");
            main.Add(new CuiElement
            {
                Parent = "main",
                Components =
                {
                    new CuiRawImageComponent
                    {
                        FadeIn = all_fadein,
                        Png = ImageLibrary.Call<string>("GetImage", main_tap)
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0.5 0.5",
                        AnchorMax = "0.5 0.5",
                        OffsetMin = "-60 0",
                        OffsetMax = "60 120"
                    }
                }
            });
            main.Add(new CuiElement
            {
                Parent = "main",
                Name = "balance",
                Components =
                {
                    new CuiRawImageComponent
                    {
                        FadeIn = all_fadein,
                        Png = ImageLibrary.Call<string>("GetImage", main_balance)
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0.5 0.5",
                        AnchorMax = "0.5 0.5",
                        OffsetMin = "-70 -50",
                        OffsetMax = "70 -25"
                    }
                }
            });


            main.Add(new CuiElement
            {
                Parent = "main",
                Components =
                {
                    new CuiRawImageComponent
                    {
                        FadeIn = all_fadein,
                        Png = ImageLibrary.Call<string>("GetImage", main_buttons)
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0.5 0",
                        AnchorMax = "0.5 0",
                        OffsetMin = "-95 20",
                        OffsetMax = "87 76"
                    }
                }
            });
            main.Add(new CuiElement
            {
                Parent = "main",
                Name = "Phone",
                Components =
                {
                    new CuiRawImageComponent
                    {
                        FadeIn = all_fadein,
                        Png = ImageLibrary.Call<string>("GetImage", main_phone)
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0 0",
                        AnchorMax = "1 1"
                    }
                }
            });
            main.Add(new CuiButton
            {
                RectTransform =
                {
                    AnchorMin = "0.5 0.5",
                    AnchorMax = "0.5 0.5",
                    OffsetMin = "-60 0",
                    OffsetMax = "60 120"
                },
                Button =
                {
                    Color = "0, 0, 0, 0",
                    Command = "RCOIN_CONS RCOIN_CLICK.MAINADD"
                },
                Text = {Text = ""}
            }, "Phone");

            main.Add(new CuiButton
            {
                RectTransform =
                {
                    AnchorMin = "0.5 0",
                    AnchorMax = "0.5 0",
                    OffsetMin = "-30 20",
                    OffsetMax = "25 75"
                },
                Button =
                {
                    Color = "0, 0, 0, 0",
                    Command = "RCOIN_CONS OPEN UPGRADES 0"
                },
                Text = {Text = ""}
            }, "Phone");
            main.Add(new CuiButton
            {
                RectTransform =
                {
                    AnchorMin = "0.5 0",
                    AnchorMax = "0.5 0",
                    OffsetMin = "-95 20",
                    OffsetMax = "-40 75"
                },
                Button =
                {
                    Color = "0, 0, 0, 0",
                    Command = "RCOIN_CONS OPEN TOP 2"
                },
                Text = {Text = ""}
            }, "Phone");
            main.Add(new CuiButton
            {
                RectTransform =
                {
                    AnchorMin = "0.5 0",
                    AnchorMax = "0.5 0",
                    OffsetMin = "30 20",
                    OffsetMax = "90 75"
                },
                Button =
                {
                    Color = "0, 0, 0, 0",
                    Command = "RCOIN_CONS OPEN TRANSFER"
                },
                Text = {Text = ""}
            }, "Phone");
            main.Add(new CuiElement
            {
                Parent = "Phone",
                Name = "UPPER_BUTTONS",
                Components =
                {
                    new CuiRawImageComponent
                    {
                        Png = GetImage(upperbuttons)
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0.5 0.5",
                        AnchorMax = "0.5 0.5",
                        OffsetMin = "-95 -130",
                        OffsetMax = "90 -95"
                    }
                }
            });
            main.Add(new CuiButton
            {
                RectTransform =
                {
                    AnchorMin = "0 0.5",
                    AnchorMax = "0 0.5",
                    OffsetMin = "7 -10",
                    OffsetMax = "90 10"
                    
                },
                Button =
                {
                    Color = "0 0 0 0",
                    Command = "RCOIN_CONS OPEN PROMO"
                },
                Text = { Text = ""}
            }, "UPPER_BUTTONS");
            main.Add(new CuiButton
            {
                RectTransform =
                {
                    AnchorMin = "1 0.5",
                    AnchorMax = "1 0.5",
                    OffsetMin = "-90 -10",
                    OffsetMax = "-5 10"
                    
                },
                Button =
                {
                    Color = "0 0 0 0",
                    Command = "RCOIN_CONS OPEN SHOP 0"
                },
                Text = { Text = ""}
            }, "UPPER_BUTTONS");

            main_balance_gui.Add(new CuiLabel
            {
                RectTransform =
                {
                    AnchorMin = "0.5 0.5",
                    AnchorMax = "0.5 0.5",
                    OffsetMin = "-60 -10",
                    OffsetMax = "40 10"
                },
                Text =
                {
                    Text = "[BALANCE]",
                    Align = TextAnchor.MiddleCenter,
                }
            }, "balance", "balance_amount");


            main_json = main.ToJson();

            main_balance_gui_json = main_balance_gui.ToJson();
            GenerateTransfer();
        }

        private List<Coroutine> _coroutines = new List<Coroutine>();

        void GenerateUpgrades()
        {
            CuiElementContainer upgrades_main = new CuiElementContainer();
            CuiElementContainer upgrade_slot = new CuiElementContainer();
            CuiElementContainer upgrade_plate = new CuiElementContainer();
            foreach (var info in _upgrades)
            {
                ImageLibrary.Call("AddImage", info.Value.url, info.Value.url);
            }

            upgrades_main.Add(new CuiPanel
            {
                RectTransform =
                {
                    AnchorMin = "1 0.5",
                    AnchorMax = "1 0.5",
                    OffsetMin = "-300 -216",
                    OffsetMax = "-50 216"
                },
                CursorEnabled = true,
                Image =
                {
                    Color = "0, 0, 0, 0"
                }
            }, "closebutton", "Upgrades");

            upgrades_main.Add(new CuiElement
            {
                Parent = "Upgrades",
                Components =
                {
                    new CuiRawImageComponent
                    {
                        Png = ImageLibrary.Call<string>("GetImage", main_upgrades)
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0 0",
                        AnchorMax = "1 1"
                    }
                }
            });
            upgrades_main.Add(new CuiElement
            {
                Parent = "Upgrades",
                Name = "Upgrade_plate",
                Components =
                {
                    new CuiRawImageComponent
                    {
                        Png = ImageLibrary.Call<string>("GetImage", main_phone)
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0 0",
                        AnchorMax = "1 1",
                    }
                }
            });
            upgrade_slot.Add(new CuiElement
            {
                Parent = "Upgrade_plate",
                Name = $"Upgrade_slot[ENC]",
                Components =
                {
                    new CuiRawImageComponent
                    {
                        Png = ImageLibrary.Call<string>("GetImage", upgrade_backimage)
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0.5 1",
                        AnchorMax = "0.5 1",
                        OffsetMin = "[OMIN]",
                        OffsetMax = "[OMAX]"
                    }
                }
            });
            upgrade_slot.Add(new CuiElement
            {
                Parent = $"Upgrade_slot[ENC]",
                Components =
                {
                    new CuiRawImageComponent
                    {
                        Png = "[IMAGE]"
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0.5 0.5",
                        AnchorMax = "0.5 0.5",
                        OffsetMin = "-80 -2",
                        OffsetMax = "-60 18"
                    }
                }
            });

            upgrade_slot.Add(new CuiLabel //LABEL
            {
                RectTransform =
                {
                    AnchorMin = "0.5 0.5",
                    AnchorMax = "0.5 0.5",
                    OffsetMin = "-55 3",
                    OffsetMax = "35 17"
                },
                Text =
                {
                    Text = "[NAME]",
                    Align = TextAnchor.MiddleLeft,
                    FontSize = 12
                }
            }, $"Upgrade_slot[ENC]");
            upgrade_slot.Add(new CuiLabel //HOWADD
            {
                RectTransform =
                {
                    AnchorMin = "0.5 0.5",
                    AnchorMax = "0.5 0.5",
                    OffsetMin = "-75 -17",
                    OffsetMax = "0 -5"
                },
                Text =
                {
                    Text = "[HOWADD]" + "RC/1m",
                    Align = TextAnchor.MiddleCenter,
                    FontSize = 10,
                    Color = "0, 1, 0, 1"
                }
            }, $"Upgrade_slot[ENC]");
            upgrade_slot.Add(new CuiLabel //COST
            {
                RectTransform =
                {
                    AnchorMin = "0.5 0.5",
                    AnchorMax = "0.5 0.5",
                    OffsetMin = "5 -16",
                    OffsetMax = "63 -3"
                },
                Text =
                {
                    Text = "[COST]",
                    Align = TextAnchor.MiddleCenter,
                    FontSize = 10,
                }
            }, $"Upgrade_slot[ENC]");
            upgrade_slot.Add(new CuiLabel //COUNT
            {
                RectTransform =
                {
                    AnchorMin = "0.5 0.5",
                    AnchorMax = "0.5 0.5",
                    OffsetMin = "45 3",
                    OffsetMax = "75 17"
                },
                Text =
                {
                    Text = "[PLAYER_LEVEL]",
                    Align = TextAnchor.MiddleCenter,
                    FontSize = 10
                }
            }, $"Upgrade_slot[ENC]");
            upgrade_slot.Add(new CuiButton
            {
                RectTransform =
                {
                    AnchorMin = "0 0",
                    AnchorMax = "1 1"
                },
                Button =
                {
                    Color = "0, 0, 0, 0",
                    Command = "RCOIN_CONS UPGRADES BUY [ID] [ENC]"
                },
                Text = {Text = ""}
            }, $"Upgrade_slot[ENC]");


            upgrade_plate.Add(new CuiButton
            {
                RectTransform =
                {
                    AnchorMin = "0 1",
                    AnchorMax = "0 1",
                    OffsetMin = "30 -35",
                    OffsetMax = "70 -10"
                },
                Text = {Text = ""},
                Button =
                {
                    Color = "0, 0, 0, 0",
                    Command = "RCOIN_CONS HOME",
                }
            }, "Upgrade_plate");
            upgrade_plate.Add(new CuiButton
            {
                RectTransform =
                {
                    AnchorMin = "1 0",
                    AnchorMax = "1 0",
                    OffsetMin = "-70 15",
                    OffsetMax = "-30 45"
                },
                Text = {Text = ""},
                Button =
                {
                    Color = "0, 0, 0, 0",
                    Command = "RCOIN_CONS OPEN UPGRADES [PAGE_NEXT]",
                }
            }, "Upgrade_plate");
            upgrade_plate.Add(new CuiButton
            {
                RectTransform =
                {
                    AnchorMin = "0 0",
                    AnchorMax = "0 0",
                    OffsetMin = "25 15",
                    OffsetMax = "65 50"
                },
                Text = {Text = ""},
                Button =
                {
                    Color = "0, 0, 0, 0",
                    Command = "RCOIN_CONS OPEN UPGRADES [PAGE_PREV]",
                }
            }, "Upgrade_plate");
            upgrade_plate.Add(new CuiLabel
            {
                Text =
                {
                    Text = "[PAGE]",
                    Align = TextAnchor.MiddleCenter,
                    FontSize = 18
                },
                RectTransform =
                {
                    AnchorMin = "0.5 0",
                    AnchorMax = "0.5 0",
                    OffsetMin = "-20 15",
                    OffsetMax = "20 47"
                }
            }, "Upgrade_plate");

            upgrades_json = upgrades_main.ToJson();
            upgrade_plate_json = upgrade_plate.ToJson();
            upgarde_slot_json = upgrade_slot.ToJson();
            GenratePromocodes();
        }

        void GenerateTop()
        {
            CuiElementContainer top_plate_all = new CuiElementContainer();
            CuiElementContainer top_plate_server = new CuiElementContainer();
            CuiElementContainer top_plate_servers_global = new CuiElementContainer();
            top_plate_all.Add(new CuiPanel
            {
                CursorEnabled = true,
                RectTransform =
                {
                    AnchorMin = "1 0.5",
                    AnchorMax = "1 0.5",
                    OffsetMin = "-300 -216",
                    OffsetMax = "-50 216"
                },
                Image =
                {
                    Color = "0, 0, 0, 0"
                }
            }, "closebutton", "Top_plate");
            int i = 0;
            foreach (var x in _topAllPlayers)
            {
                ImageLibrary.Call("GetPlayerAvatar", x.steamid.ToString());
                top_plate_all.Add(new CuiElement
                {
                    Parent = "Top_plate",
                    Components =
                    {
                        new CuiRawImageComponent
                        {
                            Png = GetImage(x.steamid.ToString())
                        },
                        new CuiRectTransformComponent
                        {
                            AnchorMin = "0 1",
                            AnchorMax = "0 1",
                            OffsetMin = $"30 {-109 - (35 * i)}",
                            OffsetMax = $"64 {-74 - (35 * i)}"
                        }
                    }
                });
                i++;
            }

            if (i < 8)
            {
                for (int j = i; j < 8; j++)
                {
                    top_plate_all.Add(new CuiElement
                    {
                        Parent = "Top_plate",
                        Components =
                        {
                            new CuiRawImageComponent
                            {
                                Png = GetImage(avatar_rc)
                            },
                            new CuiRectTransformComponent
                            {
                                AnchorMin = "0 1",
                                AnchorMax = "0 1",
                                OffsetMin = $"30 {-109 - (35 * j)}",
                                OffsetMax = $"64 {-74 - (35 * j)}"
                            }
                        }
                    });
                }
            }

            top_plate_all.Add(new CuiElement
            {
                Parent = "Top_plate",
                Name = "Top_main",
                Components =
                {
                    new CuiRawImageComponent
                    {
                        Png = GetImage(main_top)
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0 0",
                        AnchorMax = "1 1"
                    }
                }
            });
            int i1 = 0;
            foreach (var x in _topAllPlayers)
            {
                top_plate_all.Add(new CuiPanel
                {
                    RectTransform =
                    {
                        AnchorMin = "0.5 1",
                        AnchorMax = "0.5 1",
                        OffsetMin = $"-85 {-110 - (35 * i1)}",
                        OffsetMax = $"85 {-75 - (35 * i1)}"
                    },
                    Image = {Color = "0, 0, 0, 0"}
                }, "Top_main", $"Top_slot{i1}");
                top_plate_all.Add(new CuiLabel
                {
                    RectTransform =
                    {
                        AnchorMin = "0.5 0",
                        AnchorMax = "0.5 0",
                        OffsetMin = "-60 23",
                        OffsetMax = "60 35"
                    },
                    Text =
                    {
                        Text = x.name,
                        Align = TextAnchor.UpperLeft,
                        FontSize = 10,
                    }
                }, $"Top_slot{i1}");
                i++;
                top_plate_all.Add(new CuiLabel
                {
                    RectTransform =
                    {
                        AnchorMin = "0.5 0",
                        AnchorMax = "0.5 0",
                        OffsetMin = "-60 7",
                        OffsetMax = "-20 20"
                    },
                    Text =
                    {
                        Text = x.id.ToString("00000"),
                        Align = TextAnchor.MiddleCenter,
                        FontSize = 10,
                    }
                }, $"Top_slot{i1}");
                top_plate_all.Add(new CuiLabel
                {
                    RectTransform =
                    {
                        AnchorMin = "0.5 0",
                        AnchorMax = "0.5 0",
                        OffsetMin = "-15 7",
                        OffsetMax = "60 20"
                    },
                    Text =
                    {
                        Text = x.coins.ToString("0.000"),
                        Align = TextAnchor.MiddleCenter,
                        FontSize = 10,
                    }
                }, $"Top_slot{i1}");
                i1++;
            }

            top_plate_all.Add(new CuiElement
            {
                Parent = "Top_plate",
                Name = "Top_phone",
                Components =
                {
                    new CuiRawImageComponent
                    {
                        Png = GetImage(main_phone)
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0 0",
                        AnchorMax = "1 1"
                    }
                }
            });
            top_plate_all.Add(new CuiButton
            {
                RectTransform =
                {
                    AnchorMin = "0 1",
                    AnchorMax = "0 1",
                    OffsetMin = "20 -37",
                    OffsetMax = "70 -5"
                },
                Text = {Text = ""},
                Button =
                {
                    Color = "0, 0, 0, 0",
                    Command = "RCOIN_CONS HOME"
                }
            }, "Top_phone");


            top_plate_all.Add(new CuiElement //SERVER PLAYERS
            {
                Parent = "Top_phone",
                Name = "SERVER_PLAYERS",
                Components =
                {
                    new CuiRawImageComponent
                    {
                        Png = GetImage(server_players)
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0.5 0",
                        AnchorMax = "0.5 0",
                        OffsetMin = "-92 15",
                        OffsetMax = "-36 48"
                    }
                }
            });
            top_plate_all.Add(new CuiButton
            {
                RectTransform =
                {
                    AnchorMax = "1 1",
                    AnchorMin = "0 0"
                },
                Text = {Text = ""},
                Button =
                {
                    Color = "0, 0, 0, 0",
                    Command = "RCOIN_CONS OPEN TOP 1"
                }
            }, "SERVER_PLAYERS");
            top_plate_all.Add(new CuiElement //GLOBAL PLAYERS
            {
                Parent = "Top_phone",
                Name = "GLOBAL_PLAYERS",
                Components =
                {
                    new CuiRawImageComponent
                    {
                        Png = GetImage(global_players),
                        Color = "0.5 0.5 0.5 1"
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0.5 0",
                        AnchorMax = "0.5 0",
                        OffsetMin = "-32 15",
                        OffsetMax = "25 48"
                    }
                }
            });
            top_plate_all.Add(new CuiButton
            {
                RectTransform =
                {
                    AnchorMax = "1 1",
                    AnchorMin = "0 0"
                },
                Text = {Text = ""},
                Button =
                {
                    Color = "0, 0, 0, 0",
                    Command = ""
                }
            }, "GLOBAL_PLAYERS");
            top_plate_all.Add(new CuiElement //SERVERS_TOP
            {
                Parent = "Top_phone",
                Name = "SERVERS_TOP",
                Components =
                {
                    new CuiRawImageComponent
                    {
                        Png = GetImage(global_servers),
                        Color = "1 0.5 0.5 1"
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0.5 0",
                        AnchorMax = "0.5 0",
                        OffsetMin = "30 15",
                        OffsetMax = "87 48"
                    }
                }
            });
            top_plate_all.Add(new CuiButton
            {
                RectTransform =
                {
                    AnchorMax = "1 1",
                    AnchorMin = "0 0"
                },
                Text = {Text = ""},
                Button =
                {
                    Color = "0, 0, 0, 0",
                    Command = "RCOIN_CONS OPEN TOP 3"
                }
            }, "SERVERS_TOP");
            top_json_all = top_plate_all.ToJson();
/////////SERVER_TOP/////////SERVER_TOP/////SERVER_TOP/////SERVER_TOP//////SERVER_TOP////////SERVER_TOP/////////SERVER_TOP///////SERVER_TOP///////SERVER_TOP//////SERVER_TOP///////////SERVER_TOP///////
            top_plate_server.Add(new CuiPanel
            {
                CursorEnabled = true,
                RectTransform =
                {
                    AnchorMin = "1 0.5",
                    AnchorMax = "1 0.5",
                    OffsetMin = "-300 -216",
                    OffsetMax = "-50 216"
                },
                Image =
                {
                    Color = "0, 0, 0, 0"
                }
            }, "closebutton", "Top_plate");
            int ii = 0;
            foreach (var x in _topServer)
            {
                ImageLibrary.Call("GetPlayerAvatar", x.steamid.ToString());
                top_plate_server.Add(new CuiElement
                {
                    Parent = "Top_plate",
                    Components =
                    {
                        new CuiRawImageComponent
                        {
                            Png = GetImage(x.steamid.ToString())
                        },
                        new CuiRectTransformComponent
                        {
                            AnchorMin = "0 1",
                            AnchorMax = "0 1",
                            OffsetMin = $"30 {-109 - (35 * ii)}",
                            OffsetMax = $"64 {-74 - (35 * ii)}"
                        }
                    }
                });
                ii++;
            }

            if (ii < 8)
            {
                for (int j = ii; j < 8; j++)
                {
                    top_plate_server.Add(new CuiElement
                    {
                        Parent = "Top_plate",
                        Components =
                        {
                            new CuiRawImageComponent
                            {
                                Png = GetImage(avatar_rc)
                            },
                            new CuiRectTransformComponent
                            {
                                AnchorMin = "0 1",
                                AnchorMax = "0 1",
                                OffsetMin = $"30 {-109 - (35 * j)}",
                                OffsetMax = $"64 {-74 - (35 * j)}"
                            }
                        }
                    });
                }
            }

            top_plate_server.Add(new CuiElement
            {
                Parent = "Top_plate",
                Name = "Top_main",
                Components =
                {
                    new CuiRawImageComponent
                    {
                        Png = GetImage(main_top)
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0 0",
                        AnchorMax = "1 1"
                    }
                }
            });
            int ii1 = 0;
            foreach (var x in _topServer)
            {
                top_plate_server.Add(new CuiPanel
                {
                    RectTransform =
                    {
                        AnchorMin = "0.5 1",
                        AnchorMax = "0.5 1",
                        OffsetMin = $"-85 {-110 - (35 * ii1)}",
                        OffsetMax = $"85 {-75 - (35 * ii1)}"
                    },
                    Image = {Color = "0, 0, 0, 0"}
                }, "Top_main", $"Top_slot{ii1}");
                top_plate_server.Add(new CuiLabel
                {
                    RectTransform =
                    {
                        AnchorMin = "0.5 0",
                        AnchorMax = "0.5 0",
                        OffsetMin = "-60 23",
                        OffsetMax = "60 35"
                    },
                    Text =
                    {
                        Text = x.name,
                        Align = TextAnchor.UpperLeft,
                        FontSize = 10,
                    }
                }, $"Top_slot{ii1}");
                i++;
                top_plate_server.Add(new CuiLabel
                {
                    RectTransform =
                    {
                        AnchorMin = "0.5 0",
                        AnchorMax = "0.5 0",
                        OffsetMin = "-60 7",
                        OffsetMax = "-20 20"
                    },
                    Text =
                    {
                        Text = x.id.ToString("00000"),
                        Align = TextAnchor.MiddleCenter,
                        FontSize = 10,
                    }
                }, $"Top_slot{ii1}");
                top_plate_server.Add(new CuiLabel
                {
                    RectTransform =
                    {
                        AnchorMin = "0.5 0",
                        AnchorMax = "0.5 0",
                        OffsetMin = "-15 7",
                        OffsetMax = "60 20"
                    },
                    Text =
                    {
                        Text = x.coins.ToString("0.000"),
                        Align = TextAnchor.MiddleCenter,
                        FontSize = 10,
                    }
                }, $"Top_slot{ii1}");
                ii1++;
            }

            top_plate_server.Add(new CuiElement
            {
                Parent = "Top_plate",
                Name = "Top_phone",
                Components =
                {
                    new CuiRawImageComponent
                    {
                        Png = GetImage(main_phone)
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0 0",
                        AnchorMax = "1 1"
                    }
                }
            });
            top_plate_server.Add(new CuiButton
            {
                RectTransform =
                {
                    AnchorMin = "0 1",
                    AnchorMax = "0 1",
                    OffsetMin = "20 -37",
                    OffsetMax = "70 -5"
                },
                Text = {Text = ""},
                Button =
                {
                    Color = "0, 0, 0, 0",
                    Command = "RCOIN_CONS HOME"
                }
            }, "Top_phone");


            top_plate_server.Add(new CuiElement //SERVER PLAYERS
            {
                Parent = "Top_phone",
                Name = "SERVER_PLAYERS",
                Components =
                {
                    new CuiRawImageComponent
                    {
                        Png = GetImage(server_players),
                        Color = "0.5 0.5 0.5 1"
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0.5 0",
                        AnchorMax = "0.5 0",
                        OffsetMin = "-92 15",
                        OffsetMax = "-36 48"
                    }
                }
            });
            top_plate_server.Add(new CuiButton
            {
                RectTransform =
                {
                    AnchorMax = "1 1",
                    AnchorMin = "0 0"
                },
                Text = {Text = ""},
                Button =
                {
                    Color = "0, 0, 0, 0",
                    Command = ""
                }
            }, "SERVER_PLAYERS");
            top_plate_server.Add(new CuiElement //GLOBAL PLAYERS
            {
                Parent = "Top_phone",
                Name = "GLOBAL_PLAYERS",
                Components =
                {
                    new CuiRawImageComponent
                    {
                        Png = GetImage(global_players)
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0.5 0",
                        AnchorMax = "0.5 0",
                        OffsetMin = "-32 15",
                        OffsetMax = "25 48"
                    }
                }
            });
            top_plate_server.Add(new CuiButton
            {
                RectTransform =
                {
                    AnchorMax = "1 1",
                    AnchorMin = "0 0"
                },
                Text = {Text = ""},
                Button =
                {
                    Color = "0, 0, 0, 0",
                    Command = "RCOIN_CONS OPEN TOP 2"
                }
            }, "GLOBAL_PLAYERS");
            top_plate_server.Add(new CuiElement //SERVERS_TOP
            {
                Parent = "Top_phone",
                Name = "SERVERS_TOP",
                Components =
                {
                    new CuiRawImageComponent
                    {
                        Png = GetImage(global_servers),
                        Color = "1 0.5 0.5 1"
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0.5 0",
                        AnchorMax = "0.5 0",
                        OffsetMin = "30 15",
                        OffsetMax = "87 48"
                    }
                }
            });
            top_plate_server.Add(new CuiButton
            {
                RectTransform =
                {
                    AnchorMax = "1 1",
                    AnchorMin = "0 0"
                },
                Text = {Text = ""},
                Button =
                {
                    Color = "0, 0, 0, 0",
                    Command = "RCOIN_CONS OPEN TOP 3"
                }
            }, "SERVERS_TOP");
            top_json_server = top_plate_server.ToJson();
        }

        void GenerateTransfer()
        {
            CuiElementContainer transfer = new CuiElementContainer();
            transfer.Add(new CuiPanel
            {
                CursorEnabled = true,
                
                RectTransform =
                {
                    AnchorMin = "1 0.5",
                    AnchorMax = "1 0.5",
                    OffsetMin = "-300 -216",
                    OffsetMax = "-50 216"
                },
                Image = {Color = "0 0 0 0"}
            }, "closebutton", "Transfer_main");
            transfer.Add(new CuiElement
            {
                Parent = "Transfer_main",
                Name = "Transfer_plate",
                Components =
                {
                    new CuiRawImageComponent
                    {
                        Png = GetImage(transfer_main)
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0 0",
                        AnchorMax = "1 1"
                    }
                }
            });
            transfer.Add(new CuiButton
            {
            RectTransform =
            {
                AnchorMin = "0 1",
                AnchorMax = "0 1",
                OffsetMin = "30 -40",
                OffsetMax = "70 -10"
            },
            Text = {Text = ""},
            Button =
            {
                Color = "0 0 0 0", 
                Command = "RCOIN_CONS HOME"
            }
            },"Transfer_plate");
            transfer.Add(new CuiLabel
            {
                RectTransform =
                {
                    AnchorMin = "0.5 1",
                    AnchorMax = "0.5 1",
                    OffsetMin = "-70 -137",
                    OffsetMax = "37 -113"
                },
                Text =
                {
                    Align = TextAnchor.MiddleCenter,
                    Text = "[BALANCE]"
                }
                
            },"Transfer_plate");
            transfer.Add(new CuiElement
            {
                Parent = "Transfer_plate",
                Components =
                {
                    new CuiInputFieldComponent
                    {
                        LineType = InputField.LineType.MultiLineSubmit,
                        Align = TextAnchor.MiddleCenter,
                        CharsLimit = 5,
                        Command = "RCOIN_CONS TRANSFER addid "
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0.5 0.5",
                        AnchorMax = "0.5 0.5",
                        OffsetMin = "-75 -10",
                        OffsetMax = "70 20"
                    }
                }
            });
            
            transfer.Add(new CuiElement
            {
                Parent = "Transfer_plate",
                Components =
                {
                    new CuiInputFieldComponent
                    {
                        Command = "RCOIN_CONS TRANSFER addamount ",
                        LineType = InputField.LineType.MultiLineSubmit,
                        Align = TextAnchor.MiddleCenter,
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0.5 0.5",
                        AnchorMax = "0.5 0.5",
                        OffsetMin = "-75 -80",
                        OffsetMax = "70 -50"
                    }
                }
            });
            transfer.Add(new CuiButton
            {
                
                    Button = 
                    {
                        Color  = "0 0 0 0",
                        Command = "RCOIN_CONS TRANSFER sendtransfer"
                    },
                    RectTransform = 
                    {
                        AnchorMin = "0.5 0.5",
                        AnchorMax = "0.5 0.5",
                        OffsetMin = "-50 -134",
                        OffsetMax = "45 -105"
                    },
                    Text = { Text = ""}
                
            },"Transfer_plate");
            
            
            
            
            transfer_json = transfer.ToJson();

        }

        void GenratePromocodes()
        {
            CuiElementContainer promo = new CuiElementContainer();
            promo.Add(new CuiPanel
            {
                CursorEnabled = true,
                RectTransform =
                {
                    AnchorMin = "1 0.5",
                    AnchorMax = "1 0.5",
                    OffsetMin = "-300 -216",
                    OffsetMax = "-50 216"
                },
                Image =
                {
                    Color = "0, 0, 0, 0"
                }
            },"closebutton", "promo");
            promo.Add(new CuiElement
            {   
                Parent = "promo",
                Name = "promo_plate",
                Components =
                {
                    new CuiRawImageComponent
                    {
                        Png = GetImage(promo_main),
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0 0",
                        AnchorMax = "1 1"
                    }
                }
            });
            promo.Add(new CuiButton
            {
                RectTransform =
                {
                    AnchorMin = "0 1",
                    AnchorMax = "0 1",
                    OffsetMin = "30 -40",
                    OffsetMax = "70 -10"
                },
                Text = {Text = ""},
                Button =
                {
                    Color = "0 0 0 0",
                    Command = "RCOIN_CONS HOME"
                }
            },"promo_plate");
            promo.Add(new CuiElement
            {
                Parent = "promo_plate",
                Components =
                {
                    new CuiInputFieldComponent
                    {
                        Align = TextAnchor.MiddleCenter,
                        Command = "RCOIN_CONS promocode ",
                        LineType = InputField.LineType.SingleLine
                    }, 
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0.5 0.5",
                        AnchorMax = "0.5 0.5",
                        OffsetMin = "-74 45",
                        OffsetMax = "70 73"
                        
                    }
                }
            });
            promocodes_json = promo.ToJson();
            GenerateShop();
        }

        void GenerateShop()
        {
            CuiElementContainer shop = new CuiElementContainer();
            CuiElementContainer shop_slot = new CuiElementContainer();
            shop.Add(new CuiPanel
            {
                CursorEnabled = true,
                RectTransform =
                {
                    AnchorMin = "1 0.5",
                    AnchorMax = "1 0.5",
                    OffsetMin = "-300 -216",
                    OffsetMax = "-50 216"
                },
                Image =
                {
                    Color = "0, 0, 0, 0"
                }
            },"closebutton", "shop");
            shop.Add(new CuiElement
            {
                Parent = "shop",
                Name = "shop_slots_plate",
                Components =
                {
                    new CuiRawImageComponent
                    {
                        Png = GetImage(shop_main),
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0 0",
                        AnchorMax = "1 1"
                    }
                }
            });
            shop.Add(new CuiButton
            {
                RectTransform =
                {
                    AnchorMin = "1 0",
                    AnchorMax = "1 0",
                    OffsetMin = "-70 15",
                    OffsetMax = "-30 45"
                },
                Text = {Text = ""},
                Button =
                {
                    Color = "0, 0, 0, 0",
                    Command = "RCOIN_CONS OPEN SHOP [PAGE_NEXT]",
                }
            },"shop_slots_plate");
            shop.Add(new CuiButton
            {
                RectTransform =
                {
                    AnchorMin = "0 0",
                    AnchorMax = "0 0",
                    OffsetMin = "25 15",
                    OffsetMax = "65 50"
                },
                Text = {Text = ""},
                Button =
                {
                    Color = "0, 0, 0, 0",
                    Command = "RCOIN_CONS OPEN SHOP [PAGE_PREV]",
                }
            }, "shop_slots_plate");
            shop.Add(new CuiButton
            {
                RectTransform =
                {
                    AnchorMin = "0 1",
                    AnchorMax = "0 1",
                    OffsetMin = "30 -40",
                    OffsetMax = "70 -10"
                },
                Text = {Text = ""},
                Button =
                {
                    Color = "0 0 0 0",
                    Command = "RCOIN_CONS HOME"
                }
            },"shop_slots_plate");
            shop.Add(new CuiLabel
            {
                Text =
                {
                    Text = "[PAGE_NEXT]",
                    Align = TextAnchor.MiddleCenter,
                    FontSize = 18
                },
                RectTransform =
                {
                    AnchorMin = "0.5 0",
                    AnchorMax = "0.5 0",
                    OffsetMin = "-20 15",
                    OffsetMax = "20 47"
                }
            }, "shop_slots_plate");
            shop_slot.Add(new CuiPanel
            {
                RectTransform =
                {
                    AnchorMin = "0.5 1",
                    AnchorMax = "0.5 1",
                    OffsetMin = "[OMIN]",
                    OffsetMax = "[OMAX]"
                },
                Image = {Color = "0 0 0 0"}
            }, "shop_slots_plate", "shop_element[ENC]");
            shop_slot.Add(new CuiElement
            {
                Parent = "shop_element[ENC]",
                Components =
                {
                    new CuiRawImageComponent
                    {
                        Png = "[IMAGE]"
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0 0.5",
                        AnchorMax = "0 0.5",
                        OffsetMin = "8 -14",
                        OffsetMax = "36 13"
                    }
                }
            });
            shop_slot.Add(new CuiElement
            {
                Parent = "shop_element[ENC]",
                Components =
                {
                    new CuiRawImageComponent
                    {
                        Png = GetImage(shop_slotimg)
                    },
                    new CuiRectTransformComponent
                    {
                        AnchorMin = "0 0",
                        AnchorMax = "1 1",
                      
                    }
                }
            });
            shop_slot.Add(new CuiLabel
            {
                RectTransform =
                {
                    AnchorMin = "0.5 1",
                    AnchorMax = "0.5 1",
                    OffsetMin = "-45 -20",
                    OffsetMax = "70 -5"
                },
                Text =
                {
                    Align = TextAnchor.MiddleCenter,
                    Text = "[NAME]",
                    FontSize = 12
                }
            }, "shop_element[ENC]");
            shop_slot.Add(new CuiLabel
            {
                RectTransform =
                {
                    AnchorMin = "0.5 0",
                    AnchorMax = "0.5 0",
                    OffsetMin = "-6 5",
                    OffsetMax = "58 23"
                },
                Text =
                {
                    Align = TextAnchor.MiddleCenter,
                    Text = "[COST]",
                    FontSize = 12
                }
            }, "shop_element[ENC]");
            shop_slot.Add(new CuiButton
            {
                Text = { Text = ""},
                RectTransform =
                {
                    AnchorMin = "0 0",
                    AnchorMax = "1 1"
                },
                Button = { Command = "[COMMAND]", Color = "0 0 0 0"}
            },"shop_element[ENC]");
            shop_json = shop.ToJson();
            shop_slot_json = shop_slot.ToJson();
        }
        
        [ChatCommand("rcoin")]
        void OpenMenu(BasePlayer player)
        {
            DataPlayer t;

            if (!_players.TryGetValue(player, out t)) return;
            CommunityEntity.ServerInstance.ClientRPCEx(
                new Network.SendInfo {connection = player.net.connection}, null, "DestroyUI", "closebutton");
            string top;
            if (!_top.TryGetValue(player, out top)) top = "0";

            var avatar = GetImage(player.UserIDString);
            string jsonSend = main_json
                .Replace("[MAIN_AVATAR]", avatar)
                .Replace("[NICKNAME]", player.displayName)
                .Replace("[ID]", t.id.ToString("00000"))
                .Replace("[BALANCE]", t.coins.ToString("0.000"))
                .Replace("[TOP_POSITION]", top);


            CommunityEntity.ServerInstance.ClientRPCEx(
                new Network.SendInfo {connection = player.net.connection}, null, "AddUI", jsonSend);

            CommunityEntity.ServerInstance.ClientRPCEx(new Network.SendInfo {connection = player.net.connection}, null,
                "AddUI", main_balance_gui_json.Replace("[BALANCE]", t.coins.ToString("0.000")));
            _openInterface[player] = new InterfaceInfo
            {
                Interface = "main",
                Page = 0,
            };
        }

        private void UpdateBalance(BasePlayer player)
        {
            DataPlayer t;
            if (!_players.TryGetValue(player, out t)) return;
            CommunityEntity.ServerInstance.ClientRPCEx(
                new Network.SendInfo {connection = player.net.connection}, null, "DestroyUI", "balance_amount");
            CommunityEntity.ServerInstance.ClientRPCEx(
                new Network.SendInfo {connection = player.net.connection}, null, "AddUI",
                main_balance_gui_json.Replace("[BALANCE]", t.coins.ToString("0.000")));
        }

        private string GetImage(string shortname, ulong skin = 0) =>
            (string) ImageLibrary.Call("GetImage", shortname, skin);

        

        void Commands(IPlayer user, string command, string[] args)
        {
            BasePlayer player = user.Object as BasePlayer;

            switch (args[0])
            {
                case "promocode":
                {
                    if(args.Length < 2) return;
                    var promo = args[1];
                    SendPromocode(promo, _players[player].id);
                    break;
                }
                case "TRANSFER":
                {
                    switch (args[1])
                    {
                        case "sendtransfer":
                        {
                            
                            TranferSendInfo ti;
                            if (!_transferInfo.TryGetValue(player.userID, out ti))
                            {
                                ReplySend(player, "[RUST-COIN] Что то пошло не так.... Попробуйте снова.");
                                return;
                            }
                            
                            var targetid = _transferInfo[player.userID].id;
                            var coins = _transferInfo[player.userID].ammount;

                            if (coins < 1)
                            {
                                ReplySend(player,
                                    "[RUST-COIN] Че умный дохуя?!"); //ХАХАХАХАХААХАХАХАХАХАХААХААХАХХ ебать уГар ыыыыыыыыыы, типо еблан чел кидать меньше коина, ну да разрывная аХуЕТЬ -_-
                                return;
                            }

                            if (_players[player].id == targetid)
                            {
                                ReplySend(player, "[RUST-COIN] Нельзя переводить самому себе!");
                                return;
                            }

                            if (_players[player].coins < coins)
                            {
                                ReplySend(player, "[RUST-COIN] У вас не достаточно средств!");
                                return;
                            }

                            SendTransfer(_players[player].id, targetid, coins);
                            _transferInfo.Remove(player.userID);
                            timer.Once(0.5f, () => rust.RunClientCommand(player, "RCOIN_CONS OPEN HOME"));
                            
                            break;
                        }
                        case "addid":
                        {
                            if(args.Length < 3) return;
                            
                            TranferSendInfo ti;
                            if (!_transferInfo.TryGetValue(player.userID, out ti)) return;

                            if (!int.TryParse(args[2], out ti.id))
                            {
                                ReplySend(player, "[RUST-COIN] Некорректно введен RCOIN ID!");
                                return;
                            }

                            
                            break;
                        }
                        case "addamount":
                        {
                            if(args.Length < 3) return;
                            
                            TranferSendInfo ti;
                            if (!_transferInfo.TryGetValue(player.userID, out ti)) return;
                            

                            if (!int.TryParse(args[2], out ti.ammount))
                            {
                                ReplySend(player, "[RUST-COIN] Некорректно введена сумма!");
                                return;
                            }
                            
                            break;
                        }
                    }

                    break;
                }
                
                case "main_close":
                {
                    _transferInfo.Remove(player.userID);
                    _openInterface.Remove(player);
                    CommunityEntity.ServerInstance.ClientRPCEx(
                        new Network.SendInfo {connection = player.net.connection}, null, "DestroyUI", "closebutton");
                    break;
                }
                case "RCOIN_CLICK.MAINADD":
                {
                    AddMoney(player, 0.001);
                    UpdateBalance(player);
                    Effect Sound1 = new Effect("assets/bundled/prefabs/fx/notice/loot.drag.grab.fx.prefab", player, 0,
                        new Vector3(), new Vector3());
                    EffectNetwork.Send(Sound1, player.Connection);
                    break;
                }
                case "OPEN":
                {
                    switch (args[1])
                    {
                        case "UPGRADES":
                        {
                            DataPlayer t;
                            if (!_players.TryGetValue(player, out t)) return;
                            if (args[2].ToInt() < 0) return;
                            if (0 >= _upgrades.Count - args[2].ToInt() * 6) return;
                            CommunityEntity.ServerInstance.ClientRPCEx(
                                new Network.SendInfo {connection = player.net.connection}, null, "DestroyUI", "main");


                            int page = int.Parse(args[2]);
                            CommunityEntity.ServerInstance.ClientRPCEx(
                                new Network.SendInfo {connection = player.net.connection}, null, "AddUI",
                                upgrades_json);
                            CommunityEntity.ServerInstance.ClientRPCEx(
                                new Network.SendInfo {connection = player.net.connection}, null, "AddUI",
                                upgrade_plate_json
                                    .Replace("[PAGE_NEXT]", $"{page + 1}")
                                    .Replace("[PAGE_PREV]", $"{page - 1}")
                                    .Replace("[PAGE]", $"{page + 1}")
                            );
                            int i = 0;

                            foreach (var x in _upgrades.Skip(page * 6).Take(6))
                            {
                                if(!x.Value.isvisible) continue;
                                int lvl = t.upgrades.ContainsKey(x.Value.id) ? t.upgrades[x.Value.id] : 0;
                                var image = GetImage(x.Value.url);
                                CommunityEntity.ServerInstance.ClientRPCEx(
                                    new Network.SendInfo {connection = player.net.connection}, null, "AddUI",
                                    upgarde_slot_json
                                        .Replace("[ENC]", i.ToString())
                                        .Replace("[OMIN]", $"-89 {-115 - (48 * i)}")
                                        .Replace("[OMAX]", $"85 {-70 - (48 * i)}")
                                        .Replace("[IMAGE]", image)
                                        .Replace("[NAME]", x.Value.name)
                                        .Replace("[HOWADD]", (x.Value.addcoin * (lvl + 1)).ToString("0.000"))
                                        .Replace("[COST]", (x.Value.cost * Math.Pow(1.15, lvl)).ToString("0.000"))
                                        .Replace("[PLAYER_LEVEL]", lvl.ToString())
                                        .Replace("[ID]", x.Value.id.ToString())
                                );
                                i++;
                            }


                            _openInterface[player] = new InterfaceInfo
                            {
                                Interface = "upgrades",
                                Page = page
                            };


                            break;
                        }
                        case "TOP":
                        {
                            switch (args[2])
                            {
                                case "1":
                                {
                                    CommunityEntity.ServerInstance.ClientRPCEx(
                                        new Network.SendInfo {connection = player.net.connection}, null, "DestroyUI",
                                        "main");
                                    CommunityEntity.ServerInstance.ClientRPCEx(
                                        new Network.SendInfo {connection = player.net.connection}, null, "AddUI",
                                        top_json_server);
                                    break;
                                }
                                case "2":
                                {
                                    CommunityEntity.ServerInstance.ClientRPCEx(
                                        new Network.SendInfo {connection = player.net.connection}, null, "DestroyUI",
                                        "main");
                                    CommunityEntity.ServerInstance.ClientRPCEx(
                                        new Network.SendInfo {connection = player.net.connection}, null, "AddUI",
                                        top_json_all);
                                    break;
                                }
                                case "3":
                                {
                                    ReplySend(player, "Пока что тут ничего нет! Но скоро добавим!");
                                    break;
                                }
                            }

                            break;
                        }
                        case "TRANSFER":
                        {
                            if (!_transferInfo.ContainsKey(player.userID))
                            { 
                                _transferInfo.Add(player.userID, new TranferSendInfo());
                            }
                            
                            DataPlayer t;
                            if (!_players.TryGetValue(player, out t)) return;
                            
                            CommunityEntity.ServerInstance.ClientRPCEx(
                                new Network.SendInfo {connection = player.net.connection}, null, "DestroyUI",
                                "main");
                            CommunityEntity.ServerInstance.ClientRPCEx(
                                new Network.SendInfo {connection = player.net.connection}, null, "DestroyUI",
                                "Transfer_main");
                            
                            CommunityEntity.ServerInstance.ClientRPCEx(
                                new Network.SendInfo {connection = player.net.connection}, null, "AddUI",
                                transfer_json.Replace(
                                    "[BALANCE]", t.coins.ToString("0.000")));
                            break;
                        }
                        case "PROMO":
                        {
                            CommunityEntity.ServerInstance.ClientRPCEx(
                                new Network.SendInfo {connection = player.net.connection}, null, "DestroyUI",
                                "main");
                            CommunityEntity.ServerInstance.ClientRPCEx(
                                new Network.SendInfo {connection = player.net.connection}, null, "AddUI",
                                promocodes_json);
                            break;
                        }
                        case "SHOP":
                        {
                            if (0 >= _config.Tovars.Count - args[2].ToInt() * 6) return;
                            if (args[2].ToInt() < 0) return;
                            int page = args[2].ToInt();
                            if (!_config.isShopWorking)
                            {
                                ReplySend(player, "[RUST-COIN] На этом сервере нет товаров!");
                                return;
                            }
                            CommunityEntity.ServerInstance.ClientRPCEx(
                                new Network.SendInfo {connection = player.net.connection}, null, "DestroyUI",
                                "main");
                            CommunityEntity.ServerInstance.ClientRPCEx(
                                new Network.SendInfo {connection = player.net.connection}, null, "AddUI",
                                shop_json
                                    .Replace("[PAGE_NEXT]", (page+1).ToString())
                                    .Replace("[PAGE_PREV]", (page-1).ToString())
                                );
                            int i = 0;
                            foreach (var shp in _config.Tovars.Skip(page * 6).Take(6))
                            {
                                CommunityEntity.ServerInstance.ClientRPCEx(
                                    new Network.SendInfo {connection = player.net.connection}, null, "AddUI",
                                    shop_slot_json
                                        .Replace("[ENC]", i.ToString())
                                        .Replace("[OMIN]", $"-87 {-119 - 50 * i}")
                                        .Replace("[OMAX]", $"81 {-73 - 50 * i}")
                                        .Replace("[IMAGE]", GetImage(shp.Value.Image))
                                        .Replace("[NAME]", shp.Value.Name)
                                        .Replace("[COST]", shp.Value.Cost.ToString())
                                        .Replace("[COMMAND]", $"RCOIN_CONS SHOP_BUY {shp.Key}")
                                    
                                );
                                i++;
                            }
                            break;
                        }
                    }

                    break;
                }
                case "UPGRADES":
                {
                    switch (args[1])
                    {
                        case "BUY":
                        {
                            DataPlayer t;
                            if (!_players.TryGetValue(player, out t)) return;
                            int i = args[3].ToInt();
                            int x = args[2].ToInt();

                            if (t.upgrades.ContainsKey(x) && t.upgrades[x] + 1 > _upgrades[x].maxlvl)
                            {
                                ReplySend(player, "[RUST-COIN] Максимальный уровень достигнут!");
                                return;
                            }

                            int lvl = t.upgrades.ContainsKey(_upgrades[x].id) ? t.upgrades[_upgrades[x].id] : 0;
                            var image = GetImage(_upgrades[x].url);
                            CommunityEntity.ServerInstance.ClientRPCEx(
                                new Network.SendInfo {connection = player.net.connection}, null, "DestroyUI",
                                $"Upgrade_slot{i}");

                            if (!RemoveMoney(player, _upgrades[x].cost * Math.Pow(1.15, lvl)))
                            {
                                CommunityEntity.ServerInstance.ClientRPCEx(
                                    new Network.SendInfo {connection = player.net.connection}, null, "AddUI",
                                    upgarde_slot_json
                                        .Replace("[ENC]", i.ToString())
                                        .Replace("[OMIN]", $"-89 {-115 - (48 * i)}")
                                        .Replace("[OMAX]", $"85 {-70 - (48 * i)}")
                                        .Replace("[IMAGE]", image)
                                        .Replace("[NAME]", _upgrades[x].name)
                                        .Replace("[HOWADD]", (_upgrades[x].addcoin * (lvl + 1)).ToString("0.000"))
                                        .Replace("[COST]", (_upgrades[x].cost * Math.Pow(1.15, lvl)).ToString("0.000"))
                                        .Replace("[PLAYER_LEVEL]", lvl.ToString())
                                        .Replace("[ID]", _upgrades[x].id.ToString())
                                );
                                ReplySend(player, "[RUST-COIN] Недостаточно RC!!");
                                return;
                            }

                            GiveUpgrade(player, x);

                            CommunityEntity.ServerInstance.ClientRPCEx(
                                new Network.SendInfo {connection = player.net.connection}, null, "AddUI",
                                upgarde_slot_json
                                    .Replace("[ENC]", i.ToString())
                                    .Replace("[OMIN]", $"-89 {-115 - (48 * i)}")
                                    .Replace("[OMAX]", $"85 {-70 - (48 * i)}")
                                    .Replace("[IMAGE]", image)
                                    .Replace("[NAME]", _upgrades[x].name)
                                    .Replace("[HOWADD]", (_upgrades[x].addcoin * (lvl + 1)).ToString("0.000"))
                                    .Replace("[COST]", (_upgrades[x].cost * Math.Pow(1.15, lvl)).ToString("0.000"))
                                    .Replace("[PLAYER_LEVEL]", t.upgrades[_upgrades[x].id].ToString())
                                    .Replace("[ID]", _upgrades[x].id.ToString())
                            );
                            break;
                        }
                    }

                    break;
                }
                case "SHOP_BUY":
                {
                    Configuration.Shop t;
                    if(!_config.Tovars.TryGetValue(args[1].ToInt(), out t)) return;
                    if (!RemoveMoney(player, t.Cost))
                    {
                        ReplySend(player, "[RUST-COIN] У вас недостаточно RC!");
                        return;
                    }
                    ReplySend(player, "[RUST-COIN] Успешно!");
                    rust.RunServerCommand(t.Command.Replace("[STEAMID]", player.UserIDString));
                    
                    OpenMenu(player);
                    
                    
                    break;
                }
                case "HOME":
                {
                    OpenMenu(player);
                    break;
                }
            }
        }

        #endregion

        #region Mettods

        private int serverId = -1;
        private double coins = 0;

        private void ReplySend(BasePlayer player, string message) => player.SendConsoleCommand("chat.add 0",
            new object[2] {76561199274772400, $"{message} "});

        private void AddMoney(BasePlayer player, double coin)
        {
            DataPlayer info;
            if (!_players.TryGetValue(player, out info)) return;
            info.coins += coin;
            coins += coin;
        }

        private bool RemoveMoney(BasePlayer player, double coin)
        {
            DataPlayer info;
            if (!_players.TryGetValue(player, out info)) return false;
            if (info.coins - coin < 0) return false;
            info.coins -= coin;
            return true;
        }

        private void GiveUpgrade(BasePlayer player, int key)
        {
            DataPlayer info;
            int upgradelvl;
            if (!_players.TryGetValue(player, out info)) return;
            if (!info.upgrades.TryGetValue(key, out upgradelvl))
                info.upgrades.Add(key, 1);
            else
            {
                info.upgrades[key] = upgradelvl + 1;
            }
        } 

        private void StatusCheck()
        {
            try
            {
                test.Enqueue($"https://rustcoin.foxplugins.ru/rustcoin/status.php", $"",
                    (code2, response2) =>
                        _coroutines.Add(ServerMgr.Instance.StartCoroutine(CheckStatus(code2, response2))), this,
                    Core.Libraries.RequestMethod.POST);
            }
            catch (Exception e)
            {
                Console.WriteLine("5");
                throw;
            }
        }

        private void GetTransfer(int id)
        {
            try
            {
                test.Enqueue($"https://rustcoin.foxplugins.ru/rustcoin/transfer.php", $"targetid={id}",
                    (code2, response2) =>
                        _coroutines.Add(ServerMgr.Instance.StartCoroutine(Transfer(-1, id, 0, code2, response2))),
                    this,
                    Core.Libraries.RequestMethod.POST);
            }
            catch (Exception e)
            {
                Console.WriteLine("5");
                throw;
            }
        }

        private void CheckCompleteTransfer(int id)
        {
            try
            {
                test.Enqueue($"https://rustcoin.foxplugins.ru/rustcoin/transfer.php", $"playerid={id}",
                    (code2, response2) =>
                        _coroutines.Add(ServerMgr.Instance.StartCoroutine(Transfer(id, -1, 0, code2, response2))),
                    this,
                    Core.Libraries.RequestMethod.POST);
            }
            catch (Exception e)
            {
                Console.WriteLine("5");
                throw;
            }
        }

        private void SendPromocode(string promo, int id)
        {
            try
            {
                test.Enqueue($"https://rustcoin.foxplugins.ru/rustcoin/promocode.php",
                    $"promo={promo}&id={id}",
                    (code2, response2) =>
                        _coroutines.Add(
                            ServerMgr.Instance.StartCoroutine(UsePromo(id, code2, response2))),
                    this,
                    Core.Libraries.RequestMethod.POST);
            }
            catch (Exception e)
            {
                Console.WriteLine("5");
                throw;
            }
        }

        private void SendTransfer(int playerid, int targetid, int coins)
        {
            try
            {
                test.Enqueue($"https://rustcoin.foxplugins.ru/rustcoin/transfer.php",
                    $"playerid={playerid}&targetid={targetid}&coins={coins}",
                    (code2, response2) =>
                        _coroutines.Add(
                            ServerMgr.Instance.StartCoroutine(Transfer(playerid, targetid, coins, code2, response2))),
                    this,
                    Core.Libraries.RequestMethod.POST);
            }
            catch (Exception e)
            {
                Console.WriteLine("5");
                throw;
            }
        }

        private void Upgrades()
        {
            try
            {
                test.Enqueue($"https://rustcoin.foxplugins.ru/rustcoin/upgrades.php", $"",
                    (code2, response2) => _coroutines.Add(ServerMgr.Instance.StartCoroutine(UpInfo(code2, response2))),
                    this,
                    Core.Libraries.RequestMethod.POST);
            }
            catch (Exception e)
            {
                Console.WriteLine("5");
                throw;
            }
        }

        private void ServerUpdate()
        {
            try
            {
                test.Enqueue($"https://rustcoin.foxplugins.ru/rustcoin/servers.php",
                    $"ip={Uri.EscapeDataString(ConVar.Server.ip)}&port={ConVar.Server.port.ToString()}&name={Uri.EscapeDataString(ConVar.Server.hostname)}&coins={coins}",
                    (code2, response2) =>
                        _coroutines.Add(ServerMgr.Instance.StartCoroutine(ServerUpdates(code2, response2))), this,
                    Core.Libraries.RequestMethod.POST);
            }
            catch (Exception e)
            {
                Console.WriteLine("5");
                throw;
            }
        }

        private void SetServer()
        {
            try
            {
                test.Enqueue($"https://rustcoin.foxplugins.ru/rustcoin/selectserver.php",
                    $"ip={Uri.EscapeDataString(ConVar.Server.ip)}&port={ConVar.Server.port.ToString()}",
                    (code2, response2) =>
                        _coroutines.Add(ServerMgr.Instance.StartCoroutine(ServerUpdates(code2, response2))), this,
                    Core.Libraries.RequestMethod.POST);
            }
            catch (Exception e)
            {
                Console.WriteLine("5");
                throw;
            }
        }

        private void Update(BasePlayer player, double coin, int serverid, Dictionary<int, int> upgrades)
        {
            try
            {
                test.Enqueue($"https://rustcoin.foxplugins.ru/rustcoin/update.php",
                    $"steamid={player.userID}&name={Uri.EscapeDataString(player.displayName)}&coins={coin}&serverid={serverid}&upgrades={JsonConvert.SerializeObject(upgrades)}",
                    (code2, response2) => { }, this,
                    Core.Libraries.RequestMethod.POST);
            }
            catch (Exception e)
            {
                Console.WriteLine("5");
                throw;
            }
        }

        private void GetInfos(BasePlayer player)
        {
            try
            {
                test.Enqueue($"https://rustcoin.foxplugins.ru/rustcoin/getinfo.php", $"steamid={player.userID}",
                    (code, response) =>
                        _coroutines.Add(ServerMgr.Instance.StartCoroutine(GetInfo(player, code, response))), this,
                    Core.Libraries.RequestMethod.POST);
            }
            catch (Exception e)
            {
                Console.WriteLine("5");
                throw;
            }
        }

        private void GetTopPlayer(BasePlayer player)
        {
            try
            {
                test.Enqueue($"https://rustcoin.foxplugins.ru/rustcoin/top.php", $"steamid={player.userID}",
                    (code, response) =>
                        _coroutines.Add(ServerMgr.Instance.StartCoroutine(TopPlayer(player, code, response))), this,
                    Core.Libraries.RequestMethod.POST);
            }
            catch (Exception e)
            {
                Console.WriteLine("5");
                throw;
            }
        }

        private void AllPlayersTop()
        {
            try
            {
                test.Enqueue($"https://rustcoin.foxplugins.ru/rustcoin/top.php", $"max=8",
                    (code, response) =>
                        _coroutines.Add(ServerMgr.Instance.StartCoroutine(AllPlayersTops(null, code, response))), this,
                    Core.Libraries.RequestMethod.POST);
            }
            catch (Exception e)
            {
                Console.WriteLine("5");
                throw;
            }
        }

        private void GetServerTops()
        {
            try
            {
                test.Enqueue($"https://rustcoin.foxplugins.ru/rustcoin/top.php", $"id={serverId}&max=8",
                    (code, response) =>
                        _coroutines.Add(ServerMgr.Instance.StartCoroutine(TopPlayer(null, code, response))), this,
                    Core.Libraries.RequestMethod.POST);
            }
            catch (Exception e)
            {
                Console.WriteLine("5");
                throw;
            }
        }

        private IEnumerator UpdateMysql()
        {
            Debug.LogWarning("[RUST-COIN] Подождите плагин загружается......");
            yield return CoroutineEx.waitForSeconds(5f);
            while (status == 0)
            {
                yield return CoroutineEx.waitForSeconds(60f);
                StatusCheck();
            }

            SetServer();
            yield return CoroutineEx.waitForSeconds(2f);
            foreach (var basePlayer in BasePlayer.activePlayerList)
            {
                OnPlayerConnected(basePlayer);
            }
            Generate();
            Debug.LogWarning("[RUST-COIN] Плагин успешно загружен!");
            while (this.IsLoaded)
            {
                Upgrades();
                yield return CoroutineEx.waitForSeconds(60f);
                if (!IsLoaded) yield break;
                ServerUpdate();
                StatusCheck();
                foreach (var player in _players)
                {
                    GetTopPlayer(player.Key);
                    double add = player.Value.upgrades.ToList().Sum(p =>
                    {
                        var t = 0.0;
                        if (_upgrades.ContainsKey(p.Key))
                        {
                            var lvl = p.Value;
                            if (p.Value > _upgrades[p.Key].maxlvl)
                            {
                                player.Value.upgrades[p.Key] = _upgrades[p.Key].maxlvl;
                                lvl = _upgrades[p.Key].maxlvl;
                            }

                            t = _upgrades[p.Key].addcoin * lvl;
                        }
                        else
                        {
                            player.Value.upgrades.Remove(p.Key);
                        }

                        return t;
                    });


                    player.Value.coins += add;
                    player.Value.serverid = serverId;
                    coins += add;
                    GetTransfer(player.Value.id);
                    CheckCompleteTransfer(player.Value.id);
                    Update(player.Key, player.Value.coins, player.Value.serverid, player.Value.upgrades);
                    if (_openInterface.ContainsKey(player.Key))
                    {
                        if (_openInterface[player.Key].Interface == "main") UpdateBalance(player.Key);
                        else
                            player.Key.SendConsoleCommand(
                                $"RCOIN_CONS OPEN UPGRADES {_openInterface[player.Key].Page}");
                    }
                }

                AllPlayersTop();
                GetServerTops();
            }

            yield break;
        }

        private Dictionary<BasePlayer, InterfaceInfo> _openInterface = new Dictionary<BasePlayer, InterfaceInfo>();
        private Dictionary<BasePlayer, DataPlayer> _players = new Dictionary<BasePlayer, DataPlayer>();

        class PluginStatus
        {
            public int id;
            public int status;
        }

        private Dictionary<ulong, TranferSendInfo> _transferInfo = new Dictionary<ulong, TranferSendInfo>();
        class TranferSendInfo
        {
            public int id;
            public int ammount;

        }
        class InterfaceInfo
        {
            public string Interface;
            public int Page;
        }

        class DataPlayer
        {
            public int id;

            public double coins;

            public Dictionary<int, int> upgrades = new Dictionary<int, int>();

            public int serverid;
        }

        class InfoPlayer
        {
            public int id;

            public double coins;

            public string upgrades;

            public int serverid;
        }

        private Dictionary<int, UpgradesInfo> _upgrades = new Dictionary<int, UpgradesInfo>();

        class UpgradesInfo
        {
            public int id;

            public double addcoin;

            public string url;

            public double cost;

            public int maxlvl;

            public string name;

            public bool isvisible;
        }

        class TopInfo
        {
            public int id;
            public int top;
            public ulong steamid;
            public string name;
            public double coins;
        }

        class Transfers
        {
            public int id;
            public int playerid;
            public string name;
            public int coins;
            public int complete;
        }

        private List<TopInfo> _topAllPlayers = new List<TopInfo>();
        private List<TopInfo> _topServer = new List<TopInfo>();
        private Dictionary<BasePlayer, string> _top = new Dictionary<BasePlayer, string>();

        IEnumerator AllPlayersTops(BasePlayer player, int code, string response)
        {
            if (!IsLoaded) yield break;
            if (response == null) yield break;
            if (code == 200)
            {
                if (!response.Contains("top")) yield break;
                var json = JsonConvert.DeserializeObject<Dictionary<int, TopInfo>>(response);

                _topAllPlayers.Clear();
                foreach (var keyValuePair in json)
                {
                    _topAllPlayers.Add(keyValuePair.Value);
                }

                GenerateTop();
                yield break;
            }

            yield break;
        }

        IEnumerator TopPlayer(BasePlayer player, int code, string response)
        {
            if (!IsLoaded) yield break;
            if (response == null) yield break;
            if (code == 200)
            {
                int top;
                if (int.TryParse(response, out top)) _top[player] = response;
                else
                {
                    if (!response.Contains("top")) yield break;
                    var json = JsonConvert.DeserializeObject<Dictionary<int, TopInfo>>(response);
                    _topServer.Clear();
                    foreach (var keyValuePair in json)
                    {
                        _topServer.Add(keyValuePair.Value);
                    }

                    GenerateTop();
                }

                yield break;
            }

            yield break;
        }

        class ServerId
        {
            public int id;
            public double coins;
        }

        IEnumerator ServerUpdates(int code, string response)
        {
            if (!IsLoaded) yield break;
            if (response == null) yield break;
            if (code == 200)
            {
                if (!response.Contains("id"))
                {
                    ServerUpdate();
                    yield return CoroutineEx.waitForSeconds(2f);
                    if (!IsLoaded) yield break;
                    SetServer();

                    yield break;
                }

                var json = JsonConvert.DeserializeObject<ServerId>(response);
                serverId = json.id;
                coins = json.coins;
                AllPlayersTop();
                GetServerTops();
            }

            yield break;
        }

        private int status = 1;

        IEnumerator CheckStatus(int code, string response)
        {
            if (!IsLoaded) yield break;
            if (response == null) yield break;
            if (code == 200)
            {
                var json = JsonConvert.DeserializeObject<PluginStatus>(response);
                if (json == null)
                {
                    yield break;
                }

                if (json.status == 0)
                {
                    if (status == 1)
                    {
                        MediumUnload();
                        _upgrades.Clear();
                        _topAllPlayers.Clear();
                        _topServer.Clear();
                        _top.Clear();
                    }

                    if (status != json.status)
                    {
                        Debug.LogError("[RUST-COIN] Плагин временно не работает!");
                        status = json.status;
                    }
                }
                else
                {
                    if (status == 0)
                        Interface.Oxide.ReloadPlugin(Name);
                    status = 1;
                }
            }

            yield break;
        }

        IEnumerator UsePromo(int playerid, int code, string response)
        {
            if (!IsLoaded) yield break;
            if (response == null) yield break;
            if (code == 200)
            {
                var player = _players.FirstOrDefault(p => p.Value.id == playerid);
                if (player.Value == null) yield break;
                switch (response)
                {
                    case "NOT FOUND":
                        ReplySend(player.Key, $"[RUST-COIN] Такого промокода не существует!");
                        yield break;
                    case "IS USES":
                        ReplySend(player.Key, $"[RUST-COIN] Вы уже использовали этот промокод!");
                        yield break;
                    case "MAX USES":
                        ReplySend(player.Key, $"[RUST-COIN] Этот промокод уже использовали максимальное кол-во людей!");
                        yield break;
                    default:
                        player.Value.coins += double.Parse(response);
                        coins += double.Parse(response);
                        ReplySend(player.Key,
                            $"[RUST-COIN] Вам начисленно {double.Parse(response).ToString("0.000")} RC за использование промокода!");
                        OpenMenu(player.Key);
                        yield break;
                }
            }
        }

        IEnumerator Transfer(int playerid, int targetid, int coins, int code, string response)
        {
            if (!IsLoaded) yield break;
            if (response == null) yield break;
            if (code == 200)
            {
                if (response == "NOT FOUND" || response == "DONT COMPLETE") yield break;
                if (playerid != -1 && targetid != -1)
                {
                    var player = _players.FirstOrDefault(p => p.Value.id == playerid);
                    if (player.Value == null) yield break;
                    switch (response)
                    {
                        case "TARGET NOT FOUND":
                            ReplySend(player.Key, $"[RUST-COIN] Игрок по вашему запросу не найден!");
                            break;
                        case "SEND UPDATE":

                            player.Value.coins -= coins;
                            ReplySend(player.Key,
                                $"[RUST-COIN] Ваш запрос обрабатывается(Перевод поступит, когда игрок зайдет в сеть и произойдет автоматическое обновление. Вы получите оповещение, когда игрок получит средства.)");
                            break;
                    }

                    yield break;
                }

                var json = JsonConvert.DeserializeObject<Dictionary<int, Transfers>>(response);
                if (json == null)
                {
                    yield break;
                }

                foreach (var transfers in json)
                {
                    if (transfers.Value.complete == 1)
                    {
                        var player = _players.FirstOrDefault(p => p.Value.id == targetid);
                        if (player.Value == null) continue;
                        player.Value.coins += transfers.Value.coins;
                        ReplySend(player.Key,
                            $"[RUST-COIN] Игрок {transfers.Value.name} перевел вам {transfers.Value.coins} RC [/rcoin]");
                    }
                    else if (transfers.Value.complete == 2)
                    {
                        var player = _players.FirstOrDefault(p => p.Value.id == playerid);
                        if (player.Value == null) continue;
                        ReplySend(player.Key,
                            $"[RUST-COIN] Игрок {transfers.Value.name} получил от вас {transfers.Value.coins} RC [/rcoin]");
                    }
                }
            }

            yield break;
        }

        IEnumerator UpInfo(int code, string response)
        {
            if (!IsLoaded) yield break;
            if (response == null) yield break;
            if (code == 200)
            {
                List<UpgradesInfo> json = JsonConvert.DeserializeObject<List<UpgradesInfo>>(response);
                if (json == null)
                {
                    yield break;
                }

                _upgrades.Clear();
                foreach (var upgradesInfo in json)
                {
                    if (!_upgrades.ContainsKey(upgradesInfo.id)) _upgrades.Add(upgradesInfo.id, upgradesInfo);
                    else _upgrades[upgradesInfo.id] = upgradesInfo;
                }

                GenerateUpgrades();
            }

            yield break;
        }

        IEnumerator GetInfo(BasePlayer player, int code, string response)
        {
            if (!IsLoaded) yield break;

            if (response == null) yield break;
            if (code == 200)
            {
                if (response == "test")
                {
                    Update(player, 0, serverId, new Dictionary<int, int>());
                    yield return CoroutineEx.waitForSeconds(2f);
                    if (!IsLoaded) yield break;
                    GetInfos(player);
                    yield break;
                }

                InfoPlayer json = JsonConvert.DeserializeObject<InfoPlayer>(response);
                if (json == null)
                {
                    yield break;
                }

                _players.Add(player, new DataPlayer
                {
                    id = json.id,
                    coins = json.coins,
                    serverid = json.serverid,
                    upgrades = JsonConvert.DeserializeObject<Dictionary<int, int>>(json.upgrades)
                });
                GetTopPlayer(player);
                yield break;
            }

            yield break;
        }

        #endregion
    }
}