using Discord;

namespace ReplayUpload
{
    internal class Uploader
    {
        private UploadFunctionality Functionality;
        private List<string> LastFileNames;

        public Uploader()
        {
            Functionality = new UploadFunctionality();
        }

        public void AddMoreData(List<Stream> Files, List<string> fileNames)
        {
            LastFileNames = fileNames;
            Functionality.AddMoreData(Files, fileNames);
        }
        public List<EmbedBuilder> GetEmbeds()
        {
            List<EmbedBuilder> Embeds = new List<EmbedBuilder>();
            List <VerifiedData> verifiedData = Functionality.GetLastVerifiedDataList();

            for (int i = 0; i < verifiedData.Count; i++)
            {
                bool swapBack = false;
                VerifiedData info = verifiedData[i];
                string observerListText = info.ObserverList.Any() ? string.Join(", ", info.ObserverList) : "none";
                string winner = "🏆\n💥";
                string MapToDisplay = MapUrlDictionary["Unknown map"]; 

                if (info.Swap)
                {
                    (info.Player1, info.Player2) = (info.Player2, info.Player1);
                    (info.Faction1ID, info.Faction2ID) = (info.Faction2ID, info.Faction1ID);
                    winner = "💥\n🏆";
                    swapBack = true;
                }

                if (char.ToLower(LastFileNames[i][0]) != 'w' && char.ToLower(LastFileNames[i][0]) != 'l')
                {
                    winner = "-\n-";
                }

                if (MapUrlDictionary.ContainsKey(info.Map)){MapToDisplay = MapUrlDictionary[info.Map];}

                var embed = new EmbedBuilder
                {
                    Title = $"{info.FileName}",
                    Color = new Color(0, 255, 255), // 0x00FFFF
                    Fields = new List<EmbedFieldBuilder>
                    {
                        new EmbedFieldBuilder
                        {
                            Name = "Players",
                            Value = $"{FactionEmoteDictionary[info.Faction1ID]} {info.Player1}\n{FactionEmoteDictionary[info.Faction2ID]} {info.Player2}",
                            IsInline = true
                        },
                        new EmbedFieldBuilder
                        {
                             Name = "Result",
                             Value = winner,
                             IsInline = true
                        },
                        new EmbedFieldBuilder
                        {
                             Name = "Details",
                             Value = $"Map: {info.Map}\nObservers: {observerListText}\nMatch Date: {info.TimeReplayEnded} {info.DateReplayEnded}\nDuration: {info.ReplayDuration}\nReplay owner: {info.OwnerName}\nPatch Version: {info.Version}",
                             IsInline = false
                        }
                    },
                    ThumbnailUrl = $"{MapToDisplay}"
                };
                //embed.WithUrl("https://events.laterredumilieu.fr/en/league/matchs");
                Embeds.Add(embed);

                if (swapBack)
                {
                    (info.Player2, info.Player1) = (info.Player1, info.Player2);
                    (info.Faction2ID, info.Faction1ID) = (info.Faction1ID, info.Faction2ID);
                }
            }
            return Embeds;
        }

        public async Task GameApproved (int FileIndex)
        {
           await Functionality.UploadData(FileIndex);    

        }

        public List<string> FetchNicknames(string key)
        {
            return Functionality.FetchNicknames(key);
        }

        public static Dictionary<string, string> FactionEmoteDictionary = new Dictionary<string, string>()
        {
           {"1", "🌳"},
           {"2","🖐"},
           {"3", "❄️"},
           {"4", "👁"},
           {"5", "🌿"},
           {"6", "🐐"},
           {"7", "🏔"}
        };

        public static Dictionary<string, string> MapUrlDictionary = new Dictionary<string, string>()
        {
           {"Jungles of Far Harad", "https://api.laterredumilieu.fr/uploads/jungles_map_1288c12d6a.jpg?updated_at=2022-05-15T14:39:36.718Z"},
           {"Sakura Forest", "https://api.laterredumilieu.fr/uploads/Sakura_map_ab131caa63.jpg?updated_at=2022-05-15T14:41:23.826Z"},
           {"Westfold", "https://api.laterredumilieu.fr/uploads/westfold_map_c787c6055b.jpg?updated_at=2022-05-15T14:42:07.969Z"},
           {"Ettenmoors", "https://api.laterredumilieu.fr/uploads/Most_EA_Strategy_Games_Screenshot_2022_07_05_04_47_52_47_b2cf72f7d6.png"},
           {"Brandy Hills", "https://api.laterredumilieu.fr/uploads/minimappp_8d097d5a6d.png?updated_at=2022-06-09T17:34:08.766Z"},
           {"Chetwood", "https://api.laterredumilieu.fr/uploads/fore_836b20e750.png?updated_at=2022-06-09T18:23:18.462Z"},
           {"Sorow Isles", "https://api.laterredumilieu.fr/uploads/Screenshot_12_06_2022_21_25_15_e444c3abde.jpg?updated_at=2022-06-12T19:28:12.816Z"},
           {"Belfalas Abyss", "https://api.laterredumilieu.fr/uploads/Screenshot_13_06_2022_14_31_14_26d95ba696.jpg?updated_at=2022-06-13T12:35:16.389Z"},
           {"Eastfold II", "https://api.laterredumilieu.fr/uploads/Most_EA_Strategy_Games_Screenshot_2022_08_14_17_55_21_34_68cea8ca1e.png"},
           {"Rhun Shore", "https://api.laterredumilieu.fr/uploads/Most_EA_Strategy_Games_Screenshot_2022_07_03_17_53_18_66_facc3684e0.png"},
           {"Blood Gulch", "https://api.laterredumilieu.fr/uploads/Most_EA_Strategy_Games_Screenshot_2022_07_02_21_25_43_27_cf46968758.png"},
           {"Cirdan's Cove", "https://api.laterredumilieu.fr/uploads/Most_EA_Strategy_Games_Screenshot_2022_07_02_21_25_43_27_cf46968758.png"},
           {"Dunland II", "https://api.laterredumilieu.fr/uploads/Most_EA_Strategy_Games_Screenshot_2022_07_03_16_43_26_20_3e63b8ed80.png?updated_at=2022-07-03T14:58:37.368Z"},
           {"Erech 4th Age", "https://api.laterredumilieu.fr/uploads/Most_EA_Strategy_Games_Screenshot_2022_07_03_16_43_41_87_3705652a03.png"},
           {"Eryn Laer", "https://api.laterredumilieu.fr/uploads/Most_EA_Strategy_Games_Screenshot_2022_07_03_16_43_50_18_2240c98e4f.png?updated_at=2022-07-03T14:58:37.871Z"},
           {"Fangorn", "https://api.laterredumilieu.fr/uploads/Most_EA_Strategy_Games_Screenshot_2022_07_03_16_43_57_33_5a29cb7b90.png"},
           {"Firien Dale", "https://api.laterredumilieu.fr/uploads/Most_EA_Strategy_Games_Screenshot_2022_07_03_16_44_03_59_90c87470a9.png?updated_at=2022-07-03T14:58:37.719Z"},
           {"Fords Of Bruinen", "https://api.laterredumilieu.fr/uploads/Most_EA_Strategy_Games_Screenshot_2022_07_03_16_44_16_70_5327b729e5.png"},
           {"Fords Of Isen", "https://api.laterredumilieu.fr/uploads/Most_EA_Strategy_Games_Screenshot_2022_07_03_16_44_26_59_fa26fe314e.png?updated_at=2022-07-03T14:58:38.697Z"},
           {"Gap of Rohan", "https://api.laterredumilieu.fr/uploads/Most_EA_Strategy_Games_Screenshot_2022_07_03_16_44_34_12_be3a77d2e5.png"},
           {"Glenford", "https://api.laterredumilieu.fr/uploads/Most_EA_Strategy_Games_Screenshot_2022_07_03_16_44_38_08_efdf407680.png"},
           {"Hollin", "https://api.laterredumilieu.fr/uploads/Most_EA_Strategy_Games_Screenshot_2022_07_03_16_44_46_13_afd32b527f.png?updated_at=2022-07-03T14:58:39.658Z"},
           {"Last Bridge", "https://api.laterredumilieu.fr/uploads/Most_EA_Strategy_Games_Screenshot_2022_07_03_16_45_03_60_70db3f6343.png"},
           {"Lossarnach", "https://api.laterredumilieu.fr/uploads/Most_EA_Strategy_Games_Screenshot_2022_07_03_16_45_57_94_711a85db23.png"},
           {"Mering Stream", "https://api.laterredumilieu.fr/uploads/Most_EA_Strategy_Games_Screenshot_2022_07_03_16_46_11_68_af51e6ccad.png"},
           {"Mirkwood II", "https://api.laterredumilieu.fr/uploads/Most_EA_Strategy_Games_Screenshot_2022_07_03_16_46_21_37_8c0a1d55a8.png"},
           {"Plains of Lindon", "https://api.laterredumilieu.fr/uploads/Most_EA_Strategy_Games_Screenshot_2022_07_03_16_46_27_24_4c6dacad71.png"},
           {"Sirannon", "https://api.laterredumilieu.fr/uploads/Most_EA_Strategy_Games_Screenshot_2022_07_03_16_46_08_86_b6b6d69f43.png"},
           {"The Iron Hills", "https://api.laterredumilieu.fr/uploads/Most_EA_Strategy_Games_Screenshot_2022_07_03_16_46_40_83_5b791142dd.png"},
           {"Tombs of Karna", "https://api.laterredumilieu.fr/uploads/Most_EA_Strategy_Games_Screenshot_2022_07_03_16_46_48_97_9332f12681.png"},
           {"Tyrn Gorthad", "https://api.laterredumilieu.fr/uploads/Most_EA_Strategy_Games_Screenshot_2022_07_03_16_46_55_03_ab57523e15.png"},
           {"Ettenmoors II", "https://api.laterredumilieu.fr/uploads/Most_EA_Strategy_Games_Screenshot_2022_07_17_23_34_31_15_eb0c7152c1.png"},
           {"Rhudaur", "https://api.laterredumilieu.fr/uploads/unknown_225c34ac96.png"},
           {"Midgewater", "https://api.laterredumilieu.fr/uploads/Most_EA_Strategy_Games_Screenshot_2022_10_22_16_17_30_25_2_0ef0e83286.png"}, //bywater?
           {"Fords of Isen (unlame)", "https://api.laterredumilieu.fr/uploads/foi_unlame_edited_9ecb6611e7.png?updated_at=2023-09-10T16:43:29.179Z"},
            {"Unknown map", "https://api.laterredumilieu.fr/uploads/blank_map_19dd703b7b.png?updated_at=2023-09-16T18:14:56.161Z" }
        };
    }
}
