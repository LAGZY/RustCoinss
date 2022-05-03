using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Facepunch.Extend;
using Newtonsoft.Json;
using Oxide.Core.Libraries.Covalence;
using Oxide.Core.Plugins;
using Oxide.Game.Rust.Cui;
using UnityEngine;

namespace Oxide.Plugins
{
    [Info("RustCoin", "LAGZYA feat fermens and megargan", "1.0.30")]
    public class RustCoin : RustPlugin
    {
        [PluginReference] Plugin ImageLibrary;

        #region Hooks

        private void OnPlayerConnected(BasePlayer player)
        {
            GetInfos(player);
            GetTopPlayer(player);
        }

        private void OnPlayerDisconnected(BasePlayer player)
        {
            DataPlayer info;
            if (!_players.TryGetValue(player, out info)) return;
            Update(player, info.coins, info.serverid, info.upgrades);
            _players.Remove(player);
        }

        private void Unload()
        {
            foreach (var basePlayer in BasePlayer.activePlayerList)
            {
                OnPlayerDisconnected(basePlayer);
            }
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

        private void OnServerInitialized()
        {
            SetServer();
            ServerMgr.Instance.StartCoroutine(UpdateMysql());

            foreach (var basePlayer in BasePlayer.activePlayerList)
            {
                OnPlayerConnected(basePlayer);
            }

            if (!ImageLibrary)
            {
                Debug.LogError("ImageLibrary не установлена!!! Плагин работать не будет!!");
                Unload();
            }

            ImageLibrary.Call("AddImage", main_back, main_back); //main_background
            ImageLibrary.Call("AddImage", main_border, main_border); //main_border
            ImageLibrary.Call("AddImage", main_tap, main_tap); //main_tap
            ImageLibrary.Call("AddImage", main_balance, main_balance); //main_balik
            ImageLibrary.Call("AddImage", main_buttons, main_buttons); //main_buttons
            ImageLibrary.Call("AddImage", main_phone, main_phone); //main_phone
            ImageLibrary.Call("AddImage", main_upgrades, main_upgrades); //main_upgrades
            ImageLibrary.Call("AddImage", upgrade_backimage, upgrade_backimage); //upgrade_backimage
            ImageLibrary.Call("AddImage", main_top, main_top); //main_top
            ImageLibrary.Call("AddImage", avatar_rc, avatar_rc); //main_top
            ImageLibrary.Call("AddImage", server_players, server_players); //main_top
            ImageLibrary.Call("AddImage", global_players, global_players); //main_top
            ImageLibrary.Call("AddImage", global_servers, global_servers); //main_top


            AddCovalenceCommand("RCOIN_CONS", nameof(Commands));
            Generate();
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
        }

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
                    Text = "[HOWADD]" + "RC/5m",
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
                        Text = x.id.ToString("0000"),
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
                        OffsetMin = "-93 15",
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
                        Text = x.id.ToString("0000"),
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
                        OffsetMin = "-93 15",
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
                .Replace("[ID]", t.id.ToString("0000"))
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
                case "main_close":
                {
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
                            if (args[2].ToInt() > _upgrades.Count / 6) return;
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

                            foreach (var x in _upgrades.Where(p => p.Value.isvisible).Skip(page * 6).Take(6))
                            {
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
                                        .Replace("[COST]", (x.Value.cost * ((lvl + 1) * 0.75)).ToString("0.000"))
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
                                ReplySend(player, "Максимальный уровень достигнут!");
                                return;
                            }

                            int lvl = t.upgrades.ContainsKey(_upgrades[x].id) ? t.upgrades[_upgrades[x].id] : 0;
                            var image = GetImage(_upgrades[x].url);
                            CommunityEntity.ServerInstance.ClientRPCEx(
                                new Network.SendInfo {connection = player.net.connection}, null, "DestroyUI",
                                $"Upgrade_slot{i}");

                            if (!RemoveMoney(player, _upgrades[x].cost * ((lvl + 1) * 0.75)))
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
                                        .Replace("[COST]", (_upgrades[x].cost * ((lvl + 1) * 0.75)).ToString("0.000"))
                                        .Replace("[PLAYER_LEVEL]", lvl.ToString())
                                        .Replace("[ID]", _upgrades[x].id.ToString())
                                );
                                ReplySend(player, "Недостаточно RC!!");
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
                                    .Replace("[COST]", (_upgrades[x].cost * ((lvl + 1) * 0.75)).ToString("0.000"))
                                    .Replace("[PLAYER_LEVEL]", t.upgrades[_upgrades[x].id].ToString())
                                    .Replace("[ID]", _upgrades[x].id.ToString())
                            );
                            break;
                        }
                    }

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

        private void Upgrades()
        {
            webrequest.Enqueue($"https://lagzya.foxplugins.ru/rustcoin/upgrades.php", $"",
                (code2, response2) => ServerMgr.Instance.StartCoroutine(UpInfo(code2, response2)), this,
                Core.Libraries.RequestMethod.POST);
        }

        private void ServerUpdate()
        {
            webrequest.Enqueue($"https://lagzya.foxplugins.ru/rustcoin/servers.php",
                $"ip={Uri.EscapeDataString(ConVar.Server.ip)}&port={ConVar.Server.port.ToString()}&name={Uri.EscapeDataString(ConVar.Server.hostname)}&coins={coins}",
                (code2, response2) => ServerMgr.Instance.StartCoroutine(ServerUpdates(code2, response2)), this,
                Core.Libraries.RequestMethod.POST);
        }

        private void SetServer()
        {
            webrequest.Enqueue($"https://lagzya.foxplugins.ru/rustcoin/selectserver.php",
                $"ip={Uri.EscapeDataString(ConVar.Server.ip)}&port={ConVar.Server.port.ToString()}",
                (code2, response2) => ServerMgr.Instance.StartCoroutine(ServerUpdates(code2, response2)), this,
                Core.Libraries.RequestMethod.POST);
        }

        private void Update(BasePlayer player, double coin, int serverid, Dictionary<int, int> upgrades)
        {
            webrequest.Enqueue($"https://lagzya.foxplugins.ru/rustcoin/update.php",
                $"steamid={player.userID}&name={Uri.EscapeDataString(player.displayName)}&coins={coin}&serverid={serverid}&upgrades={JsonConvert.SerializeObject(upgrades)}",
                (code2, response2) => { }, this,
                Core.Libraries.RequestMethod.POST);
        }

        private void GetInfos(BasePlayer player)
        {
            webrequest.Enqueue($"https://lagzya.foxplugins.ru/rustcoin/getinfo.php", $"steamid={player.userID}",
                (code, response) => ServerMgr.Instance.StartCoroutine(GetInfo(player, code, response)), this,
                Core.Libraries.RequestMethod.POST);
        }

        private void GetTopPlayer(BasePlayer player)
        {
            webrequest.Enqueue($"https://lagzya.foxplugins.ru/rustcoin/top.php", $"steamid={player.userID}",
                (code, response) => ServerMgr.Instance.StartCoroutine(TopPlayer(player, code, response)), this,
                Core.Libraries.RequestMethod.POST);
        }

        private void AllPlayersTop()
        {
            webrequest.Enqueue($"https://lagzya.foxplugins.ru/rustcoin/top.php", $"max=8",
                (code, response) => ServerMgr.Instance.StartCoroutine(AllPlayersTops(null, code, response)), this,
                Core.Libraries.RequestMethod.POST);
        }

        private void GetServerTops()
        {
            webrequest.Enqueue($"https://lagzya.foxplugins.ru/rustcoin/top.php", $"id={serverId}&max=8",
                (code, response) => ServerMgr.Instance.StartCoroutine(TopPlayer(null, code, response)), this,
                Core.Libraries.RequestMethod.POST);
        }

        private IEnumerator UpdateMysql()
        {
            while (this.IsLoaded)
            {
                Upgrades();
                ServerUpdate();
                AllPlayersTop();
                yield return CoroutineEx.waitForSeconds(10f);
                if (!IsLoaded) yield break;
                ServerUpdate();
                GetServerTops();
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

                        return t;
                    });
                    if (_openInterface.ContainsKey(player.Key))
                    {
                        if (_openInterface[player.Key].Interface == "main") UpdateBalance(player.Key);
                        else
                            player.Key.SendConsoleCommand(
                                $"RCOIN_CONS OPEN UPGRADES {_openInterface[player.Key].Page}");
                    }

                    player.Value.coins += add;
                    player.Value.serverid = serverId;
                    coins += add;
                    Update(player.Key, player.Value.coins, player.Value.serverid, player.Value.upgrades);
                }
            }

            yield break;
        }

        private Dictionary<BasePlayer, InterfaceInfo> _openInterface = new Dictionary<BasePlayer, InterfaceInfo>();
        private Dictionary<BasePlayer, DataPlayer> _players = new Dictionary<BasePlayer, DataPlayer>();

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

        private List<TopInfo> _topAllPlayers = new List<TopInfo>();
        private List<TopInfo> _topServer = new List<TopInfo>();
        private Dictionary<BasePlayer, string> _top = new Dictionary<BasePlayer, string>();

        IEnumerator AllPlayersTops(BasePlayer player, int code, string response)
        {
            if (!IsLoaded) yield break;
            if (response == null) yield break;
            if (code == 200)
            {
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

                GetServerTops();
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
                yield break;
            }

            yield break;
        }

        #endregion
    }
}