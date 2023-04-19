using System;
using Oxide.Game.Rust.Cui;
using System.Collections.Generic;
using Oxide.Core.Libraries.Covalence;
using Oxide.Core.Plugins;
using Oxide.Core;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Text;

namespace Oxide.Plugins
{
    [Info("Menu", "ArNnIx", "1.0.0")]
    [Description("Test menu :) ")]

    public class Menu : RustPlugin
    {   
        [ChatCommand("menu")]
        void open_menu(BasePlayer player)
        {
            CuiElementContainer menu = generate_menu(player);
            CuiHelper.AddUi(player, menu);
        }

        [ConsoleCommand("menu_remove")]
        void menu_remove(ConsoleSystem.Arg Args){
            BasePlayer player = BasePlayer.FindByID(System.Convert.ToUInt64(Args.Args[0]));

            CuiHelper.DestroyUi(player, "menu_panel");
        }

        [ConsoleCommand("button_show")]
        void show_button(ConsoleSystem.Arg args)
        {
            var player = args.Player();
            if (player == null) return;

            CuiHelper.DestroyUi(player, "rules_text");
            CuiHelper.DestroyUi(player, "prikazy_text");
            CuiHelper.DestroyUi(player, "podpora_text");
            CuiHelper.DestroyUi(player, "kontakty_text");
            CuiHelper.DestroyUi(player, "button_next");
            CuiHelper.DestroyUi(player, "button_back");

            var elements = new CuiElementContainer();
            {
                var next_button = elements.Add(new CuiButton{
                    Button = {
                        Command = $"Pravidla_next",
                        Color = "0.8 0.8 0.8 0.2"
                    },
                    RectTransform = {
                        AnchorMin = "0.83 0.01", 
                        AnchorMax = "0.88 0.05" 
                    },
                    Text = {
                        Text = "Next >>",
                        FontSize = 18,
                        Align = TextAnchor.MiddleCenter
                    },
                }, "menu_panel", "button_next");

                var back_button = elements.Add(new CuiButton{
                    Button = {
                        Command = $"Pravidla_back",
                        Color = "0.8 0.8 0.8 0.2"
                    },
                    RectTransform = {
                        AnchorMin = "0.78 0.01", 
                        AnchorMax = "0.83 0.05" 
                    },
                    Text = {
                        Text = "<< Back",
                        FontSize = 18,
                        Align = TextAnchor.MiddleCenter
                    },
                }, "menu_panel", "button_back");
            }

            CuiHelper.AddUi(player, elements);
        }

        [ConsoleCommand("Pravidla_show")]
        void show_pravidla(ConsoleSystem.Arg args)
        {
            var player = args.Player();
            if (player == null) return;

            CuiHelper.DestroyUi(player, "rules_text");
            CuiHelper.DestroyUi(player, "prikazy_text");
            CuiHelper.DestroyUi(player, "podpora_text");
            CuiHelper.DestroyUi(player, "kontakty_text");
            CuiHelper.DestroyUi(player, "button_next");
            CuiHelper.DestroyUi(player, "button_back");

            show_button(args);

            var elements = new CuiElementContainer();
            {
                string filePath = Path.Combine(Interface.Oxide.PluginDirectory, "ArNnIx/Menu/Menu_text.json");
                string json = File.ReadAllText(filePath);

                // Get the allowed page range
                int minPage = 1;
                int maxPage = 2;

                // Clamp the current page number to the allowed range
                int clampedPage = Mathf.Clamp(currentPage, minPage, maxPage);

                JObject obj = JObject.Parse(json);
                JArray rulesArray = obj.GetValue($"pravidla {clampedPage}") as JArray;
                if (rulesArray != null)
                {
                    string ruleText = string.Join("\n", rulesArray.Select(x => x.ToString()));
                    var text = elements.Add(new CuiLabel{
                        RectTransform = { 
                            AnchorMin = "0.21 0.09",
                            AnchorMax = "1 0.99"
                        },
                        Text = {
                            Text = ruleText,
                            FontSize = 17,
                            Align = TextAnchor.MiddleLeft
                        },
                    }, "menu_panel", "rules_text");
                }
            }

            CuiHelper.AddUi(player, elements);
        }

        Dictionary<ulong, int> playerPages = new Dictionary<ulong, int>();
        int currentPage = 1;
        [ConsoleCommand("Pravidla_back")]
        void Pravidla_back(ConsoleSystem.Arg Args)
        {
            var player = Args.Player();
            if (player == null) return;

            if (playerPages.ContainsKey(player.userID))
            {
                currentPage = playerPages[player.userID];
            }

            currentPage--;
            if (currentPage < 1) currentPage = 1; // Ošetření minimální hodnoty stránky

            playerPages[player.userID] = currentPage; // Aktualizace proměnné pro hráče

            show_pravidla(Args);

            // player.ChatMessage($"Current page: {currentPage}");
        }

        [ConsoleCommand("Pravidla_next")]
        void Pravidla_next(ConsoleSystem.Arg Args)
        {
            var player = Args.Player();
            if (player == null) return;

            if (playerPages.ContainsKey(player.userID))
            {
                currentPage = playerPages[player.userID];
            }

            currentPage++;
            if (currentPage > 2) currentPage = 2; // Ošetření maximální hodnoty stránky

            playerPages[player.userID] = currentPage; // Aktualizace proměnné pro hráče

            show_pravidla(Args);

            // player.ChatMessage($"Current page: {currentPage}");
        }

        // [ConsoleCommand("Pravidla_show")]
        // void show_pravidla(ConsoleSystem.Arg args)
        // {
        //     var player = args.Player();
        //     if (player == null) return;

        //     CuiHelper.DestroyUi(player, "rules_text");
        //     CuiHelper.DestroyUi(player, "prikazy_text");
        //     CuiHelper.DestroyUi(player, "podpora_text");
        //     CuiHelper.DestroyUi(player, "kontakty_text");

        //     var elements = new CuiElementContainer();
        //     {
        //         string filePath = Path.Combine(Interface.Oxide.PluginDirectory, "ArNnIx/Menu/Menu_text.json");
        //         string json = File.ReadAllText(filePath);

        //         JObject obj = JObject.Parse(json);
        //         JArray rulesArray = obj.GetValue("pravidla 1") as JArray;
        //         if (rulesArray != null)
        //         {
        //             string ruleText = string.Join("\n", rulesArray.Select(x => x.ToString()));
        //             var text = elements.Add(new CuiLabel{
        //                 RectTransform = { 
        //                     AnchorMin = "0.21 0.09",
        //                     AnchorMax = "1 0.99"
        //                 },
        //                 Text = {
        //                     Text = ruleText,
        //                     FontSize = 17,
        //                     Align = TextAnchor.MiddleLeft
        //                 },
        //             }, "menu_panel", "rules_text");
        //         }
        //     }

        //     CuiHelper.AddUi(player, elements);
        // }



        [ConsoleCommand("prikazy_show")]
        void show_prikazy(ConsoleSystem.Arg args)
        {
            var player = args.Player();
            if (player == null) return;

            CuiHelper.DestroyUi(player, "rules_text");
            CuiHelper.DestroyUi(player, "prikazy_text");
            CuiHelper.DestroyUi(player, "podpora_text");
            CuiHelper.DestroyUi(player, "kontakty_text");
            CuiHelper.DestroyUi(player, "button_next");
            CuiHelper.DestroyUi(player, "button_back");

            var elements = new CuiElementContainer();
            {
                string filePath = Path.Combine(Interface.Oxide.PluginDirectory, "ArNnIx/Menu/Menu_text.json");
                string json = File.ReadAllText(filePath);

                JObject obj = JObject.Parse(json);
                JArray rulesArray = obj.GetValue("prikazy") as JArray;
                if (rulesArray != null)
                {
                    string ruleText = string.Join("\n", rulesArray.Select(x => x.ToString()));
                    var text = elements.Add(new CuiLabel{
                        RectTransform = { 
                            AnchorMin = "0.21 0.09",
                            AnchorMax = "1 0.99"
                        },
                        Text = {
                            Text = ruleText,
                            FontSize = 17,
                            Align = TextAnchor.MiddleLeft
                        },
                    }, "menu_panel", "prikazy_text");
                }
            }

            CuiHelper.AddUi(player, elements);
        }

        [ConsoleCommand("podpora_show")]
        void show_podpora(ConsoleSystem.Arg args)
        {
            var player = args.Player();
            if (player == null) return;

            CuiHelper.DestroyUi(player, "rules_text");
            CuiHelper.DestroyUi(player, "prikazy_text");
            CuiHelper.DestroyUi(player, "podpora_text");
            CuiHelper.DestroyUi(player, "kontakty_text");
            CuiHelper.DestroyUi(player, "button_next");
            CuiHelper.DestroyUi(player, "button_back");

            var elements = new CuiElementContainer();
            {
                string filePath = Path.Combine(Interface.Oxide.PluginDirectory, "ArNnIx/Menu/Menu_text.json");
                string json = File.ReadAllText(filePath);

                JObject obj = JObject.Parse(json);
                JArray rulesArray = obj.GetValue("podpora") as JArray;
                if (rulesArray != null)
                {
                    string ruleText = string.Join("\n", rulesArray.Select(x => x.ToString()));
                    var text = elements.Add(new CuiLabel{
                        RectTransform = { 
                            AnchorMin = "0.21 0.09",
                            AnchorMax = "1 0.99"
                        },
                        Text = {
                            Text = ruleText,
                            FontSize = 17,
                            Align = TextAnchor.MiddleLeft
                        },
                    }, "menu_panel", "podpora_text");
                }
            }

            CuiHelper.AddUi(player, elements);
        }

        [ConsoleCommand("kontakty_show")]
        void show_kontakty(ConsoleSystem.Arg args)
        {
            var player = args.Player();
            if (player == null) return;

            CuiHelper.DestroyUi(player, "rules_text");
            CuiHelper.DestroyUi(player, "prikazy_text");
            CuiHelper.DestroyUi(player, "podpora_text");
            CuiHelper.DestroyUi(player, "kontakty_text");
            CuiHelper.DestroyUi(player, "button_next");
            CuiHelper.DestroyUi(player, "button_back");

            var elements = new CuiElementContainer();
            {
                string filePath = Path.Combine(Interface.Oxide.PluginDirectory, "ArNnIx/Menu/Menu_text.json");
                string json = File.ReadAllText(filePath);

                JObject obj = JObject.Parse(json);
                JArray rulesArray = obj.GetValue("kontakty") as JArray;
                if (rulesArray != null)
                {
                    string ruleText = string.Join("\n", rulesArray.Select(x => x.ToString()));
                    var text = elements.Add(new CuiLabel{
                        RectTransform = { 
                            AnchorMin = "0.21 0.09",
                            AnchorMax = "1 0.99"
                        },
                        Text = {
                            Text = ruleText,
                            FontSize = 17,
                            Align = TextAnchor.MiddleLeft
                        },
                    }, "menu_panel", "kontakty_text");
                }
            }

            CuiHelper.AddUi(player, elements);
        }




        CuiElementContainer generate_menu(BasePlayer player){
            var elements = new CuiElementContainer();
            var panel = elements.Add(new CuiPanel{

                Image = {
                    Color = "0.1 0.1 0.1 0.98",
                },

                RectTransform = {
                    AnchorMin = "0.1 0.1",
                    AnchorMax = "0.9 0.9"
                },

                CursorEnabled = true
            }, "Hud", "menu_panel");

            var close_button = elements.Add(new CuiButton{

                Button = {
                    Command = "menu_remove " + player.userID.ToString(),
                    Color = "0.8 0.8 0.8 0.2"
                },

                RectTransform = {
                    AnchorMin = "0.89 0.01",
                    AnchorMax = "0.99 0.07",
                },

                Text = {
                    Text = "Close",
                    FontSize = 18,
                    Align = TextAnchor.MiddleCenter
                }

            }, panel);

            var panel1 = elements.Add(new CuiPanel{

                Image = {
                    Color = "0.7 0.7 0.7 0.2",
                },

                RectTransform = {
                    AnchorMin = "0 0",
                    AnchorMax = "0.19 1"
                },

            }, panel);

            var panel_line = elements.Add(new CuiPanel{

                Image = {
                    Color = "0.7 0.7 0.7 1"
                },

                RectTransform = {
                    AnchorMin = "0 0.75",
                    AnchorMax = "1 0.75"
                },

            }, panel1);

            var panel_label = elements.Add(new CuiLabel{

                RectTransform = { 
                    AnchorMin = "0 0.73", 
                    AnchorMax = "1 1" 
                },

                Text = {
                    Text = "Informace",
                    FontSize = 30,
                    Align = TextAnchor.MiddleCenter
                },
            }, panel1);

            var pravidla_button = elements.Add(new CuiButton{
                Button = {
                    Command = "pravidla_show",
                    Color = "0.8 0.8 0.8 0.2"
                },
                RectTransform = {
                    AnchorMin = "0 0.65",
                    AnchorMax = "1 0.73"
                },
                Text = {
                    Text = "Pravidla",
                    FontSize = 18,
                    Align = TextAnchor.MiddleCenter
                },
            }, panel1);


            var prikazy_button = elements.Add(new CuiButton{

                Button = {
                    Command = "prikazy_show",
                    Color = "0.8 0.8 0.8 0.2"
                },

                RectTransform = {
                    AnchorMin = "0 0.55",
                    AnchorMax = "1 0.63"
                },

                Text = {
                    Text = "Příkazy",
                    FontSize = 18,
                    Align = TextAnchor.MiddleCenter
                }

            }, panel1);

            var podpora_button = elements.Add(new CuiButton{

                Button = {
                    Command = "Podpora_show",
                    Color = "0.8 0.8 0.8 0.2"
                },

                RectTransform = {
                    AnchorMin = "0 0.45",
                    AnchorMax = "1 0.53"
                },

                Text = {
                    Text = "Podpora",
                    FontSize = 18,
                    Align = TextAnchor.MiddleCenter
                }

            }, panel1);

            var kontakty_button = elements.Add(new CuiButton{

                Button = {
                    Command = "Kontakty_show",
                    Color = "0.8 0.8 0.8 0.2"
                },

                RectTransform = {
                    AnchorMin = "0 0.35",
                    AnchorMax = "1 0.43"
                },

                Text = {
                    Text = "Kontakty",
                    FontSize = 18,
                    Align = TextAnchor.MiddleCenter
                }

            }, panel1);

            var create_label = elements.Add(new CuiLabel{

                RectTransform = { 
                    AnchorMin = "0 0", 
                    AnchorMax = "1 0.06" 
                },

                Text = {
                    Text = "By ArNnIx",
                    FontSize = 10,
                    Align = TextAnchor.MiddleCenter
                },

            }, panel1);

            return elements;
        }
    }
}

