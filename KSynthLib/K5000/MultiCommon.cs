using System.Text;
using System.Collections.Generic;
using System.IO;

using SyxPack;
using KSynthLib.Common;
using System;

namespace KSynthLib.K5000
{
    /// <summary>
    /// Represents common settings for a K5000S multi patch (combi on K5000W/R).
    /// </summary>
    public class MultiCommon : ISystemExclusiveData
    {
        public const int DataSize = 54;

        public EffectAlgorithm EffectAlgorithm; // 1~4 (in SysEx 0~3)
        public ReverbSettings Reverb;
        public EffectSettings Effect1;
        public EffectSettings Effect2;
        public EffectSettings Effect3;
        public EffectSettings Effect4;
        public GEQSettings GEQ;

        /// <value>
        /// Name of the multi/combi patch.
        /// </value>
        public PatchName Name;

        public PositiveLevel Volume;

        public bool[] IsSectionMuted;  // 0=muted, 1=not muted - store the inverse here

        public EffectControl EffectControl1;
        public EffectControl EffectControl2;

        public MultiCommon()
        {
            this.Name = new PatchName("NewMulti");


        }

        public MultiCommon(byte[] data)
        {
            int offset = 0;

            using (MemoryStream memory = new MemoryStream(data, false))
	        {
                using (BinaryReader reader = new BinaryReader(memory))
                {
                    Console.WriteLine($"{offset}: effect algorithm");
                    offset += 1;
                    byte effectAlgorithm = reader.ReadByte();
                    // Seems like the K5000 might set the top bits of this, so mask them off
                    this.EffectAlgorithm = (EffectAlgorithm)(effectAlgorithm & 0x03);

                    Console.WriteLine($"{offset}: reverb");
                    offset += ReverbSettings.DataSize;
                    byte[] reverbData = reader.ReadBytes(ReverbSettings.DataSize);
                    this.Reverb = new ReverbSettings(reverbData);

                    Console.WriteLine($"{offset}: effect 1 settings");
                    offset += EffectSettings.DataSize;
                    byte[] effect1Data = reader.ReadBytes(EffectSettings.DataSize);
                    this.Effect1 = new EffectSettings(effect1Data);

                    Console.WriteLine($"{offset}: effect 2 settings");
                    offset += EffectSettings.DataSize;
                    byte[] effect2Data = reader.ReadBytes(EffectSettings.DataSize);
                    this.Effect2 = new EffectSettings(effect2Data);

                    Console.WriteLine($"{offset}: effect 3 settings");
                    offset += EffectSettings.DataSize;
                    byte[] effect3Data = reader.ReadBytes(EffectSettings.DataSize);
                    this.Effect3 = new EffectSettings(effect3Data);

                    Console.WriteLine($"{offset}: effect 4 settings");
                    offset += EffectSettings.DataSize;
                    byte[] effect4Data = reader.ReadBytes(EffectSettings.DataSize);
                    this.Effect4 = new EffectSettings(effect4Data);

                    Console.WriteLine($"{offset}: GEQ settings");
                    offset += GEQSettings.DataSize;
                    byte[] geqData = reader.ReadBytes(GEQSettings.DataSize);
                    this.GEQ = new GEQSettings(geqData);

                    Console.WriteLine($"{offset}: name");
                    offset += PatchName.Length;
                    byte[] nameData = reader.ReadBytes(PatchName.Length);
                    this.Name = new PatchName(nameData);
                    Console.WriteLine($"(by the way, the name is '{this.Name}')");

                    Console.WriteLine($"{offset}: volume");
                    offset += 1;
                    this.Volume = new PositiveLevel(reader.ReadByte());

                    Console.WriteLine($"{offset}: mute");
                    offset += 1;
                    // TODO: Parse the mute byte
                    var muteByte = reader.ReadByte();

                    Console.WriteLine($"{offset}: effect control 1 data");
                    offset += 3;
                    byte[] ec1Data = reader.ReadBytes(3);
                    this.EffectControl1 = new EffectControl(ec1Data);

                    Console.WriteLine($"{offset}: effect control 2 data");
                    offset += 3;
                    byte[] ec2Data = reader.ReadBytes(3);
                    Console.WriteLine($"ec2Data length = {ec2Data.Length}");
                    this.EffectControl2 = new EffectControl(ec2Data);

                    Console.WriteLine($"{offset}: no more multi data");
                }
            }
        }

#region ISystemExclusiveData implementation for MultiCommon

        public List<byte> Data
        {
            get
            {
                var data = new List<byte>();

                data.Add((byte)this.EffectAlgorithm);
                data.AddRange(this.Reverb.Data);
                data.AddRange(this.Effect1.Data);
                data.AddRange(this.Effect2.Data);
                data.AddRange(this.Effect3.Data);
                data.AddRange(this.Effect4.Data);
                data.AddRange(this.GEQ.Data);
                data.AddRange(this.Name.Data);
                data.Add(this.Volume.ToByte());

                // TODO: the section mutes byte
                data.Add(0);

                data.AddRange(this.EffectControl1.Data);
                data.AddRange(this.EffectControl2.Data);

                return data;
            }
        }

        public int DataLength => DataSize;
    }

#endregion
}
