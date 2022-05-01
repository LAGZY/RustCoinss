using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json;
using Oxide.Core.Libraries.Covalence;
using Oxide.Core.Plugins;
using Oxide.Game.Rust.Cui;
using UnityEngine;

namespace Oxide.Plugins
{
    [Info("RustCoin", "LAGZYA feat fermens and megargan", "1.0.27")]
    public class RustCoin : RustPlugin
    {
        [PluginReference] Plugin ImageLibrary;

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

        private void Unload()
        {
            foreach (var basePlayer in BasePlayer.activePlayerList)
            {
                OnPlayerDisconnected(basePlayer);
            }
        }

        private void OnServerInitialized()
        {
            if (!ImageLibrary)
            {
                Debug.LogError("ImageLibrary не установлена!!! Плагин работать не будет!!");
                Unload();
            }

            AddCovalenceCommand("RCOIN_CONS", nameof(Commands));
            Generate();
            SetServer();
            ServerMgr.Instance.StartCoroutine(UpdateMysql());

            foreach (var basePlayer in BasePlayer.activePlayerList)
            {
                OnPlayerConnected(basePlayer);
            }
        }

        #endregion

        #region Interface

        #region VAR

        private float all_fadein = 0.7f;

        private string main_balance_gui_json;
        private string main_phone_json;
        private string main_json;
        private string upgrades_json;
        private string upgrades_phone_json;
        
        

        #endregion

        void Generate()
        {
            ///TEST//////////////////////////

            _upgrades.Add(0, new UpgradesInfo
            {
                addcoin = 0.001,
                cost = 0.1,
                id = 1,
                maxlvl = 5,
                name = "ФЫВФЫв",
                url = "https://imgur.com/m4m8g0t.png"
            });
            _upgrades.Add(1, new UpgradesInfo
            {
                addcoin = 0.001,
                cost = 0.1,
                id = 2,
                maxlvl = 5,
                name = "zalupa",
                url = "https://imgur.com/m4m8g0t.png"
            });
            _upgrades.Add(2, new UpgradesInfo
            {
                addcoin = 0.001,
                cost = 0.1,
                id = 3,
                maxlvl = 5,
                name = "govno",
                url = "https://imgur.com/m4m8g0t.png"
            });

            ///TEST//\/\/\//\/\/\/\/\/\/\//\/\//\/\/\/\/\
            
            
            
            
            string main_back = "https://imgur.com/gxOM6f8.png";
            ImageLibrary.Call("AddImage", main_back, main_back); //main_background
            string main_border = "https://imgur.com/WvsSWHB.png";
            ImageLibrary.Call("AddImage", main_border, main_border); //main_border
            string main_tap = "https://imgur.com/mnFRlYZ.png";
            ImageLibrary.Call("AddImage", main_tap, main_tap); //main_tap
            string main_balance = "https://imgur.com/lqwgcN9.png";
            ImageLibrary.Call("AddImage", main_balance, main_balance); //main_balik
            string main_buttons = "https://imgur.com/EMHab4A.png";
            ImageLibrary.Call("AddImage", main_buttons, main_buttons); //main_buttons
            string main_phone = "https://imgur.com/PDwzpbG.png";
            ImageLibrary.Call("AddImage", main_phone, main_phone); //main_phone
            string main_upgrades = "https://imgur.com/5jqO4BC.png";
            ImageLibrary.Call("AddImage", main_upgrades, main_upgrades); //main_upgrades
            string upgrade_backimage = "https://imgur.com/7MFgOuM.png";
            ImageLibrary.Call("AddImage", upgrade_backimage, upgrade_backimage); //upgrade_backimage

            foreach (var info in _upgrades)
            {
                ImageLibrary.Call("AddImage", info.Value.url, info.Value.url);
            }
            
            
            CuiElementContainer main = new CuiElementContainer();
            CuiElementContainer main_balance_gui = new CuiElementContainer();
            CuiElementContainer upgrades = new CuiElementContainer();

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
                Text = { Text = ""}
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
                        AnchorMax = "1 1",
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
                    Command = "RCOIN_CONS OPEN UPGRADES"
                },
                Text = {Text = ""}
            },"Phone");

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
            //////////////////UPGRADES//////////////////UPGRADES/////////UPGRADES///////////////UPGRADES/////////////UPGRADES////////////////////////UPGRADES
            upgrades.Add(new CuiPanel
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
            
            upgrades.Add(new CuiElement
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
            int i = 0;
            foreach (var x in _upgrades)
            {
                
                upgrades.Add(new CuiElement
                {
                    Parent = "Upgrades",
                    Name = $"Upgrade{i}",
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
                            OffsetMin = $"-89 {-115 - (48 * i)}",
                            OffsetMax = $"85 {-70 - (48 * i)}"
                        }
                    }
                });
                upgrades.Add(new CuiElement
                {
                    Parent = $"Upgrade{i}",
                    Components =
                    {
                        new CuiRawImageComponent
                        {
                            Png = ImageLibrary.Call<string>("GetImage", x.Value.url)
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
                upgrades.Add(new CuiLabel //LABEL
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
                        Text = x.Value.name,
                        Align = TextAnchor.MiddleLeft,
                        FontSize = 12
                    }
                }, $"Upgrade{i}");
                upgrades.Add(new CuiLabel //HOWADD
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
                        Text = x.Value.addcoin.ToString("0.000") + "RC/5m",
                        Align = TextAnchor.MiddleCenter,
                        FontSize = 10,
                        Color = "0, 1, 0, 1"
                    }
                }, $"Upgrade{i}");
                upgrades.Add(new CuiLabel //COST
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
                        Text = x.Value.cost.ToString("0.000"),
                        Align = TextAnchor.MiddleCenter,
                        FontSize = 10,

                    }
                }, $"Upgrade{i}");
                upgrades.Add(new CuiLabel //COUNT
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
                        Text = x.Value.maxlvl.ToString(),
                        Align = TextAnchor.MiddleCenter,
                        FontSize = 10
                    }
                }, $"Upgrade{i}");
                
                i++;
            }
            upgrades.Add(new CuiElement
            {
                Parent = "Upgrades",
                Name = "Phone_upgrades",
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
            upgrades.Add(new CuiButton
            {
                RectTransform =
                {
                    AnchorMin = "0.5 0.5",
                    AnchorMax = "0.5 0.5",
                    OffsetMin = "-95 -40",
                    OffsetMax = "-50 -5"
                },
                Text = { Text = ""},
                Button =
                {
                    Color = "0, 0, 0, 0",
                    Command = "RCOIN_CONS HOME",
                }
                
            },"Phone_upgrades");
            
            main_json = main.ToJson();
            
            upgrades_json = upgrades.ToJson();
            
            main_balance_gui_json = main_balance_gui.ToJson();
            
        }

        [ChatCommand("rcoin")]
        void OpenMenu(BasePlayer player)
        {
            DataPlayer t;
            if (!_players.TryGetValue(player, out t)) return;
            
            CommunityEntity.ServerInstance.ClientRPCEx(
                new Network.SendInfo {connection = player.net.connection}, null, "DestroyUI", "closebutton");


            var avatar = GetImage(player.UserIDString);
            string jsonSend = main_json
                .Replace("[MAIN_AVATAR]", avatar)
                .Replace("[NICKNAME]", player.displayName)
                .Replace("[ID]", t.id.ToString("0000"))
                .Replace("[BALANCE]", t.coins.ToString("0.000"))
                .Replace("[TOP_POSITION]", "00001");

            CommunityEntity.ServerInstance.ClientRPCEx(
                new Network.SendInfo {connection = player.net.connection}, null, "AddUI", jsonSend);

            CommunityEntity.ServerInstance.ClientRPCEx(new Network.SendInfo {connection = player.net.connection}, null,
                "AddUI", main_balance_gui_json.Replace("[BALANCE]", t.coins.ToString("0.000")));
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
                    CommunityEntity.ServerInstance.ClientRPCEx(
                        new Network.SendInfo {connection = player.net.connection}, null, "DestroyUI", "main");
                    switch (args[1])
                    {
                        case "UPGRADES":
                        {
                            CommunityEntity.ServerInstance.ClientRPCEx(new Network.SendInfo {connection = player.net.connection}, null, "AddUI", upgrades_json);
                            
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
            if (info.coins - coin < 0.001) return false;
            info.coins -= coin;
            return true;
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
                (code2, response2) => ServerMgr.Instance.StartCoroutine(UpdateInfo(player, code2, response2)), this,
                Core.Libraries.RequestMethod.POST);
        }

        private void GetInfos(BasePlayer player)
        {
            webrequest.Enqueue($"https://lagzya.foxplugins.ru/rustcoin/getinfo.php", $"steamid={player.userID}",
                (code, response) => ServerMgr.Instance.StartCoroutine(GetInfo(player, code, response)), this,
                Core.Libraries.RequestMethod.POST);
        }

        private IEnumerator UpdateMysql()
        {
            while (this.IsLoaded)
            {
                Upgrades();
                yield return CoroutineEx.waitForSeconds(10f);
                ServerUpdate();
                foreach (var player in _players)
                {
                    double add = player.Value.upgrades.Sum(p =>
                    {
                        var t = 0.0;
                        if (_upgrades.ContainsKey(p.Key))
                        {
                            var lvl = p.Value;
                            if (p.Value > _upgrades[p.Key].maxlvl)
                                lvl = _upgrades[p.Key].maxlvl;
                            t = _upgrades[p.Key].addcoin * lvl;
                        }

                        return t;
                    });
                    player.Value.coins += add;
                    player.Value.serverid = serverId;
                    coins += add;
                    Update(player.Key, player.Value.coins, player.Value.serverid, player.Value.upgrades);
                }
            }

            yield break;
        }

        private Dictionary<BasePlayer, DataPlayer> _players = new Dictionary<BasePlayer, DataPlayer>();

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

        IEnumerator UpdateInfo(BasePlayer player, int code, string response)
        {
            if (response == null) yield break;
            if (code == 200)
            {
                yield return CoroutineEx.waitForSeconds(2f);
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
            if (response == null) yield break;
            if (code == 200)
            {
                if (!response.Contains("id"))
                {
                    ServerUpdate();
                    yield return CoroutineEx.waitForSeconds(2f);
                    SetServer();
                    yield break;
                }

                var json = JsonConvert.DeserializeObject<ServerId>(response);
                serverId = json.id;
                coins = json.coins;
            }

            yield break;
        }

        IEnumerator UpInfo(int code, string response)
        {
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
            }

            yield break;
        }

        IEnumerator GetInfo(BasePlayer player, int code, string response)
        {
            if (response == null) yield break;
            if (code == 200)
            {
                if (response == "test")
                {
                    Update(player, 0, serverId, new Dictionary<int, int>());
                    yield return CoroutineEx.waitForSeconds(2f);
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