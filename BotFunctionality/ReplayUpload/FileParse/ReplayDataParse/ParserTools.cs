using System.Text;

namespace ReplayDataParse
{
    internal class ReplayParser : IDisposable
    {
        private Stream _fileStream;
        private BinaryReader _reader;

        public ReplayParser(Stream replayFileStream)
        {
            CreateReader(replayFileStream);
        }

        private void CreateReader(Stream replayFileStream)
        {
            _fileStream = new MemoryStream();
            //replayFileStream.CopyTo(_fileStream);
            replayFileStream.Seek(0L, SeekOrigin.Begin);
            replayFileStream.CopyTo(_fileStream);
            replayFileStream.Seek(0L, SeekOrigin.Begin);
            _fileStream.Seek(0L, SeekOrigin.Begin);
            _reader = new BinaryReader(_fileStream, Encoding.ASCII);
        }

        public void Dispose()
        {
            _reader?.Dispose();
            _fileStream?.Dispose();
        }

        public ReplayData Parse()
        {
            var replayData = new ReplayData();
            string fileType = new string(_reader.ReadChars(8));
            replayData.TimestampStart = _reader.ReadUInt32();
            replayData.TimestampEnd = _reader.ReadUInt32();

            if (replayData.TimestampEnd == 0)
            {
                replayData.TimestampEnd = replayData.TimestampStart;
            }

            byte[] trash1 = _reader.ReadBytes(21);
            string trash2 = Read2ByteString();
            ushort[] ArrayOfTrash = new ushort[8];
            ArrayOfTrash[0] = _reader.ReadUInt16();
            ArrayOfTrash[1] = _reader.ReadUInt16();
            ArrayOfTrash[2] = _reader.ReadUInt16();
            ArrayOfTrash[3] = _reader.ReadUInt16();
            ArrayOfTrash[4] = _reader.ReadUInt16();
            ArrayOfTrash[5] = _reader.ReadUInt16();
            ArrayOfTrash[6] = _reader.ReadUInt16();
            ArrayOfTrash[7] = _reader.ReadUInt16();

            trash2 = Read2ByteString();
            trash2 = Read2ByteString();
            ushort trash3 = _reader.ReadUInt16();
            replayData.VersionMajor = _reader.ReadUInt16();
            trash1 = _reader.ReadBytes(13);
            replayData.HeaderRawData = Read1ByteString();

            if (!string.IsNullOrWhiteSpace(replayData.HeaderRawData))
            {
                ParseHeader(replayData);
            }

            replayData.ReplaySaver = Read1ByteString();
            return replayData;
        }

        private void ParseHeader(ReplayData replayData)
        {
            string[] headerRawData = replayData.HeaderRawData.Split(';');

            var headerFields = new HeaderFields
            {
                MapName = headerRawData[0],
                Seed = headerRawData[3].Replace("SD=", string.Empty),
            };

            headerFields.Players = ParsePlayers(headerRawData[8]);

            try
            {
                var parsedSeed = int.Parse(headerFields.Seed);

                if (parsedSeed > 0)
                {
                    headerFields.Players = ExtractRandomFactionsFromPlayerInfo(
                        (uint)parsedSeed,
                        headerFields.Players,
                        headerFields.Players.Any(x => x.Spot >= 0));
                }
                else
                {
                    foreach (var player in headerFields.Players)
                    {
                        player.faction = RotwkFaction.FromReplayID(player.Faction).ToString();
                    }
                }
            }
            catch (System.FormatException e)
            {
                var e2 = new Exception("SD parameter (random seed) in replay file has invalid format. Not a valid uint32: " + headerFields.Seed);
                e2.Data.Add("Original exception", e);
                throw e2;
            }
            replayData.HeaderFields = headerFields;
        }

        private List<Palyer> ParsePlayers(string playerData)
        {
            var players = new List<Palyer>();

            if (string.IsNullOrWhiteSpace(playerData) || string.Equals(playerData.Substring(1, 2), "S=", StringComparison.InvariantCultureIgnoreCase))
            {
                return players;
            }

            foreach (string token in playerData.Substring(2, playerData.Length - 2).Split(':'))
            {
                if (string.IsNullOrWhiteSpace(token))
                {
                    continue;
                }

                bool isPlayer = string.Equals(token.Substring(0, 1), "H", StringComparison.InvariantCultureIgnoreCase);

                if (!isPlayer)
                {
                    continue;
                }

                players.Add(ParsePlayerData(token));
            }
            return players;
        }

        private Palyer ParsePlayerData(string token)
        {
            Palyer guy = new Palyer();
            var playerData = token.Split(',');
            guy.Name = playerData[0].Substring(1, playerData[0].Length - 1);
            guy.Faction = int.Parse(playerData[5]);
            guy.Spot = int.Parse(playerData[6]);
            return guy;
        }

        private List<Palyer> ExtractRandomFactionsFromPlayerInfo(
             uint seed,
             List<Palyer> players,
             bool spotsAreManual
         )
        {
            var rng = new RotwkRngService(seed);
            //If no spots are set, trigger one random event simulating the first player's position being picked
            if (!spotsAreManual)
            {
                rng.NextRng();
            }

            //Each observer also triggers a random spot event
            var obsCount = players.Count(p => RotwkFaction.FromReplayID(p.Faction).IsObserver());
            RepeatAction(obsCount, () => rng.NextRng());
            var skip = (int)seed % 7;

            //Resolve factions and colors for all players who didn't pick them
            foreach (var player in players)
            {
                var factionInReplay = RotwkFaction.FromReplayID(player.Faction);

                if (factionInReplay.IsRandom())
                {
                    // RNG skip the engine does, we don't know why.
                    RepeatAction(skip, () => rng.NextRng());
                    var resolvedFaction = rng.NextRandomFaction();
                    player.faction = resolvedFaction.ToString();
                }
                else
                {
                    player.faction = factionInReplay.ToString();
                }
            }
            return players;
        }

        private void RepeatAction(int repeatCount, Action action)
        {
            for (int i = 0; i < repeatCount; i++)
            {
                action();
            }
        }

        private string Read1ByteString()
        {
            var stringBuilder = new StringBuilder();
            while (true)
            {
                var character = _reader.ReadChar();
                if (character == '\0')
                {
                    break;
                }
                stringBuilder.Append(character);
            }
            return stringBuilder.ToString();
        }
        private string Read2ByteString()
        {
            var stringBuilder = new StringBuilder();
            while (true)
            {
                short characterValue = _reader.ReadInt16();
                if (characterValue == 0)
                {
                    break;
                }
                stringBuilder.Append(Convert.ToChar(char.ConvertFromUtf32(characterValue)));
            }
            return stringBuilder.ToString();
        }

    }
    internal class RotwkRngService
    {
        private const ulong _mask = 0xFFFFFFFF;
        private const uint _mult = 134775813;
        private const uint _incr = 1;

        private uint _currentSeed;

        public RotwkRngService(uint initialSeed)
        {
            _currentSeed = PerformMask((ulong)initialSeed * 0x7FFFFFED);
        }

        public uint NextRng()
        {
            // Generate the xorfactor
            var xorFactor = (ulong)_currentSeed * _mult >> 32;
            //Just for safety in case the bitshift shifts in 1s on the most significant side
            var xorFactorMasked = PerformMask(xorFactor);

            // Update the seed
            _currentSeed = PerformMask((ulong)(_currentSeed * _mult + _incr));

            // Return random value
            return _currentSeed ^ xorFactorMasked;
        }

        public int NextRngInRange(int start, int end)
        {
            var next = NextRng();
            long modu = next % (end - start + 1);

            return start + (int)modu;
        }

        public RotwkFaction NextRandomFaction()
        {
            var r = NextRngInRange(0, 1000);
            var faction = r % RotwkFaction.GetNumberOfPlayableFactions();

            return RotwkFaction.FromRandom(faction); ;
        }
        private uint PerformMask(ulong largenumber)
        {
            return (uint)(largenumber & _mask);
        }

        public uint GetSeed()
        {
            return _currentSeed;
        }
    }
}
