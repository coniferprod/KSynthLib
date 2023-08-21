using System.Text;
using System.Collections.Generic;
using System.IO;

using SyxPack;
using KSynthLib.Common;

namespace KSynthLib.K5000
{
    /// <summary>
    /// Represents common settings for a K5000S multi patch (combi on K5000W/R).
    /// </summary>
    public class MultiCommon : ISystemExclusiveData
    {
        public static readonly int DataSize = 54;

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
            using (MemoryStream memory = new MemoryStream(data, false))
	        {
                using (BinaryReader reader = new BinaryReader(memory))
                {
                    byte effectAlgorithm = reader.ReadByte();
                    // Seems like the K5000 might set the top bits of this, so mask them off
                    this.EffectAlgorithm = (EffectAlgorithm)(effectAlgorithm & 0x03);

                    byte[] reverbData = reader.ReadBytes(ReverbSettings.DataSize);
                    this.Reverb = new ReverbSettings(reverbData);

                    byte[] effect1Data = reader.ReadBytes(EffectSettings.DataSize);
                    this.Effect1 = new EffectSettings(effect1Data);

                    byte[] effect2Data = reader.ReadBytes(EffectSettings.DataSize);
                    this.Effect2 = new EffectSettings(effect2Data);

                    byte[] effect3Data = reader.ReadBytes(EffectSettings.DataSize);
                    this.Effect3 = new EffectSettings(effect3Data);

                    byte[] effect4Data = reader.ReadBytes(EffectSettings.DataSize);
                    this.Effect4 = new EffectSettings(effect4Data);

                    byte[] geqData = reader.ReadBytes(GEQSettings.DataSize);
                    this.GEQ = new GEQSettings(geqData);

                    byte[] nameData = new byte[PatchName.Length];
                    this.Name = new PatchName(nameData);

                    this.Volume = new PositiveLevel(reader.ReadByte());

                    // TODO: Parse the mute byte
                    var muteByte = reader.ReadByte();

                    byte[] ec1Data = reader.ReadBytes(3);
                    this.EffectControl1 = new EffectControl(ec1Data);

                    byte[] ec2Data = reader.ReadBytes(3);
                    this.EffectControl2 = new EffectControl(ec2Data);
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
